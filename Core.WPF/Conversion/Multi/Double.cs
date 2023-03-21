using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(double))]
public class HorizontalCenterMultiConverter : MultiConverter<double>
{
    public HorizontalCenterMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.FirstOrDefault(i => i == DependencyProperty.UnsetValue) != null)
            return double.NaN;

        double aWidth = (double)values[0];
        double bWidth = (double)values[1];
        return (aWidth / 2.0) - (bWidth / 2.0);
    }
}

[ValueConversion(typeof(object[]), typeof(double))]
public class MathMultiConverter : MultiConverter<double>
{
    public MathMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 3)
        {
            if (values[0] is double a && values[2] is double b)
            {
                var operation = (NumberOperation)values[1];
                switch (operation)
                {
                    case NumberOperation.Add:
                        return a + b;
                    case NumberOperation.Divide:
                        return a / b;
                    case NumberOperation.Multiply:
                        return a * b;
                    case NumberOperation.Subtract:
                        return a - b;
                }
            }
            else Log.Write<MathMultiConverter>(new Warning($"a = {values[0].NullString()}, b = {values[2].NullString()}"));
        }
        else if (values?.Length > 0)
        {
            if (values[0] is double firstValue)
            {
                var result = firstValue;
                NumberOperation? m = null;
                for (var i = 1; i < values.Length; i++)
                {
                    if (values[i] is NumberOperation operation)
                    {
                        m = operation;
                    }
                    else if (m != null && values[i] is double nextValue)
                    {
                        switch (m)
                        {
                            case NumberOperation.Add:
                                result += nextValue;
                                break;
                            case NumberOperation.Divide:
                                result /= nextValue;
                                break;
                            case NumberOperation.Multiply:
                                result *= nextValue;
                                break;
                            case NumberOperation.Subtract:
                                result -= nextValue;
                                break;
                        }
                        m = null;
                    }
                }
                return result;
            }
        }
        return default(double);
    }
}

[ValueConversion(typeof(object[]), typeof(double))]
public class OpacityMultiConverter : MultiConverter<double>
{
    public OpacityMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var result = 1.0;
        if (values?.Length > 0)
        {
            foreach (var i in values)
            {
                if (i is double j)
                    result *= j;
            }
        }
        return result;
    }
}

[ValueConversion(typeof(object[]), typeof(double))]
public class ZoomMultiConverter : MultiConverter<double>
{
    public ZoomMultiConverter() : base(i => i.Values[0] is double value && i.Values[1] is double zoom && zoom != 0 ? value / zoom : default) { }
}