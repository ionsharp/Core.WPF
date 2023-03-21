using Imagin.Core.Linq;
using System;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

public class UInt32UpDown : NumericUpDown<uint>
{
    public override uint AbsoluteMaximum => uint.MaxValue;

    public override uint AbsoluteMinimum => uint.MinValue;

    public override uint DefaultIncrement => 1;

    public override uint DefaultValue => 0;

    public override bool IsRational => true;

    public override bool IsSigned => false;

    public UInt32UpDown() : base() { }

    protected override uint GetValue(string input) => input.UInt32();

    protected override string ToString(uint input) => input.ToString(StringFormat);

    protected override bool CanIncrease() => Value < Maximum;

    protected override bool CanDecrease() => Value > Minimum;

    protected override object OnMaximumCoerced(object input) => Clamp((uint)input, AbsoluteMaximum, Value);

    protected override object OnMinimumCoerced(object input) => Clamp((uint)input, Value, AbsoluteMinimum);

    protected override object OnValueCoerced(object input) => Clamp((uint)input, Maximum, Minimum);

    public override void Increase() => SetCurrentValue(ValueProperty.Property, Value + Increment);

    public override void Decrease() => SetCurrentValue(ValueProperty.Property, Value - Increment);
}