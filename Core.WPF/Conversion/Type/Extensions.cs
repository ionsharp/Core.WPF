using Imagin.Core.Storage;

namespace Imagin.Core.Conversion
{
    public class ExtensionsTypeConverter : StringTypeConverter<string>
    {
        protected override int? Length => null;

        protected override char Separator => ';';

        protected override string Convert(string input) => input;

        protected override object Convert(string[] input) => new Extensions(input);
    }
}