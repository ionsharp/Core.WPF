using System.Windows;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Stroke))]
public class EmbossEffect : ImageEffect
{
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof(Width), typeof(double), typeof(EmbossEffect), new FrameworkPropertyMetadata(0.003, PixelShaderConstantCallback(1)));
    /// <summary>The separation between samples (as a fraction of input size).</summary>
    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public EmbossEffect() : base()
    {
        SetCurrentValue(AmountProperty, 0.5);
        UpdateShaderValue(AmountProperty); UpdateShaderValue(WidthProperty);
    }
}