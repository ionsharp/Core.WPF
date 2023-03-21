using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Thickness), typeof(Thickness))]
public class InverseThicknessConverter : ValueConverter<Thickness, Thickness>
{
    public InverseThicknessConverter() : base() { }

    protected override ConverterValue<Thickness> ConvertTo(ConverterData<Thickness> input) => input.Value.Invert();

    protected override ConverterValue<Thickness> ConvertBack(ConverterData<Thickness> input) => input.Value.Invert();
}

[ValueConversion(typeof(Margin), typeof(Thickness))]
public class MarginToThicknessConverter : ValueConverter<Margin, Thickness>
{
    public MarginToThicknessConverter() : base() { }

    protected override ConverterValue<Thickness> ConvertTo(ConverterData<Margin> input) => input.Value.Value;

    protected override ConverterValue<Margin> ConvertBack(ConverterData<Thickness> input) => new Margin(input.Value);
}