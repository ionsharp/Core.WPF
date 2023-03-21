using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(Point))]
public class DoubleToPointMultiConverter : MultiConverter<Point>
{
    public DoubleToPointMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
        {
            if (values[0] is double x)
            {
                if (values[1] is double y)
                    return new Point(x, y);
            }
        }
        return default(Point);
    }
}