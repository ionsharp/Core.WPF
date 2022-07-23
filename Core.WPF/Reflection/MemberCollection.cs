using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Collections.Generic;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;

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

    #region Properties

    public readonly int Depth = 0;

    //...

    bool isLoading = false;
    public bool IsLoading
    {
        get => isLoading;
        set => this.Change(ref isLoading, value);
    }

    public MemberModel Parent { get; private set; }

    public MemberSource Source { get; private set; }

    //...

    public ObservableCollection<MemberModel> Above { get; private set; } = new();

    public ObservableCollection<MemberModel> Below { get; private set; } = new();

    public ListCollectionView ViewAbove { get; private set; }

    public ListCollectionView ViewBelow { get; private set; }

    #endregion

    #region MemberCollection

    public MemberCollection() : base() 
    {
        ViewAbove = new(Above); ViewBelow = new(Below);
    }

    public MemberCollection(MemberModel parent, int depth) : this() { Parent = parent; Depth = depth; }

    #endregion

    #region Methods

    new internal void Clear()
    {
        Unsubscribe();

        Above.Clear(); Below.Clear(); base.Clear();
        Source = null;
    }

    //...

    void OnSourceChanged(object sender, PropertyChangedEventArgs e) 
        => this.Concat(Above).Concat(Below).FirstOrDefault(i => i.Name == e.PropertyName)?.RefreshSafe();

    void OnSourceLocked(object sender, LockedEventArgs e)
    {
        if (e.IsLocked)
        {
            foreach (var i in this.Concat(Above).Concat(Below))
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
                foreach (var i in this.Concat(Above).Concat(Below))
                {
                    if (i.IsLockable)
                        i.IsLocked = false;
                }
            }
        }
    }

    //...

    void Load(SourceFilter filter, Action<MemberModel> onAdded)
    {
        var isExplicit
            = Source.Type.GetAttribute<ExplicitAttribute>() != null;

        if (!Cache.Current.ContainsKey(Source.Type))
        {
            var data = new CacheData();
            foreach (var i in GetMembers(Source.Type))
            {
                if (!IsValid(i))
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

            MemberModel result 
                = i.Key is FieldInfo 
                ? new FieldModel(Parent, Source, i.Key, i.Value, Depth) 
                : i.Key is PropertyInfo 
                ? new PropertyModel(Parent, Source, i.Key, i.Value, Depth) 
                : null;

            if (result.IsFeatured)
            {
                if (result.HasAttribute<AboveAttribute>())
                    Above.Add(result);

                if (result.HasAttribute<BelowAttribute>())
                    Below.Add(result);
            }
            else Add(result);
            onAdded?.Invoke(result); //Dispatch.Invoke(() => );
        }
    }

    internal async Task Load(object source, SourceFilter filter = null, Action<MemberModel> onAdded = null)
    {
        IsLoading = true;

        Clear();
        Source = new(source);

        lock (cache) { Load(filter, onAdded); }
        await Task.Run(() => { });

        Subscribe();
        IsLoading = false;
    }

    //...

    public void Refresh()
    {
        this.Concat(Above).Concat(Below).ForEach(i => i.RefreshSoftSafe());
    }

    public void Refresh(object newSource)
    {
        Unsubscribe();
        Source = new(newSource);

        this.Concat(Above).Concat(Below)
            .ForEach(i => { i.UpdateSource(Source); i.RefreshSoftSafe(); });

        Subscribe();
    }

    //...

    public void Subscribe()
    {
        this.Concat(Above).Concat(Below).ForEach(i => { i.Unsubscribe(); i.Subscribe(); });

        Source?.Instance.If<ILock>
            (j => { j.Locked -= OnSourceLocked; j.Locked += OnSourceLocked; });
        Source?.Instance.If<INotifyPropertyChanged>
            (j => { j.PropertyChanged -= OnSourceChanged; j.PropertyChanged += OnSourceChanged; });
    }

    public void Unsubscribe()
    {
        this.Concat(Above).Concat(Below).ForEach(i => i.Unsubscribe());

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

    /// <summary>If <see cref="FieldInfo"/>, must be public. If <see cref="PropertyInfo"/>, cannot be indexor and must have <see langword="public"/> getter (with <see langword="internal"/>, <see langword="private"/>, <see langword="protected"/>, or <see langword="public"/> setter).</summary>
    /// <remarks>Only <see cref="FieldInfo"/> and <see cref="PropertyInfo"/> is supported. <see cref="MethodInfo"/> and everything else is not!</remarks>
    public static bool IsValid(MemberInfo input)
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

    #endregion
}