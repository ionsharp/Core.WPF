using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(Vector2))]
public class KelvinToChromacityConverter : Converter<double, Vector2>
{
    public static KelvinToChromacityConverter Default { get; private set; } = new();
    public KelvinToChromacityConverter() { }

    protected override ConverterValue<Vector2> ConvertTo(ConverterData<double> input)
    {
        var result = (Vector2)CCT.GetChromacity(input.Value);
        return result.Transform((i, j) => input.ActualParameter != null ? j.Round(input.Parameter) : j);
    }

    protected override ConverterValue<double> ConvertBack(ConverterData<Vector2> input) => CCT.GetTemperature((xy)input.Value);
}