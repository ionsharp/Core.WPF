using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Imagin.Core.Media;

#region NormalColorModel

[Horizontal, Serializable]
public abstract class NormalColorModel : Namable<Type>, ICloneable
{
    [field: NonSerialized]
    public event EventHandler<EventArgs> ValueChanged;

    [Hidden]
    public string Category => Name.Substring(0, 1).ToUpper();

    [Hidden]
    public override string Name
    {
        get => base.Name;
        set => base.Name = value;
    }

    [NonSerialized]
    bool normalize = false;
    [Hidden]
    public bool Normalize
    {
        get => normalize;
        set => this.Change(ref normalize, value);
    }

    [NonSerialized]
    int precision = 2;
    [Hidden]
    public int Precision
    {
        get => precision;
        set => this.Change(ref precision, value);
    }

    [Hidden]
    public override Type Value
    {
        get => base.Value;
        set
        {
            base.Value = value;
            Name = value.Name;
            this.Changed(() => Category);
        }
    }

    NormalColorModel() : this(null) { }

    public NormalColorModel(Type model) : base(model.Name, model) { }

    protected virtual void OnValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);

    public abstract object Clone();

    public override string ToString() => Name;

    public abstract ColorModel GetColor();
}

#endregion

#region NormalColorModel<T>

[Serializable]
public abstract class NormalColorModel<T> : NormalColorModel where T : IVector
{
    [Copy, Horizontal, Index(0), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus)]
    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameX))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitX))]
    public string DisplayX
    {
        get => GetDisplayValue(0);
        set => SetDisplayValue(value, 0);
    }

    [Copy, Horizontal, Index(1), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus)]
    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameY))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitY))]
    public string DisplayY
    {
        get => GetDisplayValue(1);
        set => SetDisplayValue(value, 1);
    }

    [Copy, Horizontal, Index(2), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus)]
    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameZ))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitZ))]
    public string DisplayZ
    {
        get => GetDisplayValue(2);
        set => SetDisplayValue(value, 2);
    }

    [Hidden]
    public string NameX => $"({Colour.Components[Value][0].Symbol}) {Colour.Components[Value][0].Name}";

    [Hidden]
    public string NameY => $"({Colour.Components[Value][1].Symbol}) {Colour.Components[Value][1].Name}";

    [Hidden]
    public string NameZ => $"({Colour.Components[Value][2].Symbol}) {Colour.Components[Value][2].Name}";

    [Hidden]
    public string UnitX => $"{Colour.Components[Value][0].Unit}";

    [Hidden]
    public string UnitY => $"{Colour.Components[Value][1].Unit}";

    [Hidden]
    public string UnitZ => $"{Colour.Components[Value][2].Unit}";

    One x = default;
    [Hidden]
    public One X
    {
        get => x;
        set => this.Change(ref x, value);
    }

    One y = default;
    [Hidden]
    public One Y
    {
        get => y;
        set => this.Change(ref y, value);
    }

    One z = default;
    [Hidden]
    public One Z
    {
        get => z;
        set => this.Change(ref z, value);
    }

    NormalColorModel() : this(null) { }

    public NormalColorModel(Type model) : base(model) { }

    protected abstract string GetDisplayValue(int index);

    protected abstract void SetDisplayValue(string input, int index);

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Normalize):
            case nameof(Precision):
                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);
                break;

            case nameof(X):
                this.Changed(() => DisplayX);
                OnValueChanged();
                break;

            case nameof(Y):
                this.Changed(() => DisplayY);
                OnValueChanged();
                break;

            case nameof(Z):
                this.Changed(() => DisplayZ);
                OnValueChanged();
                break;
        }
    }
}

#endregion

#region NormalColorModel3

[Serializable]
public class NormalColorModel3 : NormalColorModel<Vector3>
{
    [Hidden]
    public Vector Maximum => new(Colour.Components[Value][0].Maximum, Colour.Components[Value][1].Maximum, Colour.Components[Value][2].Maximum);

    [Hidden]
    public Vector Minimum => new(Colour.Components[Value][0].Minimum, Colour.Components[Value][1].Minimum, Colour.Components[Value][2].Minimum);

