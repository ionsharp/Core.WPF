using System.Windows;

namespace Imagin.Core.Controls
{
    public sealed class DockWindow : Window
    {
        public DockRootControl Root => Content as DockRootControl;

        public DockWindow() : base() { }
    }
}