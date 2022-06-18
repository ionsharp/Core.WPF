using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class OneToDoubleBinding : LocalBinding
    {
        public OneToDoubleBinding() : this(".") { }

        public OneToDoubleBinding(string path) : base(path)
        {
            Converter = OneToDoubleConverter.Default;
        }
    }
}