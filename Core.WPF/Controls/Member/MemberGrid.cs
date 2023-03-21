using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Threading;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Controls;

public partial class MemberGrid : DataGrid, IMemberControl, ISubscribe, IUnsubscribe
{
    public enum SourceModes { Single, Multiple }

    #region Keys

    public static readonly ResourceKey ByteUpDownKey = new();

    public static readonly ResourceKey DateTimeUpDownKey = new();

    public static readonly ResourceKey DecimalUpDownKey = new();

    public static readonly ResourceKey DoubleUpDownKey = new();

    public static readonly ResourceKey Int16UpDownKey = new();

    public static readonly ResourceKey Int32UpDownKey = new();

    public static readonly ResourceKey Int64UpDownKey = new();

    public static readonly ResourceKey SingleUpDownKey = new();

    public static readonly ResourceKey TimeSpanUpDownKey = new();

    public static readonly ResourceKey UDoubleUpDownKey = new();
    
    public static readonly ResourceKey UInt16UpDownKey = new();

    public static readonly ResourceKey UInt32UpDownKey = new();

    public static readonly ResourceKey UInt64UpDownKey = new();

    public static readonly ResourceKey USingleUpDownKey = new();

    ///

    public static readonly ResourceKey CaptionTemplateKey = new();

    ///

    public static readonly ResourceKey ComboBoxStyleKey = new();

    public static readonly ResourceKey PasswordBoxStyleKey = new();

    public static readonly ResourceKey TextBoxStyleKey = new();

    ///

    public static readonly ResourceKey DefaultGroupStyle = new();

    public static readonly ResourceKey ToolBarGroupStyle = new();

    public static readonly ResourceKey ToolBarTemplate = new();

    public static readonly ResourceKey ToolBarMemberStyle = new();

    public static readonly ResourceKey ToolBarMemberTemplate = new();

    ///

    public static readonly ResourceKey DescriptionPatternKey = new();

    public static readonly ResourceKey DescriptionStyleKey = new();

    ///

    public static readonly ResourceKey ChildObjectTemplate = new();

    public static readonly ResourceKey ObjectButtonTemplate = new();

    public static readonly ResourceKey ObjectTemplate = new();

    ///

    public static readonly ResourceKey CollectionTemplate = new();

    public static readonly ResourceKey CollectionAddCommandTemplate = new();

    public static readonly ResourceKey CollectionAddItemsTemplate = new();

    public static readonly ResourceKey CollectionAddTypeTemplate = new();

    public static readonly ResourceKey CollectionAddTypesTemplate = new();

    public static readonly ResourceKey CollectionToggleButtonTemplateKey = new();

    ///

    public static readonly ResourceKey GridSplitterStyleKey = new();

    ///

    public static readonly ResourceKey ColorTemplateKey = new();
    
    public static readonly ResourceKey ColorBoxTemplateKey = new();

    public static readonly ResourceKey ColorStringTemplateKey = new();

    public static readonly ResourceKey ColorModelTemplateKey = new();

    public static readonly ResourceKey ColorTextBoxTemplateKey = new();

    ///

    public static readonly ResourceKey EnumItemTemplateKey = new();

    ///

    public static readonly ResourceKey HorizontalTemplate = new();

    public static readonly ResourceKey VerticalTemplate = new();

    ///

    public static readonly ResourceKey HorizontalMemberTemplate = new();

    public static readonly ResourceKey VerticalMemberTemplate = new();

    public static readonly ResourceKey MemberTemplate = new();

    ///

    public static readonly ResourceKey MatrixTemplateKey = new();

    ///
    
    public static readonly ResourceKey MemberIndeterminateTemplateKey = new();

    public static readonly ResourceKey MemberMarkUpCenterTemplateKey = new();

    public static readonly ResourceKey MemberMarkUpLeftTemplateKey = new();

    public static readonly ResourceKey MemberNullTemplateKey = new();

    public static readonly ResourceKey MemberTextTemplateKey = new();

    public static readonly ResourceKey MemberToolTipKey = new();

    public static readonly ResourceKey MemberToolTipHeaderKey = new();

    public static readonly ResourceKey MemberValueTemplateKey = new();

    public static readonly ResourceKey MethodTemplateKey = new();
    
    public static readonly ResourceKey NameTemplateKey = new();

    public static readonly ResourceKey ObjectToolTipKey = new();

    public static readonly ResourceKey OptionsTemplateKey = new();
    
    ///

    public static readonly ResourceKey PinTemplate = new();

    public static readonly ResourceKey HorizontalPinTemplate = new();

    public static readonly ResourceKey VerticalPinTemplate = new();

    ///

    public static readonly ResourceKey RangeTemplateKey = new();

    public static readonly ResourceKey RangeSliderTemplateKey = new();

