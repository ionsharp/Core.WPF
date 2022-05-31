using Imagin.Core.Collections;

namespace Imagin.Core.Controls
{
    public interface IDockContentSource
    {
        DockRootControl Root { get; }

        ICollectionChanged Source { get; }
    }
}