using System.Windows;

namespace Imagin.Core.Effects;

[Category(nameof(ImageEffectCategory.Noise))]
public class NoiseEffect : ImageEffect
{
    public enum Modes { Add, Subtract, AddOrSubtract }

    new public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(nameof(Amount), typeof(double), typeof(NoiseEffect), new UIPropertyMetadata(0d, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(Modes), typeof(NoiseEffect), new UIPropertyMetadata(Modes.Add, OnModeChanged));
    [Pin(Pin.AboveOrLeft), Show]
    public Modes Mode
    {
        get => (Modes)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    static void OnModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as NoiseEffect).ModeIndex = (int)(Modes)e.NewValue;

    public static readonly DependencyProperty ModeIndexProperty = DependencyProperty.Register(nameof(ModeIndex), typeof(double), typeof(NoiseEffect), new UIPropertyMetadata(0d, PixelShaderConstantCallback(1)));
    public double ModeIndex
    {
        get => (double)GetValue(ModeIndexProperty);
        private set => SetValue(ModeIndexProperty, value);
    }

    public NoiseEffect() : base()
    {
        UpdateShaderValue(AmountProperty); UpdateShaderValue(ModeProperty);
    }
}