using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public class PatternElement : Control
{
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(PatternElement), new FrameworkPropertyMetadata(Stretch.Fill));
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public PatternElement() : base() { }
}