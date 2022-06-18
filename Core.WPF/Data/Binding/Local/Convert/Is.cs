using Imagin.Core.Conversion;
using System;

namespace Imagin.Core.Data
{
    public class IsBinding : LocalBinding
    {
        public Type Type
        {
            set => ConverterParameter = value;
        }

        public IsBinding() : this(".", null) { }

        public IsBinding(Type type) : this(".", type) { }

        public IsBinding(string path, Type type) : base(path)
        {
            Converter = IsConverter.Default;
            Type = type;
        }
    }
}