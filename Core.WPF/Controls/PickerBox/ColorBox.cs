using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public class ColorBox : PickerBox<Color>
{
    protected override Color DefaultValue => System.Windows.Media.Colors.Transparent;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Color), typeof(ColorBox), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Transparent, OnValueChanged));
    public Color Value
    {
        get => (Color)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    static void OnValueChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ColorBox>().OnValueChanged(e);

    public ColorBox() : base() { }

    protected override Color GetValue() => Value;

    protected override void SetValue(Color i) => Value = i;

    Color NewColor;

    public override void ShowDialog()
    {
        Dialog.ShowColor(Title, GetValue(), out NewColor, i =>
        {
            if (i == 0)
                SetValue(NewColor);
        });
    }
}