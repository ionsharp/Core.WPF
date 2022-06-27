using Imagin.Core.Storage;
using System;

namespace Imagin.Core.Linq
{
    public static class XStorageWindowMode
    {
        public static ItemType Convert(this StorageWindowModes input)
        {
            return input switch
            {
                StorageWindowModes.Open => ItemType.File | ItemType.Folder,
                StorageWindowModes.OpenFile or StorageWindowModes.SaveFile => ItemType.File,
                StorageWindowModes.OpenFolder => ItemType.Folder,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}