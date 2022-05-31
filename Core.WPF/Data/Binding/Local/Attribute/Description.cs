using Imagin.Core.Converters;

namespace Imagin.Core.Data
{
    public class DescriptionBinding : LocalBinding
    {
        public DescriptionBinding() : this(".") { }

        public DescriptionBinding(string path) : base(path) => Converter = DescriptionConverter.Default;
    }
}