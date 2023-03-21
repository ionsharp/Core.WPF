using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that applies a wave pattern to the input.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class WavesEffect : ImageEffect
{
    public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(double), typeof(WavesEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    public double Time
    {
        get => (double)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }
        
    public static readonly DependencyProperty WaveSizeProperty = DependencyProperty.Register(nameof(WaveSize), typeof(double), typeof(WavesEffect), new FrameworkPropertyMetadata(64.0, PixelShaderConstantCallback(1)));
    /// <summary>The distance between waves. (the higher the value the closer the waves are to their neighbor).</summary>
    public double WaveSize
    {
        get => (double)GetValue(WaveSizeProperty);
        set => SetValue(WaveSizeProperty, value);
    }

    public WavesEffect() : base()
    {
        UpdateShaderValue(TimeProperty);
        UpdateShaderValue(WaveSizeProperty);
    }
}