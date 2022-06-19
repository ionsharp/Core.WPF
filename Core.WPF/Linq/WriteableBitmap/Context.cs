using System.Windows.Media.Imaging;
using Imagin.Core.Paint;

namespace Imagin.Core.Linq
{
    /// <summary>
    /// Provides the WriteableBitmap context pixel data
    /// </summary>
    public static partial class XWriteableBitmapContext
    {
        /// <summary>
        /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
        /// </summary>
        /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Paint.BitmapContext GetBitmapContext(this WriteableBitmap bmp)
        {
            return new Paint.BitmapContext(bmp);
        }

        /// <summary>
        /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
        /// </summary>
        /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="mode">The ReadWriteMode. If set to ReadOnly, the bitmap will not be invalidated on dispose of the context, else it will</param>
        /// <returns></returns>
        public static Paint.BitmapContext GetBitmapContext(this WriteableBitmap bmp, Paint.ReadWriteMode mode)
        {
            return new Paint.BitmapContext(bmp, mode);
        }
    }
}