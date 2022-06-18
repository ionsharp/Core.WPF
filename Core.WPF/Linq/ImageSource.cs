using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Linq
{
    public static class XImageSource
    {
        public static Bitmap Bitmap(this ImageSource input, ImageExtensions extension = ImageExtensions.Jpg) => input.As<BitmapSource>().Bitmap(extension);
    }
}