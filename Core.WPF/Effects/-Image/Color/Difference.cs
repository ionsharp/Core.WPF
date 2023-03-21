using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Explicit, Name("Difference")]
public class DifferenceEffect : BaseBlendEffect
{
    public static readonly DependencyProperty RedProperty = DependencyProperty.Register("Red", typeof(double), typeof(DifferenceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(0)));
    [Range(.0, 255.0, 1.0, Style = RangeStyle.Both), Show]
    public double Red
    {
        get => (double)GetValue(RedProperty);
        set => SetValue(RedProperty, value);
    }

    public static readonly DependencyProperty GreenProperty = DependencyProperty.Register("Green", typeof(double), typeof(DifferenceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(1)));
    [Range(.0, 255.0, 1.0, Style = RangeStyle.Both), Show]
    public double Green
    {
        get => (double)GetValue(GreenProperty);
        set => SetValue(GreenProperty, value);
    }

    public static readonly DependencyProperty BlueProperty = DependencyProperty.Register("Blue", typeof(double), typeof(DifferenceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(2)));
    [Range(.0, 255.0, 1.0, Style = RangeStyle.Both), Show]
    public double Blue
    {
        get => (double)GetValue(BlueProperty);
        set => SetValue(BlueProperty, value);
    }

    public DifferenceEffect() : base()
    {
        UpdateShaderValue(RedProperty);
        UpdateShaderValue(GreenProperty);
        UpdateShaderValue(BlueProperty);
    }

    public override Color Apply(Color color, double amount = 1)
    {
        int ob = color.B, og = color.G, or = color.R;
        int nr = or, ng = og, nb = ob;

        nr = Red > or
            ? Red.Int32() - or
            : or - Red.Int32();
        ng = Green > og
            ? Green.Int32() - og
            : og - Green.Int32();
        nb = Blue > ob
            ? Blue.Int32() - ob
            : ob - Blue.Int32();

        return Color.FromArgb(color.A, Clamp(nr, 255).Byte(), Clamp(ng, 255).Byte(), Clamp(nb, 255).Byte());
    }
}