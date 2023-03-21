using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Controls;
using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
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

        static bool IsImplicit(MemberInfo input)
            => !input.HasAttribute<BrowsableAttribute>() && !input.HasAttribute<HideAttribute>() && !input.HasAttribute<ShowAttribute>();

        public static void Add(Type input)
        {
            if (input != null && !Current.ContainsKey(input))
            {
                var explicitAttribute = input.GetAttribute<ExplicitAttribute>();
                var explicitTypes = explicitAttribute?.Types ?? MemberTypes.Event | MemberTypes.Method;

                string[] ignoreNames = input.GetAttribute<IgnoreAttribute>()?.Values;

                var members = input.GetMembers(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Event | MemberTypes.Field | MemberTypes.Method | MemberTypes.Property);

                var data = new CacheData();
                foreach (var i in members)
                {
                    if ((i.GetMemberType() is Type memberType && IgnoreTypes.Contains(x => memberType.Inherits(x))) || i.IsIndexor() || ((i is FieldInfo || i is PropertyInfo) && !i.CanGet()) || ignoreNames?.Contains(i.Name) == true)
                        continue;

                    if (i is MethodInfo method)
                    {
                        if (method.IsEvent() || method.IsGetter() || method.IsSetter())
                            continue;
                    }

                    if (explicitTypes.HasFlag(i.MemberType))
                    {
                        if (IsImplicit(i))
                            continue;
                    }

                    if (i.IsHidden() || (i.HasAttribute<MenuItemAttribute>() && !i.HasAttribute<HeaderItemAttribute>() && !i.HasAttribute<HeaderOptionAttribute>())) 
                        continue;
                    
                    data.Add(i, new(i));
                }
                Current.Add(input, data);
            }
        }
    }

    class CacheData : Dictionary<MemberInfo, MemberAttributes>
    {
        public CacheData() : base() { }
    }

    #endregion

    #region (class) Tab

    public class Tab : Namable
    {
        public bool? Categorize { get; private set; }

        public string Description { get; private set; }

        public string Image { get; private set; }

        public MemberLayout Layout { get; private set; }

        public Enum Source { get; private set; }

        public Tab(Enum source, string name, string description, string image, bool? categorize, MemberLayout layout) : base(name) { Source = source; Categorize = categorize; Description = description; Image = image; Layout = layout; }
    }
    #endregion

    #region Types

    #region AssignableTypes

    /// <summary>Types that can be assigned other (derived) types.</summary>
    public static Dictionary<Type, Type[]> AssignableTypes = new() { { typeof(System.Windows.Media.Brush), XArray.New(typeof(System.Windows.Media.LinearGradientBrush), typeof(System.Windows.Media.RadialGradientBrush), typeof(System.Windows.Media.SolidColorBrush)) } };

    #endregion

    #region IgnoreTypes

    /// <summary>Types to avoid.</summary>
    public static List<Type> IgnoreTypes = new()
    {
        typeof(System.Windows.Style),

        typeof(System.Windows.Controls.ControlTemplate),
        typeof(System.Windows.Controls.ItemCollection),
        typeof(System.Windows.Controls.UIElementCollection),
        typeof(System.Windows.Media.VisualCollection),
    };

    #endregion

    #region IndeterminableTemplateTypes

    /// <summary>Types with template that supports modifying an indeterminate value.</summary>
    public static List<Type> IndeterminableTemplateTypes = new()
    {
        typeof(Imagin.Core.UDouble),

        ///

        typeof(System.Boolean),
        typeof(System.Byte),
        typeof(System.Decimal),
        typeof(System.Double),
        typeof(System.Int16),
        typeof(System.Int32),
        typeof(System.Int64),
        typeof(System.Single),
        typeof(System.UInt16),
        typeof(System.UInt32),
        typeof(System.UInt64),

        ///

        typeof(System.Windows.Input.ICommand),

        /*
        Not sure about the following...

        typeof(DateTime),
        typeof(Enum),
        typeof(Guid),
        typeof(Margin),
        typeof(string),
        typeof(TimeSpan),
        typeof(TimeZoneInfo),
        typeof(Type),
        typeof(Uri),
        typeof(Version),

        ///

        typeof(Imagin.Core.CardinalDirection),
        typeof(Imagin.Core.Matrix),

        typeof(Imagin.Core.Media.Gradient),
        typeof(Imagin.Core.Media.GradientPreview),

        typeof(Imagin.Core.Numerics.ByteVector4),
        typeof(Imagin.Core.Numerics.DoubleMatrix),
        typeof(Imagin.Core.Numerics.Int32Pattern),

        typeof(Imagin.Core.Text.Bullets),

        typeof(System.Drawing.Color),

        typeof(System.Net.NetworkCredential),

        typeof(System.Windows.FontStyle),
        typeof(System.Windows.FontWeight),
        typeof(System.Windows.Thickness),

        typeof(System.Windows.Media.Color),
        typeof(System.Windows.Media.FontFamily),
        typeof(System.Windows.Media.LinearGradientBrush),
        typeof(System.Windows.Media.PointCollection),
        typeof(System.Windows.Media.RadialGradientBrush),
        typeof(System.Windows.Media.SolidColorBrush),
        */
    };

    #endregion

    #region TemplateTypes

    /// <summary>Types that have a default template (or visual representation).</summary>
    public static List<Type> TemplateTypes = new()
    {
        typeof(Array),
        typeof(Boolean),
        typeof(Byte),
        typeof(DateTime),
        typeof(Decimal),
        typeof(Double),
        typeof(Enum),
        typeof(Guid),
        typeof(IEnumerable),
        typeof(IList),
        typeof(Int16),
        typeof(Int32),
        typeof(Int64),
        typeof(Object),
        typeof(Single),
        typeof(String),
        typeof(TimeSpan),
        typeof(TimeZoneInfo),
        typeof(Type),
        typeof(UInt16),
        typeof(UInt32),
        typeof(UInt64),
        typeof(Uri),
        typeof(Version),

        typeof(Imagin.Core.CardinalDirection),
        typeof(Imagin.Core.Margin),
        typeof(Imagin.Core.Matrix),
        typeof(Imagin.Core.UDouble),
        typeof(Imagin.Core.USingle),

        typeof(Imagin.Core.Config.ApplicationTheme),
        
        typeof(Imagin.Core.Media.Gradient),
        typeof(Imagin.Core.Media.GradientPreview),

        typeof(Imagin.Core.Media.PointShape),
        typeof(Imagin.Core.Media.VectorShape),

        typeof(Imagin.Core.Models.GroupItemForm),
        typeof(Imagin.Core.Models.PanelCollection),
        
        typeof(Imagin.Core.Numerics.ByteVector4),
        typeof(Imagin.Core.Numerics.DoubleMatrix),
        typeof(Imagin.Core.Numerics.Int32LineCollection),
        
        typeof(Imagin.Core.Text.Bullets),

        typeof(System.Drawing.Color),

        typeof(System.Net.NetworkCredential),

        typeof(System.Reflection.Assembly),
        typeof(System.Reflection.EventInfo),
        typeof(System.Reflection.MethodInfo),

        typeof(System.Windows.FontStyle),
        typeof(System.Windows.FontWeight),
        typeof(System.Windows.GridLength),
        typeof(System.Windows.Thickness),

        typeof(System.Windows.Controls.DataGridLength),

        typeof(System.Windows.Data.ListCollectionView),

        typeof(System.Windows.Input.ICommand),

        typeof(System.Windows.Media.Color),
        typeof(System.Windows.Media.FontFamily),
        typeof(System.Windows.Media.LinearGradientBrush),
        typeof(System.Windows.Media.PointCollection),
        typeof(System.Windows.Media.RadialGradientBrush),
        typeof(System.Windows.Media.SolidColorBrush),
    };

    #endregion

    #endregion

    #region Properties

    public readonly int Depth = 0;

    ///

    public MemberGroupName GroupName { get; private set; }

    ///

    public bool IsLoading { get => this.Get(false); set => this.Set(value); }

    bool IsViewOverridden = false;

    ///

    public MemberModel Parent { get; private set; }

    public MemberSource Source { get => this.Get<MemberSource>(); private set => this.Set(value); }

    ///

    internal readonly List<MemberModel> All = new();

    ///

    public MemberListView Above { get; private set; }

    public MemberListView AboveLeft { get; private set; }

    public MemberListView AboveRight { get; private set; }

    public MemberListView After { get; private set; }

    public MemberListView Before { get; private set; }

    public MemberListView Below { get; private set; }

    public MemberListView BelowLeft { get; private set; }

    public MemberListView BelowRight { get; private set; }

    ///

    public MemberListView Single { get; private set; }

    public MemberModel SelectedMember { get => this.Get<MemberModel>(); set => this.Set(value); }

    ///

    public ObservableCollection<Tab> Tabs { get => this.Get<ObservableCollection<Tab>>(); set => this.Set(value); }

    public int SelectedTab { get => this.Get(-1); set => this.Set(value); }

    public Tab ActualSelectedTab => Tabs?.Count > 0 && SelectedTab >= 0 && SelectedTab < Tabs.Count ? Tabs[SelectedTab] : null;

    ///

    public MemberView CurrentView { get => this.Get(MemberView.All); private set => this.Set(value); }

    public ListCollectionView View { get; private set; }

    #endregion

    #region MemberCollection

    public readonly IComparer Sort;

    public MemberCollection(MemberGroupName group, IComparer sort) : base()
    {
        Sort = sort;

        View = new(this) { CustomSort = sort };
        Single = new(null);

        Above = new(sort); AboveLeft = new(sort); AboveRight = new(sort); After = new(sort); Before = new(sort); Below = new(sort); BelowLeft = new(sort); BelowRight = new(sort);
        ApplyGroup(group);
    }

    public MemberCollection(MemberModel parent, int depth, MemberGroupName group, IComparer sort) : this(group, sort) { Parent = parent; Depth = depth; }

    #endregion

    #region Methods

    bool handleTab = false;

    ///

    new internal void Clear()
    {
        Unsubscribe();

        Single.Source.Clear(); 
        SelectedMember = null;

        Tabs?.Clear(); SelectedTab = -1;
        Source = null;

        EachView(i => i.Source.Clear());
        base.Clear();

        All.ForEach(i => i.Members?.Clear());
        All.Clear();
    }

    ///

    static IEnumerable<Tab> GetTabs(Type input)
    {
        var result = new List<Tab>();

        if (input == typeof(object) || input == typeof(string) || input.IsArray || input.IsEnum || input.IsInterface || input.IsPrimitive)
            return result;

        IEnumerable<Type> types = new Type[] { input };

        if (input.IsClass)
        {
            //Check the input & base types
            Try.Invoke(() => types = types.Concat(input.GetBaseTypes()));
        }
        else if (input.IsValueType)
        {
            //Check the input only
        }
        else return result;

        bool? groupDefault = null;
        foreach (var i in types)
        {
            if (i.GetAttribute<ViewAttribute>() is ViewAttribute attribute && attribute.View == MemberView.Tab)
            {
                if (attribute.ViewParameter is Type type)
                {
                    var groupAll = type.GetAttribute<CategorizeAttribute>();
                    foreach (Enum j in type.GetEnumValues())
                    {
                        var groupTab = j.GetAttribute<CategorizeAttribute>();
                        if (!result.Contains(i => $"{i.Source}" == $"{j}"))
                        {
                            var tab = new Tab(j, j.GetDisplayName() ?? $"{j}", j.GetDescription(), j.GetSmallImage(), groupTab?.Categorize ?? groupAll?.Categorize ?? groupDefault, j.GetAttribute<LayoutAttribute>()?.Layout ?? MemberLayout.Scroll);
                            result.Add(tab);
                        }
                    }
                }
            }
        }

        return result.OrderBy(i => i.Name);
    }
    
    static bool HasTabs(Type input)
    {
        if (input == typeof(object) || input == typeof(string) || input.IsArray || input.IsEnum || input.IsInterface || input.IsPrimitive)
            return false;

        IEnumerable<Type> types = new Type[] { input };

        if (input.IsClass)
        {
            Try.Invoke(() => types = types.Concat(input.GetBaseTypes()));
        }
        else if (input.IsValueType) { }
        else return false;

        return input.GetAttribute<ViewAttribute>()?.View == MemberView.Tab;

        foreach (var i in types)
        {
            if (i.GetAttribute<ViewAttribute>()?.View is MemberView view && view == MemberView.Tab)
                return true;
        }

        return false;
    }

    ///

    void EachView(Action<MemberListView> input)
    {
        input(Above); input(AboveLeft); input(AboveRight); input(After); input(Before); input(Below); input(BelowLeft); input(BelowRight);
    }

    MemberView GetView() => IsViewOverridden 
        ? CurrentView 
        : Source.Type.GetAttribute<ViewAttribute>()?.View == MemberView.Single
            ? MemberView.Single 
            : HasTabs(Source.Type) ? MemberView.Tab : MemberView.All;

    internal void OverrideView(MemberView? view, bool log = false)
    {
        if (view != null)
        {
            if (!IsLoading)
            {
                var oldView = CurrentView;
                var newView = view.Value;

                CurrentView = newView;
                IsViewOverridden = true;

                if (oldView != newView)
                    UpdateView();
            }
        }
    }

    void UpdateView(MemberView view)
    {
        static void Add(IList list, MemberModel x, Tab y)
        {
            if (y != null && x.Tab != y.Name)
                return;

            list.Add(x);
        }

        Tab tab = null;

        if (view == MemberView.Tab)
        {
            if (Tabs == null || Tabs.Count == 0 || SelectedTab < 0 || SelectedTab >= Tabs.Count) return;
            tab = Tabs[SelectedTab];
        }

        EachView(i => i.Source.Clear());
        base.Clear();

        foreach (var i in All)
        {
            if (i.HasAttribute<FloatAttribute>())
            {
                var @float = i.GetAttribute<FloatAttribute>().Float;
                switch (@float)
                {
                    case Float.Above:
                        Add(Above.Source, i, tab);
                        break;

                    case Float.AboveLeft:
                        Add(AboveLeft.Source, i, tab);
                        break;

                    case Float.AboveRight:
                        Add(AboveRight.Source, i, tab);
                        break;

                    case Float.Below:
                        Add(Below.Source, i, tab);
                        break;

                    case Float.BelowLeft:
                        Add(BelowLeft.Source, i, tab);
                        break;

                    case Float.BelowRight:
                        Add(BelowRight.Source, i, tab);
                        break;
                }
            }
            else if (i.HasAttribute<PinAttribute>())
            {
                var pin = i.GetAttribute<PinAttribute>().Pin;
                switch (pin)
                {
                    case Pin.AboveOrLeft:
                        Add(Before.Source, i, tab);
                        break;

                    case Pin.BelowOrRight:
                        Add(After.Source, i, tab);
                        break;
                }
            }
            else Add(this, i, tab);
        }

        ApplyGroup(GroupName); ApplySort();
    }

    void UpdateView()
    {
        if (CurrentView == MemberView.Tab)
        {
            EachView(i => i.Source.Clear());
            base.Clear();

            handleTab = true;

            Tabs?.Clear();
            Tabs ??= new();

            SelectedTab = -1;

            var tabs = GetTabs(Source.Type);
            tabs.ForEach(Tabs.Add);

            handleTab = false;
            SelectedTab = 0;
        }
        else UpdateView(CurrentView);
    }

    ///

    void OnSourceChanged(object sender, PropertyChangedEventArgs e) 
        => All.FirstOrDefault(i => i.Name == e.PropertyName)?.As<IAssignableMemberModel>()?.UpdateValueFromSource();

    ///

    IEnumerable<MemberModel> GetMembers(MemberSource source, MemberSourceFilter filter)
    {
        foreach (var i in Cache.Current[source.Type])
        {
            if (filter != null)
            {
                bool a, b, c, d;
                var page = i.Value.GetFirst<PageAttribute>();

                if (!filter.Ignore)
                {
                    a = !i.Value.ContainsKey(filter.Attribute);

                    b = filter.Attribute == typeof(PageAttribute);
                    c = page?.Name != filter.View;

                    if (a || (b && c))
                        continue;
                }
                else
                {
                    a = filter.Attribute != typeof(PageAttribute);
                    b = i.Value.ContainsKey(filter.Attribute);

                    c = !a;
                    d = page?.Name == filter.View;

                    if ((a && b) || (c && d))
                        continue;
                }
            }
            else if (i.Value.GetFirst<ReserveAttribute>() != null)
                continue;

            var modelType = i.Key.MemberType switch 
            {
                MemberTypes.Event 
                    => typeof(EventModel),
                MemberTypes.Field 
                    => typeof(FieldModel),
                MemberTypes.Method 
                    => typeof(MethodModel),
                MemberTypes.Property 
                    => typeof(PropertyModel),
                _ => null
            };

            if (modelType == null) 
                continue;
            
            yield return modelType.Create<MemberModel>(this, Parent, source, i.Key, i.Value, Depth, Sort);
        }

        if (Depth == 0)
        {
            if (source.FirstInstance is INotifyCollectionChanged)
            {
                if (filter == null)
                    yield return new CollectionModel(this, source, Sort);
            }

            else if (source.FirstInstance is ResourceDictionary dictionary)
            {
                foreach (DictionaryEntry i in dictionary)
                    yield return new ResourceModel(this, source, Sort, i.Key);
            }
        }
    }

    internal void Load(object newValue, MemberSourceFilter filter = null)
    {
        if (Source?.Instance == newValue || Source?.Instances == newValue)
            return;

        object[] newValues = null;
        if (newValue.GetType().IsArray && newValue is object[])
        {
            newValues = (object[])newValue;
            if (newValues.Length == 0)
            {
                return;
            }
            else if (newValues.Length == 1)
            {
                newValue = newValues[0];
                newValues = null;
            }
        }

        IsLoading = true;
        Unsubscribe();

        MemberSource 
            oldSource = Source, 
            newSource = newValues != null
            ? new(newValues)
            : new(newValue);

        if (newSource.Instances != null)
        {
            foreach (var i in newSource.Instances)
            {
                if (i == null)
                    goto Done;
            }
        }

        if (newSource.Type != null)
        {
            var refresh = oldSource?.Type == newSource.Type;
            if (!refresh)
                Clear();

            Source = newSource;

            if (refresh)
            {
                All.ForEach(i =>
                {
                    i.UpdateSource(Source);
                });
            }
            else
            {
                lock (cache)
                {
                    Cache.Add(Source.Type);
                }

                var members = GetMembers(Source, filter);
                members.ForEach(i => All.Add(i));

                if (!IsViewOverridden)
                    CurrentView = GetView();

                UpdateView();
            }
        }

        Done: { }
        Subscribe();
        IsLoading = false;
    }

    ///

    public void Refresh() => All.ForEach(i => i.If<IAssignableMemberModel>(j => j.UpdateValueFromSource()));

    ///

    /// <summary><b>Recursive</b></summary>
    public void ApplyGroup(MemberGroupName input)
    {
        GroupName = input;
        Action<ListCollectionView> action = new(i =>
        {
            if (i != null)
            {
                i.GroupDescriptions.Clear();

                if (i == View)
                {
                    if (CurrentView == MemberView.Tab)
                    {
                        if (Tabs != null && SelectedTab >= 0 && SelectedTab < Tabs.Count && Tabs[SelectedTab].Categorize == false)
                            return;
                    }

                    if (Source?.Instance?.GetAttribute<CategorizeAttribute>()?.Categorize == false)
                        return;
                }

                if (input != MemberGroupName.None)
                    i.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = MemberGroupConverterSelector.Default.Select(input) });
            }
        });

        EachView(i => action(i.View));
        action(View);

        All.ForEach(i => i.Members?.ApplyGroup(input));
    }

    /// <summary><b>Recursive</b></summary>
    public void ApplySort()
    {
        EachView(i => i.View.Refresh());
        View.Refresh();

        All.ForEach(i => i.Members?.ApplySort());
    }

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedTab))
        {
            handleTab.If(false, () => UpdateView(CurrentView));
            this.Update(() => ActualSelectedTab);
        }
    }

    public override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);
        if (e.PropertyName == nameof(SelectedMember))
        {
            e.OldValue.If<MemberModel>(Single.Source.Contains, i => Single.Source.Remove(i));
            e.NewValue.If<MemberModel>(Single.Source.Add);
        }
    }
    ///

    public void Subscribe()
    {
        Source?.EachSource(i => i.If<INotifyPropertyChanged>(j => { j.PropertyChanged -= OnSourceChanged; j.PropertyChanged += OnSourceChanged; }));
        All.ForEach(i => { i.Unsubscribe(); i.Subscribe(); });
    }

    public void Unsubscribe()
    {
        Source?.EachSource(i => i.If<INotifyPropertyChanged>(j => j.PropertyChanged -= OnSourceChanged));
        All.ForEach(i => i.Unsubscribe());
    }

    #endregion
}