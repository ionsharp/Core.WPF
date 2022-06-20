using Imagin.Core.Colors;
using Imagin.Core.Numerics;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Linq;

public static partial class XColorMatrix
{
    public static ColorMatrix Convert(WriteableBitmap input)
    {
        var result = new ColorMatrix(input.PixelHeight.UInt32(), input.PixelWidth.UInt32());
        input.ForEach((x, y, color) =>
        {
            result.SetValue(y.UInt32(), x.UInt32(), new(M.Normalize(color.A), M.Normalize(color.R), M.Normalize(color.G), M.Normalize(color.B)));
            return color;
        });
        return result;
    }

    public static unsafe ColorMatrix Resize(this ColorMatrix input, Int32Size size, Media.Interpolations interpolation)
    {
        var result = new ColorMatrix(size.Height.UInt32(), size.Width.UInt32());

        var xs = input.Columns.Single() / size.Width.Single();
        var ys = input.Rows.Single() / size.Height.Single();

        float fracx, fracy, ifracx, ifracy, sx, sy, l0, l1, rf, gf, bf;
        int c, x0, x1, y0, y1;
        byte c1a, c1r, c1g, c1b, c2a, c2r, c2g, c2b, c3a, c3r, c3g, c3b, c4a, c4r, c4g, c4b;
        byte a, r, g, b;

        int[] pixels = new int[input.Columns * input.Rows];

        var total = 0;
        for (uint x = 0; x < input.Columns; x++)
        {
            for (uint y = 0; y < input.Rows; y++)
                pixels[total++] = XColor.Convert(input.GetValue(y, x)).Encode();
        }

        int widthSource = input.Columns.Int32(), heightSource = input.Rows.Int32();

        //Nearest Neighbor
        if (interpolation == Media.Interpolations.NearestNeighbor)
        {
            for (var y = 0; y < size.Height; y++)
            {
                for (var x = 0; x < size.Width; x++)
                {
                    sx = x * xs;
                    sy = y * ys;

                    x0 = (int)sx;
                    y0 = (int)sy;

                    var i = y0 * widthSource + x0;
                    var j = pixels[i].Decode();

                    j.Convert(out Vector4 k);
                    result.SetValue(y.UInt32(), x.UInt32(), k);
                }
            }
        }

        //Bilinear
        else if (interpolation == Media.Interpolations.Bilinear)
        {
            for (var y = 0; y < size.Height; y++)
            {
                for (var x = 0; x < size.Width; x++)
                {
                    sx = x * xs;
                    sy = y * ys;

                    x0 = (int)sx;
                    y0 = (int)sy;

                    //Calculate coordinates of the 4 interpolation points
                    fracx = sx - x0;
                    fracy = sy - y0;

                    ifracx = 1f - fracx;
                    ifracy = 1f - fracy;

                    x1 = x0 + 1;
                    if (x1 >= widthSource)
                        x1 = x0;

                    y1 = y0 + 1;
                    if (y1 >= heightSource)
                        y1 = y0;

                    //Read source Vector4
                    c = pixels[y0 * widthSource + x0];
                    c1a = (byte)(c >> 24);
                    c1r = (byte)(c >> 16);
                    c1g = (byte)(c >> 8);
                    c1b = (byte)(c);

                    c = pixels[y0 * widthSource + x1];
                    c2a = (byte)(c >> 24);
                    c2r = (byte)(c >> 16);
                    c2g = (byte)(c >> 8);
                    c2b = (byte)(c);

                    c = pixels[y1 * widthSource + x0];
                    c3a = (byte)(c >> 24);
                    c3r = (byte)(c >> 16);
                    c3g = (byte)(c >> 8);
                    c3b = (byte)(c);

                    c = pixels[y1 * widthSource + x1];
                    c4a = (byte)(c >> 24);
                    c4r = (byte)(c >> 16);
                    c4g = (byte)(c >> 8);
                    c4b = (byte)(c);


                    // Calculate colors
                    // Alpha
                    l0 = ifracx * c1a + fracx * c2a;
                    l1 = ifracx * c3a + fracx * c4a;
                    a = (byte)(ifracy * l0 + fracy * l1);

                    // Red
                    l0 = ifracx * c1r + fracx * c2r;
                    l1 = ifracx * c3r + fracx * c4r;
                    rf = ifracy * l0 + fracy * l1;

                    // Green
                    l0 = ifracx * c1g + fracx * c2g;
                    l1 = ifracx * c3g + fracx * c4g;
                    gf = ifracy * l0 + fracy * l1;

                    // Blue
                    l0 = ifracx * c1b + fracx * c2b;
                    l1 = ifracx * c3b + fracx * c4b;
                    bf = ifracy * l0 + fracy * l1;

                    // Cast to byte
                    r = (byte)rf;
                    g = (byte)gf;
                    b = (byte)bf;

                    result.SetValue(y.UInt32(), x.UInt32(), new Vector4(a, r, g, b));
                }
            }
        }
        return result;
    }
}