    NormalColorModel3() : this(null) { }

    public NormalColorModel3(Type model) : base(model) { }

    public override object Clone() => new NormalColorModel3(Value);

    protected override string GetDisplayValue(int index)
    {
        One result = default;
        switch (index)
        {
            case 0:
                result = X;
                break;
            case 1:
                result = Y;
                break;
            case 2:
                result = Z;
                break;
        }

        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);
        return (Normalize == true ? (double)result : aRange.Convert(bRange.Minimum, bRange.Maximum, result)).Round(Precision).ToString();
    }

    protected override void SetDisplayValue(string input, int index)
    {
        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);

        var result = Normalize == true ? (One)(input?.Double() ?? 0) : (One)bRange.Convert(aRange.Minimum, aRange.Maximum, input?.Double() ?? 0);
        switch (index)
        {
            case 0: X = result; break;
            case 1: Y = result; break;
            case 2: Z = result; break;
        }
    }

    public override ColorModel GetColor()
    {
        var result3 = M.Denormalize(new Vector3(X, Y, Z), Minimum, Maximum);
        return Colour.New(Value, result3[0], result3[1], result3[2]);
    }
}

#endregion

#region NormalColorModel4

[Serializable]
public class NormalColorModel4 : NormalColorModel<Vector4>
{
    [Hidden]
    public Vector Maximum => new(Colour.Components[Value][0].Maximum, Colour.Components[Value][1].Maximum, Colour.Components[Value][2].Maximum, Colour.Components[Value][3].Maximum);

    [Hidden]
    public Vector Minimum => new(Colour.Components[Value][0].Minimum, Colour.Components[Value][1].Minimum, Colour.Components[Value][2].Minimum, Colour.Components[Value][3].Minimum);

    [Copy, Horizontal, Index(3), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameW))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitW))]
    public string DisplayW
    {
        get => GetDisplayValue(3);
        set => SetDisplayValue(value, 3);
    }

    [Hidden]
    public string NameW => $"({Colour.Components[Value][3].Symbol}) {Colour.Components[Value][3].Name}";

    [Hidden]
    public string UnitW => $"{Colour.Components[Value][3].Unit}";

    One w = default;
    [Hidden]
    public One W
    {
        get => w;
        set => this.Change(ref w, value);
    }

    NormalColorModel4() : this(null) { }

    public NormalColorModel4(Type model) : base(model) { }

    public override object Clone() => new NormalColorModel4(Value);

    protected override string GetDisplayValue(int index)
    {
        One result = default;
        switch (index)
        {
            case 0:
                result = X;
                break;
            case 1:
                result = Y;
                break;
            case 2:
                result = Z;
                break;
            case 3:
                result = W;
                break;
        }

        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);
        return (Normalize == true ? (double)result : aRange.Convert(bRange.Minimum, bRange.Maximum, result)).Round(Precision).ToString();
    }

    protected override void SetDisplayValue(string input, int index)
    {
        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);

        var result = Normalize == true ? (One)(input?.Double() ?? 0) : (One)bRange.Convert(aRange.Minimum, aRange.Maximum, input?.Double() ?? 0);
        switch (index)
        {
            case 0: X = result; break;
            case 1: Y = result; break;
            case 2: Z = result; break;
            case 3: W = result; break;
        }
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Normalize):
            case nameof(Precision):
                this.Changed(() => DisplayW);
                break;

            case nameof(W):
                this.Changed(() => DisplayW);
                OnValueChanged();
                break;
        }
    }

    public override ColorModel GetColor()
    {
        var result4 = M.Denormalize(new Vector4(X, Y, Z, W), Minimum, Maximum);
        return Colour.New(Value, result4[0], result4[1], result4[2], result4[3]);
    }
}

#endregion

#region NormalColorModelWrapper

public class NormalColorModelWrapper : NamableCategory<Type>
{
    public NormalColorModelWrapper(Type input) : base(input.Name.Substring(0, 1).ToUpper(), input.GetCategory() ?? "General", input) { }
}

#endregion