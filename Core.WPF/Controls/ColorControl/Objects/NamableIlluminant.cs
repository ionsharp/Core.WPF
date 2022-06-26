using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Controls;

[Serializable]
public class NamableIlluminant : Namable<Vector2>
{
    [DisplayName("Illuminant"), Horizontal]
    public override Vector2 Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public NamableIlluminant() : base() { }

    public NamableIlluminant(string name, Vector2 value) : base(name, value) { }
}