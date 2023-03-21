using Imagin.Core.Media;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Gradient), typeof(LinearGradientBrush))]
public class GradientConverter : ValueConverter<Gradient, LinearGradientBrush>
{
    public GradientConverter() : base() { }

    protected override ConverterValue<LinearGradientBrush> ConvertTo(ConverterData<Gradient> input) => input.Value.LinearBrush();

    protected override ConverterValue<Gradient> ConvertBack(ConverterData<LinearGradientBrush> input) => new Gradient(input.Value);
}

[ValueConversion(typeof(LinearGradientBrush), typeof(Gradient))]
public class LinearGradientBrushConverter : ValueConverter<LinearGradientBrush, Gradient>
{
    public LinearGradientBrushConverter() : base() { }

    protected override ConverterValue<Gradient> ConvertTo(ConverterData<LinearGradientBrush> input) => new Gradient(input.Value);

    protected override ConverterValue<LinearGradientBrush> ConvertBack(ConverterData<Gradient> input) => input.Value.LinearBrush();
}

[ValueConversion(typeof(RadialGradientBrush), typeof(Gradient))]
public class RadialGradientBrushConverter : ValueConverter<RadialGradientBrush, Gradient>
{
    public RadialGradientBrushConverter() : base() { }

    protected override ConverterValue<Gradient> ConvertTo(ConverterData<RadialGradientBrush> input) => new Gradient(input.Value);

    protected override ConverterValue<RadialGradientBrush> ConvertBack(ConverterData<Gradient> input) => input.Value.RadialBrush();
}