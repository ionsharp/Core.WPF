using Imagin.Core.Linq;
using Imagin.Core.Colors;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Converters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToSolidColorBrushConverter : Converter<Color, SolidColorBrush>
    {
        public static ColorToSolidColorBrushConverter Default { get; private set; } = new ColorToSolidColorBrushConverter();
        ColorToSolidColorBrushConverter() { }

        protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<Color> input) => new SolidColorBrush(input.Parameter == 0 ? input.Value : input.Value.A(255));

        protected override ConverterValue<Color> ConvertBack(ConverterData<SolidColorBrush> input) => input.Value.Color;
    }

    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class LightnessConverter : Converter<SolidColorBrush, SolidColorBrush>
    {
        public static LightnessConverter Default { get; private set; } = new LightnessConverter();
        LightnessConverter() { }

        protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<SolidColorBrush> input)
        {
            var lightness 
                = input.ActualParameter.Double();

            var rgb = Xrgb.Convert(input.Value.Color);

            var hsb = new HSB();
            hsb.FromRGB(rgb, WorkingProfile.Default.sRGB);

            rgb = new HSB(hsb[0], hsb[1], lightness * 100).ToRGB(WorkingProfile.Default.sRGB);
            return new SolidColorBrush(XColor.Convert(rgb));
        }

        protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
    }
}