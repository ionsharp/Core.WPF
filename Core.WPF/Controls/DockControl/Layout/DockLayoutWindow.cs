using Imagin.Core.Numerics;
using System;
using System.Windows;

namespace Imagin.Core.Controls;

[Serializable]
public class DockLayoutWindow : DockLayoutRoot
{
    public Point2 Position { get => Get<Point2>(); set => Set(value); }

    public DoubleSize Size { get => Get<DoubleSize>(); set => Set(value); }

    public virtual WindowState State { get => GetFromString(WindowState.Normal); set => SetFromString(value); }

    public DockLayoutWindow() : base() { }
}