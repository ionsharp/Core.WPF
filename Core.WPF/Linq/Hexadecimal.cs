using Imagin.Core.Numerics;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class XHexadecimal
{
    public static Color Color(this Hexadecimal input)
    {
        var result = (Vector4)input;
        return System.Windows.Media.Color.FromArgb(M.Denormalize(result.W), M.Denormalize(result.X), M.Denormalize(result.Y), M.Denormalize(result.Z));
    }

    public static SolidColorBrush SolidColorBrush(this Hexadecimal input) => new BrushConverter().ConvertFrom($"#{input}") as SolidColorBrush;
}