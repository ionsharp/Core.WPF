using System;

namespace Imagin.Core.Controls;

[Serializable]
public class AdaptationModel : Namable<Matrix>, IDescription
{
    string description;
    public string Description
    {
        get => description;
        set => this.Change(ref description, value);
    }

    [DisplayName("Matrix")]
    public override Matrix Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public AdaptationModel() : base() { }

    public AdaptationModel(string name, string description, Matrix value) : base(name, value) => Description = description;
}