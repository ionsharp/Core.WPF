using Imagin.Core.Controls;
using Imagin.Core.Models;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Document), typeof(ColorDocument))]
public class ColorDocumentConverter : ValueConverter<Document, ColorDocument>
{
    public ColorDocumentConverter() : base() { }

    protected override ConverterValue<ColorDocument> ConvertTo(ConverterData<Document> input) => (ColorDocument)input.Value;

    protected override ConverterValue<Document> ConvertBack(ConverterData<ColorDocument> input) => (Document)input.Value;
}