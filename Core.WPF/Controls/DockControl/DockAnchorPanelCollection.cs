using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Models;

namespace Imagin.Core.Controls
{
    public class DockAnchorPanelCollection : ObservableCollection<Panel>, IDockContentSource, IDockPanelSource
    {
        public DockRootControl Root { get; private set; }

        ICollectionChanged IDockContentSource.Source => this;

        ICollectionChanged IDockPanelSource.Source => this;

        public DockAnchorPanelCollection(DockRootControl root) => Root = root;
    }
}