using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Storage;
using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(bool))]
public class BooleanOrConverter : IMultiValueConverter
{
    public BooleanOrConverter() : base() { }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        foreach (object value in values)
        {
            if ((bool)value == true)
            {
                return true;
            }
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
}

///

[ValueConversion(typeof(double), typeof(bool))]
public class DoubleGreaterThanConverter : ValueConverter<double, bool>
{
    public DoubleGreaterThanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<double> input)
        => input.Value > double.Parse($"{input.ActualParameter}");
}

///

[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumFlagsToBooleanConverter : ValueConverter<Enum, bool>
{
    public EnumFlagsToBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<Enum> input) => input.ActualParameter is Enum i && input.Value.HasFlag(i);

    protected override ConverterValue<Enum> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(bool))]
public class EnumHasAttributeConverter : ValueConverter<object, bool>
{
    public EnumHasAttributeConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type attribute)
        {
            if (input.Value is Enum field)
                return field.HasAttribute(attribute);
        }
        return false;
    }
}

///

[ValueConversion(typeof(object), typeof(bool))]
public class HasAttributeConverter : ValueConverter<object, bool>
{
    public HasAttributeConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type attribute)
        {
            if (input.Value is MemberModel member)
                return member.HasAttribute(attribute);

            if (input.Value is Type type)
                return type.HasAttribute(attribute);

            return input.Value?.GetType()?.HasAttribute(attribute) == true;
        }

        return false;
    }
}

///

[ValueConversion(typeof(int), typeof(bool))]
public class Int32GreaterThanConverter : ValueConverter<int, bool>
{
    public Int32GreaterThanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<int> input)
        => input.Value > int.Parse($"{input.ActualParameter}");
}

[ValueConversion(typeof(int), typeof(bool))]
public class Int32EqualsConverter : ValueConverter<int, bool>
{
    public Int32EqualsConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<int> input)
        => input.Value == int.Parse($"{input.ActualParameter}");
}

[ValueConversion(typeof(int), typeof(bool))]
public class Int32LessThanConverter : ValueConverter<int, bool>
{
    public Int32LessThanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<int> input)
        => input.Value < int.Parse($"{input.ActualParameter}");
}

///

[ValueConversion(typeof(string), typeof(bool))]
public class FileExistsConverter : ValueConverter<string, bool>
{
    public FileExistsConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<string> input) => File.Long.Exists(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(bool))]
public class FileHiddenConverter : ValueConverter<string, bool>
{
    public FileHiddenConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<string> input) => Computer.Hidden(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(bool))]
public class FolderExistsConverter : ValueConverter<string, bool>
{
    public FolderExistsConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<string> input) => Folder.Long.Exists(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(Enum), typeof(bool))]
public class HasFlagConverter : ValueConverter<Enum, bool>
{
    public HasFlagConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<Enum> input) => input.Value.HasFlag((Enum)input.ActualParameter);

    protected override ConverterValue<Enum> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(Type), typeof(bool))]
public class HiddenConverter : ValueConverter<Type, bool>
{
    public HiddenConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<Type> input) => input.Value?.IsHidden() == true;

    protected override ConverterValue<Type> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

///

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : ValueConverter<bool, bool>
{
    public InverseBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<bool> input) => !input.Value;

    protected override ConverterValue<bool> ConvertBack(ConverterData<bool> input) => !input.Value;
}

///

[ValueConversion(typeof(object), typeof(bool))]
public class IsConverter : ValueConverter<object, bool>
{
    public IsConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        var a = input.Value.GetType();
        if (input.ActualParameter is Type b)
            return a.IsSubclassOf(b) || a.Equals(b) || (b.GetTypeInfo().IsInterface && a.Implements(b));

        return false;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(bool))]
public class IsNullConverter : ValueConverter<object, bool>
{
    public IsNullConverter() : base() { }

    protected override bool AllowNull => true;

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        var result = input.Value == null;
        return input.Parameter == 1 ? !result : result;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(bool))]
public class IsStringConverter : ValueConverter<object, bool>
{
    public IsStringConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input) => input.Value is string;

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

///

