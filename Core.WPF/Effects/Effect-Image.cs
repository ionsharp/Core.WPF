using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Effects;

public enum ImageEffectCategory { Blend, Blur, Color, Distort, Noise, Sharpen, Sketch, Stroke }

[Description("An effect that can be applied to an image.")]
[Categorize(false), Editable, HideName, Horizontal, Image(SmallImages.Fx)]
public abstract class ImageEffect : BaseEffect, ICloneable
{
    public string Category => GetType().GetAttribute<CategoryAttribute>()?.Category?.ToString();

    protected override string FilePath => $"-Image/{Category}/{Name}.ps";

    public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(nameof(Amount), typeof(double), typeof(ImageEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(0)));
    [Pin(Pin.BelowOrRight), Modify, Range(0.0, 1.0, 0.01, Style = RangeStyle.Both), Show]
    public virtual double Amount
    {
        get => (double)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }

    public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(ImageEffect), new FrameworkPropertyMetadata(true));
    [Float(Float.Above), CheckedImage(SmallImages.Hidden), Name("Show/hide"), HideName, Image(SmallImages.Visible), Index(-1), Modify, Show, Style(BooleanStyle.Button)]
    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    public string Name => GetType().Name.Replace(nameof(Effect), "");

    public ImageEffect() : base() { }

    public virtual Color Apply(Color color, double amount = 1) => color;

    public virtual ColorMatrix Apply(ColorMatrix input)
    {
        ColorMatrix result = new(input.Rows, input.Columns);
        input.Each((y, x, oldColor) =>
        {
            var newColor = Apply(Color.FromArgb(M.Denormalize(oldColor.W), M.Denormalize(oldColor.X), M.Denormalize(oldColor.Y), M.Denormalize(oldColor.Z)), Amount);
            result.SetValue(y.UInt32(), x.UInt32(), new(M.Normalize(newColor.A), M.Normalize(newColor.R), M.Normalize(newColor.G), M.Normalize(newColor.B)));
            return oldColor;
        });
        return result;
    }

    public virtual void Apply(WriteableBitmap bitmap)
    {
        bitmap.ForEach((x, y, color) =>
        {
            Apply(color, Amount);
            return color;
        });
    }

    object ICloneable.Clone() => Clone();
    public virtual ImageEffect Clone(bool Log = false) => this.DeepClone(new CloneHandler(), Log);
}