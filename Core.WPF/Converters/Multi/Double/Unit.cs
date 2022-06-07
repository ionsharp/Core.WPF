using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Converters;

[ValueConversion(typeof(double), typeof(double))]
public class UnitMultiConverter : MultiConverter<double>
{
    public static UnitMultiConverter Default { get; private set; } = new();
    UnitMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 4)
        {
            if (values[0] is double value)
            {
                if (values[1] is float resolution)
                {
                    if (values[2] is Unit from)
                    {
                        if (values[3] is Unit to)
                        {
                            var result = from.Convert(to, value, resolution);
                            if (values.Length >= 5)
                            {
                                if (values[4] is double scale)
                                    return result * scale;
                            }
                            return result;
                        }
                    }
                }
            }
        }
        return Binding.DoNothing;
    }
}