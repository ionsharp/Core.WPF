using System;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class XUri
{
    public static ImageSource GetImage(this Uri input)
    {
        try
        {
            return (ImageSource)new ImageSourceConverter().ConvertFromString(input.OriginalString);
        }
        catch
        {
            return null;
        }
    }
}