using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that pinches a circular region.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class PinchEffect : ImageEffect
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof(Center), typeof(Point), typeof(PinchEffect), new FrameworkPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));
    /// <summary>The center point of the pinched region.</summary>
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof(Radius), typeof(double), typeof(PinchEffect), new FrameworkPropertyMetadata(0.25, PixelShaderConstantCallback(1)));
    /// <summary>The radius of the pinched region.</summary>
    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(nameof(Strength), typeof(double), typeof(PinchEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(2)));
    /// <summary>The strength of the pinch effect.</summary>
    public double Strength
    {
        get => (double)GetValue(StrengthProperty);
        set => SetValue(StrengthProperty, value);
    }

    public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(nameof(AspectRatio), typeof(double), typeof(PinchEffect), new FrameworkPropertyMetadata(1.5, PixelShaderConstantCallback(3)));
    /// <summary>The aspect ratio (width / height) of the input.</summary>
    public double AspectRatio
    {
        get => (double)GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    public PinchEffect() : base()
    {
        UpdateShaderValue(CenterProperty);
        UpdateShaderValue(RadiusProperty);
        UpdateShaderValue(StrengthProperty);
        UpdateShaderValue(AspectRatioProperty);
    }
}