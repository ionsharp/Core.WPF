using System;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls;

public class ViewControl : Control
{
    [Serializable]
    public enum Views
    {
        Grid, List
    }
    
    public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(nameof(ItemSize), typeof(double), typeof(ViewControl), new FrameworkPropertyMetadata(128.0));
    public double ItemSize
    {
        get => (double)GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ViewControl), new FrameworkPropertyMetadata(null));
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(View), typeof(Views), typeof(ViewControl), new FrameworkPropertyMetadata(Views.List));
    public Views View
    {
        get => (Views)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }
}