using Imagin.Core.Collections;
using Imagin.Core.Models;

namespace Imagin.Core.Controls
{
    public class DockAnchorDocumentCollection : DocumentCollection, IDockContentSource
    {
        public DockRootControl Root { get; private set; }

        ICollectionChanged IDockContentSource.Source => this as ICollectionChanged;

        public DockAnchorDocumentCollection(DockRootControl root) => Root = root;
    }
}