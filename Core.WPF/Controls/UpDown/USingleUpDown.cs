using Imagin.Core.Linq;

namespace Imagin.Core.Controls;

public class USingleUpDown : NumericUpDown<USingle>
{
    public override USingle AbsoluteMaximum => USingle.MaxValue;

    public override USingle AbsoluteMinimum => USingle.MinValue;

    public override USingle DefaultIncrement => 1;

    public override USingle DefaultValue => 0;

    public override bool IsRational => false;

    public override bool IsSigned => false;

    public USingleUpDown() : base() { }

    protected override USingle GetValue(string input) => input.USingle();

    protected override string ToString(USingle input) => input.ToString(StringFormat);

    protected override bool CanIncrease() => Value < Maximum;

    protected override bool CanDecrease() => Value > Minimum;

    protected override object OnMaximumCoerced(object input) => input.As<USingle>().Clamp(AbsoluteMaximum, Value);

    protected override object OnMinimumCoerced(object input) => input.As<USingle>().Clamp(Value, AbsoluteMinimum);

    protected override object OnValueCoerced(object input) => input.As<USingle>().Clamp(Maximum, Minimum);

    public override void Increase() => SetCurrentValue(ValueProperty.Property, Value + Increment);

    public override void Decrease() => SetCurrentValue(ValueProperty.Property, Value - Increment);
}