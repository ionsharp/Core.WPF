using Imagin.Core.Conversion;

namespace Imagin.Core.Data
{
    public class EqualsBinding : LocalBinding
    {
        public object Parameter
        {
            set => ConverterParameter = value;
        }

        public EqualsBinding() : this(".", null) { }

        public EqualsBinding(object parameter) : this(".", parameter) { }

        public EqualsBinding(string path, object parameter) : base(path)
        {
            Converter = ValueEqualsParameterConverter.Default;
            Parameter = parameter;
        }
    }
}