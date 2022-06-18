using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class TypeBinding : LocalBinding
    {
        public TypeBinding() : this(".") { }

        public TypeBinding(string path) : base(path)
        {
            Converter = ObjectToTypeConverter.Default;
        }
    }
}