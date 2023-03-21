using System.Windows;

namespace Imagin.Core.Effects;

/// <summary>A gaussian blur effect.</summary>
[Category(ImageEffectCategory.Blur), Name("Gaussian blur"), Explicit]
public class GaussianBlurEffect : ImageEffect
{
    public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(nameof(Angle), typeof(double), typeof(GaussianBlurEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(1)));
    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public GaussianBlurEffect() : base()
    {
        UpdateShaderValue(AngleProperty);
        UpdateShaderValue(AmountProperty);
    }
}