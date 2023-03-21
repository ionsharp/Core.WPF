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
    public BulletMultiConverter() : base() { }

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
    public ColorMultiConverter() : base() { }

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
    public FileSizeMultiConverter() : base() { }

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
public abstract class StringFormatMultiConverter<T> : MultiConverter<string>
{
    public StringFormatMultiConverter() : base() { }

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

[ValueConversion(typeof(object[]), typeof(string))]
public class StringMultiConverter : MultiConverter<string>
{
    public StringMultiConverter() : base() { }

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
    public StringReplaceMultiConverter() : base() { }

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
    public SubstringMultiConverter() : base() { }

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
    public TimeLeftMultiConverter() : base() { }

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
public class ToStringMultiConverter : MultiConverter<string>
{
    public ToStringMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 1)
            return values[0].ToString();

        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(string))]
public class UnitStringMultiConverter : MultiConverter<string>
{
    public UnitStringMultiConverter() : base() { }

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

///

public class ByteStringFormatMultiConverter : StringFormatMultiConverter<byte>
{
    public ByteStringFormatMultiConverter() : base() { }

    protected override string Convert(byte a, string b) => a.ToString(b);
}

public class DecimalStringFormatMultiConverter : StringFormatMultiConverter<decimal>
{
    public DecimalStringFormatMultiConverter() : base() { }

    protected override string Convert(decimal a, string b) => a.ToString(b);
}

public class DoubleStringFormatMultiConverter : StringFormatMultiConverter<double>
{
    public DoubleStringFormatMultiConverter() : base() { }

    protected override string Convert(double a, string b) => a.ToString(b);
}

public class Int16StringFormatMultiConverter : StringFormatMultiConverter<short>
{
    public Int16StringFormatMultiConverter() : base() { }

    protected override string Convert(short a, string b) => a.ToString(b);
}

public class Int32StringFormatMultiConverter : StringFormatMultiConverter<int>
{
    public Int32StringFormatMultiConverter() : base() { }

    protected override string Convert(int a, string b) => a.ToString(b);
}

public class Int64StringFormatMultiConverter : StringFormatMultiConverter<long>
{
    public Int64StringFormatMultiConverter() : base() { }

    protected override string Convert(long a, string b) => a.ToString(b);
}

public class SingleStringFormatMultiConverter : StringFormatMultiConverter<float>
{
    public SingleStringFormatMultiConverter() : base() { }

    protected override string Convert(float a, string b) => a.ToString(b);
}

public class UDoubleStringFormatMultiConverter : StringFormatMultiConverter<UDouble>
{
    public UDoubleStringFormatMultiConverter() : base() { }

    protected override string Convert(UDouble a, string b) => a.ToString(b);
}

public class UInt16StringFormatMultiConverter : StringFormatMultiConverter<ushort>
{
    public UInt16StringFormatMultiConverter() : base() { }

    protected override string Convert(ushort a, string b) => a.ToString(b);
}

public class UInt32StringFormatMultiConverter : StringFormatMultiConverter<uint>
{
    public UInt32StringFormatMultiConverter() : base() { }

    protected override string Convert(uint a, string b) => a.ToString(b);
}

public class UInt64StringFormatMultiConverter : StringFormatMultiConverter<ulong>
{
    public UInt64StringFormatMultiConverter() : base() { }

    protected override string Convert(ulong a, string b) => a.ToString(b);
}