using Imagin.Core.Colors;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class Xrgb
{ 
    public static RGB Convert(this Color input)
        => new(input.R.Double() / 255, input.G.Double() / 255, input.B.Double() / 255);
}