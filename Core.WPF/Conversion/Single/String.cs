using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Text;
using System;
using System.Collections;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using WPFLocalizeExtension.Engine;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Type), typeof(string))]
public class RealTypeNameConverter : ValueConverter<Type, string>
{
    public RealTypeNameConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Type> input) => input.Value.GetRealName(input.Parameter == 0);

    protected override ConverterValue<Type> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(MethodInfo), typeof(string))]
public class MethodReturnTypeConverter : ValueConverter<MethodInfo, string>
{
    public MethodReturnTypeConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<MethodInfo> input)
    {
        var result = input.Value.ReturnType;
        return result.GetRealName(input.Parameter == 0);
    }

    protected override ConverterValue<MethodInfo> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(MethodInfo), typeof(string))]
public class FirstMethodParameterTypeConverter : ValueConverter<MethodInfo, string>
{
    public FirstMethodParameterTypeConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<MethodInfo> input)
    {
        var result = input.Value is MethodInfo method && method.GetParameters()?.Length > 0 && method.GetParameters()[0].ParameterType is Type type ? type : null;
        return result?.GetRealName(input.Parameter == 0);
    }

    protected override ConverterValue<MethodInfo> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(Type), typeof(string))]
public class ColorModelLongNameConverter : ValueConverter<Type, string>
{
    public ColorModelLongNameConverter() : base() { }


    protected override ConverterValue<string> ConvertTo(ConverterData<Type> input)
    {
        var components = Colour.Components[input.Value];

        var result = "";
        components.Each((i, j) =>
        {
            result += $"{j.Name} ({j.Symbol}{(j.Unit == ' ' ? "" : j.Unit)}), ";
            return j;
        });

        return result.Substring(0, result.Length - 2);
    }

    protected override ConverterValue<Type> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(IList), typeof(string))]
public class ListToStringConverter : ValueConverter<IList, string>
{
    public ListToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<IList> input) => input.Value.Count == 0 ? "(empty collection)" : input.Value.ToString(", ");

    protected override ConverterValue<IList> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(object), typeof(string))]
public class PluralConverter : ValueConverter<object, string>
{
    public PluralConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        var result = Try.Return(() => PluralizationService.CreateService(LocalizeDictionary.Instance.Culture).Pluralize($"{input.Value}"));

