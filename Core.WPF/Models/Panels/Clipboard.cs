using Imagin.Core.Input;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Clipboard"), Explicit, Image(SmallImages.Paste), Serializable]
public class ClipboardPanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    public ClipboardPanel() : base() { }
    
    ICommand clearCommand;
    [Header, Image(SmallImages.X), Name("Clear"), Show]
    public ICommand ClearCommand
        => clearCommand ??= new RelayCommand(Copy.Data.Clear, () => Copy.Data.Count > 0);

    ICommand deleteCommand;
    public ICommand DeleteCommand
        => deleteCommand ??= new RelayCommand<Copy.DataModel>(i => Copy.Data.Remove(i), i => i != null);
}