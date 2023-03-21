using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Local;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Models;

[File(DefaultName, DefaultExtension), Image(SmallImages.Options), Name(DefaultName), Serializable, View(Reflection.MemberView.Tab, typeof(Tab)), ViewSource(ShowHeader = false)]
public abstract class MainViewOptions : DataSavable
{
    enum Category { Button, Footer, General, Header, Password, Log, Window }

    enum Tab 
    { 
        [Categorize(false)]
        Dialog, 
        [Categorize(false)]
        Font, 
        General, 
        Log,
        [Categorize(false)]
        Menu, 
        Notification, 
        Theme, 
        Warnings, 
        Window 
    }

    ///

    [field: NonSerialized]
    public event EventHandler<EventArgs> LanguageChanged;

    ///

    public const string DefaultExtension = "data";

    public const string DefaultName = "Options";

    #region Properties

    protected virtual WindowState DefaultWindowState => WindowState.Normal;

    ///

    protected sealed override string FileExtension => GetType().GetAttribute<FileAttribute>().Extension;

    protected sealed override string FileName => GetType().GetAttribute<FileAttribute>().Name;

    protected override string FolderPath => Current.Get<BaseApplication>().DataFolderPath;

    #region Dialog

    [Tab(Tab.Dialog)]
    [ColorStyle(ColorStyle.String, true), Name("Background")]
    public ByteVector4 DialogBackground { get => Get(new ByteVector4("AAFFFFFF")); set => Set(value); }
    
    [Tab(Tab.Dialog)]
    [Name("Style")]
    public Controls.ButtonStyle DialogButtonStyle { get => Get(Controls.ButtonStyle.Apple); set => Set(value); }

    [Tab(Tab.Dialog), Name("Maximum height"), Range(1.0, double.MaxValue, 1.0, Style = RangeStyle.UpDown)]
    public double DialogMaximumHeight { get => Get(720.0); set => Set(value); }

    [Tab(Tab.Dialog), Name("Maximum width"), Range(1.0, double.MaxValue, 1.0, Style = RangeStyle.UpDown)]
    public double DialogMaximumWidth { get => Get(540.0); set => Set(value); } //900

    [Tab(Tab.Dialog), Name("Minimum width"), Range(1.0, double.MaxValue, 1.0, Style = RangeStyle.UpDown)]
    public double DialogMinimumWidth { get => Get(540.0); set => Set(value); } //360

    #endregion

    #region Font

    [Tab(Tab.Font), Name("Family")]
    public FontFamily FontFamily { get => GetFrom(new FontFamily("Calibri"), Conversion.Converter.Get<FontFamilyToStringConverter>()); set => SetFrom(value, Conversion.Converter.Get<FontFamilyToStringConverter>()); }

    [Tab(Tab.Font), Name("Scale"), Range(0.5, 1.5, 0.01)]
    public double FontScale { get => Get(1.0); set => Set(value); }

    #endregion

    #region General

    [Name("Auto save"), Pin(Pin.AboveOrLeft), Tab(Tab.General), Style(BooleanStyle.ToggleButton)]
    public bool AutoSave { get => Get(false); set => Set(value); }

    [Name("Show in taskbar"), Tab(Tab.General)]
    public virtual bool TaskbarIconVisibility { get => Get(true); set => Set(value); }

    #region Password

