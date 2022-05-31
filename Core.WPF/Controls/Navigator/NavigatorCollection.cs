using Imagin.Core.Collections.Generic;
using Imagin.Core.Storage;

namespace Imagin.Core.Controls
{
    public class NavigatorCollection : ObservableCollection<NavigatorGroup>
    {
        public static NavigatorCollection Default => new()
        {
            new FavoriteGroup(null),
            new FolderGroup(StoragePath.Root)
        };
    }
}