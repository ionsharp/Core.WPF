using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(ByteVector4), typeof(SolidColorBrush))]
public class ByteVector4ToBlackOrWhiteConverter : ValueConverter<ByteVector4, SolidColorBrush>
{
    public ByteVector4ToBlackOrWhiteConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<ByteVector4> input)
    {
        var color = XColor.Convert(input.Value);
        color.Convert(out System.Drawing.Color result);

        var parameter = 0.5;
        if (input.ActualParameter != null)
            double.TryParse($"{input.ActualParameter}", out parameter);

        return new SolidColorBrush(result.GetBrightness() > parameter ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.White);
    }

    protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(SolidColorBrush))]
public class ByteVector4ToSolidColorBrushConverter : ValueConverter<object, SolidColorBrush>
{
    public ByteVector4ToSolidColorBrushConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<object> input)
    {
        ByteVector4 color = input.Value is ByteVector4 i ? i : (input.Value?.GetType() == typeof(ByteVector4?) ? ((ByteVector4?)input.Value).Value : default);
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<SolidColorBrush> input) => new ByteVector4(input.Value.Color.R, input.Value.Color.G, input.Value.Color.B, input.Value.Color.A);
}

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public class ColorToBlackOrWhiteConverter : ValueConverter<Color, SolidColorBrush>
{
    public ColorToBlackOrWhiteConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<Color> input)
    {
        var color = input.Value;
        color.Convert(out System.Drawing.Color result);

        var parameter = 0.5;
        if (input.ActualParameter != null)
            double.TryParse($"{input.ActualParameter}", out parameter);

        return new SolidColorBrush(result.GetBrightness() > parameter ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.White);
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
}

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public class ColorToSolidColorBrushConverter : ValueConverter<Color, SolidColorBrush>
{
    public ColorToSolidColorBrushConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<Color> input) => new SolidColorBrush(input.Parameter == 0 ? input.Value : input.Value.A(255));

    protected override ConverterValue<Color> ConvertBack(ConverterData<SolidColorBrush> input) => input.Value.Color;
}

[ValueConversion(typeof(double), typeof(SolidColorBrush))]
public class ColorTemperatureConverter : ValueConverter<double, Color>
{
    public ColorTemperatureConverter() : base() { }

    protected override ConverterValue<Color> ConvertTo(ConverterData<double> input) => GetColor(input.Value);

    protected override ConverterValue<double> ConvertBack(ConverterData<Color> input) => Nothing.Do;

    public static Color GetColor(double input)
        => Linq.XColor.Convert(new RGB());
}

[ValueConversion(typeof(double), typeof(SolidColorBrush))]
public class SolidColorBrushTemperatureConverter : ValueConverter<double, SolidColorBrush>
{
    public SolidColorBrushTemperatureConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<double> input)
        => new SolidColorBrush(ColorTemperatureConverter.GetColor(input.Value));

    protected override ConverterValue<double> ConvertBack(ConverterData<SolidColorBrush> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(SolidColorBrush))]
public class StringToSolidColorBrushConverter : ValueConverter<string, SolidColorBrush>
{
    public StringToSolidColorBrushConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<string> input)
    {
        ByteVector4 color = new ByteVector4(input.Value);
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<SolidColorBrush> input) => new ByteVector4(input.Value.Color.R, input.Value.Color.G, input.Value.Color.B, input.Value.Color.A).ToString(false);
}

[ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
public class SolidColorBrushBrightnessConverter : ValueConverter<SolidColorBrush, SolidColorBrush>
{
    public SolidColorBrushBrightnessConverter() : base() { }

    protected override ConverterValue<SolidColorBrush> ConvertTo(ConverterData<SolidColorBrush> input)
    {
        var color = input.Value.Color;

        XColor.Convert(color, out RGB rgb);
        var hsl = rgb.To<HSL>(WorkingProfile.Default);

        double.TryParse(input.ActualParameter?.ToString(), out double z);
        hsl.Z = z;

        rgb = hsl.To<RGB>(WorkingProfile.Default);

        var result = XColor.Convert(rgb);
        return new SolidColorBrush(result);
    }
}