        var casing = input.ActualParameter is Casing i ? i : Casing.Original;
        switch (casing)
        {
            case Casing.Lower:
                return result.ToLower();

            case Casing.Upper:
                return result.ToUpper();

            case Casing.Capitalized:
                return result.Capitalize();

            case Casing.Original:
            default: return result;
        }
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class NumberPluralConverter : ValueConverter<object, string>
{
    public NumberPluralConverter() : base() { }

    protected override bool Is(object input) => input is ushort || input is short || input is uint || input is int || input is long || input is long;

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        var result = input.Value.Int32();
        return result == 1 ? string.Empty : input.Parameter == 0 ? "s" : "S";
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class RelativeTimeConverter : ValueConverter<object, string>
{
    public RelativeTimeConverter() : base() { }

    protected override bool AllowNull => true;

    protected override bool Is(object input) => input is DateTime || input is DateTime?;

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        if (input.Value == null)
            return "never";

        if (input.Value is DateTime a)
            return a.Relative();

        return input.Value.As<DateTime?>()?.Relative() ?? (ConverterValue<string>)Nothing.Do;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class ShortTimeConverter : ValueConverter<object, string>
{
    public ShortTimeConverter() : base() { }

    protected override bool Is(object input) => input is DateTime || input is DateTime? || input is int || input is string;

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        TimeSpan result = TimeSpan.Zero;
        if (input.Value is int a)
            result = TimeSpan.FromSeconds(a);

        else if (input.Value is string b)
            result = TimeSpan.FromSeconds(b.Int32());

        else
        {
            var now = DateTime.Now;
            if (input.Value is DateTime c)
                result = c > now ? c - now : now - c;

            if (input.Value is DateTime?)
            {
                var d = input.Value as DateTime?;

                if (d == null)
                    return string.Empty;

                result = d.Value > now ? d.Value - now : now - d.Value;
            }
        }

        return result.ShortTime(input.Parameter == 1);
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(object), typeof(string))]
public class CamelCaseConverter : ValueConverter<object, string>
{
    public CamelCaseConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        var result = input.Value.ToString().SplitCamel() ?? string.Empty;
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Capitalize() : input.Parameter == 2 ? result.ToLower() : throw new NotSupportedException();
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class FirstLetterConverter : ValueConverter<string, string>
{
    public FirstLetterConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
    {
        if (!input.Value.Empty())
            return input.Value.Substring(0, 1);

        return Nothing.Do;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(int), typeof(string))]
public class LeadingZeroConverter : ValueConverter<int, string>
{
    public LeadingZeroConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<int> input) => input.Value.ToString("D2");

    protected override ConverterValue<int> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(Version), typeof(string))]
public class ShortVersionConverter : ValueConverter<Version, string>
{
    public ShortVersionConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Version> input)
    {
        var result = "";

        //1.0.0.0 > 1.0
        if (input.Value.Build == 0 && input.Value.Revision == 0)
            result = $"{input.Value.Major}.{input.Value.Minor}";

        //1.0.1.0 > 1.0.1
        if (input.Value.Build > 0 && input.Value.Revision == 0)
            result = $"{input.Value.Major}.{input.Value.Minor}.{input.Value.Build}";

        //1.0.0.1 > 1.0.0.1
        if (input.Value.Build == 0 && input.Value.Revision > 0)
            result = $"{input.Value.Major}.{input.Value.Minor}.{input.Value.Build}.{input.Value.Revision}";

        return result;
    }

    protected override ConverterValue<Version> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class SubstringConverter : ValueConverter<object, string>
{
    public SubstringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        if (input.Value is string || input.Value is Enum)
        {
            var i = input.Value.ToString();
            Try.Invoke(() => i = i.Substring(0, input.Parameter == 0 ? i.Length : input.Parameter));
            return i;
        }
        return default;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class ToLowerConverter : ValueConverter<object, string>
{
    public ToLowerConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString().ToLower();

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class ToStringConverter : ValueConverter<object, string>
{
    public ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString();

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class ToUpperConverter : ValueConverter<object, string>
{
    public ToUpperConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString().ToUpper();

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(Type), typeof(string))]
public class TypeFileExtensionConverter : ValueConverter<Type, string>
{
    public TypeFileExtensionConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Type> input) => input.Value.GetAttribute<FileAttribute>().Extension;

    protected override ConverterValue<Type> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(object), typeof(string))]
public abstract class ArrayToStringConverter : ValueConverter<object, string>
{
    public ArrayToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        if (input.Value.GetType().IsArray)
        {
            if (input.Value is IList list)
            {
                var result = string.Empty;

                var j = 0;
                var count = list.Count;

                foreach (var i in list)
                {
                    result += (j == count - 1 ? $"{i}" : $"{i}{input.ActualParameter?.ToString().Char()}");
                    j++;
                }

                return result;
            }
            return input.Value.ToString();
        }
        return default;
    }
}

[ValueConversion(typeof(object), typeof(string))]
public class Int32ArrayToStringConverter : ArrayToStringConverter
{
    public Int32ArrayToStringConverter() : base() { }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => input.Value.Int32Array(input.ActualParameter?.ToString().Char()).ToArray();
}

///

[ValueConversion(typeof(ByteVector4), typeof(string))]
public class ByteVector4ToColorNameConverter : ValueConverter<ByteVector4, string>
{
    public ByteVector4ToColorNameConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<ByteVector4> input)
    {
        var color = XColor.Convert(input.Value);
        color.Convert(out ByteVector4 result);

        return Colour.FindName(result) ?? result.GetApproximateName();
    }

    protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(byte), typeof(string))]
