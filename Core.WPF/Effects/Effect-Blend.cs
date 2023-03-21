using Imagin.Core.Media;
using System.Windows;

namespace Imagin.Core.Effects;

public abstract class BaseBlendEffect : ImageEffect
{
    enum Categories { Blend }

    public static readonly DependencyProperty ActualBlendModeProperty = DependencyProperty.Register(nameof(ActualBlendMode), typeof(BlendModes), typeof(BaseBlendEffect), new FrameworkPropertyMetadata(BlendModes.Normal, OnActualBlendModeChanged));
    [Float(Float.Above), HideName, Modify, Show]
    public BlendModes ActualBlendMode
    {
        get => (BlendModes)GetValue(ActualBlendModeProperty);
        set => SetValue(ActualBlendModeProperty, value);
    }
    protected static void OnActualBlendModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        => (sender as BaseBlendEffect).BlendMode = (int)(BlendModes)e.NewValue;

    public static readonly DependencyProperty BlendModeProperty = DependencyProperty.Register(nameof(BlendMode), typeof(double), typeof(BaseBlendEffect), new FrameworkPropertyMetadata((double)(int)BlendModes.Normal, PixelShaderConstantCallback(0)));
    [Hide]
    public double BlendMode
    {
        get => (double)GetValue(BlendModeProperty);
        set => SetValue(BlendModeProperty, value);
    }

    public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(BaseBlendEffect), new FrameworkPropertyMetadata(1d, PixelShaderConstantCallback(1)));
    [Category(Categories.Blend), Modify, Range(0.0, 1.0, 0.01, Style = RangeStyle.Both), Show]
    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public BaseBlendEffect() : base()
    {
        UpdateShaderValue
            (BlendModeProperty);
        UpdateShaderValue
            (OpacityProperty);
    }
}