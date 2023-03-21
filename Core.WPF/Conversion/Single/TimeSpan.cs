using System;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(TimeSpan))]
public class DoubleToTimeSpanConverter : ValueConverter<double, TimeSpan>
{
    public DoubleToTimeSpanConverter() : base() { }

    protected override ConverterValue<TimeSpan> ConvertTo(ConverterData<double> input) => TimeSpan.FromSeconds(input.Value);

    protected override ConverterValue<double> ConvertBack(ConverterData<TimeSpan> input) => input.Value.TotalSeconds;
}