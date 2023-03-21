using System.Windows;

namespace Imagin.Core.Effects;

/// <summary>An effect that blurs in a single direction.</summary>
[Category(ImageEffectCategory.Blur), Name("Directional blur"), Explicit]
public class DirectionalBlurEffect : ImageEffect
{
    /// <summary>The scale of the blur (as a fraction of the input size).</summary>
    [Range(0.0, 0.01, 0.001, Style = RangeStyle.Both), Show]
    public override double Amount { get => base.Amount; set => base.Amount = value; }

    public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(DirectionalBlurEffect), new FrameworkPropertyMetadata(.0, PixelShaderConstantCallback(1)));
    /// <summary>The direction of the blur (in degrees).</summary>
    [Range(0.0, 359.0, 1.0, Style = RangeStyle.Both), Show]
    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public DirectionalBlurEffect() : base()
    {
        UpdateShaderValue(AngleProperty);
    }
}