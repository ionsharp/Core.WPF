using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class IsNullBinding : LocalBinding
    {
        public IsNullBinding() : this(".") { }

        public IsNullBinding(string path) : base(path) => Converter = IsNullConverter.Default;
    }
}