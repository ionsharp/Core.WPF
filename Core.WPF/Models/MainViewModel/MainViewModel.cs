using Hardcodet.Wpf.TaskbarNotification;
using Imagin.Core.Analytics;
using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Serialization;
using Imagin.Core.Storage;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Models;

#region MainViewModel

[Menu(typeof(Menu), Float = true)]
public abstract class MainViewModel : ViewModel, IElementReference, IMenuSource
{
    enum Category1 { Extensions, [HideName]General }

    enum Category2 { Debug }

    [Menu]
    enum Menu
    {
        [MenuItem(Icon = SmallImages.Puzzle)]
        Extensions,
        [MenuItem(Icon = SmallImages.GitHub)]
        GitHub,
        [MenuItem(Icon = SmallImages.Palette)]
        Theme
    }

    #region Keys

    public static readonly IElementKey TaskbarIconKey = new ReferenceKey<TaskbarIcon>();

    #endregion

    #region Properties

    protected TaskbarIcon TaskbarIcon;

    public ObjectCollection MenuItems { get; private set; } = new();

    public virtual void SetReference(IElementKey a, FrameworkElement b)
    {
        if (a == TaskbarIconKey)
            TaskbarIcon = (TaskbarIcon)b;
    }

    ///

    [Hide]
    public virtual LogPanel LogPanel { get; protected set; } = new(Current.Get<BaseApplication>().Log);

    [Hide]
    public virtual NotificationsPanel NotificationPanel { get; protected set; } = new(Current.Get<BaseApplication>().Notifications);

    ///

    [Hide]
    public virtual string Title => XAssembly.GetProperties(AssemblyType.Current).Title;

    #endregion

    #region MainViewModel

    public MainViewModel() : base()
    {
        #region ///

        Current.Add(this);

        Current.Get<MainViewOptions>().PropertyChanged 
            += OnOptionsChanged;
        Current.Get<MainViewOptions>().Saving 
            += OnOptionsSaving;

        #endregion

        #region Theme

        OnThemeChanged();

        #endregion

        #region Log

        Current.Get<ILog>().As<ICollectionChanged>().CollectionChanged 
            += OnLogChanged;

        #endregion

        #region Extensions

        var extensions = Current.Get<BaseApplication>().Extensions;
        Try.Invoke(() => System.IO.Directory.CreateDirectory(extensions.Path), 
            e => Log.Write<MainViewModel>(e));

        extensions.Subscribe();
        extensions.Refresh();

        extensions.Disabled 
            += OnExtensionDisabled;
        extensions.Enabled 
            += OnExtensionEnabled;

        var defaultTypes 
            = XAssembly.GetDerivedTypes<IExtension>(AssemblyType.Core);
        var defaultExtensions 
            = defaultTypes.Select(i => Try.Return(() => i.Create<IExtension>())).ToList();

        foreach (var i in defaultExtensions)
        {
            if (i != null)
            {
                var defaultPath = $@"{extensions.Path}\{i.Name}.ext";
                if (!System.IO.File.Exists(defaultPath))
                    BinarySerializer.Serialize(defaultPath, i);
            }
        }

        #endregion
    }