    public static readonly ResourceKey RangeUpDownTemplateKey = new();

    ///

    public static readonly ResourceKey RouteTemplateKey = new();

    public static readonly ReferenceKey<ScrollViewer> ScrollViewerKey = new();

    public static readonly ResourceKey SourceOptionsTemplateKey = new();

    public static readonly ResourceKey TabToolTipKey = new();

    public static readonly ResourceKey ValueTemplateKey = new();

    ///

    public static readonly ResourceKey ViewSingleTemplate = new();

    public static readonly ResourceKey ViewTabTemplate = new();
    
    #endregion

    #region Fields

    public event EventHandler<EventArgs<object>> SourceChanged;

    ///

    readonly MemberSortComparer SortComparer;

    ///

    readonly DataGridTemplateColumn ValueColumn;

    ///

    readonly Method<ReadOnlyValue> loadTask;

    #endregion

    #region Properties

    #region [- A]

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

    static readonly DependencyPropertyKey ActualSourceKey = DependencyProperty.RegisterReadOnly(nameof(ActualSource), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null, OnActualSourceChanged));
    public static readonly DependencyProperty ActualSourceProperty = ActualSourceKey.DependencyProperty;
    public object ActualSource
    {
        get => GetValue(ActualSourceProperty);
        private set => SetValue(ActualSourceKey, value);
    }
    static void OnActualSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnActualSourceChanged(e.Convert());

    #endregion

    #endregion

    #region [- B]

    #region BackButtonTemplate

    public static readonly DependencyProperty BackButtonTemplateProperty = DependencyProperty.Register(nameof(BackButtonTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate BackButtonTemplate
    {
        get => (DataTemplate)GetValue(BackButtonTemplateProperty);
        set => SetValue(BackButtonTemplateProperty, value);
    }

    #endregion

    #endregion

    #region [- C]

    #region CanGroup

    public static readonly DependencyProperty CanGroupProperty = DependencyProperty.Register(nameof(CanGroup), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(true));
    public bool CanGroup
    {
        get => (bool)GetValue(CanGroupProperty);
        set => SetValue(CanGroupProperty, value);
    }

    #endregion

    #region CanLog

    public static readonly DependencyProperty CanLogProperty = DependencyProperty.Register(nameof(CanLog), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public bool CanLog
    {
        get => (bool)GetValue(CanLogProperty);
        set => SetValue(CanLogProperty, value);
    }

    #region (readonly) CanNavigateBack

    static readonly DependencyPropertyKey CanNavigateBackKey = DependencyProperty.RegisterReadOnly(nameof(CanNavigateBack), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty CanNavigateBackProperty = CanNavigateBackKey.DependencyProperty;
    public bool CanNavigateBack
    {
        get => (bool)GetValue(CanNavigateBackProperty);
        private set => SetValue(CanNavigateBackKey, value);
    }

    #endregion

    #endregion

    #region CanResizeDescription

    public static readonly DependencyProperty CanResizeDescriptionProperty = DependencyProperty.Register(nameof(CanResizeDescription), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public bool CanResizeDescription
    {
        get => (bool)GetValue(CanResizeDescriptionProperty);
        set => SetValue(CanResizeDescriptionProperty, value);
    }

    #endregion

    #endregion

    #region [- D]

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

    #endregion

    #region [- F]

    #region FilterVisibility

    public static readonly DependencyProperty FilterVisibilityProperty = DependencyProperty.Register(nameof(FilterVisibility), typeof(Visibility), typeof(MemberGrid), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility FilterVisibility
    {
        get => (Visibility)GetValue(FilterVisibilityProperty);
        set => SetValue(FilterVisibilityProperty, value);
    }

    #endregion

    #endregion

    #region [- G]

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
    static void OnGroupNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnGroupNameChanged(e.Convert<MemberGroupName>());

    #endregion

    #endregion

    #region [- H]

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

    #endregion

    #region [- I]

    #region (ReadOnly) IsLoading

    static readonly DependencyPropertyKey IsLoadingKey = DependencyProperty.RegisterReadOnly(nameof(IsLoading), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty IsLoadingProperty = IsLoadingKey.DependencyProperty;
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        internal set => SetValue(IsLoadingKey, value);
    }

    #endregion

    #endregion

    #region [- L]

    #region LoaderTemplate

    public static readonly DependencyProperty LoaderTemplateProperty = DependencyProperty.Register(nameof(LoaderTemplate), typeof(DataTemplate), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public DataTemplate LoaderTemplate
    {
        get => (DataTemplate)GetValue(LoaderTemplateProperty);
        set => SetValue(LoaderTemplateProperty, value);
    }

    #endregion

    #region Log

    public static readonly DependencyProperty LogProperty = DependencyProperty.Register(nameof(Log), typeof(bool), typeof(MemberGrid), new FrameworkPropertyMetadata(false));
    public bool Log
    {
        get => (bool)GetValue(LogProperty);
        set => SetValue(LogProperty, value);
    }

    #endregion

    #endregion

    #region [- M]

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

    #endregion

    #region [- N]

    #region NameColumnWidth

    public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register(nameof(NameColumnWidth), typeof(DataGridLength), typeof(MemberGrid), new FrameworkPropertyMetadata(default(DataGridLength)));
    public DataGridLength NameColumnWidth
    {
        get => (DataGridLength)GetValue(NameColumnWidthProperty);
        set => SetValue(NameColumnWidthProperty, value);
    }

    #endregion

    #endregion

    #region [- O]

    #region Orientation

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MemberGrid), new FrameworkPropertyMetadata(Orientation.Vertical));
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    #endregion

    #region OverrideTemplates

    public static readonly DependencyProperty OverrideTemplatesProperty = DependencyProperty.Register(nameof(OverrideTemplates), typeof(KeyTemplateCollection), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public KeyTemplateCollection OverrideTemplates
    {
        get => (KeyTemplateCollection)GetValue(OverrideTemplatesProperty);
        set => SetValue(OverrideTemplatesProperty, value);
    }

    #endregion

    #endregion

    #region [- P]

    #region PlaceholderConverter

    public static readonly DependencyProperty PlaceholderConverterProperty = DependencyProperty.Register(nameof(PlaceholderConverter), typeof(IValueConverter), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public IValueConverter PlaceholderConverter
    {
        get => (IValueConverter)GetValue(PlaceholderConverterProperty);
        set => SetValue(PlaceholderConverterProperty, value);
    }

    #endregion

    #endregion

    #region [- R]

    #region (ReadOnly) Route

    static readonly DependencyPropertyKey RouteKey = DependencyProperty.RegisterReadOnly(nameof(Route), typeof(MemberRoute), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty RouteProperty = RouteKey.DependencyProperty;
    /// <summary>
    /// Stores a reference to every nested property relative to the original object; properties are stored in order of depth.
    /// </summary>
    public MemberRoute Route
    {
        get => (MemberRoute)GetValue(RouteProperty);
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

    #endregion

    #region [- S]

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
    static void OnSortDirectionChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSortDirectionChanged(e.Convert<ListSortDirection>());

    #endregion

    #region SortName

    public static readonly DependencyProperty SortNameProperty = DependencyProperty.Register(nameof(SortName), typeof(MemberSortName), typeof(MemberGrid), new FrameworkPropertyMetadata(MemberSortName.DisplayName, OnSortNameChanged));
    public MemberSortName SortName
    {
        get => (MemberSortName)GetValue(SortNameProperty);
        set => SetValue(SortNameProperty, value);
    }
    static void OnSortNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSortNameChanged(e.Convert<MemberSortName>());

    #endregion

    #region Source

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(MemberGrid), new FrameworkPropertyMetadata(null, OnSourceChanged));
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    static void OnSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSourceChanged(e.Convert());

    #endregion

    #region (ReadOnly) SourceAttribute

    static readonly DependencyPropertyKey SourceAttributeKey = DependencyProperty.RegisterReadOnly(nameof(SourceAttribute), typeof(ViewSourceAttribute), typeof(MemberGrid), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty SourceAttributeProperty = SourceAttributeKey.DependencyProperty;
    public ViewSourceAttribute SourceAttribute
    {
        get => (ViewSourceAttribute)GetValue(SourceAttributeProperty);
        private set => SetValue(SourceAttributeKey, value);
    }

    #endregion

    #region SourceMode

    public static readonly DependencyProperty SourceModeProperty = DependencyProperty.Register(nameof(SourceMode), typeof(SourceModes), typeof(MemberGrid), new FrameworkPropertyMetadata(SourceModes.Single, OnSourceModesChanged));
    public SourceModes SourceMode
    {
        get => (SourceModes)GetValue(SourceModeProperty);
        set => SetValue(SourceModeProperty, value);
    }
    static void OnSourceModesChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnSourceModesChanged(e.Convert<SourceModes>());

    #endregion
    
    #endregion

    #region [- V]

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

    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(View), typeof(MemberView?), typeof(MemberGrid), new FrameworkPropertyMetadata(null, OnViewChanged));
    public MemberView? View
    {
        get => (MemberView?)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }
    static void OnViewChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberGrid>().OnViewChanged(e);

    #endregion

    #endregion

    #endregion

    #region MemberGrid

    public MemberGrid() : base()
    {
        loadTask = new(null, Load, true);
        SortComparer = new(this);

        ///

        Members = new(GroupName, SortComparer);

        ///

        ItemsSource = Members.View;

        ///

        Route = new();

        ///

        SetCurrentValue(HeaderButtonsProperty, 
            new FrameworkElementCollection());
        SetCurrentValue(OverrideTemplatesProperty,
            new KeyTemplateCollection());

        ///

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

    async Task Load(ReadOnlyValue i, CancellationToken token)
    {
        //Source != null
        if (i.New != null)
        {
            MemberSourceFilter sourceFilter = null;
            if (i.New is MemberSourceFilter)
            {
                sourceFilter = i.New as MemberSourceFilter;
                i = new(i.Old, sourceFilter.Source);
            }

            var j = Route.New(i.Old, i.New);
            //i.Old != i.New
            if (j != null)
            {
                if (j.Value is object[] sources)
                {
                    if (sources.Length < 1)
                    {
                        SetCurrentValue(SourceProperty, null);
                        return;
                    }
                }

                IsLoading = true;

                Route.Append(j);
                ActualSource = j.Value;

                var sourceType = ActualSource.GetType().IsArray ? ActualSource.As<object[]>()[0].GetType() : ActualSource.GetType();
                SourceAttribute = sourceType.GetAttribute<ViewSourceAttribute>() ?? new();

                Members.Load(ActualSource, sourceFilter);
                IsLoading = false;
            }
        }

        //Source = null
        else { ActualSource = null; Route.Clear(); Members.Clear(); }
        SourceChanged?.Invoke(this, new EventArgs<object>(i.New));
    }

    ///

    void OnLoaded()
    {
        CanNavigateBack = BackCommand.CanExecute(null);
        Subscribe();
    }

    async void OnUnloaded()
    {
        if (loadTask.Started)
            await loadTask.CancelAsync();

        Unsubscribe();
    }

    ///

    void OnRouteChanged(object sender, NotifyCollectionChangedEventArgs e) => CanNavigateBack = BackCommand.CanExecute(null);

    #endregion

    #region Protected

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        if (newValue != Members.View && newValue != Members.Single.View)
            throw new ExternalChangeException<MemberGrid>(nameof(ItemsSource));
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        var row = e.OriginalSource.FindParent<DataGridRow>();
        row.If(i => ActiveMember = i.DataContext as MemberModel);
    }

    ///

    protected virtual void OnGroupDirectionChanged(ListSortDirection input) => Members.ApplyGroup(GroupName);

    protected virtual void OnGroupDirectionChanged(ReadOnlyValue<ListSortDirection> input) => Members.ApplyGroup(GroupName);
        
    protected virtual void OnGroupNameChanged(ReadOnlyValue<MemberGroupName> input) => Members.ApplyGroup(GroupName);

    protected virtual void OnActualSourceChanged(ReadOnlyValue input) { }

    protected virtual void OnSortDirectionChanged(ReadOnlyValue<ListSortDirection> input) => Members.ApplySort();

    protected virtual void OnSortNameChanged(ReadOnlyValue<MemberSortName> input) => Members.ApplySort();

    protected virtual void OnSourceChanged(ReadOnlyValue input) => _ = loadTask.Start(input);

    protected virtual void OnSourceModesChanged(ReadOnlyValue<SourceModes> input) { }

    protected virtual void OnViewChanged(ReadOnlyValue<MemberView?> input)
    {
        Members.OverrideView(input.New, Log);
        switch (input.New)
        {
            case MemberView.All:
            case MemberView.Tab:
                ItemsSource = Members.View;
                break;

            case MemberView.Single:
                ItemsSource = Members.Single.View;
                break;
        }
    }

    #endregion

    #region Public

    public bool CanRefresh => Members?.IsLoading == false && Members.Source != null;

    public void Refresh() => Members.Refresh();

    ///

    public void Subscribe()
    {
        if (!loadTask.Started)
            Members.Subscribe();

        Route.CollectionChanged += OnRouteChanged;
    }

    public void Unsubscribe()
    {
        Members.Unsubscribe();
        Route.CollectionChanged -= OnRouteChanged;
    }

    #endregion

    #endregion

    #region Commands

    ICommand backCommand;
    public ICommand BackCommand => backCommand
        ??= new RelayCommand(() => EditCommand.Execute(Route.Last<MemberRouteElement>(1)), () => Route.Count > 1);

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

    ICommand newCommand;
    public ICommand NewCommand => newCommand
        ??= new RelayCommand<Reference2>(i =>
        {
            if (i.First is MemberModel j)
            {
                Try.Invoke(() =>
                {
                    if (i.Second is Type n)
                        j.Value = n.Create<object>();

                    else if (i.Second is object m)
                        j.Value = m;
                },
                e => Analytics.Log.Write<MemberGrid>(e));
            }
        },
        i => i?.First is MemberModel j && !j.IsReadOnly);

    ICommand refreshCommand;
    public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(Refresh, () => CanRefresh);

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

    #endregion
}