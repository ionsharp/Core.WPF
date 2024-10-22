﻿using Imagin.Core.Linq;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object), typeof(object))]
public class NullConverter : ValueConverter<object, object>
{
    public NullConverter() : base() { }

    protected override bool AllowNull => true;

    ConverterValue<object> GetResult(object value)
    {
        if (value == null || (value is string i && i.TrimWhitespace().Empty()))
            return Nothing.Do;

        return value;
    }

    protected override ConverterValue<object> ConvertTo(ConverterData<object> input)
        => GetResult(input.ActualValue);

    protected override ConverterValue<object> ConvertBack(ConverterData<object> input)
        => GetResult(input.ActualValue);
}