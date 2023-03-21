using Imagin.Core.Data;
using System.IO;

namespace Imagin.Core.Storage;

/// <summary>Represents a <see cref="Folder"/> or <see cref="Drive"/>.</summary>
public abstract class Container : Item
{
    [Hide]
    public ItemCollection Items { get => Get(new ItemCollection()); private set => Set(value); }

    [Name("Path")]
    [Pin(Pin.AboveOrLeft)]
    [ReadOnly]
    [StringStyle(StringStyle.FolderPath)]
    public override string Path
    {
        get => base.Path;
        set
        {
            base.Path = value;
            Items.Path = value;
        }
    }

    public override FileSystemInfo Read()
    {
        FileSystemInfo result = null;
        Try.Invoke(() => result = new DirectoryInfo(Path));
        return result;
    }

    protected Container(ItemType type, Origin origin, string path) : base(type, origin, path) { }
}