using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Imagin.Core.Reflection;

public class MemberCollection : ConcurrentCollection<MemberModel>, ISubscribe, IUnsubscribe
{
    #region (class) Cache

    static readonly object cache = new();

    class Cache : Dictionary<Type, CacheData>
    {
        public static readonly Cache Current = new();

        Cache() : base() { }
    }

    class CacheData : Dictionary<MemberInfo, MemberAttributes>
    {
        public CacheData() : base() { }
    }

    #endregion

    #region Fields

    internal readonly MemberGrid Control;

    internal readonly MemberModel Parent;

    #endregion

    #region Properties

    public MemberSource Source { get; internal set; }

    //...

    bool loading = false;
    public bool Loading
    {
        get => loading;
        set
        {
            this.Change(ref loading, value);
            Control.Loading = value;
        }
    }

    #endregion

    #region MemberCollection

    public readonly int Depth = 0;

    internal MemberCollection(MemberGrid propertyGrid) : base() => Control = propertyGrid;

    internal MemberCollection(MemberGrid propertyGrid, MemberModel parent, int depth) : this(propertyGrid)
    {
        Parent 
            = parent;
        Depth 
            = depth;
    }

    internal MemberModel this[string memberName] => this.FirstOrDefault(i => i.Name == memberName);

    #endregion

    #region Methods

    //... (new)

    new internal void Clear()
    {
        Unsubscribe();
        base.Clear();

        Source = null;
    }

    //... (private)

    /// <summary>
    /// If <see cref="FieldInfo"/>, must be public. If <see cref="PropertyInfo"/>, cannot be indexor and must have <see langword="public"/> getter (with <see langword="internal"/>, <see langword="private"/>, <see langword="protected"/>, or <see langword="public"/> setter).
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    bool Supported(MemberInfo input)
    {
        if (input is FieldInfo a)
        {
            if (MemberGrid.ForbiddenTypes.Contains(a.FieldType))
                return false;

            return a.IsPublic;
        }

        if (input is PropertyInfo b)
        {
            if (MemberGrid.ForbiddenTypes.Contains(b.PropertyType))
                return false;

            return b.GetIndexParameters()?.Length == 0 && b.GetGetMethod(false) != null;
        }

        return false;
    }

    //...

    void OnSourceChanged(object sender, PropertyChangedEventArgs e) 
        => this.FirstOrDefault(i => i.Name == e.PropertyName)?.UpdateValueSafe();

    void OnSourceLocked(object sender, LockedEventArgs e)
    {
        if (e.IsLocked)
        {
            foreach (var i in this)
            {
                if (i.IsLockable)
                    i.IsLocked = true;
            }
        }
        else
        {
            var result = false;
            if (Source.Instance is ILock j)
            {
                if (j.IsLocked)
                {
                    result = true;
                }
            }
            if (!result)
            {
                foreach (var i in this)
                {
                    if (i.IsLockable)
                        i.IsLocked = false;
                }
            }
        }
    }

    //... (internal)

    void Load(IDictionary input, Action<MemberModel> onAdded)
    {
        foreach (DictionaryEntry i in input)
        {
            if (i.Value != null)
            {
                var memberData = new MemberData(this, Source, null, null);

                var result = new EntryModel(memberData) { Name = i.Key.ToString() };
                result.UpdateValue(i.Value);

                Add(result);
                Dispatch.Invoke(() => onAdded?.Invoke(result));
            }
        }
    }

    //...

