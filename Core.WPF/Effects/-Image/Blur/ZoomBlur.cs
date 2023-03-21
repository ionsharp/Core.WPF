using System.Windows;

namespace Imagin.Core.Effects;

/// <summary>An effect that applies a radial blur to the input.</summary>
[Category(ImageEffectCategory.Blur), Name("Zoom blur"), Explicit]
public class ZoomBlurEffect : ImageEffect
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(ZoomBlurEffect), new FrameworkPropertyMetadata(new Point(0.9D, 0.6D), PixelShaderConstantCallback(0)));
    /// <summary>The center of the blur.</summary>
    [Show]
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }
        
    /// <summary>The amount of blur.</summary>
    [Range(0.0, 0.2, 0.01, Style = RangeStyle.Both), Show]
    public override double Amount { get => base.Amount; set => base.Amount = value; }

    public ZoomBlurEffect() : base()
    {
        UpdateShaderValue(CenterProperty);
    }
}