using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Imagin.Core.Media;

/// <summary>Describes the location and color of a transition point in a gradient.</summary>
[Description("Describes the location and color of a transition point in a gradient.")]
[Categorize(false), HideName, Image(SmallImages.Gradient), Name("Gradient step"), Serializable]
public class GradientStep : Base
{
    public Color ActualColor { set { value.Convert(out ByteVector4 result); Color = result; } }

    [Horizontal, XmlElement]
    public ByteVector4 Color { get => Get<ByteVector4>(); set => Set(value); }

    [Horizontal, Range(0.0, 1.0, 0.01, Style = RangeStyle.Both), XmlAttribute]
    public double Offset { get => Get(.0); set => Set(value); }

    public GradientStep() : base() { }

    public GradientStep(double offset, ByteVector4 color) : this()
    {
        Offset = offset; Color = color;
    }

    public GradientStep(double offset, Color color) : this(offset, new ByteVector4(color.R, color.G, color.B, color.A)) { }
}