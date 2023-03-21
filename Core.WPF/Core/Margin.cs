using System;
using System.Windows;

namespace Imagin.Core;

/// <summary>Serializable version of <see cref="Thickness"/>.</summary>
[Serializable]
public struct Margin
{
    public readonly double Left; public readonly double Top; public readonly double Right; public readonly double Bottom;

    public Thickness Value => new(Left, Top, Right, Bottom);

    public Margin(double input) => Left = Top = Right = Bottom = input;

    public Margin(double left, double top, double right, double bottom) { Left = left; Top = top; Right = right; Bottom = bottom; }

    public Margin(Thickness input) : this(input.Left, input.Top, input.Right, input.Bottom) { }
}