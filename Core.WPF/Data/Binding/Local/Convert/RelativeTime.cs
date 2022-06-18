using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class RelativeTimeBinding : LocalBinding
    {
        public RelativeTimeBinding() : this(".") { }

        public RelativeTimeBinding(string path) : base(path)
        {
            Converter = RelativeTimeConverter.Default;
        }
    }
}