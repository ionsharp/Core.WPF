using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(ByteVector4), typeof(Color))]
public class ByteVector4ToColorConverter : ValueConverter<ByteVector4, Color>
{
    public ByteVector4ToColorConverter() : base() { }

    protected override ConverterValue<Color> ConvertTo(ConverterData<ByteVector4> input) => XColor.Convert(input.Value);

    protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<Color> input)
    {
        input.Value.Convert(out ByteVector4 j);
        return j;
    }
}

[ValueConversion(typeof(Color), typeof(Color))]
public class ColorWithoutAlphaConverter : ValueConverter<Color, Color>
{
    public ColorWithoutAlphaConverter() : base() { }

    protected override ConverterValue<Color> ConvertTo(ConverterData<Color> input) => input.Value.A(255);

    protected override ConverterValue<Color> ConvertBack(ConverterData<Color> input) => Nothing.Do;
}

[ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
public class ColorToColorConverter : ValueConverter<System.Drawing.Color, Color>
{
    public ColorToColorConverter() : base() { }

    protected override ConverterValue<Color> ConvertTo(ConverterData<System.Drawing.Color> input)
    {
            input.Value.Convert(out Color result);
        return result;
    }

    protected override ConverterValue<System.Drawing.Color> ConvertBack(ConverterData<Color> input)
    {
        input.Value.Convert(out System.Drawing.Color result);
        return result;
    }
}
    
[ValueConversion(typeof(SolidColorBrush), typeof(Color))]
public class SolidColorBrushToColorConverter : ValueConverter<SolidColorBrush, Color>
{
    public SolidColorBrushToColorConverter() : base() { }

    protected override ConverterValue<Color> ConvertTo(ConverterData<SolidColorBrush> input) => input.Value.Color;

    protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<Color> input) => new SolidColorBrush(input.Parameter == 0 ? input.Value : input.Value.A(255));
}