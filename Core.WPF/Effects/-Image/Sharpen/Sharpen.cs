using System.Windows;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Sharpen))]
public class SharpenEffect : ImageEffect
{
    new public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(nameof(Amount), typeof(double), typeof(SharpenEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(Size), typeof(SharpenEffect), new FrameworkPropertyMetadata(new Size(800D, 600D), PixelShaderConstantCallback(1)));
    public Size Size
    {
        get => (Size)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public SharpenEffect() : base()
    {
        UpdateShaderValue(AmountProperty);
        UpdateShaderValue(SizeProperty);
    }
}