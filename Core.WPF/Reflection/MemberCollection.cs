using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
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

    public MemberModel Parent { get; private set; }

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

    internal void LoadSync(SourceFilter filter, Action<MemberModel, SourceFilter> onAdded)
    {
        var isExplicit
            = Source.Type.GetAttribute<ExplicitAttribute>() != null;

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
                        if (isExplicit)
                        {
                            if (isImplicit(i))
                                continue;
                        }
                        break;

                    case MemberTypes.Property:
                        if (isExplicit)
                        {
                            if (isImplicit(i))
                                continue;
                        }
                        break;

                    default: continue;
                }

                if (Source.Type.GetAttribute<IgnoreAttribute>() is IgnoreAttribute ignore)
                {
                    if (XCollection.Contains(ignore.Values, i.Name))
                        continue;
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

            if (filter != null)
            {
                bool a, b, c, d;
                var view = i.Value.GetFirst<ViewAttribute>();

                if (!filter.Ignore)
                {
                    a = !i.Value.ContainsKey(filter.Type);

                    b = filter.Type == typeof(ViewAttribute);
                    c = view?.Name != filter.View;

                    if (a || (b && c))
                        continue;
                }
                else
                {
                    a = filter.Type != typeof(ViewAttribute);
                    b = i.Value.ContainsKey(filter.Type);

                    c = !a;
                    d = view?.Name == filter.View;

                    if ((a && b) || (c && d))
                        continue;
                }
            }

            var data = new MemberData(this, Source, i.Key, i.Value);
            MemberModel result 
                = i.Key is FieldInfo 
                ? new FieldModel(data, Depth) 
                : i.Key is PropertyInfo 
                ? new PropertyModel(data, Depth) 
                : null;

            Add(result);
            Dispatch.Invoke(() => onAdded?.Invoke(result, filter));
        }
    }

    internal async Task Load(MemberSource source, SourceFilter filter, Action<MemberModel, SourceFilter> onAdded)
    {
        Loading = true;

        Clear();
        Source = source;

        await Task.Run(() =>
        {
            lock (cache)
            {
                LoadSync(filter, onAdded);
            }
        });

        Subscribe();
        Loading = false;
    }

    internal async Task Load(SourceFilter filter, Action<MemberModel, SourceFilter> onAdded)
        => await Load(Source, filter, onAdded);

    internal void Load(object input)
    {
        Clear();
        Source = new(new MemberPathSource(input));

        LoadSync(null, null);
        Subscribe();
    }

    //... (public)

    public void Refresh() => this.ForEach(i => i.UpdateValueSafe());

    public void Refresh(MemberSource newSource)
    {
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