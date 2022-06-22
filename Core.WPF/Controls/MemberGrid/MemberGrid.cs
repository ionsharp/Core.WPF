using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Config;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Text;
using Imagin.Core.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public partial class MemberGrid : DataGrid
{
    #region Keys

    public static readonly ResourceKey<Border> HorizontalTemplate
        = new();

    public static readonly ResourceKey<Border> VerticalTemplate
        = new();

    //...

    public static readonly ResourceKey NullTemplateKey
        = new();

    public static readonly ResourceKey ReadOnlyTemplateKey
        = new();

    //...

    public static readonly ResourceKey<Border> DescriptionStyleKey
        = new();

    public static readonly ResourceKey<GridSplitter> GridSplitterStyleKey
        = new();

    //...

    public static readonly ResourceKey<ComboBox> ComboBoxStyleKey
        = new();

    public static readonly ResourceKey<PasswordBox> PasswordBoxStyleKey
        = new();

    public static readonly ResourceKey<TextBox> TextBoxStyleKey
        = new();

    //...

    public static readonly ResourceKey ColorTemplateKey
        = new();

    public static readonly ResourceKey ColorBoxTemplateKey
        = new();

    public static readonly ResourceKey ColorBothTemplateKey
        = new();

    public static readonly ResourceKey ColorTextBoxTemplateKey
        = new();
        
    //...

    public static readonly ResourceKey EnumItemTemplateKey
        = new();
        
    //...

    public static readonly ResourceKey ByteUpDownKey
        = new();

    public static readonly ResourceKey DateTimeUpDownKey
        = new();

    public static readonly ResourceKey DecimalUpDownKey
        = new();

    public static readonly ResourceKey DoubleUpDownKey
        = new();

    public static readonly ResourceKey Int16UpDownKey
        = new();

    public static readonly ResourceKey Int32UpDownKey
        = new();

    public static readonly ResourceKey Int64UpDownKey
        = new();

    public static readonly ResourceKey SingleUpDownKey
        = new();

    public static readonly ResourceKey TimeSpanUpDownKey
        = new();

    public static readonly ResourceKey UDoubleUpDownKey
        = new();

    public static readonly ResourceKey UInt16UpDownKey
        = new();

    public static readonly ResourceKey UInt32UpDownKey
        = new();

    public static readonly ResourceKey UInt64UpDownKey
        = new();

    //...

    public static readonly ResourceKey ListTemplateKey
        = new();

    public static readonly ResourceKey ListBulletTemplateKey
        = new();

    public static readonly ResourceKey ListCommaTemplateKey
        = new();

    public static readonly ResourceKey ListImageToggleButtonTemplateKey
        = new();
        
    //...

    public static readonly ResourceKey ObjectTemplateKey
        = new();

    public static readonly ResourceKey ObjectButtonTemplateKey
        = new();
        
    //...

    public static readonly ResourceKey RangeTemplateKey
        = new();

    public static readonly ResourceKey RangeSliderTemplateKey
        = new();

    public static readonly ResourceKey RangeUpDownTemplateKey
        = new();

    //...

    public static readonly ResourceKey ToolTipKey
        = new();

    //...

    public static readonly ReferenceKey<ScrollViewer> ScrollViewerKey = new();

    #endregion

    #region Fields

    public event EventHandler<EventArgs<object>> SourceChanged;

    //...

    readonly MemberSortComparer SortComparer;

    //...

    DataGridTemplateColumn NameColumn;

    readonly DataGridTemplateColumn ValueColumn;

    //...

    readonly CancelTask<Value> loadTask;

    readonly Handle handleFilter = false;

    #endregion

    #region Constants

    #region DefaultTypes

    /// <summary>
    /// Types that have a default visual representation.
    /// </summary>
    public static List<Type> DefaultTypes = new()
    {
        typeof(Alignment),
        typeof(Array),
        typeof(bool),
        typeof(Bullets),
        typeof(byte),
        typeof(CardinalDirection),
        typeof(System.Drawing.Color),
        typeof(System.Windows.Media.Color),
        typeof(DateTime),
        typeof(decimal),
        typeof(double),
        typeof(DoubleMatrix),
        typeof(Enum),
        typeof(float),
        typeof(FontFamily),
        typeof(FontStyle),
        typeof(FontWeight),
        typeof(Gradient),
        typeof(GradientStepCollection),
        typeof(Unit),
        typeof(Guid),
        typeof(Hexadecimal),
        typeof(ICommand),
        typeof(IEnumerable),
        typeof(IList),
        typeof(int),
        typeof(Int32Pattern),
        typeof(ItemCollection),
        typeof(long),
        typeof(Layouts),
        typeof(LinearGradientBrush),
        typeof(ListCollectionView),
        typeof(NetworkCredential),
        typeof(object),
        typeof(Matrix),
        typeof(PanelCollection),
        typeof(PointCollection),
        typeof(RadialGradientBrush),
        typeof(short),
        typeof(SolidColorBrush),
        typeof(string),
        typeof(StringColor),
        typeof(ApplicationResources),
        typeof(Thickness),
        typeof(TimeSpan),
        typeof(TimeZoneInfo),
        typeof(Type),
        typeof(UDouble),
        typeof(UIElementCollection),
        typeof(uint),
        typeof(ulong),
        typeof(Uri),
        typeof(ushort),
        typeof(Version),
        typeof(VisualCollection),
    };

    #endregion

    #region AssignableTypes

    /// <summary>
    /// Types that can be assigned other (derived) types.
    /// </summary>
    public static Dictionary<Type, Type[]> AssignableTypes = new()
    {
        { typeof(Brush), XArray.New<Type>(typeof(LinearGradientBrush), typeof(RadialGradientBrush), typeof(SolidColorBrush)) }
    };

    #endregion

    #region ForbiddenTypes

    /// <summary>
    /// Types to avoid.
    /// </summary>
    public static List<Type> ForbiddenTypes = new()
    {
        typeof(ItemCollection),
        typeof(UIElementCollection),
        typeof(VisualCollection),
    };

    #endregion

    #region IndeterminableTypes

    /// <summary>
    /// Types where an indeterminable state can be represented visually.
    /// </summary>
    public static List<Type> IndeterminableTypes = new()
    {
        typeof(bool),
        typeof(byte),
        typeof(DateTime),
        typeof(decimal),
        typeof(double),
        typeof(Enum),
        typeof(FontFamily),
        typeof(FontStyle),
        typeof(FontWeight),
        typeof(Guid),
        typeof(ICommand),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(object),
        typeof(float),
        typeof(string),
        typeof(TimeSpan),
        typeof(TimeZoneInfo),
        typeof(Type),
        typeof(UDouble),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(Uri),
        typeof(Version),
    };

    #endregion

    #endregion

    #region Properties

    #region (readonly) ActiveMember

    static readonly DependencyPropertyKey ActiveMemberKey = DependencyProperty.RegisterReadOnly(nameof(ActiveMember), typeof(MemberModel), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ActiveMemberProperty = ActiveMemberKey.DependencyProperty;
    public MemberModel ActiveMember
    {
        get => (MemberModel)GetValue(ActiveMemberProperty);
        private set => SetValue(ActiveMemberKey, value);
    }

    #endregion

    #region (ReadOnly) ActualSource

    static readonly DependencyPropertyKey ActualSourceKey = DependencyProperty.RegisterReadOnly(nameof(ActualSource), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ActualSourceProperty = ActualSourceKey.DependencyProperty;
    public object ActualSource
    {
        get => GetValue(ActualSourceProperty);
        private set => SetValue(ActualSourceKey, value);
    }

    #endregion

    #region BackButtonTemplate

    public static readonly DependencyProperty BackButtonTemplateProperty = DependencyProperty.Register(nameof(BackButtonTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate BackButtonTemplate
    {
        get => (DataTemplate)GetValue(BackButtonTemplateProperty);
        set => SetValue(BackButtonTemplateProperty, value);
    }

    #endregion

    #region (readonly) CanNavigateBack

    static readonly DependencyPropertyKey CanNavigateBackKey = DependencyProperty.RegisterReadOnly(nameof(CanNavigateBack), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty CanNavigateBackProperty = CanNavigateBackKey.DependencyProperty;
    public bool CanNavigateBack
    {
        get => (bool)GetValue(CanNavigateBackProperty);
        private set => SetValue(CanNavigateBackKey, value);
    }

    #endregion

    #region CanResizeDescription

    public static readonly DependencyProperty CanResizeDescriptionProperty = DependencyProperty.Register(nameof(CanResizeDescription), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public bool CanResizeDescription
    {
        get => (bool)GetValue(CanResizeDescriptionProperty);
        set => SetValue(CanResizeDescriptionProperty, value);
    }

    #endregion

    #region DefaultCategoryName

    public static readonly DependencyProperty DefaultCategoryNameProperty = DependencyProperty.Register(nameof(DefaultCategoryName), typeof(string), typeof(MemberGrid), new FrameworkPropertyMetadata("General"));
    public string DefaultCategoryName
    {
        get => (string)GetValue(DefaultCategoryNameProperty);
        set => SetValue(DefaultCategoryNameProperty, value);
    }

    #endregion

    #region DescriptionHeight

    public static readonly DependencyProperty DescriptionHeightProperty = DependencyProperty.Register(nameof(DescriptionHeight), typeof(GridLength), typeof(MemberGrid), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto)));
    public GridLength DescriptionHeight
    {
        get => (GridLength)GetValue(DescriptionHeightProperty);
        set => SetValue(DescriptionHeightProperty, value);
    }

    #endregion

    #region DescriptionTemplate

    public static readonly DependencyProperty DescriptionTemplateProperty = DependencyProperty.Register(nameof(DescriptionTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate DescriptionTemplate
    {
        get => (DataTemplate)GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    #endregion

    #region DescriptionTemplateSelector

    public static readonly DependencyProperty DescriptionTemplateSelectorProperty = DependencyProperty.Register(nameof(DescriptionTemplateSelector), typeof(DataTemplateSelector), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplateSelector DescriptionTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(DescriptionTemplateSelectorProperty);
        set => SetValue(DescriptionTemplateSelectorProperty, value);
    }

    #endregion

    #region DescriptionVisibility

    public static readonly DependencyProperty DescriptionVisibilityProperty = DependencyProperty.Register(nameof(DescriptionVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Collapsed));
    public Visibility DescriptionVisibility
    {
        get => (Visibility)GetValue(DescriptionVisibilityProperty);
        set => SetValue(DescriptionVisibilityProperty, value);
    }

    #endregion

    #region FeaturedRepeats

    public static readonly DependencyProperty FeaturedRepeatsProperty = DependencyProperty.Register(nameof(FeaturedRepeats), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public bool FeaturedRepeats
    {
        get => (bool)GetValue(FeaturedRepeatsProperty);
        set => SetValue(FeaturedRepeatsProperty, value);
    }

    #endregion

    #region FeaturedTemplate

    public static readonly DependencyProperty FeaturedTemplateProperty = DependencyProperty.Register(nameof(FeaturedTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate FeaturedTemplate
    {
        get => (DataTemplate)GetValue(FeaturedTemplateProperty);
        set => SetValue(FeaturedTemplateProperty, value);
    }

    #endregion

    #region FeaturedVisibility

    public static readonly DependencyProperty FeaturedVisibilityProperty = DependencyProperty.Register(nameof(FeaturedVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility FeaturedVisibility
    {
        get => (Visibility)GetValue(FeaturedVisibilityProperty);
        set => SetValue(FeaturedVisibilityProperty, value);
    }

    #endregion

    #region (ReadOnly) FeaturedAbove

    static readonly DependencyPropertyKey FeaturedAboveKey = DependencyProperty.RegisterReadOnly(nameof(FeaturedAbove), typeof(ObservableCollection<MemberModel>), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty FeaturedAboveProperty = FeaturedAboveKey.DependencyProperty;
    public ObservableCollection<MemberModel> FeaturedAbove
    {
        get => (ObservableCollection<MemberModel>)GetValue(FeaturedAboveProperty);
        private set => SetValue(FeaturedAboveKey, value);
    }

    #endregion

    #region (ReadOnly) FeaturedBelow

    static readonly DependencyPropertyKey FeaturedBelowKey = DependencyProperty.RegisterReadOnly(nameof(FeaturedBelow), typeof(ObservableCollection<MemberModel>), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty FeaturedBelowProperty = FeaturedBelowKey.DependencyProperty;
    public ObservableCollection<MemberModel> FeaturedBelow
    {
        get => (ObservableCollection<MemberModel>)GetValue(FeaturedBelowProperty);
        private set => SetValue(FeaturedBelowKey, value);
    }

    #endregion

    #region GroupDirection

    public static readonly DependencyProperty GroupDirectionProperty = DependencyProperty.Register(nameof(GroupDirection), typeof(ListSortDirection), typeof(MemberGrid), new FrameworkPropertyMetadata(ListSortDirection.Ascending, OnGroupDirectionChanged));
    public ListSortDirection GroupDirection
    {
        get => (ListSortDirection)GetValue(GroupDirectionProperty);
        set => SetValue(GroupDirectionProperty, value);
    }
    static void OnGroupDirectionChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnGroupDirectionChanged(e);

    #endregion

    #region GroupName

    public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(MemberGroupName), typeof(MemberGrid), new FrameworkPropertyMetadata(MemberGroupName.Category, OnGroupNameChanged));
    public MemberGroupName GroupName
    {
        get => (MemberGroupName)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }
    static void OnGroupNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnGroupNameChanged(new Value<MemberGroupName>(e));

    #endregion

    #region HeaderButtons

    public static readonly DependencyProperty HeaderButtonsProperty = DependencyProperty.Register(nameof(HeaderButtons), typeof(FrameworkElementCollection), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public FrameworkElementCollection HeaderButtons
    {
        get => (FrameworkElementCollection)GetValue(HeaderButtonsProperty);
        set => SetValue(HeaderButtonsProperty, value);
    }

    #endregion

    #region HeaderVisibility

    public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register(nameof(HeaderVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility HeaderVisibility
    {
        get => (Visibility)GetValue(HeaderVisibilityProperty);
        set => SetValue(HeaderVisibilityProperty, value);
    }

    #endregion

    #region (ReadOnly) Loading

    static readonly DependencyPropertyKey LoadingKey = DependencyProperty.RegisterReadOnly(nameof(Loading), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty LoadingProperty = LoadingKey.DependencyProperty;
    public bool Loading
    {
        get => (bool)GetValue(LoadingProperty);
        internal set => SetValue(LoadingKey, value);
    }

    #endregion

    #region LoaderTemplate

    public static readonly DependencyProperty LoaderTemplateProperty = DependencyProperty.Register(nameof(LoaderTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate LoaderTemplate
    {
        get => (DataTemplate)GetValue(LoaderTemplateProperty);
        set => SetValue(LoaderTemplateProperty, value);
    }

    #endregion

    //...

    #region MemberNullText

    public static readonly DependencyProperty MemberNullTextProperty = DependencyProperty.Register(nameof(MemberNullText), typeof(string), typeof(MemberGrid), new FrameworkPropertyMetadata("This property can't be edited"));
    public string MemberNullText
    {
        get => (string)GetValue(MemberNullTextProperty);
        set => SetValue(MemberNullTextProperty, value);
    }

    #endregion

    #region MemberNullTextStyle

    public static readonly DependencyProperty MemberNullTextStyleProperty = DependencyProperty.Register(nameof(MemberNullTextStyle), typeof(Style), typeof(MemberGrid), new FrameworkPropertyMetadata(default(Style)));
    public Style MemberNullTextStyle
    {
        get => (Style)GetValue(MemberNullTextStyleProperty);
        set => SetValue(MemberNullTextStyleProperty, value);
    }

    #endregion

    #region (ReadOnly) Members

    static readonly DependencyPropertyKey MembersKey = DependencyProperty.RegisterReadOnly(nameof(Members), typeof(MemberCollection), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty MembersProperty = MembersKey.DependencyProperty;
    public MemberCollection Members
    {
        get => (MemberCollection)GetValue(MembersProperty);
        private set => SetValue(MembersKey, value);
    }

    #endregion

    #region NameColumnHeader

    public static readonly DependencyProperty NameColumnHeaderProperty = DependencyProperty.Register(nameof(NameColumnHeader), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public object NameColumnHeader
    {
        get => GetValue(NameColumnHeaderProperty);
        set => SetValue(NameColumnHeaderProperty, value);
    }

    #endregion

    #region NameColumnHeaderTemplate

    public static readonly DependencyProperty NameColumnHeaderTemplateProperty = DependencyProperty.Register(nameof(NameColumnHeaderTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate NameColumnHeaderTemplate
    {
        get => (DataTemplate)GetValue(NameColumnHeaderTemplateProperty);
        set => SetValue(NameColumnHeaderTemplateProperty, value);
    }

    #endregion

    #region NameColumnVisibility

    public static readonly DependencyProperty NameColumnVisibilityProperty = DependencyProperty.Register(nameof(NameColumnVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible, OnNameColumnVisibilityChanged));
    public Visibility NameColumnVisibility
    {
        get => (Visibility)GetValue(NameColumnVisibilityProperty);
        set => SetValue(NameColumnVisibilityProperty, value);
    }
    static void OnNameColumnVisibilityChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnNameColumnVisibilityChanged(new Value<Visibility>(e));

    #endregion
        
    #region NameColumnWidth

    public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register(nameof(NameColumnWidth), typeof(DataGridLength), typeof(MemberGrid), new FrameworkPropertyMetadata(default(DataGridLength)));
    public DataGridLength NameColumnWidth
    {
        get => (DataGridLength)GetValue(NameColumnWidthProperty);
        set => SetValue(NameColumnWidthProperty, value);
    }

    #endregion

    #region NameTemplate

    public static readonly DependencyProperty NameTemplateProperty = DependencyProperty.Register(nameof(NameTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate NameTemplate
    {
        get => (DataTemplate)GetValue(NameTemplateProperty);
        set => SetValue(NameTemplateProperty, value);
    }

    #endregion

    #region OptionsButtonVisibility

    public static readonly DependencyProperty OptionsButtonVisibilityProperty = DependencyProperty.Register(nameof(OptionsButtonVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility OptionsButtonVisibility
    {
        get => (Visibility)GetValue(OptionsButtonVisibilityProperty);
        set => SetValue(OptionsButtonVisibilityProperty, value);
    }

    #endregion

    #region Orientation

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MemberGrid), new FrameworkPropertyMetadata(Orientation.Vertical, OnOrientationChanged));
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    static void OnOrientationChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnOrientationChanged(e);

    #endregion

    #region OverrideTemplates

    public static readonly DependencyProperty OverrideTemplatesProperty = DependencyProperty.Register(nameof(OverrideTemplates), typeof(KeyTemplateCollection), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public KeyTemplateCollection OverrideTemplates
    {
        get => (KeyTemplateCollection)GetValue(OverrideTemplatesProperty);
        set => SetValue(OverrideTemplatesProperty, value);
    }

    #endregion

    #region PlaceholderConverter

    public static readonly DependencyProperty PlaceholderConverterProperty = DependencyProperty.Register(nameof(PlaceholderConverter), typeof(IValueConverter), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public IValueConverter PlaceholderConverter
    {
        get => (IValueConverter)GetValue(PlaceholderConverterProperty);
        set => SetValue(PlaceholderConverterProperty, value);
    }

    #endregion

    #region (ReadOnly) Route

    static readonly DependencyPropertyKey RouteKey = DependencyProperty.RegisterReadOnly(nameof(Route), typeof(MemberPath), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty RouteProperty = RouteKey.DependencyProperty;
    /// <summary>
    /// Stores a reference to every nested property relative to the original object; properties are stored in order of depth.
    /// </summary>
    public MemberPath Route
    {
        get => (MemberPath)GetValue(RouteProperty);
        private set => SetValue(RouteKey, value);
    }

    #endregion

    #region RouteStringFormat

    public static readonly DependencyProperty RouteStringFormatProperty = DependencyProperty.Register(nameof(RouteStringFormat), typeof(string), typeof(MemberGrid), new FrameworkPropertyMetadata(string.Empty));
    public string RouteStringFormat
    {
        get => (string)GetValue(RouteStringFormatProperty);
        set => SetValue(RouteStringFormatProperty, value);
    }

    #endregion

    #region RouteTemplate

    public static readonly DependencyProperty RouteTemplateProperty = DependencyProperty.Register(nameof(RouteTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(default(DataTemplate)));
    public DataTemplate RouteTemplate
    {
        get => (DataTemplate)GetValue(RouteTemplateProperty);
        set => SetValue(RouteTemplateProperty, value);
    }

    #endregion

    #region RouteTemplateSelector

    public static readonly DependencyProperty RouteTemplateSelectorProperty = DependencyProperty.Register(nameof(RouteTemplateSelector), typeof(DataTemplateSelector), typeof(MemberGrid), new FrameworkPropertyMetadata(default(DataTemplateSelector)));
    public DataTemplateSelector RouteTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(RouteTemplateSelectorProperty);
        set => SetValue(RouteTemplateSelectorProperty, value);
    }

    #endregion
        
    #region Search

    public static readonly DependencyProperty SearchProperty = DependencyProperty.Register(nameof(Search), typeof(string), typeof(MemberGrid), new FrameworkPropertyMetadata(string.Empty));
    public string Search
    {
        get => (string)GetValue(SearchProperty);
        set => SetValue(SearchProperty, value);
    }

    #endregion

    #region SearchName

    public static readonly DependencyProperty SearchNameProperty = DependencyProperty.Register(nameof(SearchName), typeof(MemberSearchName), typeof(MemberGrid), new FrameworkPropertyMetadata(MemberSearchName.Name));
    public MemberSearchName SearchName
    {
        get => (MemberSearchName)GetValue(SearchNameProperty);
        set => SetValue(SearchNameProperty, value);
    }

    #endregion

    #region SearchVisibility

    public static readonly DependencyProperty SearchVisibilityProperty = DependencyProperty.Register(nameof(SearchVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Collapsed));
    public Visibility SearchVisibility
    {
        get => (Visibility)GetValue(SearchVisibilityProperty);
        set => SetValue(SearchVisibilityProperty, value);
    }

    #endregion

    #region SortDirection

    public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(nameof(SortDirection), typeof(ListSortDirection), typeof(MemberGrid), new FrameworkPropertyMetadata(ListSortDirection.Ascending, OnSortDirectionChanged));
    public ListSortDirection SortDirection
    {
        get => (ListSortDirection)GetValue(SortDirectionProperty);
        set => SetValue(SortDirectionProperty, value);
    }
    static void OnSortDirectionChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSortDirectionChanged(new Value<ListSortDirection>(e));

    #endregion

    #region SortName

    public static readonly DependencyProperty SortNameProperty = DependencyProperty.Register(nameof(SortName), typeof(MemberSortName), typeof(MemberGrid), new FrameworkPropertyMetadata(MemberSortName.DisplayName, OnSortNameChanged));
    public MemberSortName SortName
    {
        get => (MemberSortName)GetValue(SortNameProperty);
        set => SetValue(SortNameProperty, value);
    }
    static void OnSortNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSortNameChanged(new Value<MemberSortName>(e));

    #endregion

    #region Source

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null, OnSourceChanged));
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    static void OnSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSourceChanged(new Value(e));

    #endregion

    #region (ReadOnly) Tools

    static readonly DependencyPropertyKey ToolsKey = DependencyProperty.RegisterReadOnly(nameof(Tools), typeof(ObservableCollection<MemberModel>), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ToolsProperty = ToolsKey.DependencyProperty;
    public ObservableCollection<MemberModel> Tools
    {
        get => (ObservableCollection<MemberModel>)GetValue(ToolsProperty);
        private set => SetValue(ToolsKey, value);
    }

    #endregion

    #region ToolVisibility

    public static readonly DependencyProperty ToolVisibilityProperty = DependencyProperty.Register(nameof(ToolVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility ToolVisibility
    {
        get => (Visibility)GetValue(ToolVisibilityProperty);
        set => SetValue(ToolVisibilityProperty, value);
    }

    #endregion

    #region ValueColumnHeader

    public static readonly DependencyProperty ValueColumnHeaderProperty = DependencyProperty.Register(nameof(ValueColumnHeader), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public object ValueColumnHeader
    {
        get => GetValue(ValueColumnHeaderProperty);
        set => SetValue(ValueColumnHeaderProperty, value);
    }

    #endregion

    #region ValueColumnHeaderTemplate

    public static readonly DependencyProperty ValueColumnHeaderTemplateProperty = DependencyProperty.Register(nameof(ValueColumnHeaderTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate ValueColumnHeaderTemplate
    {
        get => (DataTemplate)GetValue(ValueColumnHeaderTemplateProperty);
        set => SetValue(ValueColumnHeaderTemplateProperty, value);
    }

    #endregion

    #region ValueColumnWidth

    public static readonly DependencyProperty ValueColumnWidthProperty = DependencyProperty.Register(nameof(ValueColumnWidth), typeof(DataGridLength), typeof(MemberGrid), new FrameworkPropertyMetadata(default(DataGridLength)));
    public DataGridLength ValueColumnWidth
    {
        get => (DataGridLength)GetValue(ValueColumnWidthProperty);
        set => SetValue(ValueColumnWidthProperty, value);
    }

    #endregion

    #region ValueTemplate

    public static readonly DependencyProperty ValueTemplateProperty = DependencyProperty.Register(nameof(ValueTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate ValueTemplate
    {
        get => (DataTemplate)GetValue(ValueTemplateProperty);
        set => SetValue(ValueTemplateProperty, value);
    }

    #endregion

    #region View

    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(View), typeof(MemberView), typeof(MemberGrid), new FrameworkPropertyMetadata(MemberView.All, OnViewChanged));
    public MemberView View
    {
        get => (MemberView)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }
    static void OnViewChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnViewChanged(new Value<MemberView>(e));

    #endregion

    #region (ReadOnly) ViewAll

    static readonly DependencyPropertyKey ViewAllKey = DependencyProperty.RegisterReadOnly(nameof(ViewAll), typeof(ListCollectionView), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ViewAllProperty = ViewAllKey.DependencyProperty;
    public ListCollectionView ViewAll
    {
        get => (ListCollectionView)GetValue(ViewAllProperty);
        private set => SetValue(ViewAllKey, value);
    }

    #endregion

    #region (ReadOnly) ViewAllMembers

    static readonly DependencyPropertyKey ViewAllMembersKey = DependencyProperty.RegisterReadOnly(nameof(ViewAllMembers), typeof(ObservableCollection<MemberModel>), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ViewAllMembersProperty = ViewAllMembersKey.DependencyProperty;
    public ObservableCollection<MemberModel> ViewAllMembers
    {
        get => (ObservableCollection<MemberModel>)GetValue(ViewAllMembersProperty);
        private set => SetValue(ViewAllMembersKey, value);
    }

    #endregion

    #region (ReadOnly) ViewSingle

    static readonly DependencyPropertyKey ViewSingleKey = DependencyProperty.RegisterReadOnly(nameof(ViewSingle), typeof(ListCollectionView), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ViewSingleProperty = ViewSingleKey.DependencyProperty;
    public ListCollectionView ViewSingle
    {
        get => (ListCollectionView)GetValue(ViewSingleProperty);
        private set => SetValue(ViewSingleKey, value);
    }

    #endregion

    #region (ReadOnly) ViewSingleMember

    static readonly DependencyPropertyKey ViewSingleMemberKey = DependencyProperty.RegisterReadOnly(nameof(ViewSingleMember), typeof(ObservableCollection<MemberModel>), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ViewSingleMemberProperty = ViewSingleMemberKey.DependencyProperty;
    public ObservableCollection<MemberModel> ViewSingleMember
    {
        get => (ObservableCollection<MemberModel>)GetValue(ViewSingleMemberProperty);
        private set => SetValue(ViewSingleMemberKey, value);
    }

    #endregion

    #region (ReadOnly) ViewTools

    static readonly DependencyPropertyKey ViewToolsKey = DependencyProperty.RegisterReadOnly(nameof(ViewTools), typeof(ListCollectionView), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ViewToolsProperty = ViewToolsKey.DependencyProperty;
    public ListCollectionView ViewTools
    {
        get => (ListCollectionView)GetValue(ViewToolsProperty);
        private set => SetValue(ViewToolsKey, value);
    }

    #endregion

    #region ViewMember

    public static readonly DependencyProperty ViewMemberProperty = DependencyProperty.Register(nameof(ViewMember), typeof(MemberModel), typeof(MemberGrid), new FrameworkPropertyMetadata(null, OnViewMemberChanged));
    public MemberModel ViewMember
    {
        get => (MemberModel)GetValue(ViewMemberProperty);
        set => SetValue(ViewMemberProperty, value);
    }
    static void OnViewMemberChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnViewMemberChanged(new Value<MemberModel>(e));

    #endregion

    #endregion

    #region MemberGrid

    public MemberGrid() : base()
    {
        loadTask = new(null, Load, true);
        SortComparer = new(this);

        //...

        Members =
            new MemberCollection(this);

        //...

        FeaturedAbove
            = new();
        FeaturedBelow
            = new();

        Tools
            = new();

        ViewAllMembers
            = new();
        ViewSingleMember
            = new();

        //...

        ViewAll
            = new (ViewAllMembers) { CustomSort = SortComparer };
        ViewSingle
            = new(ViewSingleMember) { CustomSort = SortComparer };
        ViewTools
            = new(Tools) { CustomSort = SortComparer };

        OnViewChanged(View);

        //...

        Route = new();

        //...

        SetCurrentValue(HeaderButtonsProperty, 
            new FrameworkElementCollection());
        SetCurrentValue(OverrideTemplatesProperty,
            new KeyTemplateCollection());

        //...

        OnNameColumnVisibilityChanged(new(NameColumnVisibility));

        ValueColumn
            = new();
        ValueColumn.Bind
            (DataGridTemplateColumn.CellTemplateProperty,
            nameof(ValueTemplate), this);
        ValueColumn.Bind
            (DataGridColumn.HeaderProperty,
            nameof(ValueColumnHeader), this);
        ValueColumn.Bind
            (DataGridColumn.HeaderTemplateProperty,
            nameof(ValueColumnHeaderTemplate), this);
        ValueColumn.Bind
            (DataGridColumn.WidthProperty,
            nameof(ValueColumnWidth), this);

        Columns.Add(ValueColumn);
        this.RegisterHandler(OnLoaded, OnUnloaded);
    }

    #endregion

    #region Methods

    #region Private

    void Clear()
    {
        FeaturedAbove.Clear();
        FeaturedBelow.Clear();
        Tools.Clear();

        ViewAllMembers.Clear();
        ViewSingleMember.Clear();
    }

    void Clear(Predicate<MemberModel> input)
    {
        ViewAllMembers
            .RemoveWhere(i => input((MemberModel)i));
        ViewSingleMember
            .RemoveWhere(i => input((MemberModel)i));
    }

    //...

    async Task Load(Value i, CancellationToken token)
    {
        //Source != null
        if (i.New != null)
        {
            SourceFilter sourceFilter = null;
            if (i.New is SourceFilter)
            {
                sourceFilter = i.New as SourceFilter;
                i = new(i.Old, sourceFilter.Source);
            }

            var j = Route.New(i.Old, i.New);
            //i.Old != i.New
            if (j != null)
            {
                var oldSource
                    = Members.Source;
                var newSource
                    = new MemberSource(j);

                ActualSource
                    = j.Value;

                Route.Append(j);

                if (oldSource?.Type != newSource.Type)
                {
                    Clear();
                    await Members.Load(newSource, sourceFilter, OnMemberAdded);
                }
                else
                {
                    Clear(i => i is EntryModel);
                    Members.Refresh(newSource);
                }
            }
        }
        //Source = null
        else
        {
            ActualSource = null;

            Route.Clear();
            Clear();

            Members.Clear();
        }
        SourceChanged?.Invoke(this, new EventArgs<object>(i.New));
    }

    //...

    void Group()
    {
        var group = new Action<ListCollectionView>(i =>
        {
            if (i != null)
            {
                i.GroupDescriptions.Clear();
                if (GroupName != MemberGroupName.None)
                    i.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = MemberGroupConverterSelector.Default.Select(GroupName) });
            }
        });
        group(ViewAll);
        group(ViewSingle);
        group(ViewTools);
    }

    void Sort()
    {
        ViewAll?.Refresh();
        ViewSingle?.Refresh();
        ViewTools?.Refresh();
    }

    //...

    void OnLoaded()
    {
        if (!loadTask.Started)
            Members.Subscribe();

        Group(); Sort();

        CanNavigateBack = BackCommand.CanExecute(null);
        Route.CollectionChanged += OnRouteChanged;
    }

    void OnMemberAdded(MemberModel i, SourceFilter filter)
    {
        i.Category
            ??= DefaultCategoryName ?? "General";

        if (i is not EntryModel)
        {
            if (filter != null)
            {
                //if (!i.Attributes.ContainsKey(filter.Type))
                    //return;
            }
            if (i.IsFeatured)
            {
                if (i.Attributes.GetFirst<FeaturedAttribute>()?.Where == AboveBelow.Above)
                    FeaturedAbove.Add(i);

                if (i.Attributes.GetFirst<FeaturedAttribute>()?.Where == AboveBelow.Below)
                    FeaturedBelow.Add(i);

                if (!FeaturedRepeats)
                    return;
            }
            else if (i.IsTool)
            {
                if (Orientation == Orientation.Vertical)
                {
                    Tools.Add(i);
                    return;
                }
            }
        }

        ViewAllMembers.Add(i);
    }

    async void OnUnloaded()
    {
        if (loadTask.Started)
            await loadTask.CancelAsync();

        Members.Unsubscribe();
        Route.CollectionChanged -= OnRouteChanged;
    }

    void OnRouteChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        CanNavigateBack = BackCommand.CanExecute(null);
    }

    //...

    void CollapseName()
    {
        NameColumn.Unbind
            (DataGridTemplateColumn.CellTemplateProperty);
        NameColumn.Unbind
            (DataGridColumn.HeaderProperty);
        NameColumn.Unbind
            (DataGridColumn.HeaderTemplateProperty);
        NameColumn.Unbind
            (DataGridColumn.WidthProperty);
        NameColumn.Unbind
            (DataGridColumn.VisibilityProperty);
        Columns.Remove(NameColumn);
        NameColumn = null;
    }

    void HideName() => NameColumn.Visibility = Visibility.Hidden;

    void ShowName()
    {
        if (NameColumn != null)
        {
            NameColumn.Visibility = Visibility.Visible;
            return;
        }

        var result
            = new DataGridTemplateColumn();
        result.Bind
            (DataGridTemplateColumn.CellTemplateProperty,
            nameof(NameTemplate), this);
        result.Bind
            (DataGridColumn.HeaderProperty,
            nameof(NameColumnHeader), this);
        result.Bind
            (DataGridColumn.HeaderTemplateProperty,
            nameof(NameColumnHeaderTemplate), this);
        result.Bind
            (DataGridColumn.WidthProperty,
            nameof(NameColumnWidth), this);
        result.Bind
            (DataGridColumn.VisibilityProperty,
            nameof(NameColumnVisibility), this);

        NameColumn = result;
        Columns.Add(NameColumn);
    }

    #endregion

    #region Protected

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        if (newValue != ViewAll && newValue != ViewSingle)
            throw new ExternalChangeException<MemberGrid>(nameof(ItemsSource));
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        var row = e.OriginalSource.FindParent<DataGridRow>();
        row.If(i => ActiveMember = i.DataContext as MemberModel);
    }

    //...

    protected virtual void OnGroupDirectionChanged(ListSortDirection input) => Group();

    protected virtual void OnGroupDirectionChanged(Value<ListSortDirection> input) => Sort();
        
    protected virtual void OnGroupNameChanged(Value<MemberGroupName> input) => Group();

    protected virtual void OnNameColumnVisibilityChanged(Value<Visibility> input)
    {
        switch (input.New)
        {
            case Visibility.Collapsed:
                CollapseName();
                break;
            case Visibility.Hidden:
                HideName();
                break;
            case Visibility.Visible:
                ShowName();
                break;
        }
    }

    protected virtual void OnOrientationChanged(Value<Orientation> input)
    {
        if (!loadTask.Started)
        {
            if (input.New == Orientation.Horizontal)
            {
                foreach (var i in Tools)
                    ViewAllMembers.Add(i);

                Tools.Clear();
            }
            else
            {
                foreach (var i in ViewAllMembers)
                {
                    if (i.IsTool)
                        Tools.Add(i);
                }

                for (var i = ViewAllMembers.Count - 1; i >= 0; i--)
                {
                    if (ViewAllMembers[i].IsTool)
                        ViewAllMembers.RemoveAt(i);
                }
            }
        }
    }

    protected virtual void OnSortDirectionChanged(Value<ListSortDirection> input) => Sort();

    protected virtual void OnSortNameChanged(Value<MemberSortName> input) => Sort();

    protected virtual void OnSourceChanged(Value input) => _ = loadTask.StartAsync(input);

    void OnViewChanged(MemberView input)
    {
        SetCurrentValue(ViewMemberProperty, null);
        switch (input)
        {
            case MemberView.All:
                SetCurrentValue(ItemsSourceProperty, ViewAll);
                break;
            case MemberView.Single:
                SetCurrentValue(ItemsSourceProperty, ViewSingle);
                break;
        }
    }
    protected virtual void OnViewChanged(Value<MemberView> input) => OnViewChanged(input.New);

    protected virtual void OnViewMemberChanged(Value<MemberModel> input)
    {
        if (View == MemberView.Single)
        {
            if (input.Old != null)
                ViewSingleMember.Remove(input.Old);

            if (input.New != null)
                ViewSingleMember.Add(input.New);
        }
    }

    #endregion

    #region Public

    public void Refresh() => Members.Refresh();

    #endregion

    #region Commands

    ICommand memberNewCommand;
    public ICommand MemberNewCommand => memberNewCommand
        ??= new RelayCommand<DoubleReference>(i =>
        {
            if (i.First is MemberModel j)
            {
                Try.Invoke(() =>
                {
                    if (i.Second is Type n)
                        j.Value = n.Create<object>();

                    if (i.Second is object m)
                        j.Value = m;
                }, 
                e => Log.Write<MemberGrid>(e));
            }
        },
        i => i?.First is MemberModel j && !j.IsReadOnly);

    ICommand memberClearCommand;
    public ICommand MemberClearCommand => memberClearCommand
        ??= new RelayCommand<MemberModel>(i => i.Value = null, i => i is not null && !i.IsReadOnly && (i.Type?.IsValueType == false || i.Type?.IsNullable() == true) && i.Value is not null);

    ICommand memberDefaultCommand;
    public ICommand MemberDefaultCommand => memberDefaultCommand
    ??= new RelayCommand<MemberModel>
        (i => Try.Invoke(() => i.Value = i.Type.GetDefaultValue(), e => Log.Write<MemberGrid>(e)), i => i?.Type?.IsAbstract == false && !i.IsReadOnly);

    ICommand memberResetCommand;
    public ICommand MemberResetCommand => memberResetCommand
        ??= new RelayCommand<MemberModel>(i =>
        {
            if (i.Value is IReset j)
                j.Reset();

            else i.Reset();
        }, 
        i => i is not null && i.Value is IReset);

    //...

    ICommand backCommand;
    public ICommand BackCommand => backCommand
        ??= new RelayCommand(() => EditCommand.Execute(Route.Last<MemberPathElement>(1)), () => Route.Count > 1);

    ICommand clearCommand;
    public ICommand ClearCommand => clearCommand
        ??= new RelayCommand<IList>(i => i.Clear(), i => i?.Count > 0);

    ICommand collapseGroupsCommand;
    public ICommand CollapseGroupsCommand => collapseGroupsCommand
        ??= new RelayCommand(() => this.GetChild(ScrollViewerKey)?.FindVisualChildren<Expander>().ForEach(i => i.IsExpanded = false), () => GroupName != MemberGroupName.None);

    ICommand editCommand;
    public ICommand EditCommand 
        => editCommand ??= new RelayCommand<object>(i => SetCurrentValue(SourceProperty, i));

    ICommand expandGroupsCommand;
    public ICommand ExpandGroupsCommand => expandGroupsCommand
        ??= new RelayCommand(() => this.GetChild(ScrollViewerKey)?.FindVisualChildren<Expander>().ForEach(i => i.IsExpanded = true), () => GroupName != MemberGroupName.None);

    ICommand groupCommand;
    public ICommand GroupCommand => groupCommand ??= new RelayCommand<MemberGroupName>(i => SetCurrentValue(GroupNameProperty, i));

    ICommand searchCommand;
    public ICommand SearchCommand => searchCommand ??= new RelayCommand<MemberSearchName>(i => SetCurrentValue(SearchNameProperty, i));

    ICommand sortCommand;
    public ICommand SortCommand => sortCommand ??= new RelayCommand<object>(i =>
    {
        if (i is MemberSortName name)
            SetCurrentValue(SortNameProperty, name);

        if (i is ListSortDirection direction)
            SetCurrentValue(SortDirectionProperty, direction);
    }, 
    i => i is MemberSortName || i is ListSortDirection);

    ICommand viewCommand;
    public ICommand ViewCommand => viewCommand ??= new RelayCommand<MemberView>(i => SetCurrentValue(ViewProperty, i));

    #endregion

    #endregion
}