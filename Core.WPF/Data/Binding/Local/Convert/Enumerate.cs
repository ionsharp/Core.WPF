using Imagin.Core.Converters;
using Imagin.Core.Linq;
using System;
using System.Windows.Data;

namespace Imagin.Core.Data
{
    public sealed class EnumerateBinding : LocalBinding
    {
        public EnumerateBinding() : this(".") { }

        public EnumerateBinding(string path) : base(path)
        {
            Converter = new SimpleConverter<object, object>
            (
                input => input?.GetType().GetEnumCollection(Appearance.Visible),
                input => throw new NotSupportedException()
            );
            Mode = BindingMode.OneTime;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}