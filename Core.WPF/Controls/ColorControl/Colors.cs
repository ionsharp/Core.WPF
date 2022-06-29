using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Media;

[Serializable]
public sealed class PrimaryColors : GroupCollection<ByteVector4>
{
    public PrimaryColors() : base("Primary")
        => typeof(Colors1).GetFields().ForEach(i => Add(new ByteVector4((string)i.GetValue(null))));
}

[Serializable]
public sealed class SecondaryColors : GroupCollection<ByteVector4>
{
    public SecondaryColors() : base("Secondary")
        => typeof(Colors2).GetFields().ForEach(i => Add(new ByteVector4((string)i.GetValue(null))));
}

[Serializable]
public sealed class TertiaryColors : GroupCollection<ByteVector4>
{
    public TertiaryColors() : base("Tertiary")
        => typeof(Colors3).GetFields().ForEach(i => Add(new ByteVector4((string)i.GetValue(null))));
}

[Serializable]
public sealed class QuaternaryColors : GroupCollection<ByteVector4>
{
    public QuaternaryColors() : base("Quaternary")
        => typeof(Colors4).GetFields().ForEach(i => Add(new ByteVector4((string)i.GetValue(null))));
}

[Serializable]
public sealed class QuinaryColors : GroupCollection<ByteVector4>
{
    public QuinaryColors() : base("Quinary")
        => typeof(Colors5).GetFields().ForEach(i => Add(new ByteVector4((string)i.GetValue(null))));
}