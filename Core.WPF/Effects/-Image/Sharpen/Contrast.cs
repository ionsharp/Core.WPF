using System.Windows;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Sharpen))]
public class ContrastEffect : ImageEffect
{
    public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register(nameof(Contrast), typeof(double), typeof(ContrastEffect), new FrameworkPropertyMetadata(0d, PixelShaderConstantCallback(0)));
    public double Contrast
    {
        get => (double)GetValue(ContrastProperty);
        set => SetValue(ContrastProperty, value);
    }

    public ContrastEffect() : this(0) { }

    public ContrastEffect(double contrast) : base()
    {
        UpdateShaderValue(ContrastProperty);
        SetCurrentValue(ContrastProperty, contrast);
    }
}