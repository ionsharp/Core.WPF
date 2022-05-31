using System.Windows.Data;

namespace Imagin.Core.Data
{
    public class Self : Binding
    {
        public Self() : this(".") { }

        public Self(string path) : base(path)
        {
            Mode = BindingMode.OneWay;
            RelativeSource = new RelativeSource(RelativeSourceMode.Self);
        }
    }
}