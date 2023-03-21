using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(int))]
public class DoubleToInt32Converter : ValueConverter<double, int>
{
    public DoubleToInt32Converter() : base() { }

    protected override ConverterValue<int> ConvertTo(ConverterData<double> input) => input.Value.Int32();

    protected override ConverterValue<double> ConvertBack(ConverterData<int> input) => input.Value.Double();
}

[ValueConversion(typeof(FrameworkElement), typeof(int))]
public class IndexConverter : ValueConverter<FrameworkElement, int>
{
    public IndexConverter() : base() { }

    protected override ConverterValue<int> ConvertTo(ConverterData<FrameworkElement> input)
    {
        var item = input.Value;
        var itemsControl = ItemsControl.ItemsControlFromItemContainer(item);

        var index = itemsControl?.ItemContainerGenerator.IndexFromContainer(item) ?? 0;
        return input.Parameter + index;
    }

    protected override ConverterValue<FrameworkElement> ConvertBack(ConverterData<int> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(int))]
public class StringLengthConverter : ValueConverter<string, int>
{
    public StringLengthConverter() : base() { }

    protected override ConverterValue<int> ConvertTo(ConverterData<string> input) => input.Value.Length;

    protected override ConverterValue<string> ConvertBack(ConverterData<int> input) => Nothing.Do;
}