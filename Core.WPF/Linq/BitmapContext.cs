using Imagin.Core.Media;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Linq;

/// <summary>
/// Provides the WriteableBitmap context pixel data
/// </summary>
public static partial class XBitmapContext
{
    /// <summary>
    /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
    /// </summary>
    /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static BitmapContext GetBitmapContext(this WriteableBitmap bmp)
        => new BitmapContext(bmp);

    /// <summary>
    /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
    /// </summary>
    /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
    /// <param name="bmp">The bitmap.</param>
    /// <param name="mode">The ReadWriteMode. If set to ReadOnly, the bitmap will not be invalidated on dispose of the context, else it will</param>
    /// <returns></returns>
    public static BitmapContext GetBitmapContext(this WriteableBitmap bmp, Media.ReadWriteMode mode)
        => new BitmapContext(bmp, mode);
}