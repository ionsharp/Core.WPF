using Imagin.Core.Colors;
using Imagin.Core.Numerics;
using System;
using System.Runtime.CompilerServices;

namespace Imagin.Core.Controls;

[Serializable]
public class MatrixModel : Namable<Matrix>, IDescription
{
    string description;
    [Index(1)]
    public string Description
    {
        get => description;
        set => this.Change(ref description, value);
    }

    [Copy, DisplayName("Matrix")]
    public override Matrix Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public MatrixModel() : base() { }

    public MatrixModel(string name, string description, Matrix value) : base(name, value) => Description = description;
}

[Serializable]
public class ChromacityMatrixModel : MatrixModel
{
    Vector2 chromacity;
    [Copy, Horizontal, Index(2)]
    public Vector2 Chromacity
    {
        get => chromacity;
        set => this.Change(ref chromacity, value);
    }

    Vector2 primaryR;
    [Copy, Horizontal, Index(3)]
    public Vector2 PrimaryR
    {
        get => primaryR;
        set => this.Change(ref primaryR, value);
    }

    Vector2 primaryG;
    [Copy, Horizontal, Index(4)]
    public Vector2 PrimaryG
    {
        get => primaryG;
        set => this.Change(ref primaryG, value);
    }

    Vector2 primaryB;
    [Copy, Horizontal, Index(5)]
    public Vector2 PrimaryB
    {
        get => primaryB;
        set => this.Change(ref primaryB, value);
    }

    [Copy, DisplayName("Matrix"), Index(6), ReadOnly]
    public override Matrix Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public ChromacityMatrixModel() : base() { }

    public ChromacityMatrixModel(string name, string description, Matrix value) : base(name, description, value) { }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Chromacity):
            case nameof(PrimaryR):
            case nameof(PrimaryG):
            case nameof(PrimaryB):
                Value = XYZ.GetMatrix(new(PrimaryR, PrimaryG, PrimaryB), (XYZ)(xyY)(xy)Chromacity);
                break;
        }
    }
}