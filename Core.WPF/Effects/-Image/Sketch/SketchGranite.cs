using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Sketch))]
public class SketchGraniteEffect : ImageEffect
{
    public static readonly DependencyProperty BrushSizeProperty = DependencyProperty.Register(nameof(BrushSize), typeof(double), typeof(SketchGraniteEffect), new FrameworkPropertyMetadata(0.003, PixelShaderConstantCallback(0)));
    public double BrushSize
    {
        get => (double)GetValue(BrushSizeProperty);
        set => SetValue(BrushSizeProperty, value);
    }

    public SketchGraniteEffect() : base()
    {
        UpdateShaderValue(BrushSizeProperty);
    }
}