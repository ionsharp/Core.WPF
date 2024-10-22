﻿using Imagin.Core.Analytics;
using Imagin.Core.Data;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

public class CarouselVisibilityBinding : MultiBind
{
    public CarouselVisibilityBinding() : base()
    {
        Converter = CarouselControl.VisibilityConverter;
        Bindings.Add(new Binding("."));
        Bindings.Add(new Ancestor("ItemsSource", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor("ItemsSource.Count", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor("Columns", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor("Index", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor("SortDirection", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor("SortName", 
            typeof(CarouselControl)));
        Bindings.Add(new Ancestor(typeof(CarouselControl)));
    }
}

public class CarouselControl : Control
{
    public static readonly ReferenceKey<ListBox> ListBoxKey = new();

    #region (IMultiValueConverter) ColumnConverter

    public static readonly IMultiValueConverter ColumnConverter = new MultiConverter<int>(i =>
    {
        if (i.Values?.Length == 2)
        {
            if (i.Values[0] is int columns)
            {
                if (i.Values[1] is int count)
                    return count < columns ? count : columns;
            }
        }
        return 0;
    });

    #endregion

    #region (IMultiValueConverter) VisibilityConverter

    public static readonly IMultiValueConverter VisibilityConverter = new MultiConverter<Visibility>(i =>
    {
        if (i.Values?.Length == 8)
        {
            if (i.Values[0] is object item)
            {
                if (i.Values[1] is IList itemsSource)
                {
                    if (i.Values[2] is int count)
                    {
                        if (i.Values[3] is int columns)
                        {
                            columns = columns > count ? count : columns;

                            if (columns == 0)
                                return Visibility.Collapsed;

                            if (i.Values[4] is int index)
                            {
                                if (i.Values[5] is ListSortDirection sortDirection)
                                {
                                    if (i.Values[6] == null || i.Values[6] is object sortName)
                                    {
                                        if (i.Values[7] is CarouselControl carousel)
                                        {
                                            var listBox = carousel.GetChild<ListBox>(ListBoxKey);
                                            if (listBox != null)
                                            {
                                                DependencyObject container = null;
                                                Try.Invoke(() => container = listBox.ItemContainerGenerator.ContainerFromItem(item), e => Log.Write<CarouselControl>(e));

                                                if (container != null)
                                                {
                                                    var actualIndex = listBox.ItemContainerGenerator.IndexFromContainer(container);
                                                    if (actualIndex >= index)
                                                    {
                                                        if (actualIndex < index + columns)
                                                            return Visibility.Visible;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return Visibility.Collapsed;
    });

    #endregion

    #region Properties

    public int ActualColumns 
        => ActualItemsSource?.Count < Columns ? ActualItemsSource.Count : Columns;

    public IList ActualItemsSource 
        => ItemsSource as IList;

    int Limit 
        => ActualItemsSource?.Count - ActualColumns ?? 0;

    ///

    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns), typeof(int), typeof(CarouselControl), new FrameworkPropertyMetadata(7, null, OnColumnsCoerced));
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }
    static object OnColumnsCoerced(DependencyObject i, object value) => i.As<CarouselControl>().OnColumnsCoerced((int)value);

    public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(CarouselControl), new FrameworkPropertyMetadata(0));
    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public Style ItemContainerStyle
    {
        get => (Style)GetValue(ItemContainerStyleProperty);
        set => SetValue(ItemContainerStyleProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty LeftButtonTemplateProperty = DependencyProperty.Register(nameof(LeftButtonTemplate), typeof(DataTemplate), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public DataTemplate LeftButtonTemplate
    {
        get => (DataTemplate)GetValue(LeftButtonTemplateProperty);
        set => SetValue(LeftButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty RightButtonTemplateProperty = DependencyProperty.Register(nameof(RightButtonTemplate), typeof(DataTemplate), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public DataTemplate RightButtonTemplate
    {
        get => (DataTemplate)GetValue(RightButtonTemplateProperty);
        set => SetValue(RightButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(CarouselControl), new FrameworkPropertyMetadata(-1));
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(System.Windows.Controls.SelectionMode), typeof(CarouselControl), new FrameworkPropertyMetadata(System.Windows.Controls.SelectionMode.Single));
    public System.Windows.Controls.SelectionMode SelectionMode
    {
        get => (System.Windows.Controls.SelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(nameof(SortDirection), typeof(ListSortDirection), typeof(CarouselControl), new FrameworkPropertyMetadata(ListSortDirection.Ascending));
    public ListSortDirection SortDirection
    {
        get => (ListSortDirection)GetValue(SortDirectionProperty);
        set => SetValue(SortDirectionProperty, value);
    }

    public static readonly DependencyProperty SortNameProperty = DependencyProperty.Register(nameof(SortName), typeof(object), typeof(CarouselControl), new FrameworkPropertyMetadata(null));
    public object SortName
    {
        get => GetValue(SortNameProperty);
        set => SetValue(SortNameProperty, value);
    }

    public static readonly DependencyProperty WrapProperty = DependencyProperty.Register(nameof(Wrap), typeof(bool), typeof(CarouselControl), new FrameworkPropertyMetadata(true));
    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    #endregion

    #region CarouselControl

    public CarouselControl() : base() { }

    #endregion

    #region Methods

    void Move(LeftRight direction)
    {
        switch (direction)
        {
            case LeftRight.Left:
                Index--;
                Index = Index < 0 ? (!Wrap ? 0 : Limit) : Index;
                break;
            case LeftRight.Right:
                Index++;
                Index = Index > Limit ? (!Wrap ? Limit : 0) : Index;
                break;
        }
    }

    protected virtual object OnColumnsCoerced(int columns) => Clamp(columns, int.MaxValue, 1);

    #endregion

    #region Commands

    ICommand nextCommand;
    public ICommand NextCommand => nextCommand ??= new RelayCommand(() => Move(LeftRight.Right), () => !Wrap ? Index < Limit : true);

    ICommand previousCommand;
    public ICommand PreviousCommand => previousCommand ??= new RelayCommand(() => Move(LeftRight.Left), () => !Wrap ? Index > 0 : true);

    #endregion
}