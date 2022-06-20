using Imagin.Core.Colors;
using Imagin.Core.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(double), typeof(SolidColorBrush))]
    public class ColorTemperatureConverter : Converter<double, Color>
    {
        public static ColorTemperatureConverter Default { get; private set; } = new();
        public ColorTemperatureConverter() : base() { }

        protected override ConverterValue<Color> ConvertTo(ConverterData<double> input) => GetColor(input.Value);

        protected override ConverterValue<double> ConvertBack(ConverterData<Color> input) => Nothing.Do;
    
        public static Color GetColor(double input)
            => Linq.XColor.Convert(new RGB());
    }

    [ValueConversion(typeof(double), typeof(SolidColorBrush))]
    public class SolidColorBrushTemperatureConverter : Converter<double, SolidColorBrush>
    {
        public static SolidColorBrushTemperatureConverter Default { get; private set; } = new();
        public SolidColorBrushTemperatureConverter() : base() { }

        protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<double> input)
            => new SolidColorBrush(ColorTemperatureConverter.GetColor(input.Value));

        protected override ConverterValue<double> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
    }

    //...

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

            input.Value.Color.Convert(out RGB rgb);

            var hsb = new HSB();
            hsb.From(rgb, WorkingProfile.Default);

            Colors.Colour.New<HSB>(hsb.X, hsb.Y, lightness * 100).To(out rgb, WorkingProfile.Default);
            return new SolidColorBrush(Linq.XColor.Convert(rgb));
        }

        protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
    }
}