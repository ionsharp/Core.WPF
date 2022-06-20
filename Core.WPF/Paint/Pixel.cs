using System.Drawing;

namespace Imagin.Core.Media
{
    public struct Pixel
    {
        public readonly Color Color;

        public readonly int X;

        public readonly int Y;

        public Pixel(Color color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
        }
    }
}