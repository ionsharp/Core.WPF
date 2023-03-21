using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Imagin.Core.Behavior;

public class ReferenceBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(ReferenceBehavior), new FrameworkPropertyMetadata(null, OnPropertyNameChanged));
    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }
    static void OnPropertyNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ReferenceBehavior behavior)
            behavior.Update(behavior.AssociatedObject);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ReferenceBehavior), new FrameworkPropertyMetadata(null, OnSourceChanged));
    public object Source
    {
        get => (object)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ReferenceBehavior behavior)
            behavior.Update(behavior.AssociatedObject);
    }

    void Update(FrameworkElement input) => Try.Invoke(() => Source?.SetPropertyValue(PropertyName, input));

    protected override void OnAttached()
    {
        base.OnAttached();
        Update(AssociatedObject);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        Update(null);
    }
}