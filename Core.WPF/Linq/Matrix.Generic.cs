using Imagin.Core.Numerics;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Linq;

public static class XMatrix
{
    public static WriteableBitmap Convert(this Matrix<Vector4> input)
    {
        if (input != null)
        {
            var result = XBitmap.New(input.Columns.Int32(), input.Rows.Int32());
            input.Each((y, x, i) =>
            {
                //result.SetPixel(x, y, System.Windows.Media.Color.FromArgb(M.Denormalize(i.W), M.Denormalize(i.X), M.Denormalize(i.Y), M.Denormalize(i.Z)));
                return i;
            });
            return result;
        }
        return null;
    }
}