using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(SolidColorBrush), typeof(ByteVector4))]
public class SolidColorBrushToByteVector4Converter : Converter<SolidColorBrush, ByteVector4>
{
    public static SolidColorBrushToByteVector4Converter Default { get; private set; } = new SolidColorBrushToByteVector4Converter();
    SolidColorBrushToByteVector4Converter() { }

    protected override ConverterValue<ByteVector4> ConvertTo(ConverterData<SolidColorBrush> input)
    {
        input.Value.Color.Convert(out ByteVector4 result);
        return result;
    }

    protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<ByteVector4> input) => new SolidColorBrush(XColor.Convert(input.Value));
}