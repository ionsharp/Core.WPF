using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class DoubleBinding : LocalBinding
    {
        public DoubleBinding() : this(".") { }

        public DoubleBinding(string path) : base(path)
        {
            Converter = ObjectToDoubleConverter.Default;
        }
    }
}