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
public class CenterToolTipMultiConverter : MultiConverter<double>
{
    public static CenterToolTipMultiConverter Default { get; private set; } = new CenterToolTipMultiConverter();
    CenterToolTipMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.FirstOrDefault(v => v == DependencyProperty.UnsetValue) != null)
        {
            return double.NaN;
        }
        double placementTargetWidth = (double)values[0];
        double toolTipWidth = (double)values[1];
        return (placementTargetWidth / 2.0) - (toolTipWidth / 2.0);
    }
}

[ValueConversion(typeof(object[]), typeof(double))]
public class MathMultiConverter : MultiConverter<double>
{
    public static MathMultiConverter Default { get; private set; } = new MathMultiConverter();
    MathMultiConverter() { }

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

[ValueConversion(typeof(object[]), typeof(double))]
public class ZoomMultiConverter : MultiConverter<double>
{
    public static ZoomMultiConverter Default { get; private set; } = new ZoomMultiConverter();
    ZoomMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values[0] is double)
        {
            var value = (double)values[0];
            var zoom = (double)values[1];

            return value / zoom;
        }
        return default(double);
    }
}