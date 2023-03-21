using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Stroke))]
public class StrokeEffect : ImageEffect
{
    public static readonly DependencyProperty ThreshholdProperty = DependencyProperty.Register(nameof(Threshhold), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(0.5, PixelShaderConstantCallback(0)));
    /// <summary>The threshold of the edge detection.</summary>
    public double Threshhold
    {
        get => (double)GetValue(ThreshholdProperty);
        set => SetValue(ThreshholdProperty, value);
    }

    public static readonly DependencyProperty K00Property = DependencyProperty.Register(nameof(K00), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(1)));
    /// <summary>Kernel first column top. Default is the Sobel operator.</summary>
    public double K00
    {
        get => (double)GetValue(K00Property);
        set => SetValue(K00Property, value);
    }

    public static readonly DependencyProperty K01Property = DependencyProperty.Register(nameof(K01), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(2.0, PixelShaderConstantCallback(2)));
    /// <summary>Kernel first column middle. Default is the Sobel operator.</summary>
    public double K01
    {
        get => (double)GetValue(K01Property);
        set => SetValue(K01Property, value);
    }

    public static readonly DependencyProperty K02Property = DependencyProperty.Register(nameof(K02), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(3)));
    /// <summary>Kernel first column bottom. Default is the Sobel operator.</summary>
    public double K02
    {
        get => (double)GetValue(K02Property);
        set => SetValue(K02Property, value);
    }

    public static readonly DependencyProperty TextureSizeProperty = DependencyProperty.Register(nameof(TextureSize), typeof(Point), typeof(StrokeEffect), new FrameworkPropertyMetadata(new Point(512.0, 512.0), PixelShaderConstantCallback(4)));
    /// <summary>The size of the texture.</summary>
    public Point TextureSize
    {
        get => (Point)GetValue(TextureSizeProperty);
        set => SetValue(TextureSizeProperty, value);
    }

    public static readonly DependencyProperty SizeXProperty = DependencyProperty.Register(nameof(SizeX), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(512.0, OnSizeChanged));
    /// <summary>The size of the texture (X).</summary>
    public double SizeX
    {
        get => (double)GetValue(SizeXProperty);
        set => SetValue(SizeXProperty, value);
    }

    public static readonly DependencyProperty SizeYProperty = DependencyProperty.Register(nameof(SizeY), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(512.0, OnSizeChanged));
    /// <summary>The size of the texture (Y).</summary>
    public double SizeY
    {
        get => (double)GetValue(SizeYProperty);
        set => SetValue(SizeYProperty, value);
    }

    static void OnSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as StrokeEffect).TextureSize = new((sender as StrokeEffect).SizeX, (sender as StrokeEffect).SizeY);

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(Color), typeof(StrokeEffect), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black, PixelShaderConstantCallback(5)));
    public Color Stroke
    {
        get => (Color)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(StrokeEffect), new FrameworkPropertyMetadata(3.0, PixelShaderConstantCallback(6)));
    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public StrokeEffect() : base()
    {
        UpdateShaderValue(K00Property);
        UpdateShaderValue(K01Property);
        UpdateShaderValue(K02Property);
        UpdateShaderValue(TextureSizeProperty);
        UpdateShaderValue(ThreshholdProperty);
        UpdateShaderValue(StrokeProperty);
        UpdateShaderValue(StrokeThicknessProperty);
    }
}