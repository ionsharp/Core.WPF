using System;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(TimeSpan), typeof(DateTime))]
public class TimeSpanToDateTimeConverter : ValueConverter<TimeSpan, DateTime>
{
    public TimeSpanToDateTimeConverter() : base() { }

    protected override ConverterValue<DateTime> ConvertTo(ConverterData<TimeSpan> input) => DateTime.Today.AddSeconds(input.Value.TotalSeconds);

    protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<DateTime> input) => Nothing.Do;
}

[ValueConversion(typeof(TimeSpan), typeof(string))]
public class TimeSpanToDateTimeStringConverter : ValueConverter<TimeSpan, string>
{
    public TimeSpanToDateTimeStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<TimeSpan> input)
    {
        var result = DateTime.Today.AddSeconds(input.Value.TotalSeconds);
        if (input.ActualParameter is string format)
            return result.ToString(format);

        return $"{result}";
    }

    protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<string> input) => Nothing.Do;
}