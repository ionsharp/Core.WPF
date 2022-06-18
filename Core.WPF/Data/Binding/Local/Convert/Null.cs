using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class NullBinding : LocalBinding
    {
        public NullBinding() : this(".") { }

        public NullBinding(string path) : base(path) => Converter = NullConverter.Default;
    }
}