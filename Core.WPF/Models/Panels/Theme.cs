using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Theme"), Image(SmallImages.Palette), Serializable]
public class ThemePanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Palette);

    [Pin(Pin.AboveOrLeft), Header, HideName, Index(-1), Name("Theme")]
    public ApplicationTheme Themes => Current.Get<ApplicationTheme>();

    public ThemePanel() : base() { }

    ICommand deleteCommand;
    [Header, Image(SmallImages.Trash), Name("Delete"), Reserve]
    public ICommand DeleteCommand => deleteCommand
        ??= new RelayCommand(() => Themes.CustomThemes.DeleteCommand.Execute(Current.Get<MainViewOptions>().Theme), () => Themes.CustomThemes.DeleteCommand.CanExecute(Current.Get<MainViewOptions>().Theme));

    ICommand renameCommand;
    [Header, Image(SmallImages.Rename), Name("Rename"), Reserve]
    public ICommand RenameCommand => renameCommand
        ??= new RelayCommand(() => Themes.CustomThemes.RenameCommand.Execute(Current.Get<MainViewOptions>().Theme), () => Themes.CustomThemes.RenameCommand.CanExecute(Current.Get<MainViewOptions>().Theme));

    ICommand saveCommand;
    [Header, Image(SmallImages.Save), Name("Save"), Reserve]
    public ICommand SaveCommand => saveCommand ??= new RelayCommand(() =>
    {
        var x = new Namable(Namable.DefaultName);
        Dialog.ShowObject($"Save", x, Resource.GetImageUri(SmallImages.Rename), j =>
        {
            if (j == 0)
                Try.Invoke(() => Themes.SaveTheme(x.Name), e => Analytics.Log.Write<ApplicationTheme>(e));
        },
        Buttons.SaveCancel);
    },
    () => Themes.CurrentTheme != null);
}