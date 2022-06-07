using Imagin.Core.Linq;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Converters;

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class AandBorCMultiConverter : MultiConverter<Visibility>
{
    public static AandBorCMultiConverter Default { get; private set; } = new();
    AandBorCMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 3)
        {
            if (values[0] is bool a)
            {
                if (values[1] is bool b)
                {
                    if (values[2] is bool c)
                    {
                        return (a && (b || c)).Visibility();
                    }
                }
            }
        }
        return Binding.DoNothing;
    }
}