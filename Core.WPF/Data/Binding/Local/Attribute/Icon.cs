using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class IconBinding : LocalBinding
    {
        public IconBinding() : this(".") { }

        public IconBinding(string path) : base(path) => Converter = IconConverter.Default;
    }
}