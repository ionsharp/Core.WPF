using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Name("Quantify"), Explicit]
public class QuantifyEffect : BaseBlendEffect
{
    enum Categories { Size }

    public static readonly DependencyProperty SizeXProperty = DependencyProperty.Register(nameof(SizeX), typeof(double), typeof(QuantifyEffect), new FrameworkPropertyMetadata(10d, PixelShaderConstantCallback(0)));
    [Category(Categories.Size), Range(0.0, 1000.0, 1.0, Style = RangeStyle.Both), Show]
    public double SizeX
    {
        get => (double)GetValue(SizeXProperty);
        set => SetValue(SizeXProperty, value);
    }

    public static readonly DependencyProperty SizeYProperty = DependencyProperty.Register(nameof(SizeY), typeof(double), typeof(QuantifyEffect), new FrameworkPropertyMetadata(10d, PixelShaderConstantCallback(1)));
    [Category(Categories.Size), Range(0.0, 1000.0, 1.0, Style = RangeStyle.Both), Show]
    public double SizeY
    {
        get => (double)GetValue(SizeYProperty);
        set => SetValue(SizeYProperty, value);
    }

    public static readonly DependencyProperty SizeZProperty = DependencyProperty.Register(nameof(SizeZ), typeof(double), typeof(QuantifyEffect), new FrameworkPropertyMetadata(10d, PixelShaderConstantCallback(2)));
    [Category(Categories.Size), Range(0.0, 1000.0, 1.0, Style = RangeStyle.Both), Show]
    public double SizeZ
    {
        get => (double)GetValue(SizeZProperty);
        set => SetValue(SizeZProperty, value);
    }

    public QuantifyEffect() : base()
    {
        UpdateShaderValue(SizeXProperty);
        UpdateShaderValue(SizeYProperty);
        UpdateShaderValue(SizeZProperty);
    }

    public override Color Apply(Color color, double amount = 1)
    {
        double r = Normalize(color.R), g = Normalize(color.G), b = Normalize(color.B); byte a = color.A;
        r *= SizeX; r = r.Round(); r /= SizeX;
        g *= SizeY; g = g.Round(); g /= SizeY;
        b *= SizeZ; b = b.Round(); b /= SizeZ;
        return Color.FromArgb(a, Denormalize(r), Denormalize(g), Denormalize(b));
    }
}