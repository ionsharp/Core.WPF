using Imagin.Core.Colors;

namespace Imagin.Core.Linq;

public static class Xrgb
{ 
    public static RGB Convert(this System.Windows.Media.Color input)
        => Colors.Colour.New<RGB>(input.R.Double(), input.G.Double(), input.B.Double());
}