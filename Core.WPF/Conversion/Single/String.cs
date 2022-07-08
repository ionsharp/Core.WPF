using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(Color), typeof(string))]
    public class ColorNameConverter : Converter<Color, string>
    {
        public static ColorNameConverter Default { get; private set; } = new ColorNameConverter();
        ColorNameConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Color> input) 
        {
            input.Value.Convert(out ByteVector4 result);
            return Colour.FindName(result); 
        }

        protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(IList), typeof(string))]
    public class ListToStringConverter : Converter<IList, string>
    {
        public static ListToStringConverter Default { get; private set; } = new ListToStringConverter();
        ListToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<IList> input) => input.Value.Count == 0 ? "(empty collection)" : input.Value.ToString(", ");

        protected override ConverterValue<IList> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    //...

    [ValueConversion(typeof(object), typeof(string))]
    public class PluralConverter : Converter<object, string>
    {
        public static PluralConverter Default { get; private set; } = new PluralConverter();
        PluralConverter() { }

        protected override bool Is(object input) => input is ushort || input is short || input is uint || input is int || input is long || input is long;

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
        {
            var result = input.Value.Int32();
            return result == 1 ? string.Empty : input.Parameter == 0 ? "s" : "S";
        }

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class RelativeTimeConverter : Converter<object, string>
    {
        public static RelativeTimeConverter Default { get; private set; } = new RelativeTimeConverter();
        public RelativeTimeConverter() : base() { }

        protected override bool Is(object input) => input is DateTime || input is DateTime?;

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
        {
            if (input.Value is DateTime a)
                return a.Relative();

            return input.Value.As<DateTime?>()?.Relative() ?? (ConverterValue<string>)Nothing.Do;
        }

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ShortTimeConverter : Converter<object, string>
    {
        public static ShortTimeConverter Default { get; private set; } = new ShortTimeConverter();
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

    //...

    [ValueConversion(typeof(object), typeof(string))]
    public class CamelCaseConverter : Converter<object, string>
    {
        public static CamelCaseConverter Default { get; private set; } = new CamelCaseConverter();
        CamelCaseConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
        {
            var result = input.Value.ToString().SplitCamel() ?? string.Empty;
            return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Capitalize() : input.Parameter == 2 ? result.ToLower() : throw new NotSupportedException();
        }

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class FirstLetterConverter : Converter<string, string>
    {
        public static FirstLetterConverter Default { get; private set; } = new FirstLetterConverter();
        FirstLetterConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
        {
            if (!input.Value.Empty())
                return input.Value.Substring(0, 1);

            return Nothing.Do;
        }

        protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class LeadingZeroConverter : Converter<int, string>
    {
        public static LeadingZeroConverter Default { get; private set; } = new LeadingZeroConverter();
        LeadingZeroConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<int> input) => input.Value.ToString("D2");

        protected override ConverterValue<int> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class SubstringConverter : Converter<object, string>
    {
        public static SubstringConverter Default { get; private set; } = new SubstringConverter();
        SubstringConverter() { }

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
    public class ToLowerConverter : Converter<object, string>
    {
        public static ToLowerConverter Default { get; private set; } = new ToLowerConverter();
        ToLowerConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString().ToLower();

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ToStringConverter : Converter<object, string>
    {
        public static ToStringConverter Default { get; private set; } = new ToStringConverter();
        public ToStringConverter() : base() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString();

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ToUpperConverter : Converter<object, string>
    {
        public static ToUpperConverter Default { get; private set; } = new ToUpperConverter();
        ToUpperConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<object> input) => input.Value.ToString().ToUpper();

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    //...

    [ValueConversion(typeof(object), typeof(string))]
    public abstract class ArrayToStringConverter : Converter<object, string>
    {
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
        public static Int32ArrayToStringConverter Default { get; private set; } = new Int32ArrayToStringConverter();
        Int32ArrayToStringConverter() { }

        protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => input.Value.Int32Array(input.ActualParameter?.ToString().Char()).ToArray();
    }

    //...

    [ValueConversion(typeof(byte), typeof(string))]
    public class ByteToStringConverter : Converter<byte, string>
    {
        public static ByteToStringConverter Default { get; private set; } = new ByteToStringConverter();
        ByteToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<byte> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<byte> ConvertBack(ConverterData<string> input) => input.Value.Byte();
    }

    [ValueConversion(typeof(ByteVector4), typeof(string))]
    public class ByteVector4ToStringConverter : Converter<ByteVector4, string>
    {
        public static ByteVector4ToStringConverter Default { get; private set; } = new();
        ByteVector4ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<ByteVector4> input) => input.Value.ToString(false);

        protected override ConverterValue<ByteVector4> ConvertBack(ConverterData<string> input) => new ByteVector4(input.Value);
    }

    [ValueConversion(typeof(char), typeof(string))]
    public class CharacterToStringConverter : Converter<char, string>
    {
        public static CharacterToStringConverter Default { get; private set; } = new CharacterToStringConverter();
        CharacterToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<char> input) => input.Value.ToString();

        protected override ConverterValue<char> ConvertBack(ConverterData<string> input) => input.Value.Char();
    }

    [ValueConversion(typeof(Color), typeof(string))]
    public class ColorToStringConverter : Converter<Color, string>
    {
        public static ColorToStringConverter Default { get; private set; } = new ColorToStringConverter();
        ColorToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Color> input)
        {
            input.Value.Convert(out ByteVector4 result);
            return result.ToString(input.Parameter == 1);
        }

        protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => XColor.Convert(new ByteVector4(input.Value)).A(i => input.Parameter == 1 ? i : input.Parameter == 0 ? (byte)255 : throw new NotSupportedException());
    }

    [ValueConversion(typeof(Color), typeof(string))]
    public class ColorToShortStringConverter : Converter<Color, string>
    {
        public static ColorToShortStringConverter Default { get; private set; } = new();
        ColorToShortStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Color> input)
        {
            input.Value.Convert(out ByteVector4 i);
            var j = i.ToString(false);
            if (j?.Length == 6)
            {
                if (j[0] == j[1] && j[2] == j[3] && j[4] == j[5])
                {
                    var k = $"{j[0]}{j[2]}{j[4]}";
                    return input.ActualParameter?.ToString().F(k) ?? k;
                }
            }
            return "";
        }

        protected override ConverterValue<Color> ConvertBack(ConverterData<string> input) => Nothing.Do;
    }

    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToStringConverter : Converter<DateTime, string>
    {
        public static DateTimeToStringConverter Default { get; private set; } = new();
        DateTimeToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<DateTime> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<DateTime> ConvertBack(ConverterData<string> input) => input.Value.DateTime();
    }

    [ValueConversion(typeof(decimal), typeof(string))]
    public class DecimalToStringConverter : Converter<decimal, string>
    {
        public static DecimalToStringConverter Default { get; private set; } = new DecimalToStringConverter();
        DecimalToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<decimal> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<decimal> ConvertBack(ConverterData<string> input) => input.Value.Decimal();
    }

    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : Converter<double, string>
    {
        public static DoubleToStringConverter Default { get; private set; } = new DoubleToStringConverter();
        DoubleToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<double> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<double> ConvertBack(ConverterData<string> input) => input.Value.Double();
    }

    [ValueConversion(typeof(Guid), typeof(string))]
    public class GuidToStringConverter : Converter<Guid, string>
    {
        public static GuidToStringConverter Default { get; private set; } = new GuidToStringConverter();
        GuidToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Guid> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<Guid> ConvertBack(ConverterData<string> input)
        {
            Guid.TryParse(input.Value, out Guid result);
            return result;
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class Int16ToStringConverter : Converter<short, string>
    {
        public static Int16ToStringConverter Default { get; private set; } = new Int16ToStringConverter();
        Int16ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<short> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<short> ConvertBack(ConverterData<string> input) => input.Value.Int16();
    }

    [ValueConversion(typeof(System.Drawing.Color), typeof(string))]
    public class Int32ColorToStringConverter : Converter<System.Drawing.Color, string>
    {
        public static Int32ColorToStringConverter Default { get; private set; } = new Int32ColorToStringConverter();
        Int32ColorToStringConverter() { }

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
    public class Int32ToStringConverter : Converter<int, string>
    {
        public static Int32ToStringConverter Default { get; private set; } = new Int32ToStringConverter();
        Int32ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<int> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<int> ConvertBack(ConverterData<string> input) => input.Value.Int32();
    }

    [ValueConversion(typeof(long), typeof(string))]
    public class Int64ToStringConverter : Converter<long, string>
    {
        public static Int64ToStringConverter Default { get; private set; } = new Int64ToStringConverter();
        Int64ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<long> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<long> ConvertBack(ConverterData<string> input) => input.Value.Int64();
    }

    [ValueConversion(typeof(float), typeof(string))]
    public class SingleToStringConverter : Converter<float, string>
    {
        public static SingleToStringConverter Default { get; private set; } = new SingleToStringConverter();
        SingleToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<float> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<float> ConvertBack(ConverterData<string> input) => input.Value.Single();
    }
    
    [ValueConversion(typeof(SolidColorBrush), typeof(string))]
    public class SolidColorBrushToStringConverter : Converter<SolidColorBrush, string>
    {
        public static SolidColorBrushToStringConverter Default { get; private set; } = new SolidColorBrushToStringConverter();
        SolidColorBrushToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<SolidColorBrush> input)
        {
            input.Value.Color.Convert(out ByteVector4 result);
            return result.ToString(true);
        }

        protected override ConverterValue<SolidColorBrush> ConvertBack(ConverterData<string> input) => new SolidColorBrush(XColor.Convert(new ByteVector4(input.Value)));
    }

    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToStringConverter : Converter<TimeSpan, string>
    {
        public static TimeSpanToStringConverter Default { get; private set; } = new();
        TimeSpanToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<TimeSpan> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<string> input) => input.Value.TimeSpan();
    }

    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToDateTimeToStringConverter : Converter<TimeSpan, string>
    {
        public static TimeSpanToDateTimeToStringConverter Default { get; private set; } = new();
        TimeSpanToDateTimeToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<TimeSpan> input) => (DateTime.Now.Date.AddSeconds(input.Value.TotalSeconds)).ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<TimeSpan> ConvertBack(ConverterData<string> input) => input.Value.DateTime().TimeOfDay;
    }

    [ValueConversion(typeof(UDouble), typeof(string))]
    public class UDoubleToStringConverter : Converter<UDouble, string>
    {
        public static UDoubleToStringConverter Default { get; private set; } = new UDoubleToStringConverter();
        UDoubleToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<UDouble> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<UDouble> ConvertBack(ConverterData<string> input) => input.Value.UDouble();
    }

    [ValueConversion(typeof(ushort), typeof(string))]
    public class UInt16ToStringConverter : Converter<ushort, string>
    {
        public static UInt16ToStringConverter Default { get; private set; } = new UInt16ToStringConverter();
        UInt16ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<ushort> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<ushort> ConvertBack(ConverterData<string> input) => input.Value.UInt16();
    }

    [ValueConversion(typeof(uint), typeof(string))]
    public class UInt32ToStringConverter : Converter<uint, string>
    {
        public static UInt32ToStringConverter Default { get; private set; } = new UInt32ToStringConverter();
        UInt32ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<uint> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<uint> ConvertBack(ConverterData<string> input) => input.Value.UInt32();
    }

    [ValueConversion(typeof(ulong), typeof(string))]
    public class UInt64ToStringConverter : Converter<ulong, string>
    {
        public static UInt64ToStringConverter Default { get; private set; } = new UInt64ToStringConverter();
        UInt64ToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<ulong> input) => input.Value.ToString(input.ActualParameter?.ToString());

        protected override ConverterValue<ulong> ConvertBack(ConverterData<string> input) => input.Value.UInt64();
    }

    [ValueConversion(typeof(Uri), typeof(string))]
    public class UriToStringConverter : Converter<Uri, string>
    {
        public static UriToStringConverter Default { get; private set; } = new();
        UriToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Uri> input) => input.Value.ToString();

        protected override ConverterValue<Uri> ConvertBack(ConverterData<string> input) => input.Value.Uri();
    }

    [ValueConversion(typeof(Version), typeof(string))]
    public class VersionToStringConverter : Converter<Version, string>
    {
        public static VersionToStringConverter Default { get; private set; } = new();
        VersionToStringConverter() { }

        protected override ConverterValue<string> ConvertTo(ConverterData<Version> input) => input.Value.ToString();

        protected override ConverterValue<Version> ConvertBack(ConverterData<string> input) => input.Value.Version();
    }
}