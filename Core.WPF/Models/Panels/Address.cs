using Imagin.Core.Input;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Explicit, Image(SmallImages.Shutdown), Name(DefaultTitle), Serializable]
public class AddressPanel : Panel
{
    public static ResourceKey TemplateKey = new();

    public const string DefaultTitle = "Address";

    public override bool CanShare => false;

    public override bool TitleVisibility => false;

    public string Path { get => Get(""); set => Set(value); }

    public AddressPanel() : base() { }

    public void Refresh() { }

    ICommand refreshCommand;
    public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(Refresh);
}