using System;

namespace Imagin.Core.Storage;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string itemPath) : base($"The file or folder '{itemPath}' does not exist.") { }
}

public class ItemsNotFoundException : Exception
{
    public ItemsNotFoundException(string folderPath) : base($"The folder '{folderPath}' does not contain anything.") { }
}