    void OnLogChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            foreach (LogEntry i in e.NewItems)
            {
                if 
                (
                (i.Result is Error && Current.Get<MainViewOptions>().NotifyOnLogError)
                ||
                (i.Result is Message && Current.Get<MainViewOptions>().NotifyOnLogMessage)
                ||
                (i.Result is Success && Current.Get<MainViewOptions>().NotifyOnLogSuccess)
                ||
                (i.Result is Warning && Current.Get<MainViewOptions>().NotifyOnLogWarning)
                )
                { Current.Get<BaseApplication>().Notifications.Add(new Notification(i.Sender, i.Result, System.TimeSpan.Zero)); }
            }
        }
    }

    #endregion

    #region Menu

    #region About

    ICommand aboutCommand;
    [MenuItem(Header = "About", Icon = SmallImages.Info)]
    public ICommand AboutCommand
        => aboutCommand ??= new RelayCommand(() => Dialog.ShowAbout());

    #endregion

    #region Extensions

    [MenuItem(Parent = Menu.Extensions, Header = "Export", Icon = SmallImages.Export)]
    public ICommand ExportExtensionsCommand => Current.Get<BaseApplication>().Extensions.ExportCommand;

    [MenuItem(Parent = Menu.Extensions, Header = "Import", Icon = SmallImages.Import)]
    public ICommand ImportExtensionsCommand => Current.Get<BaseApplication>().Extensions.ImportCommand;

    [MenuItem(Parent = Menu.Extensions, Header = "Reset", Icon = SmallImages.Reset)]
    public ICommand ResetExtensionsCommand => Current.Get<BaseApplication>().Extensions.ResetCommand;

    [Category(Category1.Extensions), Index(1)]
    [MenuItemCollection(Parent = Menu.Extensions,

        ItemCheckable = true,
        ItemCheckableMode = BindingMode.TwoWay,
        ItemCheckablePath = nameof(IExtension.IsEnabled),
        ItemHeaderPath = nameof(IExtension.Name),
        ItemIconPath = nameof(IExtension.Icon),
        ItemType = typeof(IExtension),

        ItemInputGestureTextPath = nameof(IExtension.Version),
        ItemInputGestureTextConverter = typeof(ShortVersionConverter),

        ItemToolTipPath = ".",
        ItemToolTipTemplateSource = typeof(MemberGrid),
        ItemToolTipTemplateKey = nameof(MemberGrid.ObjectToolTipKey),

        SortDirection = ListSortDirection.Ascending,
        SortName = nameof(IExtension.Name))]
    public object Extensions => Current.Get<BaseApplication>().Extensions.Source;

    #endregion

    #region GitHub

    ICommand gitHubCodeCommand;
    [MenuItem(Parent = Menu.GitHub, Header = "Home", Icon = SmallImages.Home)]
    public ICommand GitHubCodeCommand
        => gitHubCodeCommand ??= new RelayCommand(() => System.Diagnostics.Process.Start($@"{Current.Get<BaseApplication>().GitUrl}/{XAssembly.GetProperties(AssemblyType.Current).Product}"));

    ICommand gitHubIssuesCommand;
    [MenuItem(Parent = Menu.GitHub, Header = "Issues", Icon = SmallImages.Bug)]
    public ICommand GitHubIssuesCommand
        => gitHubIssuesCommand ??= new RelayCommand(() => System.Diagnostics.Process.Start($@"{Current.Get<BaseApplication>().GitUrl}/{XAssembly.GetProperties(AssemblyType.Current).Product}/issues"));

    ICommand gitHubWikiCommand;
    [MenuItem(Parent = Menu.GitHub, Header = "Wiki", Icon = SmallImages.Info)]
    public ICommand GitHubWikiCommand
        => gitHubWikiCommand ??= new RelayCommand(() => System.Diagnostics.Process.Start($@"{Current.Get<BaseApplication>().GitUrl}/{XAssembly.GetProperties(AssemblyType.Current).Product}/wiki"));

    ICommand gitHubOtherProjectsCommand;
    [MenuItem(Parent = Menu.GitHub, Header = "Explore", Icon = SmallImages.LightBulb, SubCategory = 1)]
    public ICommand GitHubOtherProjectsCommand
        => gitHubOtherProjectsCommand ??= new RelayCommand(() => System.Diagnostics.Process.Start($@"{Current.Get<BaseApplication>().GitUrl}"));

    #endregion

    #region Log

    ICommand logCommand;
    [MenuItem(Header = "Log", Icon = SmallImages.Log)]
    public ICommand LogCommand
        => logCommand ??= new RelayCommand(() => Dialog.ShowPanel(LogPanel));

    #endregion

    #region Options

    ICommand optionsCommand;
    [HeaderItem, Image(SmallImages.Options), Name("Options"), Reserve, Show]
    [MenuItem(Header = "Options", Icon = SmallImages.Options)]
    public virtual ICommand OptionsCommand
        => optionsCommand ??= new RelayCommand(() => Dialog.ShowObject("Options", Current.Get<MainViewOptions>(), Resource.GetImageUri(SmallImages.Options), Buttons.Done));

    #endregion

    #region Theme

    [MenuItemCollection(Parent = Menu.Theme,
        IsInline = true,

        ItemCommandName = nameof(ThemeCommand),
        ItemCommandParameterPath = ".",

        ItemHeaderPath = ".",

        ItemIconPath = ".",
        ItemIconTemplateSource = typeof(XExplorer),
        ItemIconTemplateKey = nameof(XExplorer.IconTemplateKey),

        ItemToolTipPath = ".",
        ItemToolTipTemplateSource = typeof(XExplorer),
        ItemToolTipTemplateKey = nameof(XExplorer.ToolTipTemplateKey),

        ItemType = typeof(string))]
    public StringCollection Themes => new StringCollection(typeof(DefaultThemes).GetEnumValues<DefaultThemes>(Appearance.Visible).Select(i => i.ToString()));

    #endregion

    #endregion

    #region Methods

    #region Private

    void OnOptionsSaving(object sender, System.EventArgs e) => OnOptionsSaving(sender as MainViewOptions);

    void OnOptionsChanged(object sender, PropertyChangedEventArgs e) => OnOptionsChanged(e);

    #endregion

    #region Protected

    private void OnExtensionDisabled(object sender, EventArgs<IExtension> e)
        => OnExtensionDisabled(e.Value);

    protected virtual void OnExtensionDisabled(IExtension extension)
    {
        MenuItems.Remove(extension);
    }

    private void OnExtensionEnabled(object sender, EventArgs<IExtension> e)
        => OnExtensionEnabled(e.Value);

    protected virtual void OnExtensionEnabled(IExtension extension)
    {
        MenuItems.Add(extension);
    }

    protected virtual void OnOptionsChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MainViewOptions.PasswordEnable):
                
                break;

            case nameof(MainViewOptions.Theme):
                OnThemeChanged();
                break;
        }
    }

    protected virtual void OnOptionsSaving(MainViewOptions options) { }

    protected virtual void OnThemeChanged()
    {
        //This doesn't change automatically when the theme does...
        TaskbarIcon?.ContextMenu?.UpdateDefaultStyle();
    }

    #endregion

    #endregion

    #region Commands

    ICommand themeCommand;
    [Hide]
    public ICommand ThemeCommand => themeCommand ??= new RelayCommand<string>(i => Current.Get<MainViewOptions>().Theme = i, i => true);

    #endregion
}

