using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

public class MultiConverterData
{
    public readonly CultureInfo Culture;

    public readonly object[] Values;

    public readonly object Parameter;

    public readonly Type TargetType;

    public MultiConverterData(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Values = values; TargetType = targetType; Parameter = parameter; Culture = culture;
    }
}

public abstract class MultiConverter : IMultiValueConverter
{
    public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();

    public static T Get<T>() where T : IMultiValueConverter => (T)Converter.Instances[typeof(T)];
}

[ValueConversion(typeof(object[]), typeof(object))]
public class MultiConverter<Result> : MultiConverter
{
    readonly Func<MultiConverterData, Result> To;

    public MultiConverter() : base() { }

    public MultiConverter(Func<MultiConverterData, Result> to) => To = to;

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => To.Invoke(new MultiConverterData(values, targetType, parameter, culture));

    public Result Convert(object value, object parameter = null) => Convert(new object[] { value }, parameter);

    public Result Convert(object[] values, object parameter = null) => (Result)Convert(values, null, parameter, null);
}