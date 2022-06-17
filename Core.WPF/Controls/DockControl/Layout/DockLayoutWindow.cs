using Imagin.Core.Numerics;
using System;
using System.Windows;

namespace Imagin.Core.Controls
{
    [Serializable]
    public class DockLayoutWindow : DockLayoutRoot
    {
        Point2 position;
        public Point2 Position
        {
            get => position;
            set => this.Change(ref position, value);
        }

        DoubleSize size;
        public DoubleSize Size
        {
            get => size;
            set => this.Change(ref size, value);
        }

        string state = $"{WindowState.Normal}";
        public virtual WindowState State
        {
            get => (WindowState)Enum.Parse(typeof(WindowState), state);
            set => this.Change(ref state, $"{value}");
        }

        public DockLayoutWindow() : base() { }
    }
}