    [Hide]
    public bool PasswordErrorNeverShow { get => Get(false); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Enable")]
    public bool PasswordEnable { get => Get(false); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Password"), StringStyle(StringStyle.Password)]
    [VisibilityTrigger(nameof(PasswordType), Operators.Equal, PasswordType.Default)]
    public string PasswordDefault { get => Get(""); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Pattern")]
    [VisibilityTrigger(nameof(PasswordType), Operators.Equal, PasswordType.Pattern)]
    public Int32LineCollection PasswordPattern { get => Get<Int32LineCollection>(new()); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Pin"), Range(0, int.MaxValue, 1, Style = RangeStyle.Default)]
    [VisibilityTrigger(nameof(PasswordType), Operators.Equal, PasswordType.Pin)]
    public int PasswordPin { get => Get(0); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Type")]
    public PasswordType PasswordType { get => Get(PasswordType.Default); set => Set(value); }

    [Category(Category.Password), Tab(Tab.General)]
    [Name("Ask for after (seconds)"), Range(0.0, double.MaxValue, 1.0, Style = RangeStyle.UpDown)]
    public double PasswordAskAfter { get => Get(60.0 * 60.0); set => Set(value); }

    #endregion

    #endregion

    #region Language

    [Name("Language"), Tab(nameof(Language))]
    public Language Language { get => Get(Language.English); set => Set(value); }

    #endregion

    #region Log

    [Tab(Tab.Log)]
    [Name("Enable")]
    public bool LogEnable { get => Get(true); set => Set(value); }

    [Tab(Tab.Log)]
    [Name("Clear on app close")]
    public bool LogClearOnAppClose { get => Get(true); set => Set(value); }

    #endregion

    #region Menu

    [Tab(Tab.Menu)]
    [Name("Show top level icons")]
    public bool WindowMenuShowTopLevelIcons { get => Get(true); set => Set(value); }

    [Tab(Tab.Menu)]
    [Name("Orientation")]
    public Orientation WindowMenuOrientation { get => Get(Orientation.Horizontal); set => Set(value); }

    #endregion

    #region Notification

    [Category(Category.Log), Tab(Tab.Notification)]
    [Name("Error")]
    public bool NotifyOnLogError { get => Get(true); set => Set(value); }

    [Category(Category.Log), Tab(Tab.Notification)]
    [Name("Message")]
    public bool NotifyOnLogMessage { get => Get(true); set => Set(value); }

    [Category(Category.Log), Tab(Tab.Notification)]
    [Name("Success")]
    public bool NotifyOnLogSuccess { get => Get(true); set => Set(value); }

    [Category(Category.Log), Tab(Tab.Notification)]
    [Name("Warning")]
    public bool NotifyOnLogWarning { get => Get(true); set => Set(value); }

    #endregion

    #region Theme

    [Index(1), Name("Theme"), Tab(Tab.Theme)]
    public ApplicationTheme Themes => Current.Get<ApplicationTheme>();

    [Hide]
    public string Theme { get => Get($"{DefaultThemes.Light}"); set => Set(value); }

    [Index(0), Name("Auto save"), Tab(Tab.Theme)]
    public bool AutoSaveTheme { get => Get(true); set => Set(value); }

    #endregion

    #region Warnings

    [Category(Category.Window), Name("Before closing"), Tab(Tab.Warnings)]
    public bool WarnOnClose { get => Get(true); set => Set(value); }

    [Category(Category.Window), Name("Before closing with unsaved documents"), Tab(Tab.Warnings)]
    public bool WarnOnCloseWithUnsavedDocuments { get => Get(true); set => Set(value); }

    #endregion

    #region Window

    [Category(Category.Button), Tab(Tab.Window)]
    [Name("Style")]
    public Controls.ButtonStyle WindowButtonStyle { get => Get(Controls.ButtonStyle.Apple); set => Set(value); }

    [Category(Category.Footer), Tab(Tab.Window)]
    [Name("Show")]
    public bool FooterVisibility { get => Get(true); set => Set(value); }

    [Category(Category.Header), Tab(Tab.Window)]
    [Name("Show")]
    public bool HeaderVisibility { get => Get(true); set => Set(value); }

    [Category(Category.Header), Tab(Tab.Window)]
    [Name("Placement")]
    public TopBottom HeaderPlacement { get => Get(TopBottom.Top); set => Set(value); }

    [Hide]
    public virtual double WindowHeight { get => Get(900.0); set => Set(value); }

    [Hide]
    public virtual double WindowWidth { get => Get(1200.0); set => Set(value); }

    [Hide]
    public virtual WindowState WindowState { get => Get(DefaultWindowState, false); set => Set(value, false); }

    #endregion

    #endregion

    #region MainViewOptions

    public MainViewOptions() : base() { }

    #endregion

    #region Methods

    protected override void OnLoaded()
    {
        base.OnLoaded();
        Current.Add(this);
    }

    protected override void OnSaving()
    {
        if (AutoSaveTheme)
        {
            if (Storage.File.Long.Exists(Theme))
                Themes.SaveTheme(Path.GetFileNameWithoutExtension(Theme));
        }
    }

    protected virtual void OnLanguageChanged() => LanguageChanged?.Invoke(this, EventArgs.Empty);

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Language):
                Language.Set();
                OnLanguageChanged();
                break;
            
            case nameof(Theme):
                Themes.LoadTheme(Theme);
                break;
        }

        base.OnPropertyChanged(e);
        //if (AutoSave) Save();
    }

    #endregion

    #region Commands

    [field: NonSerialized]
    ICommand resetCommand;
    [HeaderItem, Image(SmallImages.Reset), Name("Reset"), Reserve]
    public virtual ICommand ResetCommand => resetCommand ??= new RelayCommand(() =>
    {

    });

    [field: NonSerialized]
    ICommand saveCommand;
    [HeaderItem, Image(SmallImages.Save), Name("Save"), Reserve]
    public virtual ICommand SaveCommand => saveCommand ??= new RelayCommand(() => Save());

    #endregion
}