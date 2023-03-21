using Imagin.Core.Linq;
using Imagin.Core.Media;
using System.Windows;

namespace Imagin.Core.Effects;

public class BlendEffect : BaseEffect
{
    public static readonly DependencyProperty ActualBlendModeProperty = DependencyProperty.Register(nameof(ActualBlendMode), typeof(BlendModes), typeof(BlendEffect), new FrameworkPropertyMetadata(BlendModes.Normal, OnActualBlendModeChanged));
    public BlendModes ActualBlendMode
    {
        get => (BlendModes)GetValue(ActualBlendModeProperty);
        set => SetValue(ActualBlendModeProperty, value);
    }
    static void OnActualBlendModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        => sender.As<BlendEffect>().BlendMode = (int)(BlendModes)e.NewValue;

    public static readonly DependencyProperty ActualMaskProperty = DependencyProperty.Register(nameof(ActualMask), typeof(BlendMasks), typeof(BlendEffect), new FrameworkPropertyMetadata(BlendMasks.None, OnActualMaskChanged));
    public BlendMasks ActualMask
    {
        get => (BlendMasks)GetValue(ActualMaskProperty);
        set => SetValue(ActualMaskProperty, value);
    }
    static void OnActualMaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        => sender.As<BlendEffect>().Mask = (int)(BlendMasks)e.NewValue;

    public static readonly DependencyProperty BInputProperty = RegisterPixelShaderSamplerProperty(nameof(BInput), typeof(BlendEffect), 1);
    public System.Windows.Media.Brush BInput
    {
        get => (System.Windows.Media.Brush)GetValue(BInputProperty);
        set => SetValue(BInputProperty, value);
    }

    public static readonly DependencyProperty CInputProperty = RegisterPixelShaderSamplerProperty(nameof(CInput), typeof(BlendEffect), 2);
    public System.Windows.Media.Brush CInput
    {
        get => (System.Windows.Media.Brush)GetValue(CInputProperty);
        set => SetValue(CInputProperty, value);
    }

    public static readonly DependencyProperty BlendModeProperty = DependencyProperty.Register(nameof(BlendMode), typeof(double), typeof(BlendEffect), new FrameworkPropertyMetadata((double)(int)BlendModes.Normal, PixelShaderConstantCallback(0)));
    public double BlendMode
    {
        get => (double)GetValue(BlendModeProperty);
        set => SetValue(BlendModeProperty, value);
    }

    public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(nameof(Mask), typeof(double), typeof(BlendEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(1)));
    public double Mask
    {
        get => (double)GetValue(MaskProperty);
        set => SetValue(MaskProperty, value);
    }

    public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(BlendEffect), new FrameworkPropertyMetadata(1d, PixelShaderConstantCallback(2)));
    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public static readonly DependencyProperty InvertProperty = DependencyProperty.Register(nameof(Invert), typeof(bool), typeof(BlendEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(3)));
    public bool Invert
    {
        get => (bool)GetValue(InvertProperty);
        set => SetValue(InvertProperty, value);
    }

    public BlendEffect() : base()
    {
        UpdateShaderValue(BInputProperty);
        UpdateShaderValue(CInputProperty);
        //SetCurrentValue(CInputProperty, new ImageBrush() { ImageSource = XBitmap.New(1, 1, Colors.White) });

        UpdateShaderValue(BlendModeProperty);
        UpdateShaderValue(InvertProperty);
        UpdateShaderValue(MaskProperty);
        UpdateShaderValue(OpacityProperty);
    }
}