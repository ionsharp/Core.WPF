using Imagin.Core.Conversion;
using Imagin.Core.Storage;
using System.Windows.Data;
using Imagin.Core.Data;

namespace Imagin.Core.Controls;

public class BrowserGroupConverterSelector : ConverterSelector
{
    public static readonly BrowserGroupConverterSelector Default = new();
    BrowserGroupConverterSelector() { }

    public override IValueConverter Select(object input)
    {
        return $"{input}" switch
        {
            nameof(ItemProperty.Name) => new ValueConverter<Item, string>(i => Converter.Get<FirstLetterConverter>().Convert(i.Name, null, null, null)?.ToString()),
            nameof(ItemProperty.Type) => new ValueConverter<Item, string>(i => Computer.FriendlyDescription(i.Path)),
            _ => default,
        };
    }
}