using Imagin.Core.Media;
using System;
using System.Windows.Markup;

namespace Imagin.Core.Markup;

public class PolygonGeometry : MarkupExtension
{
    public double Angle { get; set; } = 0;

    public double Height { get; set; } = 100;

    public double Width { get; set; } = 100;

    public uint Sides { get; set; } = 3;

    public PolygonGeometry() : base() { }

    public override object ProvideValue(IServiceProvider serviceProvider) => Shape.GetPolygonGeometry(Sides, Angle, Height, Width);
}