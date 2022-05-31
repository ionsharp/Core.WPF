using Imagin.Core.Collections;

namespace Imagin.Core.Controls
{
    public interface IDockPanelSource
    {
        DockRootControl Root { get; }

        ICollectionChanged Source { get; }
    }
}