    internal void LoadSync(Type filterAttribute, bool filterAttributeIgnore, Action<MemberModel> onAdded)
    {
        var visibility
            = Source.Type.GetAttribute<MemberVisibilityAttribute>() ?? new();

            if (!Cache.Current.ContainsKey(Source.Type))
            {
                var data = new CacheData();
                foreach (var i in GetMembers(Source.Type))
                {
                    if (!Supported(i))
                        continue;

                    bool isImplicit(MemberInfo j)
                        => !j.HasAttribute<BrowsableAttribute>() && !j.HasAttribute<HiddenAttribute>() && !j.HasAttribute<VisibleAttribute>();

                    switch (i.MemberType)
                    {
                        case MemberTypes.Field:
                            if (visibility.Field == MemberVisibility.Explicit)
                            {
                                if (isImplicit(i))
                                    continue;
                            }
                            break;

                        case MemberTypes.Property:
                            if (visibility.Property == MemberVisibility.Explicit)
                            {
                                if (isImplicit(i))
                                    continue;
                            }
                            break;

                        default: continue;
                    }

                    var attributes = new MemberAttributes(i);
                    if (attributes.Hidden)
                        continue;

                    data.Add(i, new(i));
                }
                Cache.Current.Add(Source.Type, data);
            }

        foreach (var i in Cache.Current[Source.Type])
        {
            switch (i.Key.MemberType)
            {
                case MemberTypes.Field: case MemberTypes.Property: break;
                default: continue;
            }

            if (filterAttribute != null)
            {
                if (!filterAttributeIgnore)
                {
                    if (!i.Value.ContainsKey(filterAttribute))
                        continue;
                }
                else
                {
                    if (i.Value.ContainsKey(filterAttribute))
                        continue;
                }
            }

            var data = new MemberData(this, Source, i.Key, i.Value);

            MemberModel result = null;
            if (i.Key is FieldInfo field)
            {
                var templateType = MemberModel.GetTemplateType(field.FieldType);

                result = templateType == typeof(INotifyCollectionChanged)
                ? new ListFieldModel(data, Depth)
                : new FieldModel(data, Depth);
            }

            else if (i.Key is PropertyInfo property)
            {
                var templateType = MemberModel.GetTemplateType(property.PropertyType);

                result = templateType == typeof(INotifyCollectionChanged)
                ? new ListPropertyModel(data, Depth)
                : new PropertyModel(data, Depth);
            }

            Add(result);
            Dispatch.Invoke(() => onAdded?.Invoke(result));
        }

        if (Source.Instance is IDictionary dictionary)
            Load(dictionary, onAdded);
    }

    internal async Task Load(MemberSource source, Type filterAttribute, bool filterAttributeIgnore, Action<MemberModel> onAdded)
    {
        Loading = true;

        Clear();
        Source = source;

        await Task.Run(() =>
        {
            lock (cache)
            {
                LoadSync(filterAttribute, filterAttributeIgnore, onAdded);
            }
        });

        Subscribe();
        Loading = false;
    }

    internal async Task Load(Type filterAttribute, bool filterAttributeIgnore, Action<MemberModel> onAdded)
        => await Load(Source, filterAttribute, filterAttributeIgnore, onAdded);

    internal void Load(object input)
    {
        Clear();
        Source = new(new MemberPathSource(input));

        LoadSync(null, false, null);
        Subscribe();
    }

    //... (public)

    public void Refresh() => this.ForEach(i => i.UpdateValueSafe());

    public void Refresh(MemberSource newSource)
    {
        for (var i = Count - 1; i >= 0; i--)
        {
            if (this[i] is EntryModel j)
            {
                j.Unsubscribe();
                RemoveAt(i);
            }
        }

        Unsubscribe();
        Source = newSource;

        XList.ForEach<MemberModel>(this, i =>
        {
            i.UpdateSource(newSource);
            i.UpdateValueSafe();
        });

        Subscribe();
    }

    public void Subscribe()
    {
        this.ForEach(i => { i.Unsubscribe(); i.Subscribe(); });

        Source?.Instance.If<ILock>
            (j => { j.Locked -= OnSourceLocked; j.Locked += OnSourceLocked; });
        Source?.Instance.If<INotifyPropertyChanged>
            (j => { j.PropertyChanged -= OnSourceChanged; j.PropertyChanged += OnSourceChanged; });
    }

    public void Unsubscribe()
    {
        this.ForEach(i => i.Unsubscribe());
        Source?.Instance.If<ILock>
            (j => { j.Locked -= OnSourceLocked; });
        Source?.Instance.If<INotifyPropertyChanged>
            (j => { j.PropertyChanged -= OnSourceChanged; });
    }

    //...

    public static IEnumerable<MemberInfo> GetMembers(Type input) => input.GetMembers(BindingFlags.Instance | BindingFlags.Public).Select(i => i).Where(i =>
    {
        if (i is FieldInfo field && !field.IsStatic)
            return true;

        if (i is PropertyInfo property && !property.GetAccessors(true)[0].IsStatic)
            return true;

        return false;
    });

    #endregion
}