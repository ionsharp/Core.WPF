using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(bool), typeof(TextWrapping))]
public class TextWrappingConverter : ValueConverter<bool, TextWrapping>
{
    public TextWrappingConverter() : base() { }

    protected override ConverterValue<TextWrapping> ConvertTo(ConverterData<bool> input) 
        => input.Value ? TextWrapping.Wrap : TextWrapping.NoWrap;

    protected override ConverterValue<bool> ConvertBack(ConverterData<TextWrapping> input) 
        => input.Value == TextWrapping.Wrap;
}