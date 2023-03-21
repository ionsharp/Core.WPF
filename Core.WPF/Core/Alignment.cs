using System;
using System.Windows;

namespace Imagin.Core;

[Serializable]
public class Alignment : Base
{
    public static Alignment Center => new(HorizontalAlignment.Center, VerticalAlignment.Center);

    [Horizontal]
    public HorizontalAlignment Horizontal { get => Get(HorizontalAlignment.Left); set => Set(value); }

    [Horizontal]
    public VerticalAlignment Vertical { get => Get(VerticalAlignment.Top); set => Set(value); }

    public Alignment() : base() { }

    public Alignment(HorizontalAlignment horizontal, VerticalAlignment vertical) : this()
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }
}