using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Data;

[ValueConversion(typeof(object), typeof(bool))]
public class HasAttributeConverter : Conversion.Converter<object, bool>
{
    public static HasAttributeConverter Default { get; private set; } = new();
    public HasAttributeConverter() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type type)
        {
            foreach (var i in MemberCollection.GetMembers(input.Value is Type j ? j : input.Value.GetType()))
            {
                if (i.HasAttribute(type))
                    return true;
            }
        }
        return false;
    }
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class HasAttributeVisibilityConverter : Conversion.Converter<object, Visibility>
{
    public static HasAttributeVisibilityConverter Default { get; private set; } = new();
    public HasAttributeVisibilityConverter() { }

    protected override ConverterValue<Visibility> ConvertTo(ConverterData<object> input)
    {
        if (input.ActualParameter is Type type)
        {
            foreach (var i in MemberCollection.GetMembers(input.Value.GetType()))
            {
                if (i.HasAttribute(type))
                    return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }
}

public class HasAttributeBinding : Binding
{
    public HasAttributeBinding(Type type) : base()
    {
        Converter = HasAttributeConverter.Default;
        ConverterParameter = type;
    }
}

public class HasAttributeVisibilityBinding : Binding
{
    public HasAttributeVisibilityBinding(Type type) : base()
    {
        Converter = HasAttributeVisibilityConverter.Default;
        ConverterParameter = type;
    }
}