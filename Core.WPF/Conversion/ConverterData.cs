using System;

namespace Imagin.Core.Conversion;

public class ConverterData<T>
{
    public readonly int Parameter;

    public T Value => ActualValue is T i ? i : default;

    public readonly object ActualParameter;

    public readonly object ActualValue;

    public ArgumentOutOfRangeException InvalidParameter => new(nameof(Parameter));

    public ConverterData(object value, object parameter)
    {
        ActualValue = value;
        ActualParameter = parameter;
        int.TryParse(ActualParameter?.ToString(), out Parameter);
    }
}