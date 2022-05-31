using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Linq;

public static partial class XBitmap
{
    public static System.Drawing.Bitmap Bitmap<T>(this WriteableBitmap input) where T : BitmapEncoder, new()
    {
        System.Drawing.Bitmap result = default;
        using (var stream = new MemoryStream())
        {
            T encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(input));
            encoder.Save(stream);
            result = new System.Drawing.Bitmap(stream);
        }
        return result;
    }

    public static WriteableBitmap Clone(this WriteableBitmap input)
    {
        var result = new WriteableBitmap(input.PixelWidth, input.PixelHeight, input.DpiX, input.DpiY, input.Format, null);
        input.ForEach((x, y, color) => { result.SetPixel(x, y, color.A, color.R, color.G, color.B); return color; });
        return result;
    }

    public static Matrix<Vector4> Convert(this WriteableBitmap input)
    {
        if (input != null)
        {
            var result = new Matrix<Vector4>(input.PixelHeight.UInt32(), input.PixelWidth.UInt32());
            input.ForEach((x, y, color) =>
            {
                result.SetValue(y.UInt32(), x.UInt32(), new Vector4(M.Normalize(color.A), M.Normalize(color.R), M.Normalize(color.G), M.Normalize(color.B)));
                return color;
            });
            return result;
        }
        return null;
    }

    public static WriteableBitmap Convert(Matrix<Vector4> input)
    {
        if (input != null)
        {
            var result = XBitmap.New((int)input.Columns, (int)input.Rows);
            input.Each((x, y, i) => { result.SetPixel(x, y, XColor.Convert(i)); return i; });
            return result;
        }
        return null;
    }

    public static WriteableBitmap Resize(this WriteableBitmap input, double scale)
    {
        var s = new System.Windows.Media.ScaleTransform(scale, scale);

        var result = new TransformedBitmap(input, s);

        WriteableBitmap a(BitmapSource b)
        {
            // Calculate stride of source
            int stride = b.PixelWidth * (b.Format.BitsPerPixel / 8);

            // Create data array to hold source pixel data
            byte[] data = new byte[stride * b.PixelHeight];

            // Copy source image pixels to the data array
            b.CopyPixels(data, stride, 0);

            // Create WriteableBitmap to copy the pixel data to.      
            WriteableBitmap target = new(b.PixelWidth, b.PixelHeight, b.DpiX, b.DpiY, b.Format, null);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(new Int32Rect(0, 0, b.PixelWidth, b.PixelHeight), data, stride, 0);

            return target;
        }

        return a(result);
    }

    //...

    /// <summary>
    /// Creates a new <see cref="WriteableBitmap"/> of the specified width and height
    /// </summary>
    /// <remarks>For WPF, the default DPI is 96x96 and PixelFormat is Pbgra32</remarks>
    public static WriteableBitmap New(int pixelWidth, int pixelHeight, Color color = default) => New(new System.Drawing.Size(pixelWidth, pixelHeight), color);

    /// <summary>
    /// Creates a new <see cref="WriteableBitmap"/> of the specified width and height
    /// </summary>
    /// <remarks>For WPF, the default DPI is 96x96 and PixelFormat is Pbgra32</remarks>
    public static WriteableBitmap New(System.Drawing.Size size, Color color = default)
    {
        var result = new WriteableBitmap(size.Width < 1 ? 1 : size.Width, size.Height < 1 ? 1 : size.Height, 96.0, 96.0, PixelFormats.Pbgra32, null);
        if (color != default)
            result.Clear(color);

        return result;
    }

    /// <summary>
    /// Creates a new <see cref="WriteableBitmap"/> from the given <see cref="ColorMatrix"/>.
    /// </summary>
    public static WriteableBitmap New(ColorMatrix input)
    {
        if (input != null)
        {
            var result = New(input.Columns.Int32(), input.Rows.Int32());
            input.Each((y, x, i) =>
            {
                result.SetPixel(x, y, Color.FromArgb(M.Denormalize(i.W), M.Denormalize(i.X), M.Denormalize(i.Y), M.Denormalize(i.Z)));
                return i;
            });
            return result;
        }
        return null;
    }

    //...

    public static System.Drawing.Bitmap NewBitmap(System.Drawing.Size size) => NewBitmap(size.Height, size.Width);

    public static System.Drawing.Bitmap NewBitmap(int height, int width) => new(width, height);

    //...

    /// <summary>
    /// Converts the input BitmapSource to the Pbgra32 format WriteableBitmap which is internally used by the WriteableBitmapEx.
    /// </summary>
    /// <param name="source">The source bitmap.</param>
    /// <returns></returns>
    public static WriteableBitmap ConvertToPbgra32Format(BitmapSource source)
    {
        // Convert to Pbgra32 if it's a different format
        if (source.Format == PixelFormats.Pbgra32)
            return new WriteableBitmap(source);

        var formatedBitmapSource = new FormatConvertedBitmap();
        formatedBitmapSource.BeginInit();
        formatedBitmapSource.Source = source;
        formatedBitmapSource.DestinationFormat = PixelFormats.Pbgra32;
        formatedBitmapSource.EndInit();
        return new WriteableBitmap(formatedBitmapSource);
    }

    /// <summary>
    /// Loads an image from the applications resource file and returns a new WriteableBitmap.
    /// </summary>
    /// <param name="relativePath">Only the relative path to the resource file. The assembly name is retrieved automatically.</param>
    /// <returns>A new WriteableBitmap containing the pixel data.</returns>
    public static WriteableBitmap FromResource(string relativePath)
    {
        var fullName = Assembly.GetCallingAssembly().FullName;
        var asmName = new AssemblyName(fullName).Name;
        return FromContent(asmName + ";component/" + relativePath);
    }

    /// <summary>
    /// Loads an image from the applications content and returns a new WriteableBitmap.
    /// </summary>
    /// <param name="relativePath">Only the relative path to the content file.</param>
    /// <returns>A new WriteableBitmap containing the pixel data.</returns>
    public static WriteableBitmap FromContent(string relativePath)
    {
        using (var bmpStream = Application.GetResourceStream(new Uri(relativePath, UriKind.Relative)).Stream)
        {
            return FromStream(bmpStream);
        }
    }

    /// <summary>
    /// Loads the data from an image stream and returns a new WriteableBitmap.
    /// </summary>
    /// <param name="stream">The stream with the image data.</param>
    /// <returns>A new WriteableBitmap containing the pixel data.</returns>
    public static WriteableBitmap FromStream(Stream stream)
    {
        var bmpi = new BitmapImage();
        bmpi.BeginInit();
        bmpi.CreateOptions = BitmapCreateOptions.None;
        bmpi.StreamSource = stream;
        bmpi.EndInit();
        var bmp = new WriteableBitmap(bmpi);
        bmpi.UriSource = null;
        return bmp;
    }
}