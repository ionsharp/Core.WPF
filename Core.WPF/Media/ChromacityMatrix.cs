using Imagin.Core.Numerics;
using Imagin.Core.Colors;
using System;
using System.Runtime.CompilerServices;

namespace Imagin.Core.Controls;

[Image(SmallImages.MatrixXY), Name("Chromacity matrix"), Serializable]
public class ChromacityMatrix : DoubleMatrix
{
    [Vertical, Index(2)]
    public Vector2 Chromacity { get => Get<Vector2>(); set => Set(value); }

    [Vertical, Index(3)]
    public Vector2 PrimaryR { get => Get<Vector2>(); set => Set(value); }

    [Vertical, Index(4)]
    public Vector2 PrimaryG { get => Get<Vector2>(); set => Set(value); }

    [Vertical, Index(5)]
    public Vector2 PrimaryB { get => Get<Vector2>(); set => Set(value); }

    [Index(6)]
    public DoubleMatrix Value { get => Get<DoubleMatrix>(); set => Set(value); }

    public ChromacityMatrix() : base() { }

    public ChromacityMatrix(Matrix value) : base(value) => Value = new(value);

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Chromacity):
            case nameof(PrimaryR):
            case nameof(PrimaryG):
            case nameof(PrimaryB):
                values = XYZ.GetMatrix(new(PrimaryR, PrimaryG, PrimaryB), (XYZ)(xyY)(xy)Chromacity);
                Value = new(values);
                break;
        }
    }
}