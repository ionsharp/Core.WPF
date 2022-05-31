using Imagin.Core.Converters;

namespace Imagin.Core.Data
{
    public class DisplayNameBinding : LocalBinding
    {
        public DisplayNameBinding() : this(".") { }

        public DisplayNameBinding(string path) : base(path) => Converter = DisplayNameConverter.Default;
    }
}