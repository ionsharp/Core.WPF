using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

public class AttributeMultiConverter<TConverter> : MultiConverter<object> where TConverter : IValueConverter
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 0)
        {
            if (values[0] is object value)
                return Converter.Get(typeof(TConverter)).Convert(value, targetType, parameter, culture);
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object), typeof(object))]
public abstract class AttributeConverter<TAttribute, TValue> : ValueConverter<object, TValue> where TAttribute : Attribute
{
    public AttributeConverter() : base() { }

    ///

    protected virtual Attribute GetAttribute(Enum input) => input.GetAttribute<TAttribute>();

    protected virtual Attribute GetAttribute(MemberInfo input) => input.GetAttribute<TAttribute>();

    protected virtual Attribute GetAttribute(MemberModel input) => input.GetAttribute<TAttribute>();

    protected virtual Attribute GetAttribute(Type input) => input.GetAttribute<TAttribute>();

    ///

    protected virtual TValue GetFallback(Enum input) => default;

    protected virtual TValue GetFallback(MemberInfo input) => default;

    protected virtual TValue GetFallback(MemberModel input) => default;

    protected virtual TValue GetFallback(Type input) => default;

    ///

    protected virtual Type GetFallbackType(Enum input) => null;

    protected virtual Type GetFallbackType(MemberInfo input) => null;

    protected virtual Type GetFallbackType(MemberModel input) => input.ValueType;

    ///

    protected abstract TValue GetResult(Attribute input, object parameter);

    protected override ConverterValue<TValue> ConvertTo(ConverterData<object> input)
    {
        if (input.Value is Enum a)
            return GetResult(GetAttribute(a) ?? GetFallbackType(a).IfReturn<Type, Attribute>(GetAttribute), input.ActualParameter) ?? GetFallback(a);

        if (input.Value is MemberInfo b)
            return GetResult(GetAttribute(b) ?? GetFallbackType(b).IfReturn<Type, Attribute>(GetAttribute), input.ActualParameter) ?? GetFallback(b);

        if (input.Value is MemberModel c)
            return GetResult(GetAttribute(c) ?? GetFallbackType(c).IfReturn<Type, Attribute>(GetAttribute), input.ActualParameter) ?? GetFallback(c);

        Type targetType = null;

        if (input.Value is Type d)
            targetType = d;

        else
        {
            object target = input.Value;
            if (input.Value is object[] f && f.Length > 0)
                target = f[0];

            if (target is IGeneric g)
                targetType = g.GetGenericType();

            targetType ??= target?.GetType();
        }

        if (targetType != null)
            return GetResult(GetAttribute(targetType), input.ActualParameter) ?? GetFallback(targetType);

        return Nothing.Do;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<TValue> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class AbbreviationAttributeConverter : AttributeConverter<AbbreviationAttribute, string>
{
    public AbbreviationAttributeConverter() : base() { }

    protected override string GetResult(Attribute input, object parameter)
        => input.As<AbbreviationAttribute>().Abbreviation;
}

[ValueConversion(typeof(object), typeof(Type))]
public class BaseAttributeConverter : AttributeConverter<BaseAttribute, Type>
{
    public BaseAttributeConverter() : base() { }

    protected override Type GetResult(Attribute input, object parameter)
        => input.As<BaseAttribute>()?.Type;
}

[ValueConversion(typeof(object), typeof(string))]
public class CategoryAttributeConverter : AttributeConverter<CategoryAttribute, string>
{
    public CategoryAttributeConverter() : base() { }

    ///

    protected override Attribute GetAttribute(Enum input)
        => input.GetAttribute<CategoryAttribute>()
        ?? input.GetAttribute<System.ComponentModel.CategoryAttribute>() as Attribute;

    protected override Attribute GetAttribute(Type input)
        => input.GetAttribute<CategoryAttribute>()
        ?? input.GetAttribute<System.ComponentModel.CategoryAttribute>() as Attribute;

    ///

    protected override string GetResult(Attribute input, object parameter)
        => input.As<CategoryAttribute>()?.Category?.ToString() ?? input.As<System.ComponentModel.CategoryAttribute>()?.Category;
}

[ValueConversion(typeof(object), typeof(SolidColorBrush))]
public class ColorAttributeConverter : AttributeConverter<ColorAttribute, SolidColorBrush>
{
    public ColorAttributeConverter() : base() { }

    protected override SolidColorBrush GetResult(Attribute input, object parameter)
        => input.As<ColorAttribute>()?.Color is ByteVector4 color ? new SolidColorBrush(XColor.Convert(color)) : null;
}

[ValueConversion(typeof(object), typeof(string))]
public class DescriptionAttributeConverter : AttributeConverter<DescriptionAttribute, string>
{
    public DescriptionAttributeConverter() : base() { }

    ///

    protected override Attribute GetAttribute(Enum input)
        => input.GetAttribute<DescriptionAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DescriptionAttribute>() as Attribute;

    protected override Attribute GetAttribute(MemberInfo input)
        => input.GetAttribute<DescriptionAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DescriptionAttribute>() as Attribute;

    protected override Attribute GetAttribute(MemberModel input)
        => input.GetAttribute<DescriptionAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DescriptionAttribute>() as Attribute;

    protected override Attribute GetAttribute(Type input)
        => input.GetAttribute<DescriptionAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DescriptionAttribute>() as Attribute;

    ///

    protected override string GetResult(Attribute input, object parameter)
    {
        if (input is DescriptionAttribute x)
        {
            if (x.Localize)
                return x.Description.Translate();

            return x.Description;
        }
        return input.As<System.ComponentModel.DescriptionAttribute>()?.Description;
    }
}

[ValueConversion(typeof(object), typeof(string))]
public class ImageAttributeConverter : AttributeConverter<ImageAttribute, string>
{
    public ImageAttributeConverter() : base() { }

    protected override string GetResult(Attribute input, object parameter)
    {
        parameter ??= 0;
        return (int)parameter switch
        {
            0 => input.As<ImageAttribute>()?.SmallImage,
            1 => input.As<ImageAttribute>()?.LargeImage,
            _ => null
        };
    }
}

[ValueConversion(typeof(object), typeof(string))]
public class NameAttributeConverter : AttributeConverter<NameAttribute, string>
{
    public NameAttributeConverter() : base() { }

    ///

    protected override Attribute GetAttribute(Enum input)
        => input.GetAttribute<NameAttribute>() 
        ?? input.GetAttribute<System.ComponentModel.DisplayNameAttribute>() as Attribute;

    protected override Attribute GetAttribute(MemberInfo input)
        => input.GetAttribute<NameAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DisplayNameAttribute>() as Attribute;

    protected override Attribute GetAttribute(MemberModel input) => new NameAttribute(input.DisplayName);

    protected override Attribute GetAttribute(Type input)
        => input.GetAttribute<NameAttribute>()
        ?? input.GetAttribute<System.ComponentModel.DisplayNameAttribute>() as Attribute;

    ///

    protected override string GetFallback(Enum input) => input.ToString().SplitCamel();

    protected override string GetFallback(MemberInfo input) => input.Name.SplitCamel();

    protected override string GetFallback(MemberModel input) => input.Name;

    protected override string GetFallback(Type input) => input.Name.SplitCamel();

    ///

    protected override string GetResult(Attribute input, object parameter)
    {
        if (input is NameAttribute x)
        {
            if (x.Localize)
                return x.Name.Translate();

            return x.Name;
        }
        return input.As<System.ComponentModel.DisplayNameAttribute>()?.DisplayName;
    }
}