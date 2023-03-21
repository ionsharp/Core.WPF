using System;
using System.Windows.Data;

namespace Imagin.Core.Data;

public abstract class RelativeBinding : Bind
{
    protected RelativeBinding(string path) : base(path) { }

    protected RelativeBinding(string path, RelativeSourceMode from) : this(path) { From = from; }
}

public class Ancestor : RelativeBinding
{
    public Type Type { set => FromType = value; }

    public Ancestor() : this(".", null) { }

    public Ancestor(Type fromType) : this(".", fromType) { }

    public Ancestor(string path, Type fromType) : base(path) { Type = fromType; }
}

public sealed class PreviousData : RelativeBinding
{
    public PreviousData() : this(".") { }

    public PreviousData(string path) : base(path, RelativeSourceMode.PreviousData) { }
}

public class Self : RelativeBinding
{
    public Self() : this(".") { }

    public Self(string path) : base(path, RelativeSourceMode.Self) { }
}

public sealed class TemplatedParent : RelativeBinding
{
    public TemplatedParent() : this(".") { }

    public TemplatedParent(string path) : base(path, RelativeSourceMode.TemplatedParent) { }
}