using Imagin.Core.Conversion;
using System.Windows.Media;

namespace Imagin.Core.Media;

public class FontFamilyToStringConverter : ValueConverter<FontFamily, string>
{
    protected override bool AllowNull => true;

    protected override ConverterValue<FontFamily> ConvertBack(ConverterData<string> input)
    {
        FontFamily result = null;
        Try.Invoke(() => result = new FontFamily(input.Value));
        return result;
    }

    protected override ConverterValue<string> ConvertTo(ConverterData<FontFamily> input) => input.Value?.Source ?? BinaryFont.DefaultName;
}

public class BinaryFont : BinaryValue<FontFamily, string, FontFamilyToStringConverter>
{
    public const string DefaultName = "Calibri";

    public BinaryFont() : base() { }

    public BinaryFont(string font) : this() => Value = ConvertBack(font);
}