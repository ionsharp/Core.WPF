using Imagin.Core.Conversion;
using Imagin.Core.Numerics;
using System;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Media;

[Serializable]
public class BinaryBitmap : BinaryValue<WriteableBitmap, Matrix<Vector4>, WriteableBitmapToMatrixConverter>
{
    public BinaryBitmap() : this(null) { }

    public BinaryBitmap(WriteableBitmap value) : base(value) { }
}