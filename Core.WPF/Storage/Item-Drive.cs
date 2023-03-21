using Imagin.Core.Conversion;
using System;
using System.IO;

namespace Imagin.Core.Storage;

[Name("Drive"), Serializable]
public sealed class Drive : Container
{
    [Name("Available free space"), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long AvailableFreeSpace { get => Get(0L); set => Set(value); }

    [Hide]
    public string Format { get => Get(""); set => Set(value); }

    [Category("Attributes"), Name("Hidden"), ReadOnly]
    public override bool IsHidden
    {
        get => base.IsHidden;
        set => base.IsHidden = value;
    }

    [Category("Attributes"), Name("ReadOnly"), ReadOnly]
    public override bool IsReadOnly
    {
        get => base.IsReadOnly;
        set => base.IsReadOnly = value;
    }

    [Name("Total size"), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long TotalSize { get => Get(0L); set => Set(value); }

    public Drive(DriveInfo driveInfo) : base(ItemType.Drive, Origin.Local, driveInfo.Name)
    {
        AvailableFreeSpace
            = driveInfo.AvailableFreeSpace;
        Format
            = driveInfo.DriveFormat;
        TotalSize
            = driveInfo.TotalSize;
    }

    public Drive(string path) : base(ItemType.Drive, Origin.Local, path) { }

    public override FileSystemInfo Read() => null;
}