#endregion

#region MainViewModel<T> where T : MainWindow

public abstract class MainViewModel<T> : MainViewModel, IMainViewModel where T : MainWindow
{
    bool handleExit = false;

    ///

    Window IMainViewModel.View
    {
        get => View;
        set => View = value.As<T>();
    }

    [Hide]
    public T View { get => Get<T>(); set => Set(value); }

    public MainViewModel() : base() 
    {
        
    }

    ///

    void OnWindowClosed(object sender, System.EventArgs e) => OnWindowClosed();

    void OnWindowClosing(object sender, CancelEventArgs e) => OnWindowClosing(e);

    ///

    protected override void OnOptionsChanged(PropertyChangedEventArgs e) 
    {
        base.OnOptionsChanged(e);
        switch (e.PropertyName)
        {
            case nameof(MainViewOptions.TaskbarIconVisibility):
                if (!Current.Get<MainViewOptions>().TaskbarIconVisibility)
                    Show();

                break;
        }
    }

    protected virtual void OnWindowClosed() { }

    protected virtual void OnWindowClosing(CancelEventArgs e)
    {
        return;
        if (Current.Get<MainViewOptions>().TaskbarIconVisibility)
        {
            if (!handleExit)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(View))
        {
            if (View != null)
            {
                View.DataContext = this;
                View.Closed += OnWindowClosed;
                View.Closing += OnWindowClosing;

                View.Loaded += OnViewLoaded;
            }
        }
    }

    ///

    void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        if (Current.Get<MainViewOptions>().PasswordEnable)
            CheckPassword();
    }

    void CheckPassword(bool error = false)
    {
        var form = new PasswordForm(Current.Get<MainViewOptions>().PasswordType);
        Dialog.ShowObject("Unlock", form, Resource.GetImageUri(SmallImages.Lock), i =>
        {
            if (i == 0 && form.Password == Current.Get<MainViewOptions>().PasswordDefault)
            {

            }
            else
            {
                var neverShow = new BooleanAccessor(() => Current.Get<MainViewOptions>().PasswordErrorNeverShow, i => Current.Get<MainViewOptions>().PasswordErrorNeverShow = i);
                if (neverShow.Get())
                {
                    CheckPassword(true);
                }
                else Dialog.ShowError("Wrong password", new Error(new WrongPasswordException()), neverShow, i => CheckPassword(true), Buttons.Ok);
            }
        },
        Buttons.Continue);
    }

    ///

    void Hide()
    {
        View.Hide();
        View.ShowInTaskbar = false;
    }

    void Show()
    {
        View.Show();
        View.ShowInTaskbar = true;
    }

    ///

    public virtual void OnLoaded(IList<string> arguments) { }

    public virtual void OnReloaded(IList<string> arguments) { }

    ///

    ICommand exitCommand;
    [Hide]
    public ICommand ExitCommand => exitCommand ??= new RelayCommand(() => View.Close(), () => true);

    ICommand forceExitCommand;
    [Hide]
    public ICommand ForceExitCommand => forceExitCommand ??= new RelayCommand(() =>
    {
        handleExit = true;
        View.Close();
    },
    () => true);

    ICommand hideCommand;
    [Hide]
    public ICommand HideCommand => hideCommand ??= new RelayCommand(() => Hide(), () => View.IsVisible);

    ICommand showCommand;
    [Hide]
    public ICommand ShowCommand => showCommand ??= new RelayCommand(() => Show(), () => !View.IsVisible);
}

#endregion