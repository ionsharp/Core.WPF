using Imagin.Core.Numerics;
using System;
using System.Windows;
using System.Windows.Input;

namespace Imagin.Core.Linq;

public static class XVector
{
    public static Point BoundSize(this System.Windows.Vector input, Point? origin, Point offset, Size mSize, Size size, double snap, bool limit = true)
    {
        var x = Math.Round(offset.X + input.X);
        var y = Math.Round(offset.Y + input.Y);

        var delta_x = x; var delta_y = y;

        if (origin != null)
        {
            x = origin.Value.X + x;
            y = origin.Value.Y + y;
        }

        if (limit)
        {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            x = (x + size.Width) > mSize.Width ? mSize.Width - size.Width : x;
            y = (y + size.Height) > mSize.Height ? mSize.Height - size.Height : y;
        }

        x = M.NearestFactor(x, snap);
        y = M.NearestFactor(y, snap);

        if (origin != null)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                y = origin.Value.Y;
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                x = origin.Value.X;
        }

        return new Point(x, y);
    }
}