using Imagin.Core.Colors;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class Xrgb
{ 
    public static RGB Convert(this Color input)
        => new(input.R.Double(), input.G.Double(), input.B.Double());
}