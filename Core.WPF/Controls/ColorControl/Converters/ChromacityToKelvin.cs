using Imagin.Core.Colors;
using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Vector2), typeof(double))]
public class ChromacityToKelvinConverter : Converter<Vector2, double>
{
    public static ChromacityToKelvinConverter Default { get; private set; } = new();
    public ChromacityToKelvinConverter() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Vector2> input) => CCT.GetTemperature((xy)input.Value);

    protected override ConverterValue<Vector2> ConvertBack(ConverterData<double> input) => (Vector2)CCT.GetChromacity(input.Value);
}