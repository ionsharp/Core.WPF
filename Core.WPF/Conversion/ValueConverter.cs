using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object), typeof(object))]
public class ValueConverter<Input, Output> : IValueConverter, IConvert, IConvert<Input, Output>
{
    protected virtual bool AllowNull { get; set; } = false;

    ///

    readonly Func<ConverterData<Output>, ConverterValue<Input>> CustomConvertBack;

    readonly Func<ConverterData<Input>, ConverterValue<Output>> CustomConvertTo;

    ///

    readonly Func<Output, Input> DefaultConvertBack;

    readonly Func<Input, Output> DefaultConvertTo;

    ///

    Type IConvert.InputType => typeof(Input);

    Type IConvert.OutputType => typeof(Output);

    protected ValueConverter() : base() { }

    //Custom
    public ValueConverter(bool allowNull, Func<ConverterData<Input>, ConverterValue<Output>> to, Func<ConverterData<Output>, ConverterValue<Input>> back = null) : base()
    {
        AllowNull = allowNull;
        CustomConvertTo = to; CustomConvertBack = back;
    }

    //Default
    public ValueConverter(Func<Input, Output> to, Func<Output, Input> back = null) : base()
    {
        DefaultConvertTo = to; DefaultConvertBack = back;
    }

    ///

    public Output ConvertTo(Input input) => DefaultConvertTo == null ? default : DefaultConvertTo(input);

    public Input ConvertBack(Output input) => DefaultConvertBack == null ? default : DefaultConvertBack(input);

    ///

    protected virtual ConverterValue<Output> ConvertTo(ConverterData<Input> input)
        => CustomConvertTo != null ? CustomConvertTo(input) : DefaultConvertTo != null ? DefaultConvertTo(input.Value) : Nothing.Do;

    protected virtual ConverterValue<Input> ConvertBack(ConverterData<Output> output)
        => CustomConvertBack != null ? CustomConvertBack(output) : DefaultConvertBack != null ? DefaultConvertBack(output.Value) : Nothing.Do;

    ///

    protected virtual bool Is(object input) => input is Input;

    ///

    public Output Convert(object value, object parameter = null) => (Output)Convert(value, null, parameter, null);

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (AllowNull || Is(value))
        {
            var result = ConvertTo(new ConverterData<Input>(value, parameter));
            if (result.ActualValue is not Nothing)
                return result.ActualValue;
        }

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, object parameter = null) => ConvertBack(value, null, parameter, null);

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Output i)
        {
            var result = ConvertBack(new ConverterData<Output>(value, parameter));
            if (result.ActualValue is not Nothing)
                return result.ActualValue;
        }
        return Binding.DoNothing;
    }
}