public class ByteToStringConverter : ValueConverter<byte, string>
{
    public ByteToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<byte> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<byte> ConvertBack(ConverterData<string> input) => input.Value.Byte();
}

[ValueConversion(typeof(ByteVector4), typeof(string))]
public class ByteVector4ToStringConverter : ValueConverter<ByteVector4, string>
{
    public ByteVector4ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<ByteVector4> input) => input.Value.ToString(input.Parameter == 1);

    protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<string> input) => new ByteVector4(input.Value);
}

[ValueConversion(typeof(char), typeof(string))]
public class CharacterToStringConverter : ValueConverter<char, string>
{
    public CharacterToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<char> input) => input.Value.ToString();

    protected override ConverterValue<char> ConvertBack(ConverterData<string> input) => input.Value.Char();
}

[ValueConversion(typeof(Color), typeof(string))]
public class ColorNameConverter : ValueConverter<Color, string>
{
    public ColorNameConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out ByteVector4 result);
        return Colour.FindName(result) ?? result.GetApproximateName();
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(Color), typeof(string))]
public class ColorToStringConverter : ValueConverter<Color, string>
{
    public ColorToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out ByteVector4 result);
        return result.ToString(input.Parameter == 1);
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => XColor.Convert(new ByteVector4(input.Value)).A(i => input.Parameter == 1 ? i : input.Parameter == 0 ? (byte)255 : throw new NotSupportedException());
}

[ValueConversion(typeof(Color), typeof(string))]
public class ColorToShortStringConverter : ValueConverter<Color, string>
{
    public ColorToShortStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Color> input)
    {
        input.Value.Convert(out ByteVector4 i);

        var result = i.ToShortString();
        return input.ActualParameter?.ToString().F(result) ?? result;
    }

    protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(DateTime), typeof(string))]
public class DateTimeToStringConverter : ValueConverter<DateTime, string>
{
    public DateTimeToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<DateTime> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<DateTime> ConvertBack(ConverterData<string> input) => input.Value.DateTime();
}

[ValueConversion(typeof(decimal), typeof(string))]
public class DecimalToStringConverter : ValueConverter<decimal, string>
{
    public DecimalToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<decimal> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<decimal> ConvertBack(ConverterData<string> input) => input.Value.Decimal();
}

[ValueConversion(typeof(double), typeof(string))]
public class DoubleToStringConverter : ValueConverter<double, string>
{
    public DoubleToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<double> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<double> ConvertBack(ConverterData<string> input) => input.Value.Double();
}

[ValueConversion(typeof(Guid), typeof(string))]
public class GuidToStringConverter : ValueConverter<Guid, string>
{
    public GuidToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Guid> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<Guid> ConvertBack(ConverterData<string> input)
    {
        Guid.TryParse(input.Value, out Guid result);
        return result;
    }
}

[ValueConversion(typeof(short), typeof(string))]
public class Int16ToStringConverter : ValueConverter<short, string>
{
    public Int16ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<short> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<short> ConvertBack(ConverterData<string> input) => input.Value.Int16();
}

[ValueConversion(typeof(System.Drawing.Color), typeof(string))]
public class Int32ColorToStringConverter : ValueConverter<System.Drawing.Color, string>
{
    public Int32ColorToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<System.Drawing.Color> input)
    {
        input.Value.Convert(out Color color);
        color.Convert(out ByteVector4 result);
        return result.ToString(input.Parameter == 1);
    }

    protected override ConverterValue<System.Drawing.Color> ConvertBack(ConverterData<string> input)
    {
        var color = XColor.Convert(new ByteVector4(input.Value)).A(i => input.Parameter == 1 ? i : input.Parameter == 0 ? (byte)255 : throw new NotSupportedException());
        color.Convert(out System.Drawing.Color result);
        return result;
    }
}