[ValueConversion(typeof(IEnumerable), typeof(bool))]
public class IEnumerableToBooleanConverter : ValueConverter<IEnumerable, bool>
{
    public IEnumerableToBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<IEnumerable> input) => !input.Value.Empty();

    protected override ConverterValue<IEnumerable> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(IList), typeof(bool))]
public class IListToBooleanConverter : ValueConverter<IList, bool>
{
    public IListToBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<IList> input) => input.Value.Count > 0;

    protected override ConverterValue<IList> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(int), typeof(bool))]
public class IntToBooleanConverter : ValueConverter<int, bool>
{
    public IntToBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<int> input) => input.Value > 0;

    protected override ConverterValue<int> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

///

[ValueConversion(typeof(MemberModel), typeof(bool))]
public class MemberModelHasAttributeConverter : ValueConverter<MemberModel, bool>
{
    public MemberModelHasAttributeConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<MemberModel> input)
        => input.ActualParameter is Type type && input.Value.HasAttribute(type);
}

///

[ValueConversion(typeof(object), typeof(bool))]
public class ObjectHasFieldConverter : ValueConverter<object, bool>
{
    public ObjectHasFieldConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input) => input.Value.IfGet(i => input.ActualParameter is object name ? input.Value.GetField($"{name}") != null : false);

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(bool))]
public class ObjectHasMemberWithAttributeConverter : ValueConverter<object, bool>
{
    public ObjectHasMemberWithAttributeConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type attribute)
        {
            var type = input.Value is Type t ? t : input.Value.GetType();
            return type.HasMemberWithAttribute(attribute);
        }
        return false;
    }
}

[ValueConversion(typeof(object), typeof(bool))]
public class ObjectHasPropertyConverter : ValueConverter<object, bool>
{
    public ObjectHasPropertyConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input) => input.Value.IfGet(i => input.ActualParameter is object name ? input.Value.GetProperty($"{name}") != null : false);

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectHasFieldVisibilityConverter : ValueConverter<object, Visibility>
{
    public ObjectHasFieldVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        try
        {
            if (input.ActualParameter is object name)
            {
                var field = input.Value.GetField($"{name}");
                return (field != null).Visibility();
            }
        }
        catch { }
        return Visibility.Visible;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectHasPropertyVisibilityConverter : ValueConverter<object, Visibility>
{
    public ObjectHasPropertyVisibilityConverter() : base() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        try
        {
            if (input.ActualParameter is object name)
            {
                var property = input.Value.GetProperty($"{name}");
                return (property != null).Visibility();
            }
        }
        catch { }
        return Visibility.Visible;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<Visibility> input) => Nothing.Do;
}

///

[ValueConversion(typeof(Orientation), typeof(bool))]
public class OrientationToBooleanConverter : ValueConverter<Orientation, bool>
{
    public OrientationToBooleanConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<Orientation> input) => input.Value == (Orientation)Enum.Parse(typeof(Orientation), (string)input.ActualParameter);

    protected override ConverterValue<Orientation> ConvertBack(ConverterData<bool> input) => input.Value ? (Orientation)Enum.Parse(typeof(Orientation), (string)input.ActualParameter) : default;
}

[ValueConversion(typeof(string), typeof(bool))]
public class StringToBooleanConverter : ValueConverter<string, bool>
{
    public StringToBooleanConverter() : base() { }

    protected override bool AllowNull => true;

    protected override ConverterValue<bool> ConvertTo(ConverterData<string> input) => input.Value.NullOrEmpty() == false;

    protected override ConverterValue<string> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

///

[ValueConversion(typeof(object), typeof(bool))]
public class ValueEqualsParameterConverter : ValueConverter<object, bool>
{
    public ValueEqualsParameterConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input) => input.Value.Equals(input.ActualParameter);

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input)
    {
        if (!input.Value || input.ActualParameter == null)
            return Nothing.Do;

        return input.ActualParameter;
    }
}

[ValueConversion(typeof(object), typeof(bool))]
public class ValueNotEqualToParameterConverter : ValueConverter<object, bool>
{
    public ValueNotEqualToParameterConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input) => !input.Value.Equals(input.ActualParameter);

    protected override ConverterValue<object> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}