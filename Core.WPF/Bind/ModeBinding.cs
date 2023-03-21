using System.Windows.Data;

namespace Imagin.Core.Data;

public abstract class ModeBinding : Bind
{
    protected ModeBinding(string path) : base(path) { }
}

public class OneWay : ModeBinding
{
    new public BindingMode Mode { get => base.Mode; private set => base.Mode = value; }

    public OneWay() : this(".") { }

    public OneWay(string path) : base(path) => Mode = BindingMode.OneWay;
}

public class OneWayToSource : ModeBinding
{
    new public BindingMode Mode { get => base.Mode; private set => base.Mode = value; }

    public OneWayToSource() : this(".") { }

    public OneWayToSource(string path) : base(path) => Mode = BindingMode.OneWayToSource;
}

public class TwoWay : ModeBinding
{
    new public BindingMode Mode { get => base.Mode; private set => base.Mode = value; }

    public TwoWay() : this(".") { }

    public TwoWay(string path) : base(path) => Mode = BindingMode.TwoWay;
}