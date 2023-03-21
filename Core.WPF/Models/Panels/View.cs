using Imagin.Core.Controls;
using Imagin.Core.Linq;
using Imagin.Core.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Imagin.Core.Models;

[Serializable]
public abstract class ViewPanel : Panel, IElementReference
{
    public static ReferenceKey<Selector> SelectorKey = new();

    enum Category { Show, Sort, View }

    [Category(nameof(Category.View)), Description("The type of bullet."), Index(0), Option, Show, VisibilityTrigger(nameof(View), Views.Detail)]
    public Bullets Bullet { get => Get(Bullets.Circle); set => Set(value); }

    [Category(nameof(Category.View)), Description("The number of columns."), Index(0), Option, Range(1, 16, 1, Style = RangeStyle.Both), Show, VisibilityTrigger(nameof(View), Views.Carousel)]
    public int Columns { get => Get(7); set => Set(value); }

    [Category(nameof(Category.View)), Description("The size of items."), Name("Item size"), Index(1), Option, Range(32.0, 512.0, 4.0, Style = RangeStyle.Both), Show]
    public virtual double ItemSize { get => Get(128.0); set => Set(value); }

    [Hide]
    public virtual string ItemName => "item";

    [Category(nameof(Category.View)), Description("The direction to arrange items."), Index(-1), Option, Show]
    public Orientation Orientation { get => Get(Orientation.Horizontal); set => Set(value); }

    [Hide]
    public int SelectedIndex { get => Get(-1); set => Set(value); }

    [Hide]
    public object[] SelectedItems { get => Get<object[]>(); set => Set(value); }

    List<object> selection = new();

    [Category(Category.Sort), Name("Direction"), Option, Show]
    public ListSortDirection SortDirection { get => Get(ListSortDirection.Descending); set => Set(value); }

    [Category(Category.Sort), Name("Name"), Option, Show]
    [Int32Style(Int32Style.Index, nameof(SortNames))]
    public virtual int SortNameIndex { get => Get(0); set => Set(value); }

    [Hide]
    public abstract IList SortNames { get; }

    [Category(nameof(Category.View)), Description("The size of tiles."), Name("Tile size"), Index(2), Option, Range(32.0, 512.0, 1, Style = RangeStyle.Both), Show]
    public virtual double TileSize { get => Get(300.0); set => Set(value); }

    [Category(nameof(Category.View)), Description("The type of view."), Index(-1), Option, Show]
    public Views View { get => Get(Views.Thumb); set => Set(value); }

    [Category(Category.Show), Name("Description"), Option, Show]
    public bool ViewDescription { get => Get(false); set => Set(value); }

    [Category(Category.Show), Name("Detail"), Option, Show]
    public bool ViewDetail { get => Get(false); set => Set(value); }

    [Category(Category.Show), Name("Name"), Option, Show]
    public bool ViewName { get => Get(true); set => Set(value); }

    void IElementReference.SetReference(IElementKey key, FrameworkElement element)
    {
        if (key == SelectorKey)
        {
            if (element is Selector selector)
            {
                selector.SelectionChanged -= OnSelectionChanged;
                selector.SelectionChanged += OnSelectionChanged;

                selector.Unloaded -= OnSelectorUnloaded; 
                selector.Unloaded += OnSelectorUnloaded;
            }
        }
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        e.AddedItems?.ForEach(i => selection.Add(i));
        e.RemovedItems?.ForEach(i => selection.Remove(i));

        SelectedItems = selection.Count == 0 ? null : selection.ToArray();
    }

    void OnSelectorUnloaded(object sender, RoutedEventArgs e)
    {
        sender.If<Selector>(i => { i.SelectionChanged -= OnSelectionChanged; i.Unloaded -= OnSelectorUnloaded; });
        SelectedItems = null;
    }
}