using Imagin.Core.Numerics;
using System.Windows.Media;

namespace Imagin.Core.Linq
{
    public static partial class XVector4
    {
        public static Vector4 Convert(Color input)
            => new(M.Normalize(input.A), M.Normalize(input.R), M.Normalize(input.G), M.Normalize(input.B));
    }
}
