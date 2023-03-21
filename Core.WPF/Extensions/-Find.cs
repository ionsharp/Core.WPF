using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Effects;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Config;

public interface IMenuSource
{
    ObjectCollection MenuItems { get; }
}

[Description("Find and replace text in documents.")]
[Image(SmallImages.Search), Name("Find"), Serializable]
public class FindExtension : PanelExtension
{
    enum Menu { Find }

    public override string Author => nameof(Imagin);

    public override string Description => this.GetDescription();

    public override string Icon => this.GetAttribute<ImageAttribute>().SmallImage;

    public override string Name => this.GetName();

    public override string Uri => null;

    public override Version Version => new Version(1, 0, 0, 0);

    [NonSerialized]
    FindPanel panel;
    [Hide]
    public override Models.Panel Panel => panel;

    public override IExtensionResources Resources => null;

    public FindExtension() : base()
    {
        panel = new();
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        panel = new();
    }

    ///

    ICommand findCommand;
    [Hide, MenuItem(Menu.Find, Header = "Find", Icon = SmallImages.Find)]
    public ICommand FindCommand
        => findCommand ??= new RelayCommand(() => { } /*DockControl.Find(ActiveDocument)*/, () => true /*ActiveDocument is IFind && DockControl != null*/);

    ///

    [Hide, MenuItem(Menu.Find, Header = "FindAll", Icon = SmallImages.FindAll)]
    public ICommand FindAllCommand => panel.FindAllCommand;

    [Hide, MenuItem(Menu.Find, Header = "FindNext", Icon = SmallImages.FindNext)]
    public ICommand FindNextCommand => panel.FindNextCommand;

    [Hide, MenuItem(Menu.Find, Header = "FindPrevious", Icon = SmallImages.FindPrevious)]
    public ICommand FindPreviousCommand => panel.FindPreviousCommand;

    ///

    [Hide, MenuItem(Menu.Find, Header = "ReplaceAll", Icon = SmallImages.ReplaceAll)]
    public ICommand ReplaceAllCommand => panel.ReplaceAllCommand;

    [Hide, MenuItem(Menu.Find, Header = "ReplaceNext", Icon = SmallImages.ReplaceNext)]
    public ICommand ReplaceNextCommand => panel.ReplaceNextCommand;

    [Hide, MenuItem(Menu.Find, Header = "ReplacePrevious", Icon = SmallImages.ReplacePrevious)]
    public ICommand ReplacePreviousCommand => panel.ReplacePreviousCommand;
}