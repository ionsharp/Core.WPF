namespace Imagin.Core.Linq
{
    public static partial class XPixelFormat
    {
        #region Imagin.Core.Paint

        public static System.Drawing.Imaging.PixelFormat Imaging(this Paint.PixelFormat format)
        {
            return format switch
            {
                /*
                                case Drawing.PixelFormat.Bgr101010:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.BlackWhite:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Cmyk32:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Gray2:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Gray32Float:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Gray4:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Gray8:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Indexed2:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Prgba128Float:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Rgb128Float:
                                    return System.Drawing.Imaging.PixelFormat.;
                                case Drawing.PixelFormat.Rgba128Float:
                                    return System.Drawing.Imaging.PixelFormat.;
                                */
                Paint.PixelFormat.Bgr24 => System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                Paint.PixelFormat.Bgr32 => System.Drawing.Imaging.PixelFormat.Format32bppRgb,
                Paint.PixelFormat.Bgr555 => System.Drawing.Imaging.PixelFormat.Format16bppRgb555,
                Paint.PixelFormat.Bgr565 => System.Drawing.Imaging.PixelFormat.Format16bppRgb565,
                Paint.PixelFormat.Bgra32 => System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                Paint.PixelFormat.Gray16 => System.Drawing.Imaging.PixelFormat.Format16bppGrayScale,
                Paint.PixelFormat.Indexed1 => System.Drawing.Imaging.PixelFormat.Format1bppIndexed,
                Paint.PixelFormat.Indexed4 => System.Drawing.Imaging.PixelFormat.Format4bppIndexed,
                Paint.PixelFormat.Indexed8 => System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                Paint.PixelFormat.Pbgra32 => System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                Paint.PixelFormat.Prgba64 => System.Drawing.Imaging.PixelFormat.Format64bppPArgb,
                Paint.PixelFormat.Rgb24 => System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                Paint.PixelFormat.Rgb48 => System.Drawing.Imaging.PixelFormat.Format48bppRgb,
                Paint.PixelFormat.Rgba64 => System.Drawing.Imaging.PixelFormat.Format64bppArgb,
                _ => default,
            };
        }

        public static System.Windows.Media.PixelFormat Convert(this Paint.PixelFormat format)
        {
            return format switch
            {
                Paint.PixelFormat.Bgr101010 => System.Windows.Media.PixelFormats.Bgr101010,
                Paint.PixelFormat.Bgr24 => System.Windows.Media.PixelFormats.Bgr24,
                Paint.PixelFormat.Bgr32 => System.Windows.Media.PixelFormats.Bgr32,
                Paint.PixelFormat.Bgr555 => System.Windows.Media.PixelFormats.Bgr555,
                Paint.PixelFormat.Bgr565 => System.Windows.Media.PixelFormats.Bgr565,
                Paint.PixelFormat.Bgra32 => System.Windows.Media.PixelFormats.Bgra32,
                Paint.PixelFormat.BlackWhite => System.Windows.Media.PixelFormats.BlackWhite,
                Paint.PixelFormat.Cmyk32 => System.Windows.Media.PixelFormats.Cmyk32,
                Paint.PixelFormat.Gray16 => System.Windows.Media.PixelFormats.Gray16,
                Paint.PixelFormat.Gray2 => System.Windows.Media.PixelFormats.Gray2,
                Paint.PixelFormat.Gray32Float => System.Windows.Media.PixelFormats.Gray32Float,
                Paint.PixelFormat.Gray4 => System.Windows.Media.PixelFormats.Gray4,
                Paint.PixelFormat.Gray8 => System.Windows.Media.PixelFormats.Gray8,
                Paint.PixelFormat.Indexed1 => System.Windows.Media.PixelFormats.Indexed1,
                Paint.PixelFormat.Indexed2 => System.Windows.Media.PixelFormats.Indexed2,
                Paint.PixelFormat.Indexed4 => System.Windows.Media.PixelFormats.Indexed4,
                Paint.PixelFormat.Indexed8 => System.Windows.Media.PixelFormats.Indexed8,
                Paint.PixelFormat.Pbgra32 => System.Windows.Media.PixelFormats.Pbgra32,
                Paint.PixelFormat.Prgba128Float => System.Windows.Media.PixelFormats.Prgba128Float,
                Paint.PixelFormat.Prgba64 => System.Windows.Media.PixelFormats.Prgba64,
                Paint.PixelFormat.Rgb24 => System.Windows.Media.PixelFormats.Rgb24,
                Paint.PixelFormat.Rgb128Float => System.Windows.Media.PixelFormats.Rgb128Float,
                Paint.PixelFormat.Rgb48 => System.Windows.Media.PixelFormats.Rgb48,
                Paint.PixelFormat.Rgba128Float => System.Windows.Media.PixelFormats.Rgba128Float,
                Paint.PixelFormat.Rgba64 => System.Windows.Media.PixelFormats.Rgba64,
                _ => default,
            };
        }

        #endregion

        #region System.Windows.Media

        public static Paint.PixelFormat Convert(this System.Windows.Media.PixelFormat format)
        {
            if (format == System.Windows.Media.PixelFormats.Bgr101010)
                return Paint.PixelFormat.Bgr101010;
            if (format == System.Windows.Media.PixelFormats.Bgr24)
                return Paint.PixelFormat.Bgr24;
            if (format == System.Windows.Media.PixelFormats.Bgr32)
                return Paint.PixelFormat.Bgr32;
            if (format == System.Windows.Media.PixelFormats.Bgr555)
                return Paint.PixelFormat.Bgr555;
            if (format == System.Windows.Media.PixelFormats.Bgr565)
                return Paint.PixelFormat.Bgr565;
            if (format == System.Windows.Media.PixelFormats.Bgra32)
                return Paint.PixelFormat.Bgra32;
            if (format == System.Windows.Media.PixelFormats.BlackWhite)
                return Paint.PixelFormat.BlackWhite;
            if (format == System.Windows.Media.PixelFormats.Cmyk32)
                return Paint.PixelFormat.Cmyk32;
            if (format == System.Windows.Media.PixelFormats.Gray16)
                return Paint.PixelFormat.Gray16;
            if (format == System.Windows.Media.PixelFormats.Gray2)
                return Paint.PixelFormat.Gray2;
            if (format == System.Windows.Media.PixelFormats.Gray32Float)
                return Paint.PixelFormat.Gray32Float;
            if (format == System.Windows.Media.PixelFormats.Gray4)
                return Paint.PixelFormat.Gray4;
            if (format == System.Windows.Media.PixelFormats.Gray8)
                return Paint.PixelFormat.Gray8;
            if (format == System.Windows.Media.PixelFormats.Indexed1)
                return Paint.PixelFormat.Indexed1;
            if (format == System.Windows.Media.PixelFormats.Indexed2)
                return Paint.PixelFormat.Indexed2;
            if (format == System.Windows.Media.PixelFormats.Indexed4)
                return Paint.PixelFormat.Indexed4;
            if (format == System.Windows.Media.PixelFormats.Indexed8)
                return Paint.PixelFormat.Indexed8;
            if (format == System.Windows.Media.PixelFormats.Pbgra32)
                return Paint.PixelFormat.Pbgra32;
            if (format == System.Windows.Media.PixelFormats.Prgba128Float)
                return Paint.PixelFormat.Prgba128Float;
            if (format == System.Windows.Media.PixelFormats.Prgba64)
                return Paint.PixelFormat.Prgba64;
            if (format == System.Windows.Media.PixelFormats.Rgb24)
                return Paint.PixelFormat.Rgb24;
            if (format == System.Windows.Media.PixelFormats.Rgb128Float)
                return Paint.PixelFormat.Rgb128Float;
            if (format == System.Windows.Media.PixelFormats.Rgb48)
                return Paint.PixelFormat.Rgb48;
            if (format == System.Windows.Media.PixelFormats.Rgba128Float)
                return Paint.PixelFormat.Rgba128Float;
            if (format == System.Windows.Media.PixelFormats.Rgba64)
                return Paint.PixelFormat.Rgba64;
            return default;
        }

        #endregion
    }
}