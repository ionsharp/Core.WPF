using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Name("Shading"), Explicit]
public class ShadingEffect : BaseBlendEffect
{
    public static readonly DependencyProperty RedProperty = DependencyProperty.Register("Red", typeof(double), typeof(ShadingEffect), new FrameworkPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(0)));
    [Show]
    [Range(0.0, 100.0, 1.0, Style = RangeStyle.Both)]
    public double Red
    {
        get => (double)GetValue(RedProperty);
        set => SetValue(RedProperty, value);
    }

    public static readonly DependencyProperty GreenProperty = DependencyProperty.Register("Green", typeof(double), typeof(ShadingEffect), new FrameworkPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(1)));
    [Show]
    [Range(0.0, 100.0, 1.0, Style = RangeStyle.Both)]
    public double Green
    {
        get => (double)GetValue(GreenProperty);
        set => SetValue(GreenProperty, value);
    }

    public static readonly DependencyProperty BlueProperty = DependencyProperty.Register("Blue", typeof(double), typeof(ShadingEffect), new FrameworkPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(2)));
    [Show]
    [Range(0.0, 100.0, 1.0, Style = RangeStyle.Both)]
    public double Blue
    {
        get => (double)GetValue(BlueProperty);
        set => SetValue(BlueProperty, value);
    }

    public ShadingEffect() : base()
    {
        UpdateShaderValue(RedProperty);
        UpdateShaderValue(GreenProperty);
        UpdateShaderValue(BlueProperty);
    }

    public ShadingEffect(double red, double green, double blue) : this()
    {
        SetCurrentValue(RedProperty, red);
        SetCurrentValue(GreenProperty, green);
        SetCurrentValue(BlueProperty, blue);
    }

    public override Color Apply(Color color, double amount = 1)
    {
        var r = Clamp((color.R.Double() * (Red.Double() / 100.0)).Round().Int32(), 255);
        var g = Clamp((color.G.Double() * (Green.Double() / 100.0)).Round().Int32(), 255);
        var b = Clamp((color.B.Double() * (Blue.Double() / 100.0)).Round().Int32(), 255);
        return Color.FromArgb(color.A, r.Byte(), g.Byte(), b.Byte());
    }
}