using Imagin.Core.Linq;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(long))]
public class DoubleToInt64Converter : ValueConverter<double, long>
{
    public DoubleToInt64Converter() : base() { }

    protected override ConverterValue<long> ConvertTo(ConverterData<double> input) => input.Value.Int64();

    protected override ConverterValue<double> ConvertBack(ConverterData<long> input) => input.Value.Double();
}