[ValueConversion(typeof(int), typeof(string))]
public class Int32ToStringConverter : ValueConverter<int, string>
{
    public Int32ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<int> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<int> ConvertBack(ConverterData<string> input) => input.Value.Int32();
}

[ValueConversion(typeof(long), typeof(string))]
public class Int64ToStringConverter : ValueConverter<long, string>
{
    public Int64ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<long> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<long> ConvertBack(ConverterData<string> input) => input.Value.Int64();
}

[ValueConversion(typeof(float), typeof(string))]
public class SingleToStringConverter : ValueConverter<float, string>
{
    public SingleToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<float> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<float> ConvertBack(ConverterData<string> input) => input.Value.Single();
}
    
[ValueConversion(typeof(SolidColorBrush), typeof(string))]
public class SolidColorBrushToStringConverter : ValueConverter<SolidColorBrush, string>
{
    public SolidColorBrushToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<SolidColorBrush> input)
    {
        input.Value.Color.Convert(out ByteVector4 result);
        return result.ToString(true);
    }

    protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<string> input) => new SolidColorBrush(XColor.Convert(new ByteVector4(input.Value)));
}

[ValueConversion(typeof(TimeSpan), typeof(string))]
public class TimeSpanToStringConverter : ValueConverter<TimeSpan, string>
{
    public TimeSpanToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<TimeSpan> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<string> input) => input.Value.TimeSpan();
}

[ValueConversion(typeof(TimeSpan), typeof(string))]
public class TimeSpanToDateTimeToStringConverter : ValueConverter<TimeSpan, string>
{
    public TimeSpanToDateTimeToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<TimeSpan> input) => (DateTime.Now.Date.AddSeconds(input.Value.TotalSeconds)).ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<string> input) => input.Value.DateTime().TimeOfDay;
}

[ValueConversion(typeof(UDouble), typeof(string))]
public class UDoubleToStringConverter : ValueConverter<UDouble, string>
{
    public UDoubleToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<UDouble> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<UDouble> ConvertBack(ConverterData<string> input) => input.Value.UDouble();
}

[ValueConversion(typeof(ushort), typeof(string))]
public class UInt16ToStringConverter : ValueConverter<ushort, string>
{
    public UInt16ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<ushort> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<ushort> ConvertBack(ConverterData<string> input) => input.Value.UInt16();
}

[ValueConversion(typeof(uint), typeof(string))]
public class UInt32ToStringConverter : ValueConverter<uint, string>
{
    public UInt32ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<uint> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<uint> ConvertBack(ConverterData<string> input) => input.Value.UInt32();
}

[ValueConversion(typeof(ulong), typeof(string))]
public class UInt64ToStringConverter : ValueConverter<ulong, string>
{
    public UInt64ToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<ulong> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<ulong> ConvertBack(ConverterData<string> input) => input.Value.UInt64();
}

[ValueConversion(typeof(USingle), typeof(string))]
public class USingleToStringConverter : ValueConverter<USingle, string>
{
    public USingleToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<USingle> input) => input.Value.ToString(input.ActualParameter?.ToString());

    protected override ConverterValue<USingle> ConvertBack(ConverterData<string> input) => input.Value.USingle();
}

[ValueConversion(typeof(Uri), typeof(string))]
public class UriToStringConverter : ValueConverter<Uri, string>
{
    public UriToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Uri> input) => input.Value.ToString();

    protected override ConverterValue<Uri> ConvertBack(ConverterData<string> input) => input.Value.Uri();
}

[ValueConversion(typeof(Version), typeof(string))]
public class VersionToStringConverter : ValueConverter<Version, string>
{
    public VersionToStringConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Version> input) => input.Value.ToString();

    protected override ConverterValue<Version> ConvertBack(ConverterData<string> input) => input.Value.Version();
}