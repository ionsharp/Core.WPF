using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Blend), Name("Gradient overlay"), Explicit]
public class GradientOverlayEffect : BaseBlendEffect
{
    #region Properties

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == MaximumProperty || e.Property == MinimumProperty || e.Property == ValueProperty)
            SetCurrentValue(ProgressProperty, new DoubleRange(Minimum, Maximum).Convert(0, 1, Value));
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(1.0));
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0.0));
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0.0));
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #region (C0-8)

    public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(nameof(Angle), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(2)));
    [Range(0d, 359d, 1d, Style = RangeStyle.Both)]
    [Show]
    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public static readonly DependencyProperty GradientProperty = DependencyProperty.Register(nameof(Gradient), typeof(Gradient), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(null, OnGradientChanged));
    [Show]
    public Gradient Gradient
    {
        get => (Gradient)GetValue(GradientProperty);
        set => SetValue(GradientProperty, value);
    }
    static void OnGradientChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<GradientOverlayEffect>().OnGradientChanged(e);

    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(3)));
    [Show]
    [Range(0, int.MaxValue, 1, Style = RangeStyle.UpDown)]
    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(4)));
    [Show]
    [Range(0, int.MaxValue, 1, Style = RangeStyle.UpDown)]
    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public static readonly DependencyProperty ReflectProperty = DependencyProperty.Register(nameof(Reflect), typeof(bool), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(5)));
    [Show]
    public bool Reflect
    {
        get => (bool)GetValue(ReflectProperty);
        set => SetValue(ReflectProperty, value);
    }

    public static readonly DependencyProperty ReverseProperty = DependencyProperty.Register(nameof(Reverse), typeof(bool), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(6)));
    [Show]
    public bool Reverse
    {
        get => (bool)GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }

    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(7)));
    [Range(0d, 1d, 0.01d, Style = RangeStyle.Both)]
    [Show]
    public double Scale
    {
        get => (double)GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(GradientType), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(GradientType.Linear, OnTypeChanged));
    [Pin(Pin.AboveOrLeft), Show]
    public GradientType Type
    {
        get => (GradientType)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }
    static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<GradientOverlayEffect>().TypeIndex = (int)(GradientType)e.NewValue;

    public static readonly DependencyProperty TypeIndexProperty = DependencyProperty.Register(nameof(TypeIndex), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(3d, PixelShaderConstantCallback(8)));
    public double TypeIndex
    {
        get => (double)GetValue(TypeIndexProperty);
        set => SetValue(TypeIndexProperty, value);
    }

    #endregion

    #region (C9-40) Color[1-16], Offset[1-16]

    public static readonly DependencyProperty Color1Property = DependencyProperty.Register(nameof(Color1), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(9)));
    public Color Color1
    {
        get => (Color)GetValue(Color1Property);
        set => SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Offset1Property = DependencyProperty.Register(nameof(Offset1), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(10)));
    public double Offset1
    {
        get => (double)GetValue(Offset1Property);
        set => SetValue(Offset1Property, value);
    }

    public static readonly DependencyProperty Color2Property = DependencyProperty.Register(nameof(Color2), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(11)));
    public Color Color2
    {
        get => (Color)GetValue(Color2Property);
        set => SetValue(Color2Property, value);
    }

    public static readonly DependencyProperty Offset2Property = DependencyProperty.Register(nameof(Offset2), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(12)));
    public double Offset2
    {
        get => (double)GetValue(Offset2Property);
        set => SetValue(Offset2Property, value);
    }

    public static readonly DependencyProperty Color3Property = DependencyProperty.Register(nameof(Color3), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(13)));
    public Color Color3
    {
        get => (Color)GetValue(Color3Property);
        set => SetValue(Color3Property, value);
    }

    public static readonly DependencyProperty Offset3Property = DependencyProperty.Register(nameof(Offset3), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(14)));
    public double Offset3
    {
        get => (double)GetValue(Offset3Property);
        set => SetValue(Offset3Property, value);
    }

    public static readonly DependencyProperty Color4Property = DependencyProperty.Register(nameof(Color4), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(15)));
    public Color Color4
    {
        get => (Color)GetValue(Color4Property);
        set => SetValue(Color4Property, value);
    }

    public static readonly DependencyProperty Offset4Property = DependencyProperty.Register(nameof(Offset4), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(16)));
    public double Offset4
    {
        get => (double)GetValue(Offset4Property);
        set => SetValue(Offset4Property, value);
    }

    public static readonly DependencyProperty Color5Property = DependencyProperty.Register(nameof(Color5), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(17)));
    public Color Color5
    {
        get => (Color)GetValue(Color5Property);
        set => SetValue(Color5Property, value);
    }

    public static readonly DependencyProperty Offset5Property = DependencyProperty.Register(nameof(Offset5), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(18)));
    public double Offset5
    {
        get => (double)GetValue(Offset5Property);
        set => SetValue(Offset5Property, value);
    }

    public static readonly DependencyProperty Color6Property = DependencyProperty.Register(nameof(Color6), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(19)));
    public Color Color6
    {
        get => (Color)GetValue(Color6Property);
        set => SetValue(Color6Property, value);
    }

    public static readonly DependencyProperty Offset6Property = DependencyProperty.Register(nameof(Offset6), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(20)));
    public double Offset6
    {
        get => (double)GetValue(Offset6Property);
        set => SetValue(Offset6Property, value);
    }

    public static readonly DependencyProperty Color7Property = DependencyProperty.Register(nameof(Color7), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(21)));
    public Color Color7
    {
        get => (Color)GetValue(Color7Property);
        set => SetValue(Color7Property, value);
    }

    public static readonly DependencyProperty Offset7Property = DependencyProperty.Register(nameof(Offset7), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(22)));
    public double Offset7
    {
        get => (double)GetValue(Offset7Property);
        set => SetValue(Offset7Property, value);
    }

    public static readonly DependencyProperty Color8Property = DependencyProperty.Register(nameof(Color8), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(23)));
    public Color Color8
    {
        get => (Color)GetValue(Color8Property);
        set => SetValue(Color8Property, value);
    }

    public static readonly DependencyProperty Offset8Property = DependencyProperty.Register(nameof(Offset8), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(24)));
    public double Offset8
    {
        get => (double)GetValue(Offset8Property);
        set => SetValue(Offset8Property, value);
    }

    public static readonly DependencyProperty Color9Property = DependencyProperty.Register(nameof(Color9), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(25)));
    public Color Color9
    {
        get => (Color)GetValue(Color9Property);
        set => SetValue(Color9Property, value);
    }

    public static readonly DependencyProperty Offset9Property = DependencyProperty.Register(nameof(Offset9), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(26)));
    public double Offset9
    {
        get => (double)GetValue(Offset9Property);
        set => SetValue(Offset9Property, value);
    }

    public static readonly DependencyProperty Color10Property = DependencyProperty.Register(nameof(Color10), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(27)));
    public Color Color10
    {
        get => (Color)GetValue(Color10Property);
        set => SetValue(Color10Property, value);
    }

    public static readonly DependencyProperty Offset10Property = DependencyProperty.Register(nameof(Offset10), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(28)));
    public double Offset10
    {
        get => (double)GetValue(Offset10Property);
        set => SetValue(Offset10Property, value);
    }

    public static readonly DependencyProperty Color11Property = DependencyProperty.Register(nameof(Color11), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(29)));
    public Color Color11
    {
        get => (Color)GetValue(Color11Property);
        set => SetValue(Color11Property, value);
    }

    public static readonly DependencyProperty Offset11Property = DependencyProperty.Register(nameof(Offset11), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(30)));
    public double Offset11
    {
        get => (double)GetValue(Offset11Property);
        set => SetValue(Offset11Property, value);
    }

    public static readonly DependencyProperty Color12Property = DependencyProperty.Register(nameof(Color12), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(31)));
    public Color Color12
    {
        get => (Color)GetValue(Color12Property);
        set => SetValue(Color12Property, value);
    }

    public static readonly DependencyProperty Offset12Property = DependencyProperty.Register(nameof(Offset12), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(32)));
    public double Offset12
    {
        get => (double)GetValue(Offset12Property);
        set => SetValue(Offset12Property, value);
    }

    public static readonly DependencyProperty Color13Property = DependencyProperty.Register(nameof(Color13), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(33)));
    public Color Color13
    {
        get => (Color)GetValue(Color13Property);
        set => SetValue(Color13Property, value);
    }

    public static readonly DependencyProperty Offset13Property = DependencyProperty.Register(nameof(Offset13), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(34)));
    public double Offset13
    {
        get => (double)GetValue(Offset13Property);
        set => SetValue(Offset13Property, value);
    }

    public static readonly DependencyProperty Color14Property = DependencyProperty.Register(nameof(Color14), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(35)));
    public Color Color14
    {
        get => (Color)GetValue(Color14Property);
        set => SetValue(Color14Property, value);
    }

    public static readonly DependencyProperty Offset14Property = DependencyProperty.Register(nameof(Offset14), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(36)));
    public double Offset14
    {
        get => (double)GetValue(Offset14Property);
        set => SetValue(Offset14Property, value);
    }

    public static readonly DependencyProperty Color15Property = DependencyProperty.Register(nameof(Color15), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(37)));
    public Color Color15
    {
        get => (Color)GetValue(Color15Property);
        set => SetValue(Color15Property, value);
    }

    public static readonly DependencyProperty Offset15Property = DependencyProperty.Register(nameof(Offset15), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(38)));
    public double Offset15
    {
        get => (double)GetValue(Offset15Property);
        set => SetValue(Offset15Property, value);
    }

    public static readonly DependencyProperty Color16Property = DependencyProperty.Register(nameof(Color16), typeof(Color), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(39)));
    public Color Color16
    {
        get => (Color)GetValue(Color16Property);
        set => SetValue(Color16Property, value);
    }

    public static readonly DependencyProperty Offset16Property = DependencyProperty.Register(nameof(Offset16), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(40)));
    public double Offset16
    {
        get => (double)GetValue(Offset16Property);
        set => SetValue(Offset16Property, value);
    }

    #endregion

    #region (C41) Length

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(nameof(Length), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(41)));
    public double Length
    {
        get => (double)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    #endregion

    #region (C42) Progress

    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(42)));
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    #endregion

    #region (C43) Thickness

    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(43)));
    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    #endregion

    /*
    public static readonly DependencyProperty ColorsProperty = RegisterPixelShaderSamplerProperty(nameof(Colors), typeof(GradientOverlayEffect), 1);
    public System.Windows.Media.Brush Colors
    {
        get => (System.Windows.Media.Brush)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    public static readonly DependencyProperty ColorsLengthProperty = DependencyProperty.Register(nameof(ColorsLength), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(9)));
    public double ColorsLength
    {
        get => (double)GetValue(ColorsLengthProperty);
        set => SetValue(ColorsLengthProperty, value);
    }

    public static readonly DependencyProperty OffsetsProperty = RegisterPixelShaderSamplerProperty(nameof(Offsets), typeof(GradientOverlayEffect), 2);
    public System.Windows.Media.Brush Offsets
    {
        get => (System.Windows.Media.Brush)GetValue(OffsetsProperty);
        set => SetValue(OffsetsProperty, value);
    }

    public static readonly DependencyProperty OffsetsLengthProperty = DependencyProperty.Register(nameof(OffsetsLength), typeof(double), typeof(GradientOverlayEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(10)));
    public double OffsetsLength
    {
        get => (double)GetValue(OffsetsLengthProperty);
        set => SetValue(OffsetsLengthProperty, value);
    }
    */

    #endregion

    #region GradientOverlayEffect

    public GradientOverlayEffect() : base()
    {
        UpdateShaderValue(AngleProperty);
        UpdateShaderValue(OffsetXProperty);
        UpdateShaderValue(OffsetYProperty);
        UpdateShaderValue(ProgressProperty);
        UpdateShaderValue(OffsetYProperty);
        UpdateShaderValue(ReflectProperty);
        UpdateShaderValue(ReverseProperty);
        UpdateShaderValue(ScaleProperty);
        UpdateShaderValue(ThicknessProperty);
        UpdateShaderValue(TypeIndexProperty);

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

        ///

        SetCurrentValue(GradientProperty, Gradient.Default);

        /*
        SetValue(ColorsLengthProperty, 2d);
        var colors = XBitmap.New(Gradient.Steps.Count, 1);
        colors.ForEach((x, y, color) => Gradient.Steps[x].Color);
        SetValue(ColorsProperty, new ImageBrush() { ImageSource = colors });

        var testOffsets = new float[2] { 0.2f, 0.8f };

        SetValue(OffsetsLengthProperty, 2d);
        var offsets = XBitmap.New(Gradient.Steps.Count, 1);
        offsets.ForEach((x, y, color) =>
        {
            byte[] offset = System.BitConverter.GetBytes(testOffsets[x]);
            return Color.FromArgb(offset[0], offset[1], offset[2], offset[3]);
        });
        SetValue(OffsetsProperty, new ImageBrush() { ImageSource = offsets });
        */
    }

    #endregion

    #region Methods

    public static Matrix<Color> RenderAngle(Gradient input, Int32Size size, double angle)
    {
        int cx = size.Width / 2, cy = size.Height / 2;

        GradientStep a = null, b = null;

        var result = new Matrix<Color>((uint)size.Height, (uint)size.Width);
        for (var y = 0; y < size.Height; y++)
        {
            for (var x = 0; x < size.Width; x++)
            {
                var r = 2 * Math.PI;

                var t = Math.Atan2(y - cy, x - cx) + Core.Numerics.Angle.GetRadian(angle);
                t = t + Math.PI;
                if (t > r)
                    t = t - r;

                t = t / r;

                GetColors(input, t, ref a, ref b);
                if (a == null || b == null)
                    continue;

                var p = 1 - ((t - a.Offset) / (b.Offset - a.Offset));
                result[(uint)y, (uint)x] = Color.Add(Color.Multiply(XColor.Convert(a.Color), p.Single()), Color.Multiply(XColor.Convert(b.Color), (1 - p).Single())).A(255);
            }
        }
        return result;
    }

    /// <remarks>To do: Observe angle.</remarks>
    public static Matrix<Color> RenderCircle(Gradient input, Int32Size size, double angle)
    {
        float cx = size.Width * 0.5f, cy = size.Height * 0.5f;

        GradientStep aStep = null, bStep = null;

        var result = new Matrix<Color>((uint)size.Height, (uint)size.Width);
        for (int y = 0; y < size.Height; y++)
        {
            for (int x = 0; x < size.Width; x++)
            {
                float md = GetDistance(cx, cy, size.Width, size.Height);
                float nd = Math.Min(GetDistance(cx, cy, x, y) / md, 1);

                var s = Convert.ToByte(FLerp(nd, 255, 0));
                var t = s.Double() / 255d;

                GetColors(input, t, ref aStep, ref bStep);
                if (aStep == null || bStep == null)
                    continue;

                var p = (t - aStep.Offset) / (bStep.Offset - aStep.Offset);
                result[(uint)y, (uint)x] = XColor.Convert(aStep.Color).Blend(XColor.Convert(bStep.Color).A((p * 255d).Byte()));
            }
        }
        return result;
    }

    /// <remarks>To do: Observe angle.</remarks>
    public static Matrix<Color> RenderDiamond(Gradient input, Int32Size size, double angle)
    {
        int cx = size.Width / 2, cy = size.Height / 2;

        GradientStep aStep = null, bStep = null;

        int x0 = cx, y0 = cy;
        int h = 1, w = 1;

        Color Blend(double p)
            => XColor.Convert(aStep.Color).Blend(XColor.Convert(bStep.Color).A((p * 255d).Byte()));

        double P()
        {
            float dm = GetDistance(cx, cy, size.Width, size.Height);
            float dn = Math.Min(GetDistance(x0, y0, cx, cy) / dm, 1);

            var t = Convert.ToByte(FLerp(dn, 255, 0)).Double() / 255d;
            GetColors(input, t, ref aStep, ref bStep);
            if (aStep == null || bStep == null)
                return 0;

            return (t - aStep.Offset) / (bStep.Offset - aStep.Offset);
        }

        var result = new Matrix<Color>((uint)size.Height, (uint)size.Width);

        //Top
        while (y0 >= 0)
        {
            if (y0 < 0) break;
            var p = P();

            var x1 = x0 + w;
            for (var x2 = x0; x2 < x1; x2++)
            {
                if (x2 < 0 || x2 >= size.Width)
                    continue;

                result[(uint)y0, (uint)x2] = Blend(p);
            }

            x0--; y0--;
            w += 2;

            if (y0 == 0)
                break;
        }

        x0 = cx; y0 = cy;
        w = 1;

        //Bottom
        while (y0 <= size.Height - 1)
        {
            if (y0 >= size.Height) break;
            var p = P();

            var x1 = x0 + w;
            for (var x2 = x0; x2 < x1; x2++)
            {
                if (x2 < 0 || x2 >= size.Width)
                    continue;

                result[(uint)y0, (uint)x2] = Blend(p);
            }

            x0--; y0++;
            w += 2;

            if (y0 == size.Height - 1)
                break;
        }

        x0 = cx; y0 = cy;

        //Right
        while (x0 <= size.Width - 1)
        {
            if (x0 >= size.Width) break;
            var p = P();

            var y1 = y0 + h;
            for (var y2 = y0; y2 < y1; y2++)
            {
                if (y2 < 0 || y2 >= size.Height)
                    continue;

                result[(uint)y2, (uint)x0] = Blend(p);
            }

            x0++; y0--;
            h += 2;

            if (x0 == size.Width - 1)
                break;
        }

        x0 = cx; y0 = cy;
        h = 1;

        //Left
        while (x0 >= 0)
        {
            if (x0 < 0) break;
            var p = P();

            var y1 = y0 + h;
            for (var y2 = y0; y2 < y1; y2++)
            {
                if (y2 < 0 || y2 >= size.Height)
                    continue;

                result[(uint)y2, (uint)x0] = Blend(p);
            }

            x0--; y0--;
            h += 2;

            if (x0 == 0)
                break;
        }

        return result;
    }

    /// <remarks>To do: Observe angle.</remarks>
    public static Matrix<Color> RenderLinear(Gradient input, Int32Size size, double angle)
    {
        GradientStep aStep = null, bStep = null;

        var result = new Matrix<Color>((uint)size.Height, (uint)size.Width);
        for (int y = 0; y < size.Height; y++)
        {
            for (int x = 0; x < size.Width; x++)
            {
                var t = x.Double() / size.Width.Double();

                var count = input.Steps.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    if (i - 1 >= 0)
                    {
                        if (t > input.Steps[i - 1].Offset && t <= input.Steps[i].Offset)
                        {
                            aStep = input.Steps[i - 1];
                            bStep = input.Steps[i];
                            break;
                        }
                    }
                }

                if (aStep == null || bStep == null)
                    continue;

                var q = bStep.Offset - aStep.Offset;
                if (q == 0)
                    continue;

                var p = (t - aStep.Offset) / q;

                var color = XColor.Convert(aStep.Color).Blend(XColor.Convert(bStep.Color).A(Clamp(p * 255d, 255).Byte()));
                result[(uint)y, (uint)x] = color;
            }
        }
        return result;
    }

    static float FLerp(float norm, float min, float max)
        => (max - min) * norm + min;

    static void GetColors(Gradient input, double t, ref GradientStep a, ref GradientStep b)
    {
        var count = input.Steps.Count;
        for (var i = count - 1; i >= 0; i--)
        {
            if (i - 1 >= 0)
            {
                if (t > input.Steps[i - 1].Offset && t <= input.Steps[i].Offset)
                {
                    b = input.Steps[i - 1];
                    a = input.Steps[i];
                    break;
                }
            }
        }
    }

    static float GetDistance(float x1, float y1, float x2, float y2)
        => (float)(Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2)));

    public static GradientStep[] GetSteps(Gradient gradient, bool reflect, bool reverse)
    {
        var result = new List<GradientStep>();
        gradient.Steps.ForEach(i => result.Add(new(i.Offset, i.Color)));

        if (reverse)
        {
            result.Reverse();
            for (var i = 0; i < result.Count; i++)
                result[i].Offset = gradient.Steps[i].Offset;
        }

        if (reflect)
        {
            var oldRange = new DoubleRange(0, 1);
            foreach (var i in result)
                i.Offset = oldRange.Convert(0, 0.5, i.Offset);

            for (var i = result.Count - 1; i >= 0; i--)
            {
                var j = new GradientStep(1 - result[i].Offset, result[i].Color);
                result.Add(j);
            }
        }
        return result.ToArray();
    }

    void Register(Gradient gradient, int i)
    {
        var step = gradient.Steps[i - 1];
        var x = XColor.Convert(step.Color); var y = step.Offset;
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

    void Update(Gradient gradient)
    {
        Length = gradient.Steps.Count;
        for (var i = 0; i < gradient.Steps.Count; i++)
            Register(gradient, i + 1);
    }

    protected virtual void OnGradientChanged(ReadOnlyValue<Gradient> input)
    {
        if (input.Old != null)
            input.Old.Steps.Changed -= OnGradientChanged;

        if (input.New != null)
        {
            input.New.Steps.Changed -= OnGradientChanged;
            input.New.Steps.Changed += OnGradientChanged;
            Update(input.New);
        }
        else Length = 0;
    }

    void OnGradientChanged(object sender) => Update(Gradient);

    protected Matrix<Color> Render(WriteableBitmap input)
    {
        if (Gradient == null || Gradient.Steps.Count == 0)
            return null;

        Matrix<Color> result = default;
        Try.Invoke(() =>
        {
            var gradient = new Gradient(GetSteps(Gradient, Reflect, Reverse));

            var size = new Int32Size((input.PixelHeight * Scale).Int32(), (input.PixelWidth * Scale).Int32());
            switch (Type)
            {
                case GradientType.Angle:
                    result = RenderAngle(gradient, size, Angle);
                    break;

                case GradientType.Circle:
                    result = RenderCircle(gradient, size, Angle);
                    break;

                case GradientType.Diamond:
                    result = RenderDiamond(gradient, size, Angle);
                    break;

                case GradientType.Linear:
                    result = RenderLinear(gradient, size, Angle);
                    break;
            }
        },
        e => Log.Write<GradientOverlayEffect>(e));
        return result;
    }

    #endregion
}