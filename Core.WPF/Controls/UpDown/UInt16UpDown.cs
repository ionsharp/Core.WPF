using System;
using Imagin.Core.Linq;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

public class UInt16UpDown : NumericUpDown<ushort>
{
    public override ushort AbsoluteMaximum => ushort.MaxValue;

    public override ushort AbsoluteMinimum => ushort.MinValue;

    public override ushort DefaultIncrement => 1;

    public override ushort DefaultValue => Convert.ToUInt16(0);

    public override bool IsRational => true;

    public override bool IsSigned => false;

    public UInt16UpDown() : base() { }

    protected override ushort GetValue(string input) => input.UInt16();

    protected override string ToString(ushort input) => input.ToString(StringFormat);

    protected override bool CanIncrease() => Value < Maximum;

    protected override bool CanDecrease() => Value > Minimum;

    protected override object OnMaximumCoerced(object input) => Clamp((ushort)input, AbsoluteMaximum, Value);

    protected override object OnMinimumCoerced(object input) => Clamp((ushort)input, Value, AbsoluteMinimum);

    protected override object OnValueCoerced(object input) => Clamp((ushort)input, Maximum, Minimum);

    public override void Increase() => SetCurrentValue(ValueProperty.Property, Convert.ToUInt16(Value + Increment));

    public override void Decrease() => SetCurrentValue(ValueProperty.Property, Convert.ToUInt16(Value - Increment));
}