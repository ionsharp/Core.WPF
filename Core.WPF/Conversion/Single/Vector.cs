using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(Vector2))]
public class KelvinToChromacityConverter : ValueConverter<double, Vector2>
{
    public KelvinToChromacityConverter() : base() { }

    protected override ConverterValue<Vector2> ConvertTo(ConverterData<double> input)
    {
        var result = (Vector2)CCT.GetChromacity(input.Value);
        return result.Transform((i, j) => input.ActualParameter != null ? j.Round(input.Parameter) : j);
    }

    protected override ConverterValue<double> ConvertBack(ConverterData<Vector2> input) => CCT.GetTemperature((xy)input.Value);
}

[ValueConversion(typeof(Vector2), typeof(Vector3))]
public class XYAsVector2ToXYZAsVector3Converter : ValueConverter<Vector2, Vector3>
{
    public XYAsVector2ToXYZAsVector3Converter() : base() { }

    protected override ConverterValue<Vector3> ConvertTo(ConverterData<Vector2> input) => (Vector3)(XYZ)(xy)input.Value;

    protected override ConverterValue<Vector2> ConvertBack(ConverterData<Vector3> input) => Nothing.Do;
}