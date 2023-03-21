using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Name("Threshold"), Explicit]
public class ThresholdEffect : BaseBlendEffect
{
    public static readonly DependencyProperty Color1Property = DependencyProperty.Register("Color1", typeof(Color), typeof(ThresholdEffect), new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 0, 0), PixelShaderConstantCallback(0)));
    [Show]
    public Color Color1
    {
        get => (Color)GetValue(Color1Property);
        set => SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Color2Property = DependencyProperty.Register("Color2", typeof(Color), typeof(ThresholdEffect), new FrameworkPropertyMetadata(Color.FromArgb(255, 1, 1, 1), PixelShaderConstantCallback(1)));
    [Show]
    public Color Color2
    {
        get => (Color)GetValue(Color2Property);
        set => SetValue(Color2Property, value);
    }

    public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(double), typeof(ThresholdEffect), new FrameworkPropertyMetadata(100.0, PixelShaderConstantCallback(2)));
    [Show]
    [Range(1.0, 255.0, 1.0, Style = RangeStyle.Both)]
    public double Level
    {
        get => (double)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public ThresholdEffect() : base()
    {
        UpdateShaderValue(Color1Property);
        UpdateShaderValue(Color2Property);
        UpdateShaderValue(LevelProperty);
    }

    public override Color Apply(Color color, double amount = 1)
    {
        var brightness = color.GetBrightness();
        return brightness > Level.Double() / 255.0 ? Color1 : Color2;
    }
}