using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

[Category(ImageEffectCategory.Blend), Name("Color overlay"), Explicit]
public class ColorOverlayEffect : BaseBlendEffect
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorOverlayEffect), new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 0, 0), PixelShaderConstantCallback(2)));
    [Show]
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly DependencyProperty ReverseProperty = DependencyProperty.Register(nameof(Reverse), typeof(bool), typeof(ColorOverlayEffect), new FrameworkPropertyMetadata(false, PixelShaderConstantCallback(3)));
    [Show]
    public bool Reverse
    {
        get => (bool)GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }

    public ColorOverlayEffect() : base()
    {
        UpdateShaderValue(ColorProperty);
        UpdateShaderValue(ReverseProperty);
    }
}