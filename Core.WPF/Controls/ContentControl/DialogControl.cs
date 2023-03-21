using System.Windows;

namespace Imagin.Core.Controls;

public class DialogControl : ContentControl<DialogReference>
{
    public static readonly ResourceKey DropShadowEffectKey = new();

    public static readonly ResourceKey HeaderPatternKey = new();

    public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(ButtonStyle), typeof(DialogControl), new FrameworkPropertyMetadata(ButtonStyle.Apple));
    public ButtonStyle ButtonStyle
    {
        get => (ButtonStyle)GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }
    
    public DialogControl() : base() { }
}