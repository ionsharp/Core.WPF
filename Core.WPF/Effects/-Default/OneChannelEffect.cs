using System.Windows;

namespace Imagin.Core.Effects;

public class OneChannelEffect : BaseEffect
{
    public static readonly DependencyProperty GreyProperty = DependencyProperty.Register(nameof(Grey), typeof(double), typeof(OneChannelEffect), new FrameworkPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(0)));
    public double Grey
    {
        get => (double)GetValue(GreyProperty);
        set => SetValue(GreyProperty, value);
    }

    public static readonly DependencyProperty ChannelProperty = DependencyProperty.Register(nameof(Channel), typeof(double), typeof(OneChannelEffect), new FrameworkPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(1)));
    public double Channel
    {
        get => (double)GetValue(ChannelProperty);
        set => SetValue(ChannelProperty, value);
    }

    public OneChannelEffect() : base()
    {
        UpdateShaderValue(GreyProperty);
        UpdateShaderValue(ChannelProperty);
    }
}