using Imagin.Core.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(ControlLength), typeof(GridLength))]
public class GridLengthConverter : ValueConverter<ControlLength, GridLength>
{
    public GridLengthConverter() : base() { }

    protected override ConverterValue<GridLength> ConvertTo(ConverterData<ControlLength> input) => (GridLength)input.Value;

    protected override ConverterValue<ControlLength> ConvertBack(ConverterData<GridLength> input) => (ControlLength)input.Value;
}

[ValueConversion(typeof(DataGridLength), typeof(GridLength))]
public class DataGridLengthConverter : ValueConverter<DataGridLength, GridLength>
{
    public DataGridLengthConverter() : base() { }

    protected override ConverterValue<GridLength> ConvertTo(ConverterData<DataGridLength> input) => new GridLength(input.Value.Value, input.Value.UnitType == DataGridLengthUnitType.Star ? GridUnitType.Star : input.Value.UnitType == DataGridLengthUnitType.Pixel ? GridUnitType.Pixel : GridUnitType.Auto);

    protected override ConverterValue<DataGridLength> ConvertBack(ConverterData<GridLength> input) => new DataGridLength(input.Value.Value, input.Value.GridUnitType == GridUnitType.Star ? DataGridLengthUnitType.Star : input.Value.GridUnitType == GridUnitType.Pixel ? DataGridLengthUnitType.Pixel : DataGridLengthUnitType.Auto);
}