using Imagin.Core.Colors;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Component4), typeof(double))]
public class Component4ToDoubleConverter : Converter<Component4, double>
{
    public static Component4ToDoubleConverter Default { get; private set; } = new();
    public Component4ToDoubleConverter() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Component4> input) => (int)input.Value;

    protected override ConverterValue<Component4> ConvertBack(ConverterData<double> input) => (Component4)(int)input.Value;
}