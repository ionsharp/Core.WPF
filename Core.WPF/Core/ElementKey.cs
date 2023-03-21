using System.Windows;

namespace Imagin.Core;

public abstract class ElementKey : IElementKey { }

public class ReferenceKey<T> : ElementKey where T : FrameworkElement
{
    public ReferenceKey() : base() { }
}

public class ResourceKey : ElementKey { }