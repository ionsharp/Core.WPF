using Imagin.Core.Collections.Serialization;
using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Local;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Models;

[DisplayName("Options"), Serializable]
public abstract class MainViewOptions : BaseSavable, IApplicationReady
{
    enum Category { General, Font, Log, Save, Theme, Window }

    #region Events

    [field: NonSerialized]
    public event EventHandler<EventArgs<string>> ThemeChanged;

    #endregion

    #region Properties

    #region Font

    string fontFamily = "Calibri";
    [Category(Category.Font), DisplayName("Font")]
    public FontFamily FontFamily
    {
        get
        {
            FontFamily result = null;
            Try.Invoke(() => result = new FontFamily(fontFamily));
            return result;
        }
        set => this.Change(ref fontFamily, value?.Source);
    }

    double fontScale = 1.0;
    [Category(Category.Font), DisplayName("Scale"), Range(0.5, 1.5, 0.01)]
    public double FontScale
    {
        get => fontScale;
        set => this.Change(ref fontScale, value);
    }

    #endregion

    #region Language

    Language language;
    [Category(nameof(Language))]
    [DisplayName("Language")]
    [Localize(false)]
    public Language Language
    {
        get => language;
        set => this.Change(ref language, value);
    }

    #endregion

    #region Log

    bool logEnable = true;
    [Category(Category.Log)]
    [DisplayName("Enable")]
    public bool LogEnable
    {
        get => logEnable;
        set => this.Change(ref logEnable, value);
    }

    bool logClearOnExit = true;
    [Category(Category.Log)]
    [DisplayName("Clear on exit")]
    public bool LogClearOnExit
    {
        get => logClearOnExit;
        set => this.Change(ref logClearOnExit, value);
    }

    #endregion

    #region Save

    bool autoSave = false;
    [DisplayName("Auto save"), Feature, ToggleButton]
    public bool AutoSave
    {
        get => autoSave;
        set => this.Change(ref autoSave, value);
    }

    bool saveWithDialog = true;
    [Category(Category.Save), DisplayName("With dialog")]
    public bool SaveWithDialog
    {
        get => saveWithDialog;
        set => this.Change(ref saveWithDialog, value);
    }

    #endregion

    #region Theme

    [Category(Category.Theme)]
    [DisplayName("Theme")]
    [Index(1)]
    public ApplicationResources Themes => Get.Current<ApplicationResources>();

    string theme = $"{DefaultThemes.Light}";
    [Hidden]
    public string Theme
    {
        get => theme;
        set
        {
            this.Change(ref theme, value);
            Themes.LoadTheme(value);
            OnThemeChanged(value);
        }
    }

    bool autoSaveTheme = true;
    [Category(Category.Theme)]
    [DisplayName("Auto save")]
    [Index(0)]
    public bool AutoSaveTheme
    {
        get => autoSaveTheme;
        set => this.Change(ref autoSaveTheme, value);
    }

    #endregion

    #region Window

    protected virtual WindowState DefaultWindowState 
        => WindowState.Normal;

    bool windowShowInTaskBar = false;
    [Category(Category.Window)]
    [DisplayName("Show in taskbar")]
    public virtual bool WindowShowInTaskBar
    {
        get => windowShowInTaskBar;
        set => this.Change(ref windowShowInTaskBar, value);
    }

    double windowHeight = 900;
    [Hidden]
    public virtual double WindowHeight
    {
        get => windowHeight;
        set => this.Change(ref windowHeight, value);
    }

    double windowWidth = 1300;
    [Hidden]
    public virtual double WindowWidth
    {
        get => windowWidth;
        set => this.Change(ref windowWidth, value);
    }

    string windowState = null;
    [Hidden]
    public virtual WindowState WindowState
    {
        get
        {
            windowState ??= $"{DefaultWindowState}";
            return (WindowState)Enum.Parse(typeof(WindowState), windowState);
        }
        set => this.Change(ref windowState, $"{value}");
    }

    #endregion

    #endregion

    #region MainViewOptions

    public MainViewOptions() : base() => OnLoaded();

    #endregion

    #region Methods

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext input) => OnLoaded();

    protected virtual IEnumerable<IWriter> GetData() => default;

    protected virtual void OnLoaded()
    {
        Get.Register(GetType(), this);
        GetData().If(i => i.Count() > 0, i => i.ForEach(j => j.Load()));
    }

    protected override string FilePath => Get.Where<SingleApplication>().Properties.FilePath;

    protected override void OnSaved()
    {
        GetData().If(i => i.Count() > 0, i => i.ForEach(j => j.Save()));
    }

    protected override void OnSaving()
    {
        if (AutoSaveTheme)
        {
            if (Storage.File.Long.Exists(theme))
                Themes.SaveTheme(Path.GetFileNameWithoutExtension(theme));
        }
    }

    protected virtual void OnThemeChanged(string i) => ThemeChanged?.Invoke(this, new EventArgs<string>(i));

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        switch (propertyName)
        {
            case nameof(Language):
                Language.Set();
                break;
        }

        base.OnPropertyChanged(propertyName);
        if (AutoSave) Save();
    }

    public virtual void OnApplicationReady() { }

    #endregion

    #region Commands

    [field: NonSerialized]
    ICommand deleteThemeCommand;
    [Hidden]
    public virtual ICommand DeleteThemeCommand => deleteThemeCommand ??= new RelayCommand(() => Computer.Recycle(theme), () =>
    {
        var result = false;
        Try.Invoke(() => result = Storage.File.Long.Exists(theme));
        return result;
    });

    [field: NonSerialized]
    ICommand resetCommand;
    [Hidden]
    public virtual ICommand ResetCommand => resetCommand ??= new RelayCommand(() =>
    {

    });

    [field: NonSerialized]
    ICommand saveCommand;
    [DisplayName("Save")]
    [Feature(AboveBelow.Below)]
    public virtual ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
        
    [field: NonSerialized]
    ICommand saveThemeCommand;
    [Hidden]
    public virtual ICommand SaveThemeCommand => saveThemeCommand ??= new RelayCommand(() =>
    {
        var x = new BaseNamable(theme);
        MemberWindow.ShowDialog("Save theme", x, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);

        if (result == 0)
        {
            Themes.SaveTheme(x.Name);

            theme = Themes.ThemePath(x.Name);
            this.Changed(() => Theme);

            Dialog.Show("Save theme", "Theme saved!", DialogImage.Information, Buttons.Ok);
        }
    },
    () => true);

    #endregion
}