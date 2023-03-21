using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Find"), Explicit, Image(SmallImages.Search), Serializable]
public class FindPanel : Panel, IElementReference
{
    public static readonly ReferenceKey<FindControl> ControlKey = new();

    public static readonly ResourceKey TemplateKey = new();

    FindControl Control;

    public string FindText { get => Get(""); set => Set(value); }

    public override Uri Icon => Resource.GetImageUri(SmallImages.Search);

    public bool MatchCase { get => Get(false); set => Set(value); }

    public bool MatchWord { get => Get(false); set => Set(value); }

    public FindSource Source { get => Get(FindSource.CurrentDocument); set => Set(value); }

    public string ReplaceText { get => Get(""); set => Set(value); }

    public override string Title => "Find";

    public FindPanel() : base() { }

    void IElementReference.SetReference(IElementKey key, System.Windows.FrameworkElement element)
    {
        if (key == ControlKey)
            Control = element as FindControl;
    }

    ///

    ICommand findAllCommand;
    public ICommand FindAllCommand 
        => findAllCommand ??= new RelayCommand(() => Control.FindAllCommand.Execute(), () => Control?.FindAllCommand.CanExecute(null) == true);

    ICommand findNextCommand;
    public ICommand FindNextCommand
        => findNextCommand ??= new RelayCommand(() => Control.FindNextCommand.Execute(), () => Control?.FindNextCommand.CanExecute(null) == true);

    ICommand findPreviousCommand;
    public ICommand FindPreviousCommand 
        => findPreviousCommand ??= new RelayCommand(() => Control.FindNextCommand.Execute(), () => Control?.FindNextCommand.CanExecute(null) == true);

    ///

    ICommand replaceAllCommand;
    public ICommand ReplaceAllCommand 
        => replaceAllCommand ??= new RelayCommand(() => Control.ReplaceAllCommand.Execute(), () => Control?.ReplaceAllCommand.CanExecute(null) == true);

    ICommand replaceNextCommand;
    public ICommand ReplaceNextCommand 
        => replaceNextCommand ??= new RelayCommand(() => Control.ReplaceNextCommand.Execute(), () => Control?.ReplaceNextCommand.CanExecute(null) == true);

    ICommand replacePreviousCommand;
    public ICommand ReplacePreviousCommand 
        => replacePreviousCommand ??= new RelayCommand(() => Control.ReplacePreviousCommand.Execute(), () => Control?.ReplacePreviousCommand.CanExecute(null) == true);

    ///

    ICommand resultsCommand;
    public ICommand ResultsCommand => resultsCommand ??= new RelayCommand<FindResultCollection>(i =>
    {
        if (ViewModel.Panels.FirstOrDefault<FindResultsPanel>() is FindResultsPanel oldPanel)
        {
            if (!oldPanel.KeepResults)
            {
                oldPanel.Results = i;
                return;
            }
        }
        ViewModel.Panels.Add(new FindResultsPanel(i));
    }, 
    i => i != null);
}