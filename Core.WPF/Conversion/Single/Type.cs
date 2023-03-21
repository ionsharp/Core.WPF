using System;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Type), typeof(Type))]
public class BaseTypeConverter : ValueConverter<Type, Type>
{
    public BaseTypeConverter() : base() { }

    protected override ConverterValue<Type> ConvertTo(ConverterData<Type> input) => input.Value.BaseType;

    protected override ConverterValue<Type> ConvertBack(ConverterData<Type> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(Type))]
public class GetTypeConverter : ValueConverter<object, Type>
{
    public GetTypeConverter() : base() { }

    protected override ConverterValue<Type> ConvertTo(ConverterData<object> input) => input.Value.GetType();

    protected override ConverterValue<object> ConvertBack(ConverterData<Type> input) => Nothing.Do;
}