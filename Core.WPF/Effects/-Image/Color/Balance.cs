using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Color), Name("Balance"), Explicit]
public class BalanceEffect : BaseBlendEffect
{
    new enum Category { Components }

    protected override string FilePath => $"{DefaultFolder}/ColorModelEffect.ps";

    static readonly DependencyProperty ComponentProperty = DependencyProperty.Register(nameof(Component), typeof(Component3), typeof(BalanceEffect), new FrameworkPropertyMetadata(Component3.X));
    Component3 Component
    {
        get => (Component3)GetValue(ComponentProperty);
        set => SetValue(ComponentProperty, value);
    }

    static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(ColorModelEffect.Modes), typeof(BalanceEffect), new FrameworkPropertyMetadata(ColorModelEffect.Modes.XYZ, null, OnModeCoerced));
    ColorModelEffect.Modes Mode
    {
        get => (ColorModelEffect.Modes)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    static object OnModeCoerced(DependencyObject sender, object input) => ColorModelEffect.Modes.XYZ;

    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof(Model), typeof(Type), typeof(BalanceEffect), new FrameworkPropertyMetadata(typeof(RGB)));
    [Pin(Pin.AboveOrLeft), Show]
    public Type Model
    {
        get => (Type)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    ///(C0)

    public static readonly DependencyProperty ActualModelProperty = DependencyProperty.Register(nameof(ActualModel), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    public double ActualModel
    {
        get => (double)GetValue(ActualModelProperty);
        set => SetValue(ActualModelProperty, value);
    }

    ///(C1)

    static readonly DependencyProperty ActualComponentProperty = DependencyProperty.Register(nameof(ActualComponent), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(1)));
    double ActualComponent
    {
        get => (double)GetValue(ActualComponentProperty);
        set => SetValue(ActualComponentProperty, value);
    }

    ///(C2)

    static readonly DependencyProperty ActualModeProperty = DependencyProperty.Register(nameof(ActualMode), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata((double)(int)ColorModelEffect.Modes.XYZ, PixelShaderConstantCallback(2)));
    double ActualMode
    {
        get => (double)GetValue(ActualModeProperty);
        set => SetValue(ActualModeProperty, value);
    }

    ///(C3)

    public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(3)));
    [Category(Category.Components)]
    [Range(-100.0, 100.0, 1.0, Style = RangeStyle.Both)]
    [Show]
    public double X
    {
        get => (double)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    ///(C4)

    public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(4)));
    [Category(Category.Components)]
    [Range(-100.0, 100.0, 1.0, Style = RangeStyle.Both)]
    [Show]
    public double Y
    {
        get => (double)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    ///(C5)

    public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(5)));
    [Category(Category.Components)]
    [Range(-100.0, 100.0, 1.0, Style = RangeStyle.Both)]
    [Show]
    public double Z
    {
        get => (double)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    ///(C6-7)

    public static readonly DependencyProperty HighlightAmountProperty = DependencyProperty.Register(nameof(HighlightAmount), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(6)));
    [Category(nameof(Tones.Highlights))]
    [Range(-1.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double HighlightAmount
    {
        get => (double)GetValue(HighlightAmountProperty);
        set => SetValue(HighlightAmountProperty, value);
    }

    public static readonly DependencyProperty HighlightRangeProperty = DependencyProperty.Register(nameof(HighlightRange), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(7)));
    [Category(nameof(Tones.Highlights))]
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double HighlightRange
    {
        get => (double)GetValue(HighlightRangeProperty);
        set => SetValue(HighlightRangeProperty, value);
    }

    ///(C8-9)

    public static readonly DependencyProperty MidtoneAmountProperty = DependencyProperty.Register(nameof(MidtoneAmount), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(8)));
    [Category(nameof(Tones.Midtones))]
    [Range(-1.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double MidtoneAmount
    {
        get => (double)GetValue(MidtoneAmountProperty);
        set => SetValue(MidtoneAmountProperty, value);
    }

    public static readonly DependencyProperty MidtoneRangeProperty = DependencyProperty.Register(nameof(MidtoneRange), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0.66, PixelShaderConstantCallback(9)));
    [Category(nameof(Tones.Midtones))]
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double MidtoneRange
    {
        get => (double)GetValue(MidtoneRangeProperty);
        set => SetValue(MidtoneRangeProperty, value);
    }

    ///(C10-11)

    public static readonly DependencyProperty ShadowAmountProperty = DependencyProperty.Register(nameof(ShadowAmount), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(10)));
    [Category(nameof(Tones.Shadows))]
    [Range(-1.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double ShadowAmount
    {
        get => (double)GetValue(ShadowAmountProperty);
        set => SetValue(ShadowAmountProperty, value);
    }

    public static readonly DependencyProperty ShadowRangeProperty = DependencyProperty.Register(nameof(ShadowRange), typeof(double), typeof(BalanceEffect), new FrameworkPropertyMetadata(0.33, PixelShaderConstantCallback(11)));
    [Category(nameof(Tones.Shadows))]
    [Range(0.0, 1.0, 0.01, Style = RangeStyle.Both)]
    [Show]
    public double ShadowRange
    {
        get => (double)GetValue(ShadowRangeProperty);
        set => SetValue(ShadowRangeProperty, value);
    }

    ///

    public BalanceEffect() : base()
    {
            
        this.Bind(ActualComponentProperty,
            $"{nameof(Component)}", this, BindingMode.OneWay, new ValueConverter<Component3, double>(i => (int)i));
        this.Bind(ActualModelProperty,
            $"{nameof(Model)}", this, BindingMode.OneWay, new ValueConverter<Type, double>(i => Core.Colors.Colour.GetIndex(i)));
        this.Bind(ActualModeProperty,
            $"{nameof(Mode)}", this, BindingMode.OneWay, new ValueConverter<ColorModelEffect.Modes, double>(i => (int)i));

        UpdateShaderValue(ActualModelProperty);
        UpdateShaderValue(ActualComponentProperty);

        UpdateShaderValue(ActualModeProperty);

        UpdateShaderValue(XProperty);
        UpdateShaderValue(YProperty);
        UpdateShaderValue(ZProperty);

        UpdateShaderValue
            (HighlightAmountProperty);
        UpdateShaderValue
            (HighlightRangeProperty);

        UpdateShaderValue
            (MidtoneAmountProperty);
        UpdateShaderValue
            (MidtoneRangeProperty);

        UpdateShaderValue
            (ShadowAmountProperty);
        UpdateShaderValue
            (ShadowRangeProperty);
    }

    public BalanceEffect(double x, double y, double z, Type model) : this()
    {
        SetCurrentValue(ModelProperty, model);
        SetCurrentValue(XProperty, x);
        SetCurrentValue(YProperty, y);
        SetCurrentValue(ZProperty, z);
    }

    ///

    public override Color Apply(Color color, double amount = 1)
    {
        int b = color.B, g = color.G, r = color.R;

        color.Convert(out System.Drawing.Color d);
        var l = d.GetBrightness();

        Color result(int r0, int g0, int b0) => Color.FromArgb(color.A, Clamp(r + r0, 255).Byte(), Clamp(g + g0, 255).Byte(), Clamp(b + b0, 255).Byte());
        /*
        //Highlight
        if (Range == ColorRanges.Highlights && l > 0.66)
            return result(X.Int32(), Y.Int32(), Z.Int32());

        //Midtone
        else if (Range == ColorRanges.Midtones && l > 0.33)
            return result(X.Int32(), Y.Int32(), Z.Int32());

        //Shadow
        else if (Range == ColorRanges.Shadows && l <= 0.33)
            return result(X.Int32(), Y.Int32(), Z.Int32());
        */
        return result((X.Double() * l).Round().Int32(), (Y.Double() * l).Round().Int32(), (Z.Double() * l).Round().Int32());
    }
}