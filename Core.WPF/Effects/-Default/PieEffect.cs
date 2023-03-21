using System.Windows;

namespace Imagin.Core.Effects;

public class PieEffect : BaseEffect
{
    public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(double), typeof(PieEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    public double Index
    {
        get => (double)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
    
    public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(double), typeof(PieEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(1)));
    public double Total
    {
        get => (double)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(PieEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(3)));
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public PieEffect() : base()
    {
        UpdateShaderValue(IndexProperty);
        UpdateShaderValue(TotalProperty);
    }
}