using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Imagin.Core.Media;

[Serializable]
public class ColorGroupCollection : GroupCollection<ByteVector4>
{
    public ColorGroupCollection() : base() { }

    public ColorGroupCollection(string name, Type type = null) : base(name) => type.If(i => Add(i));

    public ColorGroupCollection(string name, IEnumerable<ByteVector4> items) : base(name, items) { }

    public ColorGroupCollection(string name, IEnumerable<GroupItem<ByteVector4>> items) : base(name, items) { }

    protected void Add(Type input)
        => XList.ForEach<FieldInfo>(input.GetFields(), i => Add(i.Name.SplitCamel(), new ByteVector4((string)i.GetValue(null))));
}

[Serializable]
public sealed class PrimaryColors : ColorGroupCollection
{
    public PrimaryColors() : base("Primary", typeof(Colors1)) { }
}

[Serializable]
public sealed class SecondaryColors : ColorGroupCollection
{
    public SecondaryColors() : base("Secondary") => Add(typeof(Colors2));
}

[Serializable]
public sealed class TertiaryColors : ColorGroupCollection
{
    public TertiaryColors() : base("Tertiary") => Add(typeof(Colors3));
}

[Serializable]
public sealed class QuaternaryColors : ColorGroupCollection
{
    public QuaternaryColors() : base("Quaternary") => Add(typeof(Colors4));
}

[Serializable]
public sealed class QuinaryColors : ColorGroupCollection
{
    public QuinaryColors() : base("Quinary") => Add(typeof(Colors5));
}