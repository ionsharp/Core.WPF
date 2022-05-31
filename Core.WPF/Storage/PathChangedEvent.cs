using Imagin.Core.Linq;
using System.Windows;

namespace Imagin.Core.Storage
{
    public delegate void PathChangedEventHandler(object sender, PathChangedEventArgs e);

    public class PathChangedEventArgs : RoutedEventArgs
    {
        public readonly string Path;

        public PathChangedEventArgs(RoutedEvent input, string path) : base(input) => Path = path;
    }
}