using Imagin.Core.Models;
using System.Windows;

namespace Imagin.Core.Controls
{
    public class DockDragEvent : Base
    {
        public object ActualContent => Content == null ? Root.DockControl.Convert(Window.Root.Child) : Content;

        public readonly Content[] Content;

        public readonly DockRootControl Root;

        public readonly DockContentControl Source;

        public readonly DockWindow Window;

        public IDockControl MouseOver { get => Get<IDockControl>(); internal set => Set(value); }

        public Point MousePosition { get; internal set; }

        internal DockDragEvent(DockContentControl source, DockRootControl root, Content[] content, DockWindow window)
        {
            Source = source;
            Root = root;
            Content = content;
            Window = window;
        }
    }
}