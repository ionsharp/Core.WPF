using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Models;

namespace Imagin.Core.Controls;

public class DockAnchorDocumentCollection : ObservableCollection<Document>, IDockContentSource
{
    public DockRootControl Root { get; private set; }

    ICollectionChanged IDockContentSource.Source => this as ICollectionChanged;

    public DockAnchorDocumentCollection(DockRootControl root) => Root = root;
}