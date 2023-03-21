using Imagin.Core.Media;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(System.Windows.Media.Geometry))]
public class ShapeToGeometryMultiConverter : MultiConverter<System.Windows.Media.Geometry>
{
    public ShapeToGeometryMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 3)
        {
            if (values[0] is Shape shape)
            {
                if (values[1] is double height)
                {
                    if (values[2] is double width)
                        return shape.GetGeometry(height, width);
                }
            }
        }
        return Binding.DoNothing;
    }
}