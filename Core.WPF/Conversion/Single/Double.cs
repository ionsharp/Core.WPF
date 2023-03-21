using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows.Data;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(long), typeof(double))]
public class BytesToMegaBytesConverter : ValueConverter<long, double>
{
    public BytesToMegaBytesConverter() : base() { }

    protected override bool AllowNull => true;

    protected override ConverterValue<double> ConvertTo(ConverterData<long> input)
    {
        if (input.ActualValue == null)
            return 0;

        double.TryParse(input.Value.ToString(), out double result);
        return (result / 1024d / 1024d).Round(3);
    }

    protected override ConverterValue<long> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(Vector2), typeof(double))]
public class ChromacityToKelvinConverter : ValueConverter<Vector2, double>
{
    public ChromacityToKelvinConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Vector2> input) => CCT.GetTemperature((xy)input.Value);

    protected override ConverterValue<Vector2> ConvertBack(ConverterData<double> input) => (Vector2)CCT.GetChromacity(input.Value);
}

[ValueConversion(typeof(Component4), typeof(double))]
public class Component4ToDoubleConverter : ValueConverter<Component4, double>
{
    public Component4ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Component4> input) => (int)input.Value;

    protected override ConverterValue<Component4> ConvertBack(ConverterData<double> input) => (Component4)(int)input.Value;
}

[ValueConversion(typeof(double), typeof(double))]
public class HalfDoubleConverter : ValueConverter<double, double>
{
    public HalfDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<double> input) => input.Value / 2.0;

    protected override ConverterValue<double> ConvertBack(ConverterData<double> input) => input.Value * 2.0;
}

[ValueConversion(typeof(double), typeof(double))]
public class InverseDoubleConverter : ValueConverter<double, double>
{
    public InverseDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<double> input) => 1 - Clamp(input.Value, 1);

    protected override ConverterValue<double> ConvertBack(ConverterData<double> input) => 1 - Clamp(input.Value, 1);
}

[ValueConversion(typeof(double), typeof(double))]
public class PercentConverter : ValueConverter<double, double>
{
    public PercentConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<double> input) => input.Value * 100.0;

    protected override ConverterValue<double> ConvertBack(ConverterData<double> input) => input.Value / 100.0;
}

[ValueConversion(typeof(double), typeof(string))]
public class PercentStringConverter : ValueConverter<double, string>
{
    public PercentStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<double> input) => $"{(input.Value * 100.0).Round(input.ActualParameter != null ? input.Parameter : int.MaxValue)}%";

    protected override ConverterValue<double> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(Color), typeof(double))]
public class HueConverter : ValueConverter<Color, double>
{
    public HueConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out System.Drawing.Color result);
        return result.GetHue();
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(Color), typeof(double))]
public class SaturationConverter : ValueConverter<Color, double>
{
    public SaturationConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out System.Drawing.Color result);
        return result.GetSaturation();
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(Color), typeof(double))]
public class BrightnessConverter : ValueConverter<Color, double>
{
    public BrightnessConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out System.Drawing.Color result);
        return result.GetBrightness();
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

///

[ValueConversion(typeof(double), typeof(double))]
public class RadiusToDiameterConverter : ValueConverter<double, double>
{
    public RadiusToDiameterConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<double> input) => input.Value * 2.0;

    protected override ConverterValue<double> ConvertBack(ConverterData<double> input) => input.Value / 2.0;
}

///

[ValueConversion(typeof(int), typeof(double))]
public class SubtractConverter : ValueConverter<int, double>
{
    public SubtractConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<int> input) => input.Value - input.Parameter;

    protected override ConverterValue<int> ConvertBack(ConverterData<double> input) => (input.Value + input.Parameter).Int32();
}

///

[ValueConversion(typeof(byte), typeof(double))]
public class ByteToDoubleConverter : ValueConverter<byte, double>
{
    public ByteToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<byte> input) => input.Value.Double() / byte.MaxValue.Double();

    protected override ConverterValue<byte> ConvertBack(ConverterData<double> input) => (input.Value * byte.MaxValue.Double()).Byte();
}

