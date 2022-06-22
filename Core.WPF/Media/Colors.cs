using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Media;

[Serializable]
public sealed class PrimaryColors : GroupCollection<StringColor>
{
    public PrimaryColors() : base("Primary")
        => typeof(Colors1).GetFields().ForEach(i => Add(new StringColor(new Hexadecimal((string)i.GetValue(null)).Color())));
}

[Serializable]
public sealed class SecondaryColors : GroupCollection<StringColor>
{
    public SecondaryColors() : base("Secondary")
        => typeof(Colors2).GetFields().ForEach(i => Add(new StringColor(new Hexadecimal((string)i.GetValue(null)).Color())));
}

[Serializable]
public sealed class TertiaryColors : GroupCollection<StringColor>
{
    public TertiaryColors() : base("Tertiary")
        => typeof(Colors3).GetFields().ForEach(i => Add(new StringColor(new Hexadecimal((string)i.GetValue(null)).Color())));
}

[Serializable]
public sealed class QuaternaryColors : GroupCollection<StringColor>
{
    public QuaternaryColors() : base("Quaternary")
        => typeof(Colors4).GetFields().ForEach(i => Add(new StringColor(new Hexadecimal((string)i.GetValue(null)).Color())));
}

[Serializable]
public sealed class QuinaryColors : GroupCollection<StringColor>
{
    public QuinaryColors() : base("Quinary")
        => typeof(Colors5).GetFields().ForEach(i => Add(new StringColor(new Hexadecimal((string)i.GetValue(null)).Color())));
}