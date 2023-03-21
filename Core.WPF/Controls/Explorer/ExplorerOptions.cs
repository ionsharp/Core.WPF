using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Imagin.Core.Controls;

[Categorize, Description("Options for exploring file system."), Name("Explorer options"), Serializable]
public class ExplorerOptions : ControlOptions<Explorer>
{
    enum Category { File, Folder, General }

    readonly Dictionary<string, FolderOptions> FolderOptions = new();

    ///

    [Categorize, Category(Category.Folder), HideName, Description("Options for current folder."), Name("Options"), Show]
    public FolderOptions CurrentFolderOptions => GetOptions(Path);

    [Assign(nameof(DefaultPaths)), Category(Category.General), Description("The default folder to open."), Pin(Pin.AboveOrLeft), StringStyle(StringStyle.FolderPath), Horizontal]
    public string DefaultPath { get => Get(StoragePath.Root); set => Set(value); }

    [Hide]
    public object DefaultPaths => new StringCollection(new List<Environment.SpecialFolder>()
    {
        Environment.SpecialFolder.Desktop, Environment.SpecialFolder.MyDocuments, Environment.SpecialFolder.Favorites, Environment.SpecialFolder.MyMusic, Environment.SpecialFolder.MyPictures, Environment.SpecialFolder.MyVideos,
    }
    .Select(i => i.GetPath()));

    [Category(Category.File), Description("Only show files with these attributes."), Name("FileAttributes")]
    [Style(EnumStyle.Flags)]
    public Attributes FileAttributes { get => Get(Attributes.All); set => Set(value); }

    [Category(Category.File), Description("Only show files with these extensions (separated by semicolon). Show all files if empty."), Name("FileExtensions"), StringStyle(StringStyle.Tokens), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus)]
    public string FileExtensions { get => Get(string.Empty); set => Set(value); }

    [Category(Category.Folder), Description("Only show folders with these attributes."), Name("FolderAttributes")]
    [Style(EnumStyle.Flags)]
    public Attributes FolderAttributes { get => Get(Attributes.All); set => Set(value); }

    [Hide]
    public StringHistory History { get => Get(new StringHistory(Explorer.DefaultLimit)); set => Set(value); }

    [Hide]
    public string Path
    {
        get => Get(StoragePath.Root);
        set
        {
            if (value.NullOrEmpty())
                return;

            Set(value);
            Update(() => CurrentFolderOptions);
        }
    }

    ///

    public FolderOptions GetOptions(string path, FolderOptions defaultValue = null)
    {
        if (path.NullOrEmpty()) return null;

        if (FolderOptions.ContainsKey(path))
            return FolderOptions[path];

        var result = defaultValue ?? new();
        FolderOptions.Add(path, result);
        return result;
    }
}