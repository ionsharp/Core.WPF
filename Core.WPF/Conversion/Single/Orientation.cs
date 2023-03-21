using Imagin.Core.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Orientation), typeof(Orientation))]
public class InverseOrientationConverter : ValueConverter<Orientation, Orientation>
{
    public InverseOrientationConverter() : base() { }

    protected override ConverterValue<Orientation> ConvertTo(ConverterData<Orientation> input) => input.Value.Invert();

    protected override ConverterValue<Orientation> ConvertBack(ConverterData<Orientation> input) => input.Value.Invert();
}