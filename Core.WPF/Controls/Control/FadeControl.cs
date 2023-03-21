using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls;

public class FadeControl : Control
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FadeControl), new FrameworkPropertyMetadata(false));
    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly DependencyProperty Content1Property = DependencyProperty.Register(nameof(Content1), typeof(object), typeof(FadeControl), new FrameworkPropertyMetadata(null));
    public object Content1
    {
        get => (object)GetValue(Content1Property);
        set => SetValue(Content1Property, value);
    }

    public static readonly DependencyProperty Content2Property = DependencyProperty.Register(nameof(Content2), typeof(object), typeof(FadeControl), new FrameworkPropertyMetadata(null));
    public object Content2
    {
        get => (object)GetValue(Content2Property);
        set => SetValue(Content2Property, value);
    }
    
    public FadeControl() : base() { }
}