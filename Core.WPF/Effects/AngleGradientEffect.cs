using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

public class AngleGradientEffect : BaseEffect
{
    public override string FilePath => "AngleGradientEffect.ps";

    #region Properties

    public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(nameof(Brush), typeof(LinearGradientBrush), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(null, OnBrushChanged));
    public LinearGradientBrush Brush
    {
        get => (LinearGradientBrush)GetValue(BrushProperty);
        set => SetValue(BrushProperty, value);
    }
    static void OnBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<AngleGradientEffect>().OnBrushChanged(e);

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(1.0));
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0.0));
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0.0));
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #region (C0-8)

    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(1)));
    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(nameof(Angle), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(2)));
    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(3)));
    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(4)));
    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public static readonly DependencyProperty ReflectProperty = DependencyProperty.Register(nameof(Reflect), typeof(bool), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(5)));
    public bool Reflect
    {
        get => (bool)GetValue(ReflectProperty);
        set => SetValue(ReflectProperty, value);
    }

    public static readonly DependencyProperty ReverseProperty = DependencyProperty.Register(nameof(Reverse), typeof(bool), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(6)));
    public bool Reverse
    {
        get => (bool)GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }

    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(7)));
    public double Scale
    {
        get => (double)GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0.2, PixelShaderConstantCallback(8)));
    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    #endregion

    #region (C9-40) Color[1-16], Offset[1-16]

    public static readonly DependencyProperty Color1Property = DependencyProperty.Register(nameof(Color1), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(9)));
    public Color Color1
    {
        get => (Color)GetValue(Color1Property);
        set => SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Offset1Property = DependencyProperty.Register(nameof(Offset1), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(10)));
    public double Offset1
    {
        get => (double)GetValue(Offset1Property);
        set => SetValue(Offset1Property, value);
    }

    public static readonly DependencyProperty Color2Property = DependencyProperty.Register(nameof(Color2), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(11)));
    public Color Color2
    {
        get => (Color)GetValue(Color2Property);
        set => SetValue(Color2Property, value);
    }

    public static readonly DependencyProperty Offset2Property = DependencyProperty.Register(nameof(Offset2), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(12)));
    public double Offset2
    {
        get => (double)GetValue(Offset2Property);
        set => SetValue(Offset2Property, value);
    }

    public static readonly DependencyProperty Color3Property = DependencyProperty.Register(nameof(Color3), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(13)));
    public Color Color3
    {
        get => (Color)GetValue(Color3Property);
        set => SetValue(Color3Property, value);
    }

    public static readonly DependencyProperty Offset3Property = DependencyProperty.Register(nameof(Offset3), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(14)));
    public double Offset3
    {
        get => (double)GetValue(Offset3Property);
        set => SetValue(Offset3Property, value);
    }

    public static readonly DependencyProperty Color4Property = DependencyProperty.Register(nameof(Color4), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(15)));
    public Color Color4
    {
        get => (Color)GetValue(Color4Property);
        set => SetValue(Color4Property, value);
    }

    public static readonly DependencyProperty Offset4Property = DependencyProperty.Register(nameof(Offset4), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(16)));
    public double Offset4
    {
        get => (double)GetValue(Offset4Property);
        set => SetValue(Offset4Property, value);
    }

    public static readonly DependencyProperty Color5Property = DependencyProperty.Register(nameof(Color5), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(17)));
    public Color Color5
    {
        get => (Color)GetValue(Color5Property);
        set => SetValue(Color5Property, value);
    }

    public static readonly DependencyProperty Offset5Property = DependencyProperty.Register(nameof(Offset5), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(18)));
    public double Offset5
    {
        get => (double)GetValue(Offset5Property);
        set => SetValue(Offset5Property, value);
    }

    public static readonly DependencyProperty Color6Property = DependencyProperty.Register(nameof(Color6), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(19)));
    public Color Color6
    {
        get => (Color)GetValue(Color6Property);
        set => SetValue(Color6Property, value);
    }

    public static readonly DependencyProperty Offset6Property = DependencyProperty.Register(nameof(Offset6), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(20)));
    public double Offset6
    {
        get => (double)GetValue(Offset6Property);
        set => SetValue(Offset6Property, value);
    }

    public static readonly DependencyProperty Color7Property = DependencyProperty.Register(nameof(Color7), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(21)));
    public Color Color7
    {
        get => (Color)GetValue(Color7Property);
        set => SetValue(Color7Property, value);
    }

    public static readonly DependencyProperty Offset7Property = DependencyProperty.Register(nameof(Offset7), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(22)));
    public double Offset7
    {
        get => (double)GetValue(Offset7Property);
        set => SetValue(Offset7Property, value);
    }

    public static readonly DependencyProperty Color8Property = DependencyProperty.Register(nameof(Color8), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(23)));
    public Color Color8
    {
        get => (Color)GetValue(Color8Property);
        set => SetValue(Color8Property, value);
    }

    public static readonly DependencyProperty Offset8Property = DependencyProperty.Register(nameof(Offset8), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(24)));
    public double Offset8
    {
        get => (double)GetValue(Offset8Property);
        set => SetValue(Offset8Property, value);
    }

    public static readonly DependencyProperty Color9Property = DependencyProperty.Register(nameof(Color9), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(25)));
    public Color Color9
    {
        get => (Color)GetValue(Color9Property);
        set => SetValue(Color9Property, value);
    }

    public static readonly DependencyProperty Offset9Property = DependencyProperty.Register(nameof(Offset9), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(26)));
    public double Offset9
    {
        get => (double)GetValue(Offset9Property);
        set => SetValue(Offset9Property, value);
    }

    public static readonly DependencyProperty Color10Property = DependencyProperty.Register(nameof(Color10), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(27)));
    public Color Color10
    {
        get => (Color)GetValue(Color10Property);
        set => SetValue(Color10Property, value);
    }

    public static readonly DependencyProperty Offset10Property = DependencyProperty.Register(nameof(Offset10), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(28)));
    public double Offset10
    {
        get => (double)GetValue(Offset10Property);
        set => SetValue(Offset10Property, value);
    }

    public static readonly DependencyProperty Color11Property = DependencyProperty.Register(nameof(Color11), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(29)));
    public Color Color11
    {
        get => (Color)GetValue(Color11Property);
        set => SetValue(Color11Property, value);
    }

    public static readonly DependencyProperty Offset11Property = DependencyProperty.Register(nameof(Offset11), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(30)));
    public double Offset11
    {
        get => (double)GetValue(Offset11Property);
        set => SetValue(Offset11Property, value);
    }

    public static readonly DependencyProperty Color12Property = DependencyProperty.Register(nameof(Color12), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(31)));
    public Color Color12
    {
        get => (Color)GetValue(Color12Property);
        set => SetValue(Color12Property, value);
    }

    public static readonly DependencyProperty Offset12Property = DependencyProperty.Register(nameof(Offset12), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(32)));
    public double Offset12
    {
        get => (double)GetValue(Offset12Property);
        set => SetValue(Offset12Property, value);
    }

    public static readonly DependencyProperty Color13Property = DependencyProperty.Register(nameof(Color13), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(33)));
    public Color Color13
    {
        get => (Color)GetValue(Color13Property);
        set => SetValue(Color13Property, value);
    }

    public static readonly DependencyProperty Offset13Property = DependencyProperty.Register(nameof(Offset13), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(34)));
    public double Offset13
    {
        get => (double)GetValue(Offset13Property);
        set => SetValue(Offset13Property, value);
    }

    public static readonly DependencyProperty Color14Property = DependencyProperty.Register(nameof(Color14), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(35)));
    public Color Color14
    {
        get => (Color)GetValue(Color14Property);
        set => SetValue(Color14Property, value);
    }

    public static readonly DependencyProperty Offset14Property = DependencyProperty.Register(nameof(Offset14), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(36)));
    public double Offset14
    {
        get => (double)GetValue(Offset14Property);
        set => SetValue(Offset14Property, value);
    }

    public static readonly DependencyProperty Color15Property = DependencyProperty.Register(nameof(Color15), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(37)));
    public Color Color15
    {
        get => (Color)GetValue(Color15Property);
        set => SetValue(Color15Property, value);
    }

    public static readonly DependencyProperty Offset15Property = DependencyProperty.Register(nameof(Offset15), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(38)));
    public double Offset15
    {
        get => (double)GetValue(Offset15Property);
        set => SetValue(Offset15Property, value);
    }

    public static readonly DependencyProperty Color16Property = DependencyProperty.Register(nameof(Color16), typeof(Color), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(39)));
    public Color Color16
    {
        get => (Color)GetValue(Color16Property);
        set => SetValue(Color16Property, value);
    }

    public static readonly DependencyProperty Offset16Property = DependencyProperty.Register(nameof(Offset16), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(40)));
    public double Offset16
    {
        get => (double)GetValue(Offset16Property);
        set => SetValue(Offset16Property, value);
    }

    #endregion

    #region (C41)

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(nameof(Length), typeof(double), typeof(AngleGradientEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(41)));
    public double Length
    {
        get => (double)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    #endregion

    #endregion

    #region AngleGradientEffect

    public AngleGradientEffect() : base()
    {
        UpdateShaderValue(ProgressProperty);
        UpdateShaderValue(AngleProperty);
        UpdateShaderValue(OffsetXProperty);
        UpdateShaderValue(OffsetYProperty);
        UpdateShaderValue(ReflectProperty);
        UpdateShaderValue(ReverseProperty);
        UpdateShaderValue(ScaleProperty);
        UpdateShaderValue(ThicknessProperty);

        UpdateShaderValue(Color1Property);
        UpdateShaderValue(Offset1Property);
        UpdateShaderValue(Color2Property);
        UpdateShaderValue(Offset2Property);
        UpdateShaderValue(Color3Property);
        UpdateShaderValue(Offset3Property);
        UpdateShaderValue(Color4Property);
        UpdateShaderValue(Offset4Property);
        UpdateShaderValue(Color5Property);
        UpdateShaderValue(Offset5Property);
        UpdateShaderValue(Color6Property);
        UpdateShaderValue(Offset6Property);
        UpdateShaderValue(Color7Property);
        UpdateShaderValue(Offset7Property);
        UpdateShaderValue(Color8Property);
        UpdateShaderValue(Offset8Property);
        UpdateShaderValue(Color9Property);
        UpdateShaderValue(Offset9Property);
        UpdateShaderValue(Color10Property);
        UpdateShaderValue(Offset10Property);
        UpdateShaderValue(Color11Property);
        UpdateShaderValue(Offset11Property);
        UpdateShaderValue(Color12Property);
        UpdateShaderValue(Offset12Property);
        UpdateShaderValue(Color13Property);
        UpdateShaderValue(Offset13Property);
        UpdateShaderValue(Color14Property);
        UpdateShaderValue(Offset14Property);
        UpdateShaderValue(Color15Property);
        UpdateShaderValue(Offset15Property);
        UpdateShaderValue(Color16Property);
        UpdateShaderValue(Offset16Property);

        UpdateShaderValue(LengthProperty);
    }

    #endregion

    #region Methods

    void Register(LinearGradientBrush gradient, int i)
    {
        var step = gradient.GradientStops[i - 1];
        var x = step.Color; var y = step.Offset;
        switch (i)
        {
            case 1:
                Color1 = x;
                Offset1 = y;
                break;
            case 2:
                Color2 = x;
                Offset2 = y;
                break;
            case 3:
                Color3 = x;
                Offset3 = y;
                break;
            case 4:
                Color4 = x;
                Offset4 = y;
                break;
            case 5:
                Color5 = x;
                Offset5 = y;
                break;
            case 6:
                Color6 = x;
                Offset6 = y;
                break;
            case 7:
                Color7 = x;
                Offset7 = y;
                break;
            case 8:
                Color8 = x;
                Offset8 = y;
                break;
            case 9:
                Color9 = x;
                Offset9 = y;
                break;
            case 10:
                Color10 = x;
                Offset10 = y;
                break;
            case 11:
                Color11 = x;
                Offset11 = y;
                break;
            case 12:
                Color12 = x;
                Offset12 = y;
                break;
            case 13:
                Color13 = x;
                Offset13 = y;
                break;
            case 14:
                Color14 = x;
                Offset14 = y;
                break;
            case 15:
                Color15 = x;
                Offset15 = y;
                break;
            case 16:
                Color16 = x;
                Offset16 = y;
                break;
        }
    }

    void Update(LinearGradientBrush gradient)
    {
        Length = gradient.GradientStops.Count;
        for (var i = 0; i < gradient.GradientStops.Count; i++)
            Register(gradient, i + 1);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == MaximumProperty || e.Property == MinimumProperty || e.Property == ValueProperty)
            SetCurrentValue(ProgressProperty, new DoubleRange(Minimum, Maximum).Convert(0, 1, Value));
    }

    protected virtual void OnBrushChanged(Value<LinearGradientBrush> input)
    {
        if (input.New != null)
        {
            Update(input.New);
        }
        else Length = 0;
    }

    #endregion
}