using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect mimics the look of glass tiles.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class TilesEffect : ImageEffect
{
    public static readonly DependencyProperty TilesProperty = DependencyProperty.Register(nameof(Tiles), typeof(double), typeof(TilesEffect), new FrameworkPropertyMetadata(5.0, PixelShaderConstantCallback(0)));
    /// <summary>The approximate number of tiles per row/column.</summary>
    public double Tiles
    {
        get => (double)GetValue(TilesProperty);
        set => SetValue(TilesProperty, value);
    }

    public static readonly DependencyProperty BevelWidthProperty = DependencyProperty.Register(nameof(BevelWidth), typeof(double), typeof(TilesEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(1)));
    public double BevelWidth
    {
        get => (double)GetValue(BevelWidthProperty);
        set => SetValue(BevelWidthProperty, value);
    }
        
    public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(nameof(Offset), typeof(double), typeof(TilesEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(3)));
    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }
        
    public static readonly DependencyProperty GroutColorProperty = DependencyProperty.Register(nameof(GroutColor), typeof(Color), typeof(TilesEffect), new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 0, 0), PixelShaderConstantCallback(2)));
    public Color GroutColor
    {
        get => (Color)GetValue(GroutColorProperty);
        set => SetValue(GroutColorProperty, value);
    }

    public TilesEffect() : base()
    {
        UpdateShaderValue(TilesProperty);
        UpdateShaderValue(BevelWidthProperty);
        UpdateShaderValue(OffsetProperty);
        UpdateShaderValue(GroutColorProperty);
    }
}