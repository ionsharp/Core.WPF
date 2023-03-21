using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

[Serializable]
public enum Views
{
    [Image(SmallImages.ViewCarousel)]
    Carousel,
    [Image(SmallImages.ViewDetails)]
    Detail,
    [Image(SmallImages.ViewList)]
    List,
    [Image(SmallImages.ViewThumbnails)]
    Thumb,
    [Image(SmallImages.ViewTiles)]
    Tile,
}

public class ViewControl : Control
{
    public static readonly ResourceKey MaximumToolTipWidth = new();

    public static readonly ResourceKey MinimumToolTipWidth = new();
    
    ///

    public static readonly ResourceKey DefaultStyle = new();
    
    public static readonly ResourceKey CarouselStyleKey = new();

    public static readonly ResourceKey DetailStyleKey = new();

    public static readonly ResourceKey ListStyleKey = new();

    public static readonly ResourceKey ThumbStyleKey = new();

    public static readonly ResourceKey TileStyleKey = new();

    ///

    public static readonly ResourceKey ViewCarouselKey = new();

    public static readonly ResourceKey ViewListKey = new();

    public static readonly ResourceKey ViewThumbKey = new();

    public static readonly ResourceKey ViewTileKey = new();

    ///

    public static readonly DependencyProperty BulletProperty = DependencyProperty.Register(nameof(Bullet), typeof(Bullets), typeof(ViewControl), new FrameworkPropertyMetadata(Bullets.Circle));
    public Bullets Bullet
    {
        get => (Bullets)GetValue(BulletProperty);
        set => SetValue(BulletProperty, value);
    }

    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns), typeof(int), typeof(ViewControl), new FrameworkPropertyMetadata(1));
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public static readonly DependencyProperty DescriptionTemplateProperty = DependencyProperty.Register(nameof(DescriptionTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate DescriptionTemplate
    {
        get => (DataTemplate)GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    public static readonly DependencyProperty DetailNameProperty = DependencyProperty.Register(nameof(DetailName), typeof(string), typeof(ViewControl), new FrameworkPropertyMetadata(""));
    public string DetailName
    {
        get => (string)GetValue(DetailNameProperty);
        set => SetValue(DetailNameProperty, value);
    }

    public static readonly DependencyProperty DetailTemplateProperty = DependencyProperty.Register(nameof(DetailTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate DetailTemplate
    {
        get => (DataTemplate)GetValue(DetailTemplateProperty);
        set => SetValue(DetailTemplateProperty, value);
    }

    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(ViewControl), new FrameworkPropertyMetadata(false));
    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }
    
    public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(nameof(ItemSize), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(128.0, null, OnItemSizeCoerced));
    public double ItemSize
    {
        get => (double)GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }
    static object OnItemSizeCoerced(DependencyObject sender, object input) => sender is ViewControl control && input is double result ? Clamp(result, control.ItemSizeMaximum, control.ItemSizeMinimum) : sender.As<ViewControl>().ItemSizeMinimum;

    public static readonly DependencyProperty ItemSizeMaximumProperty = DependencyProperty.Register(nameof(ItemSizeMaximum), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(512.0, OnItemSizeMaximumChanged));
    public double ItemSizeMaximum
    {
        get => (double)GetValue(ItemSizeMaximumProperty);
        set => SetValue(ItemSizeMaximumProperty, value);
    }
    static void OnItemSizeMaximumChanged(object sender, DependencyPropertyChangedEventArgs e) => sender.As<ViewControl>().OnItemSizeMaximumChanged(e);
    void OnItemSizeMaximumChanged(ReadOnlyValue<double> input) => InvalidateProperty(ItemSizeProperty);

    public static readonly DependencyProperty ItemSizeMinimumProperty = DependencyProperty.Register(nameof(ItemSizeMinimum), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(16.0, OnItemSizeMinimumChanged));
    public double ItemSizeMinimum
    {
        get => (double)GetValue(ItemSizeMinimumProperty);
        set => SetValue(ItemSizeMinimumProperty, value);
    }
    static void OnItemSizeMinimumChanged(object sender, DependencyPropertyChangedEventArgs e) => sender.As<ViewControl>().OnItemSizeMinimumChanged(e);
    void OnItemSizeMinimumChanged(ReadOnlyValue<double> input) => InvalidateProperty(ItemSizeProperty);

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    ///

    public static readonly DependencyProperty ItemToolTipTemplateProperty = DependencyProperty.Register(nameof(ItemToolTipTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemToolTipTemplate
    {
        get => (DataTemplate)GetValue(ItemToolTipTemplateProperty);
        set => SetValue(ItemToolTipTemplateProperty, value);
    }

    public static readonly DependencyProperty ItemToolTipHeaderTemplateProperty = DependencyProperty.Register(nameof(ItemToolTipHeaderTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemToolTipHeaderTemplate
    {
        get => (DataTemplate)GetValue(ItemToolTipHeaderTemplateProperty);
        set => SetValue(ItemToolTipHeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty ItemToolTipHeaderIconTemplateProperty = DependencyProperty.Register(nameof(ItemToolTipHeaderIconTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemToolTipHeaderIconTemplate
    {
        get => (DataTemplate)GetValue(ItemToolTipHeaderIconTemplateProperty);
        set => SetValue(ItemToolTipHeaderIconTemplateProperty, value);
    }

    ///

    public static readonly DependencyProperty NameTemplateProperty = DependencyProperty.Register(nameof(NameTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate NameTemplate
    {
        get => (DataTemplate)GetValue(NameTemplateProperty);
        set => SetValue(NameTemplateProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ViewControl), new FrameworkPropertyMetadata(Orientation.Horizontal));
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty PanelProperty = DependencyProperty.Register(nameof(Panel), typeof(object), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public object Panel
    {
        get => GetValue(PanelProperty);
        set => SetValue(PanelProperty, value);
    }

    public static readonly DependencyProperty SelectedGroupProperty = DependencyProperty.Register(nameof(SelectedGroup), typeof(object), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public object SelectedGroup
    {
        get => GetValue(SelectedGroupProperty);
        set => SetValue(SelectedGroupProperty, value);
    }

    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ViewControl), new FrameworkPropertyMetadata(-1));
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register(nameof(TileSize), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(300.0, null, OnTileSizeCoerced));
    public double TileSize
    {
        get => (double)GetValue(TileSizeProperty);
        set => SetValue(TileSizeProperty, value);
    }
    static object OnTileSizeCoerced(DependencyObject sender, object input) => sender is ViewControl control && input is double result ? Clamp(result, control.TileSizeMaximum, control.TileSizeMinimum) : sender.As<ViewControl>().TileSizeMinimum;

    public static readonly DependencyProperty TileSizeMaximumProperty = DependencyProperty.Register(nameof(TileSizeMaximum), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(512.0, OnTileSizeMaximumChanged));
    public double TileSizeMaximum
    {
        get => (double)GetValue(TileSizeMaximumProperty);
        set => SetValue(TileSizeMaximumProperty, value);
    }
    static void OnTileSizeMaximumChanged(object sender, DependencyPropertyChangedEventArgs e) => sender.As<ViewControl>().OnTileSizeMaximumChanged(e);
    void OnTileSizeMaximumChanged(ReadOnlyValue<double> input) => InvalidateProperty(TileSizeProperty);

    public static readonly DependencyProperty TileSizeMinimumProperty = DependencyProperty.Register(nameof(TileSizeMinimum), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(128.0, OnTileSizeMinimumChanged));
    public double TileSizeMinimum
    {
        get => (double)GetValue(TileSizeMinimumProperty);
        set => SetValue(TileSizeMinimumProperty, value);
    }
    static void OnTileSizeMinimumChanged(object sender, DependencyPropertyChangedEventArgs e) => sender.As<ViewControl>().OnTileSizeMinimumChanged(e);
    void OnTileSizeMinimumChanged(ReadOnlyValue<double> input) => InvalidateProperty(TileSizeProperty);

    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(View), typeof(Views), typeof(ViewControl), new FrameworkPropertyMetadata(Views.Thumb));
    public Views View
    {
        get => (Views)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    ///

    ICommand editCommand;
    public ICommand EditCommand => editCommand ??= new RelayCommand<object>(i => IsEditing = $"{i}" == "1", i => i != null);
}

public class GroupControl : ViewControl { }