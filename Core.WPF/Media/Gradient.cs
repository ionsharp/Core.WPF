using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Imagin.Core.Media;

[Description("An array of position-dependent colors.")]
[Base(typeof(Gradient)), ContentProperty(nameof(Steps)), Categorize(false), Name(nameof(Gradient)), Image(SmallImages.Gradient), Serializable]
public class Gradient : Base, ICloneable
{
    public static Gradient Default => new(new GradientStep(0, System.Windows.Media.Colors.White), new GradientStep(1, System.Windows.Media.Colors.Black));

    public static Gradient Rainbow => new
    (
        new GradientStep(0.000, Color.FromArgb(255, 255,   0, 0)),
        new GradientStep(0.166, Color.FromArgb(255, 255, 255, 0)),
        new GradientStep(0.332, Color.FromArgb(255,   0, 255, 0)),
        new GradientStep(0.500, Color.FromArgb(255,   0, 255, 255)),
        new GradientStep(0.666, Color.FromArgb(255,   0,   0, 255)),
        new GradientStep(0.832, Color.FromArgb(255, 255,   0, 255)),
        new GradientStep(1.000, Color.FromArgb(255, 255,   0, 0))
    );

    ///

    public static LinearGradientBrush DefaultBrush => new(System.Windows.Media.Colors.White, System.Windows.Media.Colors.Black, Horizontal.X, Horizontal.Y);

    public static Vector2<Point> Horizontal => new(new Point(0, 0.5), new Point(1, 0.5));

    public static Vector2<Point> Vertical => new(new Point(0.5, 0), new Point(0.5, 1));

    ///

    [CollectionStyle(AddType = typeof(GradientStep))]
    [HideName, Categorize(false)]
    public GradientStepCollection Steps { get => Get(new GradientStepCollection()); private set => Set(value); }

    ///

    public Gradient() : base() { }

    public Gradient(params GradientStep[] input) : this()
    {
        input?.ForEach(i => Steps.Add(i));
    }

    public Gradient(GradientStepCollection input) : this()
    {
        input?.ForEach(i => Steps.Add(new GradientStep(i.Offset, i.Color)));
    }

    public Gradient(GradientStopCollection input) : this()
    {
        input?.ForEach(i =>
        {
            i.Color.Convert(out ByteVector4 j);
            Steps.Add(new GradientStep(i.Offset, j));
        });
    }

    public Gradient(LinearGradientBrush input) : this(input.GradientStops) { }

    public Gradient(RadialGradientBrush input) : this(input.GradientStops) { }

    ///

    object ICloneable.Clone() => Clone();
    public virtual Gradient Clone()
    {
        var result = new Gradient();
        result.CopyFrom(this);
        return result;
    }

    public LinearGradientBrush LinearBrush()
    {
        var result = new LinearGradientBrush()
        {
            EndPoint = Horizontal.Y,
            StartPoint = Horizontal.X,
            Opacity = 1,
        };

        Steps.ForEach(i => result.GradientStops.Add(new GradientStop(XColor.Convert(i.Color), i.Offset)));
        return result;
    }

    public RadialGradientBrush RadialBrush()
    {
        var result = new RadialGradientBrush()
        {
            RadiusX = 0.5,
            RadiusY = 0.5,
            Opacity = 1,
        };

        Steps.ForEach(i => result.GradientStops.Add(new GradientStop(XColor.Convert(i.Color), i.Offset)));
        return result;
    }

    public void CopyFrom(Gradient input)
    {
        Steps.Clear();
        foreach (var i in input.Steps)
            Steps.Add(new GradientStep(i.Offset, i.Color));
    }

    public virtual void Reset() => CopyFrom(Default);
}