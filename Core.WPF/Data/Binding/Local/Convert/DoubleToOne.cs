using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class DoubleToOneBinding : LocalBinding
    {
        public DoubleToOneBinding() : this(".") { }

        public DoubleToOneBinding(string path) : base(path)
        {
            Converter = DoubleToOneConverter.Default;
        }
    }
}