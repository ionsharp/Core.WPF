using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Explore"), Explicit, Image(SmallImages.Folder), Serializable]
public class ExplorePanel : Panel
{
    enum Category { Explorer }

    public static readonly ResourceKey TemplateKey = new();

    [field: NonSerialized]
    public event EventHandler<EventArgs<string>> OpenedFile;

    [Category(Category.Explorer), HideName, Name("Options"), Option, Show]
    public ExplorerOptions Options { get => Get<ExplorerOptions>(); set => Set(value); }

    public ExplorePanel() : base() => Options = new();

    public ExplorePanel(IEnumerable<string> fileExtensions) : this() => Options.FileExtensions = fileExtensions.ToString(";");
    
    void OnOpenedFile(string i) => OpenedFile?.Invoke(this, new EventArgs<string>(i));

    ICommand openedFileCommand;
    public ICommand OpenedFileCommand => openedFileCommand ??= new RelayCommand<string>(i => OnOpenedFile(i), i => i is string);
}