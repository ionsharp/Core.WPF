using Imagin.Core.Storage;
using System;

namespace Imagin.Core.Linq;

public static class XStorageWindowMode
{
    public static ItemType Convert(this StorageDialogMode input)
    {
        return input switch
        {
            StorageDialogMode.Open => ItemType.File | ItemType.Folder,
            StorageDialogMode.OpenFile or StorageDialogMode.SaveFile => ItemType.File,
            StorageDialogMode.OpenFolder => ItemType.Folder,
            _ => throw new InvalidOperationException(),
        };
    }
}