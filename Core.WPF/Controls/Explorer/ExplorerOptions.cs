using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Options")]
[Serializable]
public class ExplorerOptions : ControlOptions<Explorer>
{
    enum Category { Browser, General }

    #region Browser

    BrowserOptions browserOptions = new();
    [Category(Category.Browser), DisplayName("Options")]
    public BrowserOptions BrowserOptions
    {
        get => browserOptions;
        set => this.Change(ref browserOptions, value);
    }

    #endregion

    #region General

    string defaultPath = StoragePath.Root;
    [Category(Category.General)]
    [FolderPath]
    public string DefaultPath
    {
        get => defaultPath;
        set => this.Change(ref defaultPath, value);
    }

    History history = new(Explorer.DefaultLimit);
    [Hidden]
    public History History
    {
        get => history;
        set => this.Change(ref history, value);
    }

    string path = StoragePath.Root;
    [Hidden]
    public string Path
    {
        get => path;
        set
        {
            if (value.NullOrEmpty())
                return;

            this.Change(ref path, value);
        }
    }

    #endregion
}