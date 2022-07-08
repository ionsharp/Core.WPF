using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Controls;

[Serializable]
public class ChromacityModel : Namable<Vector2>, IDescription
{
    [DisplayName("Illuminant"), Horizontal]
    public override Vector2 Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    string description;
    public string Description
    {
        get => description;
        set => this.Change(ref description, value);
    }

    public ChromacityModel() : base() { }

    public ChromacityModel(string name, string description, Vector2 value) : base(name, value) => Description = description;
}