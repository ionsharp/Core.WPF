using Imagin.Core.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Visibility), typeof(Visibility))]
public class InverseVisibilityConverter : ValueConverter<Visibility, Visibility>
{
    public InverseVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<Visibility> input) => input.Value.Invert();

    protected override ConverterValue<Visibility> ConvertBack(ConverterData<Visibility> input) => input.Value.Invert();
}

///

[ValueConversion(typeof(int), typeof(Visibility))]
public class Int32GreaterThanVisibilityConverter : ValueConverter<int, Visibility>
{
    public Int32GreaterThanVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<int> input)
        => (input.Value > int.Parse($"{input.ActualParameter}")).Visibility();
}

[ValueConversion(typeof(int), typeof(Visibility))]
public class Int32EqualsVisibilityConverter : ValueConverter<int, Visibility>
{
    public Int32EqualsVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<int> input)
        => (input.Value == int.Parse($"{input.ActualParameter}")).Visibility();
}

[ValueConversion(typeof(int), typeof(Visibility))]
public class Int32LessThanVisibilityConverter : ValueConverter<int, Visibility>
{
    public Int32LessThanVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<int> input)
        => (input.Value < int.Parse($"{input.ActualParameter}")).Visibility();
}

///

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityConverter : ValueConverter<object, object>
{
    public BooleanToVisibilityConverter() : base() { }

    protected override bool Is(object input) => input is bool || input is bool? || input is Handle || input is Visibility;

    protected override ConverterValue<object> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualValue is bool || input.ActualValue is bool?)
        {
            var i = input.ActualValue is bool ? (bool)input.ActualValue : input.ActualValue is bool? ? ((bool?)input.ActualValue).Value : throw new NotSupportedException();
            var result = i.Visibility(input.ActualParameter is Visibility ? (Visibility)input.ActualParameter : Visibility.Collapsed);

            return input.Parameter == 0
                ? result
                : input.Parameter == 1
                    ? result.Invert()
                    : throw input.InvalidParameter;
        }
        return ConvertBack(input);
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<object> input)
    {
        if (input.ActualValue is Visibility visibility)
        {
            var result = ((Visibility)input.ActualValue).Boolean();
            return input.Parameter == 0
                ? result
                : input.Parameter == 1
                    ? !result
                    : throw input.InvalidParameter;
        }

        return ConvertTo(input);
    }
}

[ValueConversion(typeof(double), typeof(Visibility))]
public class DoubleToVisibilityConverter : ValueConverter<double, Visibility>
{
    public DoubleToVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<double> input)
    {
        var result = (input.Value > 0).Visibility();
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<double> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(int), typeof(Visibility))]
public class Int32ToVisibilityConverter : ValueConverter<int, Visibility>
{
    public Int32ToVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<int> input)
    {
        var result = (input.Value > 0).Visibility();
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<int> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(long), typeof(Visibility))]
public class Int64ToVisibilityConverter : ValueConverter<long, Visibility>
{
    public Int64ToVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<long> input)
    {
        var result = (input.Value > 0).Visibility();
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<long> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectHasMemberWithAttributeVisibilityConverter : Conversion.ValueConverter<object, Visibility>
{
    public ObjectHasMemberWithAttributeVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type attribute)
        {
            var type = input.Value is Type t ? t : input.Value.GetType();
            return type.HasMemberWithAttribute(attribute).Visibility();
        }
        return Visibility.Collapsed;
    }
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectToVisibilityConverter : ValueConverter<object, Visibility>
{
    public ObjectToVisibilityConverter() : base() { }

    protected override bool AllowNull => true;

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        var result = (input.ActualValue != null).Visibility();
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectIsToVisibilityConverter : ValueConverter<object, Visibility>
{
    public ObjectIsToVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type i)
            return input.Value.GetType().IsSubclassOf(i).Visibility();

        return Nothing.Do;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(Orientation), typeof(Visibility))]
public class OrientationToVisibilityConverter : ValueConverter<Orientation, Visibility>
{
    public OrientationToVisibilityConverter() : base() { }

    Visibility Convert(Orientation input) => input == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed;

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<Orientation> input)
    {
        return input.Parameter == 0 ? Convert(input.Value) : input.Parameter == 1 ? Convert(input.Value).Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<Orientation> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(Visibility))]
public class StringToVisibilityConverter : ValueConverter<string, Visibility>
{
    public StringToVisibilityConverter() : base() { }

    protected override bool AllowNull => true;

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<string> input)
    {
        var result = (!input.Value.NullOrEmpty()).Visibility();
        return input.Parameter == 0 ? result : input.Parameter == 1 ? result.Invert() : throw input.InvalidParameter;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

///

[ValueConversion(typeof(object), typeof(Visibility))]
public class ValueEqualsParameterVisibilityConverter : ValueConverter<object, Visibility>
{
    public ValueEqualsParameterVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input) => input.Value.Equals(input.ActualParameter).Visibility();

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => input.ActualParameter;
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ValueNotEqualToParameterVisibilityConverter : ValueConverter<object, Visibility>
{
    public ValueNotEqualToParameterVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input) => (!input.Value.Equals(input.ActualParameter)).Visibility();

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}