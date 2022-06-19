using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls;

public class ColorSelectorView : Control
{
    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(nameof(Document), typeof(ColorDocument), typeof(ColorSelectorView), new FrameworkPropertyMetadata(null));
    public ColorDocument Document
    {
        get => (ColorDocument)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    public ColorSelectorView() : base() { }
}