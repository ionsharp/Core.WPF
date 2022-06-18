using Imagin.Core.Numerics;
using System.Windows.Media;

namespace Imagin.Core.Linq
{
    public static partial class XVector3
    {
        public static Vector3 Convert(Color input)
            => new(M.Normalize(input.R), M.Normalize(input.G), M.Normalize(input.B));
    }
}
