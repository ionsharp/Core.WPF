using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using System;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object), typeof(ObservableCollection<Enum>))]
public class EnumConverter : ValueConverter<object, ObservableCollection<Enum>>
{
    public EnumConverter() : base() { }

    protected override ConverterValue<ObservableCollection<Enum>> ConvertTo(ConverterData<object> input)
    {
        Type type = input.Value is Type i ? i : input.Value?.GetType();
        return type?.GetEnumCollection(Appearance.Visible);
    }
}

[ValueConversion(typeof(object), typeof(ObservableCollection<Type>))]
public class InheritanceConverter : ValueConverter<object, ObservableCollection<Type>>
{
    public InheritanceConverter() : base() { }

    protected override ConverterValue<ObservableCollection<Type>> ConvertTo(ConverterData<object> input)
    {
        var result = new ObservableCollection<Type>();
        (input.Value is Type i ? i : input.Value.GetType()).GetBaseTypes()?.ForEach(i => result.Insert(0, i));
        return result;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<ObservableCollection<Type>> input) => Nothing.Do;
}