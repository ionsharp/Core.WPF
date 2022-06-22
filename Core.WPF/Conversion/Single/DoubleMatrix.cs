using Imagin.Core.Numerics;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(Matrix), typeof(DoubleMatrix))]
public class MatrixToDoubleMatrixConverter : Converter<Matrix, DoubleMatrix>
{
    public static MatrixToDoubleMatrixConverter Default { get; private set; } = new();
    public MatrixToDoubleMatrixConverter() { }

    protected override ConverterValue<DoubleMatrix> ConvertTo(ConverterData<Matrix> input)
        => new DoubleMatrix((double[,])input.Value);

    protected override ConverterValue<Matrix> ConvertBack(ConverterData<DoubleMatrix> input)
        => new Matrix(input.Value);
}