using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Controls;

[ValueConversion(typeof(Vector2), typeof(Vector3))]
public class XYAsVector2ToXYZAsVector3Converter : Conversion.Converter<Vector2, Vector3>
{
    public static XYAsVector2ToXYZAsVector3Converter Default { get; private set; } = new();
    public XYAsVector2ToXYZAsVector3Converter() { }

    protected override ConverterValue<Vector3> ConvertTo(ConverterData<Vector2> input) => (Vector3)(XYZ)(xy)input.Value;

    protected override ConverterValue<Vector2> ConvertBack(ConverterData<Vector3> input) => Nothing.Do;
}