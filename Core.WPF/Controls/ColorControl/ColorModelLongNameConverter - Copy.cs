using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using System;
using System.Windows.Data;

namespace Imagin.Core.Controls;

[ValueConversion(typeof(Type), typeof(string))]
public class ColorModelLongNameConverter : Conversion.Converter<Type, string>
{
    public static ColorModelLongNameConverter Default { get; private set; } = new ColorModelLongNameConverter();
    public ColorModelLongNameConverter() { }


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