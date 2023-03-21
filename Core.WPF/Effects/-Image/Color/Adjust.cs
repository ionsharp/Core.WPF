using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Name("Adjust"), Explicit]
public class AdjustEffect : BaseBlendEffect
{
    new public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(nameof(Amount), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(1d, PixelShaderConstantCallback(9)));

    public static readonly DependencyProperty X0Property = DependencyProperty.Register(nameof(X0), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.393, PixelShaderConstantCallback(0)));
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double X0
    {
        get => (double)GetValue(X0Property);
        set => SetValue(X0Property, value);
    }

    public static readonly DependencyProperty Y0Property = DependencyProperty.Register(nameof(Y0), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.769, PixelShaderConstantCallback(1)));
    [Range(0.0, 0.95, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Y0
    {
        get => (double)GetValue(Y0Property);
        set => SetValue(Y0Property, value);
    }

    public static readonly DependencyProperty Z0Property = DependencyProperty.Register(nameof(Z0), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.189, PixelShaderConstantCallback(2)));
    [Range(0.0, 0.82, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Z0
    {
        get => (double)GetValue(Z0Property);
        set => SetValue(Z0Property, value);
    }

    public static readonly DependencyProperty X1Property = DependencyProperty.Register(nameof(X1), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.349, PixelShaderConstantCallback(3)));
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double X1
    {
        get => (double)GetValue(X1Property);
        set => SetValue(X1Property, value);
    }

    public static readonly DependencyProperty Y1Property = DependencyProperty.Register(nameof(Y1), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.686, PixelShaderConstantCallback(4)));
    [Range(0.0, 0.95, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Y1
    {
        get => (double)GetValue(Y1Property);
        set => SetValue(Y1Property, value);
    }

    public static readonly DependencyProperty Z1Property = DependencyProperty.Register(nameof(Z1), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.168, PixelShaderConstantCallback(5)));
    [Range(0.0, 0.82, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Z1
    {
        get => (double)GetValue(Z1Property);
        set => SetValue(Z1Property, value);
    }

    public static readonly DependencyProperty X2Property = DependencyProperty.Register(nameof(X2), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.272, PixelShaderConstantCallback(6)));
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double X2
    {
        get => (double)GetValue(X2Property);
        set => SetValue(X2Property, value);
    }

    public static readonly DependencyProperty Y2Property = DependencyProperty.Register(nameof(Y2), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.534, PixelShaderConstantCallback(7)));
    [Range(0.0, 0.95, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Y2
    {
        get => (double)GetValue(Y2Property);
        set => SetValue(Y2Property, value);
    }

    public static readonly DependencyProperty Z2Property = DependencyProperty.Register(nameof(Z2), typeof(double), typeof(AdjustEffect), new FrameworkPropertyMetadata(0.131, PixelShaderConstantCallback(8)));
    [Range(0.0, 0.82, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double Z2
    {
        get => (double)GetValue(Z2Property);
        set => SetValue(Z2Property, value);
    }

    public AdjustEffect() : base()
    {
        UpdateShaderValue(AmountProperty);

        UpdateShaderValue(X0Property);
        UpdateShaderValue(Y0Property);
        UpdateShaderValue(Z0Property);

        UpdateShaderValue(X1Property);
        UpdateShaderValue(Y1Property);
        UpdateShaderValue(Z1Property);

        UpdateShaderValue(X2Property);
        UpdateShaderValue(Y2Property);
        UpdateShaderValue(Z2Property);
    }

    public override Color Apply(Color color, double amount = 1)
    {
        double r = color.R, g = color.G, b = color.B;
        var nr = Clamp((r * 0.393) + (g * 0.769) + (b * 0.189), 255).Byte();
        var ng = Clamp((r * 0.349) + (g * 0.686) + (b * 0.168), 255).Byte();
        var nb = Clamp((r * 0.272) + (g * 0.534) + (b * 0.131), 255).Byte();
        return Color.FromArgb(color.A, nr, ng, nb);
    }
}