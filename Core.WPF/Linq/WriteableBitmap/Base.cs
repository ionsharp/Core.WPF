using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Imagin.Core.Media;

namespace Imagin.Core.Linq
{
    /// <summary>
    /// Collection of extension methods for the WriteableBitmap class.
    /// </summary>
    public unsafe static partial class XBitmap
    {
        #region Fields

        internal const int SizeOfArgb = 4;

        #endregion

        #region Methods

        #region General

        public static int ConvertColor(double opacity, Color color)
        {
            if (opacity < 0.0 || opacity > 1.0)
            {
                throw new ArgumentOutOfRangeException("opacity", "Opacity must be between 0.0 and 1.0");
            }

            color.A = (byte)(color.A * opacity);

            return ConvertColor(color);
        }

        public static int ConvertColor(Color color)
        {
            var col = 0;

            if (color.A != 0)
            {
                var a = color.A + 1;
                col = (color.A << 24)
                  | ((byte)((color.R * a) >> 8) << 16)
                  | ((byte)((color.G * a) >> 8) << 8)
                  | ((byte)((color.B * a) >> 8));
            }

            return col;
        }

        /// <summary>
        /// Fills the whole WriteableBitmap with a color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="color">The color used for filling.</param>
        public static void Clear(this WriteableBitmap bmp, Color color)
        {
            var col = ConvertColor(color);
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var len = w * SizeOfArgb;

                // Fill first line
                for (var x = 0; x < w; x++)
                {
                    pixels[x] = col;
                }

                // Copy first line
                var blockHeight = 1;
                var y = 1;
                while (y < h)
                {
                    BitmapContext.BlockCopy(context, 0, context, y * len, blockHeight * len);
                    y += blockHeight;
                    blockHeight = Math.Min(2 * blockHeight, h - y);
                }
            }
        }

        /// <summary>
        /// Fills the whole WriteableBitmap with an empty color (0).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        public static void Clear(this WriteableBitmap bmp)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Clear();
            }
        }

        /// <summary>
        /// Clones the specified WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <returns>A copy of the WriteableBitmap.</returns>
        public static WriteableBitmap Clone(this WriteableBitmap bmp)
        {
            using (var srcContext = bmp.GetBitmapContext(Media.ReadWriteMode.ReadOnly))
            {
                var result = XBitmap.New(srcContext.Width, srcContext.Height);
                using (var destContext = result.GetBitmapContext())
                {
                    BitmapContext.BlockCopy(srcContext, 0, destContext, 0, srcContext.Length * SizeOfArgb);
                }
                return result;
            }
        }

        #endregion

        #region ForEach

        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                int w = context.Width;
                int h = context.Height;
                int index = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var color = func(x, y);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y, source color and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var index = 0;

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var c = pixels[index];

                        // Premultiplied Alpha!
                        var a = (byte)(c >> 24);
                        // Prevent division by zero
                        int ai = a;
                        if (ai == 0)
                        {
                            ai = 1;
                        }
                        // Scale inverse alpha to use cheap integer mul bit shift
                        ai = ((255 << 8) / ai);
                        var srcColor = Color.FromArgb(a,
                                                      (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                                                      (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                                                      (byte)((((c & 0xFF) * ai) >> 8)));

                        var color = func(x, y, srcColor);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }

        #endregion

        #region Get Pixel / Brightness

        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as integer.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y.</returns>
        public static int GetPixeli(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext(Media.ReadWriteMode.ReadOnly))
            {
                return context.Pixels[y * context.Width + x];
            }
        }

        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as a Color struct.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y as a Color struct.</returns>
        public static Color GetPixel(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext(Media.ReadWriteMode.ReadOnly))
            {
                var c = context.Pixels[y * context.Width + x];
                var a = (byte)(c >> 24);

                // Prevent division by zero
                int ai = a;
                if (ai == 0)
                {
                    ai = 1;
                }

                // Scale inverse alpha to use cheap integer mul bit shift
                ai = ((255 << 8) / ai);
                return Color.FromArgb(a,
                                     (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                                     (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                                     (byte)((((c & 0xFF) * ai) >> 8)));
            }
        }

        /// <summary>
        /// Gets the brightness / luminance of the pixel at the x, y coordinate as byte.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The brightness of the pixel at x, y.</returns>
        public static byte GetBrightness(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext(Media.ReadWriteMode.ReadOnly))
            {
                // Extract color components
                var c = context.Pixels[y * context.Width + x];
                var r = (byte)(c >> 16);
                var g = (byte)(c >> 8);
                var b = (byte)(c);

                // Convert to gray with constant factors 0.2126, 0.7152, 0.0722
                return (byte)((r * 6966 + g * 23436 + b * 2366) >> 15);
            }
        }

        #endregion

        #region SetPixel

        #region Without alpha

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }

        #endregion

        #region With alpha

        /// <summary>
        /// Sets the color of the pixel including the alpha value and using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        /// <summary>
        /// Sets the color of the pixel including the alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        #endregion

        #region With System.Windows.Media.Color

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = ConvertColor(color);
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = ConvertColor(color);
            }
        }

        /// <summary>
        /// Sets the color of the pixel using an extra alpha value and a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[index] = (a << 24)
                           | ((byte)((color.R * ai) >> 8) << 16)
                           | ((byte)((color.G * ai) >> 8) << 8)
                           | ((byte)((color.B * ai) >> 8));
            }
        }

        /// <summary>
        /// Sets the color of the pixel using an extra alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[y * context.Width + x] = (a << 24)
                                             | ((byte)((color.R * ai) >> 8) << 16)
                                             | ((byte)((color.G * ai) >> 8) << 8)
                                             | ((byte)((color.B * ai) >> 8));
            }
        }

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster).  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = color;
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = color;
            }
        }

        #endregion

        #endregion

        #region Other

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

        #endregion

        #endregion
    }
}