using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that superimposes rippling waves upon the input.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class RippleEffect : ImageEffect
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof(Center), typeof(Point), typeof(RippleEffect), new FrameworkPropertyMetadata(new Point(0.5D, 0.5D), PixelShaderConstantCallback(0)));
    /// <summary>The center point of the ripples.</summary>
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public static readonly DependencyProperty AmplitudeProperty = DependencyProperty.Register(nameof(Amplitude), typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(0.1, PixelShaderConstantCallback(1)));
    /// <summary>The amplitude of the ripples.</summary>
    public double Amplitude
    {
        get => (double)GetValue(AmplitudeProperty);
        set => SetValue(AmplitudeProperty, value);
    }

    public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register(nameof(Frequency), typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(70.0, PixelShaderConstantCallback(2)));
    /// <summary>The frequency of the ripples.</summary>
    public double Frequency
    {
        get => (double)GetValue(FrequencyProperty);
        set => SetValue(FrequencyProperty, value);
    }

    public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(nameof(Phase), typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(3)));
    /// <summary>The phase of the ripples.</summary>
    public double Phase
    {
        get => (double)GetValue(PhaseProperty);
        set => SetValue(PhaseProperty, value);
    }

    public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(nameof(AspectRatio), typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(1.5, PixelShaderConstantCallback(4)));
    /// <summary>The aspect ratio (width / height) of the input.</summary>
    public double AspectRatio
    {
        get => (double)GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    public RippleEffect() : base()
    {
        UpdateShaderValue(CenterProperty);
        UpdateShaderValue(AmplitudeProperty);
        UpdateShaderValue(FrequencyProperty);
        UpdateShaderValue(PhaseProperty);
        UpdateShaderValue(AspectRatioProperty);
    }
}