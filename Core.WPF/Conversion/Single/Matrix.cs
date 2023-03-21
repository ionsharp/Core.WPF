using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(WriteableBitmap), typeof(Matrix<Vector4>))]
public class WriteableBitmapToMatrixConverter : ValueConverter<WriteableBitmap, Matrix<Vector4>>
{
    public WriteableBitmapToMatrixConverter() : base() { }

    protected override ConverterValue<WriteableBitmap> ConvertBack(ConverterData<Matrix<Vector4>> input)
        => input.Value.Convert();

    protected override ConverterValue<Matrix<Vector4>> ConvertTo(ConverterData<WriteableBitmap> input)
        => input.Value.Convert();
}