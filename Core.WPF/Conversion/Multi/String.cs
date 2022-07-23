using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using Imagin.Core.Text;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(string))]
public class BulletMultiConverter : MultiConverter<string>
{
    public static BulletMultiConverter Default { get; private set; } = new BulletMultiConverter();
    BulletMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 0)
        {
            if (values[0] is Bullets bullet)
            {
                switch (values.Length)
                {
                    case 1:
                        return bullet.ToString(1);

                    case 2:
                        double.TryParse($"{values[1]}", out double index);
                        return bullet.ToString(index);
                }
            }
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class ColorMultiConverter : MultiConverter<string>
{
    public static ColorMultiConverter Default { get; private set; } = new();
    public ColorMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 4)
        {
            if (values[0] is Color color)
            {
                if (values[1] is string model)
                {
                    if (values[2] is bool normalize)
                    {
                        if (values[3] is int precision)
                        {
                            color.Convert(out RGB rgb);

                            Type type = Colour.Types.FirstOrDefault(i => i.Name == model);
                            var result = rgb.To(type, WorkingProfile.Default);

                            Vector finalResult = normalize
                                ? M.Normalize(result.Values, Colour.Minimum(type), Colour.Maximum(type)).Transform(i => i.Round(precision))
                                : (Vector)result.Values.Transform(i => i.Round(precision));

                            return $"{finalResult}";
                        }
                    }
                }
            }
        }
        return null;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class FileSizeMultiConverter : MultiConverter<string>
{
    public static FileSizeMultiConverter Default { get; private set; } = new FileSizeMultiConverter();
    FileSizeMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
        {
            if (values[1] is FileSizeFormat format)
            {
                if (values[0] is long a)
                    return new FileSize(a).ToString(format);

                if (values[0] is ulong b)
                    return new FileSize(b).ToString(format);
            }
        }
        return string.Empty;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class PropertyChangedMultiConverter : MultiConverter<string>
{
    public static PropertyChangedMultiConverter Default { get; private set; } = new PropertyChangedMultiConverter();
    PropertyChangedMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 1)
            return values[0].ToString();

        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class StringMultiConverter : MultiConverter<string>
{
    public static StringMultiConverter Default { get; private set; } = new StringMultiConverter();
    StringMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null)
        {
            var result = string.Empty;
            foreach (var i in values)
                result = $"{result}{i}";

            return result;
        }
        return null;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class StringReplaceMultiConverter : MultiConverter<string>
{
    public static StringReplaceMultiConverter Default { get; private set; } = new StringReplaceMultiConverter();
    StringReplaceMultiConverter() { }

    public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 3)
        {
            if (values[0] is object i)
            {
                if (values[1] is string a)
                {
                    if (values[2] is string b)
                        return $"{i}"?.Replace(a, b);
                }
            }
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class SubstringMultiConverter : MultiConverter<string>
{
    public static SubstringMultiConverter Default { get; private set; } = new SubstringMultiConverter();
    SubstringMultiConverter() { }

    public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 2)
        {
            if (values[0] is object a)
            {
                if (values[1] is int b)
                {
                    var result = $"{a}";
                    return Try.Invoke(() =>
                    {
                        if (values?.Length == 3)
                        {
                            if (values[2] is int c)
                            {
                                result = result.Substring(b, c);
                                return;
                            }
                        }
                        result = result.Substring(b);
                    })
                    ? result : "";
                }
            }
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class TimeLeftMultiConverter : MultiConverter<string>
{
    public static TimeLeftMultiConverter Default { get; private set; } = new TimeLeftMultiConverter();
    TimeLeftMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 3)
        {
            if (values[0] is TimeSpan && values[1] is long && values[2] is long)
            {
                var bytesRead = (long)values[1];
                var bytesTotal = (long)values[2];

                var result = (TimeSpan)values[0];
                return TimeSpan.FromSeconds(result.Left(bytesRead, bytesTotal).TotalSeconds.Round()).ToString();
            }
        }
        return string.Empty;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class UnitStringMultiConverter : MultiConverter<string>
{
    public static UnitStringMultiConverter Default { get; private set; } = new UnitStringMultiConverter();
    UnitStringMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 2)
        {
            var i = values[0];
            if ((i is double || i is double? || i is int || i is int?) && values[1] is Unit)
            {
                var value = double.Parse($"{i}");

                var funit = parameter is Unit ? (Unit)parameter : Unit.Pixel;
                var tunit = values[1].As<Unit>();

                var resolution = values.Length > 2 ? (float)values[2] : 72f;
                var places = values.Length > 3 ? (int)values[3] : 3;

                return $"{funit.Convert(tunit, value, resolution).Round(places)} {tunit.GetAttribute<AbbreviationAttribute>().Abbreviation}";
            }
        }
        return string.Empty;
    }
}

//...

[ValueConversion(typeof(object[]), typeof(string))]
public abstract class StringFormatMultiConverter<T> : MultiConverter<string>
{
    protected abstract string Convert(T a, string b);

    public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
        {
            if (values[0] is T a)
            {
                if (values[1] is string b)
                {
                    var i = b.Between("{", "}");
                    if (!i.NullOrEmpty())
                        return b.ReplaceBetween('{', '}', "0").F(Convert(a, i));

                    return Convert(a, b);
                }
            }
        }
        return Binding.DoNothing;
    }
}

public class ByteStringFormatMultiConverter : StringFormatMultiConverter<byte>
{
    public static ByteStringFormatMultiConverter Default { get; private set; } = new ByteStringFormatMultiConverter();
    ByteStringFormatMultiConverter() { }

    protected override string Convert(byte a, string b) => a.ToString(b);
}

public class DecimalStringFormatMultiConverter : StringFormatMultiConverter<decimal>
{
    public static DecimalStringFormatMultiConverter Default { get; private set; } = new DecimalStringFormatMultiConverter();
    DecimalStringFormatMultiConverter() { }

    protected override string Convert(decimal a, string b) => a.ToString(b);
}

public class DoubleStringFormatMultiConverter : StringFormatMultiConverter<double>
{
    public static DoubleStringFormatMultiConverter Default { get; private set; } = new DoubleStringFormatMultiConverter();
    DoubleStringFormatMultiConverter() { }

    protected override string Convert(double a, string b) => a.ToString(b);
}

public class Int16StringFormatMultiConverter : StringFormatMultiConverter<short>
{
    public static Int16StringFormatMultiConverter Default { get; private set; } = new Int16StringFormatMultiConverter();
    Int16StringFormatMultiConverter() { }

    protected override string Convert(short a, string b) => a.ToString(b);
}

public class Int32StringFormatMultiConverter : StringFormatMultiConverter<int>
{
    public static Int32StringFormatMultiConverter Default { get; private set; } = new Int32StringFormatMultiConverter();
    Int32StringFormatMultiConverter() { }

    protected override string Convert(int a, string b) => a.ToString(b);
}

public class Int64StringFormatMultiConverter : StringFormatMultiConverter<long>
{
    public static Int64StringFormatMultiConverter Default { get; private set; } = new Int64StringFormatMultiConverter();
    Int64StringFormatMultiConverter() { }

    protected override string Convert(long a, string b) => a.ToString(b);
}

public class SingleStringFormatMultiConverter : StringFormatMultiConverter<float>
{
    public static SingleStringFormatMultiConverter Default { get; private set; } = new SingleStringFormatMultiConverter();
    SingleStringFormatMultiConverter() { }

    protected override string Convert(float a, string b) => a.ToString(b);
}

public class UDoubleStringFormatMultiConverter : StringFormatMultiConverter<UDouble>
{
    public static UDoubleStringFormatMultiConverter Default { get; private set; } = new UDoubleStringFormatMultiConverter();
    UDoubleStringFormatMultiConverter() { }

    protected override string Convert(UDouble a, string b) => a.ToString(b);
}

public class UInt16StringFormatMultiConverter : StringFormatMultiConverter<ushort>
{
    public static UInt16StringFormatMultiConverter Default { get; private set; } = new UInt16StringFormatMultiConverter();
    UInt16StringFormatMultiConverter() { }

    protected override string Convert(ushort a, string b) => a.ToString(b);
}

public class UInt32StringFormatMultiConverter : StringFormatMultiConverter<uint>
{
    public static UInt32StringFormatMultiConverter Default { get; private set; } = new UInt32StringFormatMultiConverter();
    UInt32StringFormatMultiConverter() { }

    protected override string Convert(uint a, string b) => a.ToString(b);
}

public class UInt64StringFormatMultiConverter : StringFormatMultiConverter<ulong>
{
    public static UInt64StringFormatMultiConverter Default { get; private set; } = new UInt64StringFormatMultiConverter();
    UInt64StringFormatMultiConverter() { }

    protected override string Convert(ulong a, string b) => a.ToString(b);
}