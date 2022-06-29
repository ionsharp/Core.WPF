using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Media;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(ByteVector4), typeof(Color))]
    public class ByteVector4ToColorConverter : Converter<ByteVector4, Color>
    {
        public static ByteVector4ToColorConverter Default { get; private set; } = new ByteVector4ToColorConverter();
        ByteVector4ToColorConverter() { }

        protected override ConverterValue<Color> ConvertTo(ConverterData<ByteVector4> input) => XColor.Convert(input.Value);

        protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<Color> input)
        {
            input.Value.Convert(out ByteVector4 j);
            return j;
        }
    }

    [ValueConversion(typeof(Color), typeof(Color))]
    public class ColorWithoutAlphaConverter : Converter<Color, Color>
    {
        public static ColorWithoutAlphaConverter Default { get; private set; } = new ColorWithoutAlphaConverter();
        ColorWithoutAlphaConverter() { }

        protected override ConverterValue<Color> ConvertTo(ConverterData<Color> input) => input.Value.A(255);

        protected override ConverterValue<Color> ConvertBack(ConverterData<Color> input) => Nothing.Do;
    }

    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    public class ColorToColorConverter : Converter<System.Drawing.Color, Color>
    {
        public static ColorToColorConverter Default { get; private set; } = new ColorToColorConverter();
        ColorToColorConverter() { }

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
    public class SolidColorBrushToColorConverter : Converter<SolidColorBrush, Color>
    {
        public static SolidColorBrushToColorConverter Default { get; private set; } = new SolidColorBrushToColorConverter();
        SolidColorBrushToColorConverter() { }

        protected override ConverterValue<Color> ConvertTo(ConverterData<SolidColorBrush> input) => input.Value.Color;

        protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<Color> input) => new SolidColorBrush(input.Parameter == 0 ? input.Value : input.Value.A(255));
    }
}