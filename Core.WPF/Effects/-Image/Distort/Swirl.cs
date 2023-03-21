using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that swirls the input in a spiral.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class SwirlEffect : ImageEffect
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof(Center), typeof(Point), typeof(SwirlEffect), new FrameworkPropertyMetadata(new Point(0.5D, 0.5D), PixelShaderConstantCallback(0)));
    /// <summary>The center point of the spiral. (1,1) is lower right corner</summary>
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public static readonly DependencyProperty SpiralStrengthProperty = DependencyProperty.Register(nameof(SpiralStrength), typeof(double), typeof(SwirlEffect), new FrameworkPropertyMetadata(10.0, PixelShaderConstantCallback(1)));
    /// <summary>The amount of twist to the spiral.</summary>
    public double SpiralStrength
    {
        get => (double)GetValue(SpiralStrengthProperty);
        set => SetValue(SpiralStrengthProperty, value);
    }

    public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(nameof(AspectRatio), typeof(double), typeof(SwirlEffect), new FrameworkPropertyMetadata(1.5, PixelShaderConstantCallback(2)));
    /// <summary>The aspect ratio (width / height) of the input.</summary>
    public double AspectRatio
    {
        get => (double)GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    public SwirlEffect() : base()
    {
        UpdateShaderValue(CenterProperty);
        UpdateShaderValue(SpiralStrengthProperty);
        UpdateShaderValue(AspectRatioProperty);
    }
}