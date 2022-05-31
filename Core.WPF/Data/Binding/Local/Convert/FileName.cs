using Imagin.Core.Converters;

namespace Imagin.Core.Data
{
    public class FileNameBinding : LocalBinding
    {
        public FileNameBinding() : this(".") { }

        public FileNameBinding(string path) : base(path) => Converter = FileNameConverter.Default;
    }
}