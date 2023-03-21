using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Effects;

/// <summary>An effect that turns the input into blocky pixels.</summary>
[Category(nameof(ImageEffectCategory.Distort))]
public class PixelateEffect : ImageEffect
{
    public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(nameof(Offset), typeof(double), typeof(PixelateEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    /// <summary>The amount to shift alternate rows (use 1 to get a brick wall look).</summary>
    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(Size), typeof(PixelateEffect), new FrameworkPropertyMetadata(new Size(60.0, 40.0), PixelShaderConstantCallback(1)));
    /// <summary>The number of horizontal and vertical pixel blocks.</summary>
    public Size Size
    {
        get => (Size)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly DependencyProperty PixelHeightProperty = DependencyProperty.Register(nameof(PixelHeight), typeof(double), typeof(PixelateEffect), new FrameworkPropertyMetadata(60.0, OnSizeChanged));
    /// <summary>The number of horizontal and vertical pixel blocks.</summary>
    public double PixelHeight
    {
        get => (double)GetValue(PixelHeightProperty);
        set => SetValue(PixelHeightProperty, value);
    }

    public static readonly DependencyProperty PixelWidthProperty = DependencyProperty.Register(nameof(PixelWidth), typeof(double), typeof(PixelateEffect), new FrameworkPropertyMetadata(40.0, OnSizeChanged));
    /// <summary>The number of horizontal and vertical pixel blocks.</summary>
    public double PixelWidth
    {
        get => (double)GetValue(PixelWidthProperty);
        set => SetValue(PixelWidthProperty, value);
    }

    static void OnSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as PixelateEffect).Size = new((sender as PixelateEffect).PixelWidth, (sender as PixelateEffect).PixelHeight);
        
    public PixelateEffect() : base()
    {
        UpdateShaderValue(OffsetProperty);
        UpdateShaderValue(SizeProperty);
    }
}