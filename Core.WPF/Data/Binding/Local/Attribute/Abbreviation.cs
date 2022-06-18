using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class AbbreviationBinding : LocalBinding
    {
        public AbbreviationBinding() : this(".") { }

        public AbbreviationBinding(string path) : base(path) => Converter = AbbreviationConverter.Default;
    }
}