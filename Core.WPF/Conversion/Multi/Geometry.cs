using Imagin.Core.Media;
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(Geometry))]
public class ShapeClipMultiConverter : MultiConverter<object>
{
    public ShapeClipMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 1)
        {
            if (values[0] is Shape i)
                return Try.Return(i.GetNormalGeometry) ?? Binding.DoNothing;
        }
        else if (values?.Length == 2)
        {
            if (values[0] is IList i)
            {
                if (values[1] is int j)
                {
                    if (j >= 0 && j < i.Count)
                    {
                        if (i[j] is NamableCategory<Shape> k)
                            return Try.Return(k.Value.GetNormalGeometry) ?? Binding.DoNothing;
                    }
                }
            }
        }
        return Binding.DoNothing;
    }
}