using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(One))]
public class DoubleToOneConverter : Converter<double, One>
{
    public static DoubleToOneConverter Default { get; private set; } = new();
    public DoubleToOneConverter() { }

    protected override ConverterValue<One> ConvertTo(ConverterData<double> input) => (One)input.Value;

    protected override ConverterValue<double> ConvertBack(ConverterData<One> input) => (double)input.Value;
}