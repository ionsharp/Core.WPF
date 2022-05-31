using System.Windows.Data;

namespace Imagin.Core.Data
{
    public sealed class PreviousData : Binding
    {
        public PreviousData() : this(".") { }

        public PreviousData(string path) : base(path)
        {
            Mode = BindingMode.OneWay;
            RelativeSource = new RelativeSource(RelativeSourceMode.PreviousData);
        }
    }
}