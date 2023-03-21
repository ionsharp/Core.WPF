using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(Point))]
public class DoubleToPointConverter : ValueConverter<double, Point>
{
    public DoubleToPointConverter() : base() { }

    protected override ConverterValue<Point> ConvertTo(ConverterData<double> input)
        => new Point(input.Value, input.Value);
}