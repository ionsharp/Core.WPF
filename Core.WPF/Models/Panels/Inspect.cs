using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Models;

[Flags]
public enum ElementTypes
{
    [Hide]
    None = 0,
    Logical = 1,
    Visual = 2,
    [Hide]
    Both = Logical | Visual
}

public class ElementViewModel : ViewModel<FrameworkElement>
{
    public readonly ElementTypes ElementType; 

    public bool IsExpanded { get => Get(false); set => Set(value); }

    public bool IsSelected { get => Get(false); set => Set(value); }
    
    public ObservableCollection<ElementViewModel> Items { get => Get<ObservableCollection<ElementViewModel>>(new()); set => Set(value); }

    public string Name { get => Get(""); set => Set(value); }

    public readonly InspectPanel Panel;

    public ElementViewModel(InspectPanel panel, FrameworkElement view, ElementTypes elementType) : base(view) 
    {
        Panel = panel; Name = view.GetType().Name; ElementType = elementType;
    }

    void UpdateItems() 
    {
        Items.Clear();
        foreach (var i in View.FindLogicalChildren<FrameworkElement>(false))
            Items.Add(new(Panel, i, ElementTypes.Logical));

        foreach (var i in View.FindVisualChildren<FrameworkElement>(false))
            Items.Add(new(Panel, i, ElementTypes.Visual));
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(IsExpanded))
        {
            if ((bool)e.OldValue == false && (bool)e.NewValue == true)
                UpdateItems();
        }
        if (e.PropertyName == nameof(IsSelected))
        {
            if (IsSelected)
                Panel.SelectedItem = View;
        }
    }
}

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class InspectPanelVisibilityConverter : MultiConverter<Visibility>
{
    public InspectPanelVisibilityConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 2)
        {
            if (values[0] is ElementViewModel element)
            {
                if (values[1] is ElementTypes types)
                {
                    if (types == ElementTypes.Both)
                        return Visibility.Visible;

                    if (types == ElementTypes.None)
                        return Visibility.Collapsed;

                    if (types.HasFlag(ElementTypes.Logical))
                        return (element.ElementType == ElementTypes.Logical).Visibility();

                    if (types.HasFlag(ElementTypes.Visual))
                        return (element.ElementType == ElementTypes.Visual).Visibility();

                    return Visibility.Collapsed;
                }
            }
        }
        return Binding.DoNothing;
    }
}

[Name("Inspect"), Image(SmallImages.Tree), Serializable]
public class InspectPanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    [Hide]
    public object SelectedItem { get => Get<object>(); set => Set(value); }

    [Hide]
    public ObservableCollection<ElementViewModel> Source { get => Get<ObservableCollection<ElementViewModel>>(new()); set => Set(value); }

    [Header, HideName, Style(EnumStyle.Flags)]
    public ElementTypes Types { get => Get(ElementTypes.Both); set => Set(value); }

    public InspectPanel() : base() { }
}