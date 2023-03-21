using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that magnifies a circular region with a smooth boundary.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class MagnifyEffect : ImageEffect
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof(Center), typeof(Point), typeof(MagnifyEffect), new FrameworkPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));
    /// <summary>The center point of the magnified region.</summary>
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(nameof(InnerRadius), typeof(double), typeof(MagnifyEffect), new FrameworkPropertyMetadata(0.2, PixelShaderConstantCallback(1)));
    /// <summary>The inner radius of the magnified region.</summary>
    public double InnerRadius
    {
        get => (double)GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(nameof(OuterRadius), typeof(double), typeof(MagnifyEffect), new FrameworkPropertyMetadata(0.4, PixelShaderConstantCallback(2)));
    /// <summary>The outer radius of the magnified region.</summary>
    public double OuterRadius
    {
        get => (double)GetValue(OuterRadiusProperty);
        set => SetValue(OuterRadiusProperty, value);
    }

    public static readonly DependencyProperty MagnificationProperty = DependencyProperty.Register(nameof(Magnification), typeof(double), typeof(MagnifyEffect), new FrameworkPropertyMetadata(2.0, PixelShaderConstantCallback(3)));
    /// <summary>The magnification factor.</summary>
    public double Magnification
    {
        get => (double)GetValue(MagnificationProperty);
        set => SetValue(MagnificationProperty, value);
    }

    public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(nameof(AspectRatio), typeof(double), typeof(MagnifyEffect), new FrameworkPropertyMetadata(1.5, PixelShaderConstantCallback(4)));
    /// <summary>The aspect ratio (width / height) of the input.</summary>
    public double AspectRatio
    {
        get => (double)GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    public MagnifyEffect() : base()
    {
        UpdateShaderValue(CenterProperty);
        UpdateShaderValue(InnerRadiusProperty);
        UpdateShaderValue(OuterRadiusProperty);
        UpdateShaderValue(MagnificationProperty);
        UpdateShaderValue(AspectRatioProperty);
    }
}