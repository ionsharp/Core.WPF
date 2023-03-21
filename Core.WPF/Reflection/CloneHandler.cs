using Imagin.Core.Linq;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Reflection;

public class CloneHandler : ICloneHandler
{
    public CloneHandler() : base() { }

    public object Clone(object input)
    {
        if (input is Bitmap a)
            return XBitmap.Clone(a.WriteableBitmap());

        if (input is System.Windows.Media.Color b)
            return b;

        if (input is LinearGradientBrush c)
            return new LinearGradientBrush(c.GradientStops);

        if (input is RadialGradientBrush d)
            return new RadialGradientBrush(d.GradientStops);

        if (input is SolidColorBrush e)
            return new SolidColorBrush(e.Color);

        if (input is System.Windows.Media.Brush f)
            return null;

        if (input is WriteableBitmap g)
            return XBitmap.Clone(g);

        return null;
    }
}