[ValueConversion(typeof(DateTime), typeof(double))]
public class DateTimeToDoubleConverter : ValueConverter<DateTime, double>
{
    public DateTimeToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<DateTime> input) => input.Value.Ticks.Double();

    protected override ConverterValue<DateTime> ConvertBack(ConverterData<double> input) => new DateTime(input.Value.Int64());
}

[ValueConversion(typeof(decimal), typeof(double))]
public class DecimalToDoubleConverter : ValueConverter<decimal, double>
{
    public DecimalToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<decimal> input) => input.Value.Double();

    protected override ConverterValue<decimal> ConvertBack(ConverterData<double> input) => input.Value.Decimal();
}

[ValueConversion(typeof(short), typeof(double))]
public class Int16ToDoubleConverter : ValueConverter<short, double>
{
    public Int16ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<short> input) => input.Value.Double();

    protected override ConverterValue<short> ConvertBack(ConverterData<double> input) => input.Value.Int16();
}

[ValueConversion(typeof(int), typeof(double))]
public class Int32ToDoubleConverter : ValueConverter<int, double>
{
    public Int32ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<int> input) => input.Value.Double();

    protected override ConverterValue<int> ConvertBack(ConverterData<double> input) => input.Value.Int32();
}

[ValueConversion(typeof(long), typeof(double))]
public class Int64ToDoubleConverter : ValueConverter<long, double>
{
    public Int64ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<long> input) => input.Value.Double();

    protected override ConverterValue<long> ConvertBack(ConverterData<double> input) => input.Value.Int64();
}

[ValueConversion(typeof(object), typeof(double))]
public class ObjectToDoubleConverter : ValueConverter<object, double>
{
    public ObjectToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<object> input) => input.Value.Double() is double result && double.IsNaN(result) ? Nothing.Do : result;

    protected override ConverterValue<object> ConvertBack(ConverterData<double> input) => input.Value;
}

[ValueConversion(typeof(One), typeof(double))]
public class OneToDoubleConverter : ValueConverter<One, double>
{
    public OneToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<One> input) => (double)input.Value;

    protected override ConverterValue<One> ConvertBack(ConverterData<double> input) => (One)input.Value;
}

[ValueConversion(typeof(float), typeof(double))]
public class SingleToDoubleConverter : ValueConverter<float, double>
{
    public SingleToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<float> input) => input.Value.Double();

    protected override ConverterValue<float> ConvertBack(ConverterData<double> input) => input.Value.Single();
}

[ValueConversion(typeof(TimeSpan), typeof(double))]
public class TimeSpanToDoubleConverter : ValueConverter<TimeSpan, double>
{
    public TimeSpanToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<TimeSpan> input) => input.Value.TotalMilliseconds;

    protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<double> input) => TimeSpan.FromMilliseconds(input.Value);
}

[ValueConversion(typeof(UDouble), typeof(double))]
public class UDoubleToDoubleConverter : ValueConverter<UDouble, double>
{
    public UDoubleToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<UDouble> input) => (double)input.Value;

    protected override ConverterValue<UDouble> ConvertBack(ConverterData<double> input) => (UDouble)input.Value;
}

[ValueConversion(typeof(ushort), typeof(double))]
public class UInt16ToDoubleConverter : ValueConverter<ushort, double>
{
    public UInt16ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<ushort> input) => input.Value.Double();

    protected override ConverterValue<ushort> ConvertBack(ConverterData<double> input) => input.Value.UInt16();
}

[ValueConversion(typeof(uint), typeof(double))]
public class UInt32ToDoubleConverter : ValueConverter<uint, double>
{
    public UInt32ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<uint> input) => input.Value.Double();

    protected override ConverterValue<uint> ConvertBack(ConverterData<double> input) => input.Value.UInt32();
}

[ValueConversion(typeof(ulong), typeof(double))]
public class UInt64ToDoubleConverter : ValueConverter<ulong, double>
{
    public UInt64ToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<ulong> input) => input.Value.Double();

    protected override ConverterValue<ulong> ConvertBack(ConverterData<double> input) => input.Value.UInt64();
}

[ValueConversion(typeof(USingle), typeof(double))]
public class USingleToDoubleConverter : ValueConverter<USingle, double>
{
    public USingleToDoubleConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<USingle> input) => (double)input.Value;

    protected override ConverterValue<USingle> ConvertBack(ConverterData<double> input) => (USingle)input.Value;
}