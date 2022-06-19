using System.Windows;

namespace Imagin.Core
{
    public abstract class FrameworkKey : IFrameworkKey { }

    public class ReferenceKey<T> : FrameworkKey where T : FrameworkElement
    {
        public ReferenceKey() : base() { }
    }

    public class ResourceKey : FrameworkKey { }

    public class ResourceKey<T> : ResourceKey { }
}