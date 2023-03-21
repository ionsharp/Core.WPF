using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using Imagin.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Imagin.Core.Config;

public abstract class BaseApplication : Application, IApplication
{
    public static readonly ResourceKey SplashWindowStyleKey = new();

    ///

    public const double ExitDelay = 1.5; //Seconds

    public const string ExitLabel = "Saving...";

    ///

    public event UnhandledExceptionEventHandler ExceptionUnhandled;

    public event EventHandler<EventArgs> Loaded;

    ///

    readonly StartQueue StartTasks = new();

    ///

    public virtual DataFolders DataFolder => DataFolders.Documents;

    public string DataFolderPath
        => RootFolderPath + $@"\{XAssembly.GetProperties(AssemblyType.Current).Title}";

    public string RootFolderPath
    {
        get
        {
            var result = GetFolderPath(DataFolder);
            return DataFolder switch
            {
                DataFolders.Documents => $@"{result}\{XAssembly.GetProperties(AssemblyType.Core).Company}",
                DataFolders.Execution => $@"{result}\Data",
                _ => throw new NotSupportedException(),
            };
        }
    }

    ///

    public Extensions Extensions { get; private set; } = null;

    ///

    public abstract ApplicationLink Link { get; }

    public string GitUrl => $@"https://github.com/imagin-code";

    ///

    public LogWriter Log  { get; private set; }

    public NotificationWriter Notifications { get; private set; }

    public ApplicationTheme Theme { get; private set; }

    ///

    public IMainViewModel MainViewModel 
        { get; private set; }

    new public MainWindow MainWindow 
        { get => (MainWindow)base.MainWindow; set => base.MainWindow = value; }

    public MainViewOptions Options 
        { get; private set; }

    ///

    public BaseApplication() : base()
    {
        Core.Current.Add(this);

        ///

        AppDomain.CurrentDomain.UnhandledException 
            += OnExceptionUnhandled;
        DispatcherUnhandledException 
            += OnExceptionUnhandled;
        StartTasks.Completed
            += OnStarted;
        TaskScheduler.UnobservedTaskException 
            += OnExceptionUnhandled;

        ///

        Extensions = new Extensions(DataFolderPath + $@"\Extensions");

        ///

        Log = Core.Current.Create<LogWriter>(DataFolderPath, LogWriter.DefaultLimit);

        Notifications = new NotificationWriter(DataFolderPath, NotificationWriter.DefaultLimit);

        Theme = Core.Current.Create<ApplicationTheme>(this);
        Theme.LoadTheme(DefaultThemes.Light);

        ///

        StartTasks.Add(false, nameof(Log), 
            () => Log.Load());
        StartTasks.Add(false, nameof(Notifications), 
            () => Notifications.Load());
        StartTasks.Add(true, nameof(Extensions), 
            () => { });
        StartTasks.Add(true,  nameof(Options), 
        () => 
        {
            var file = typeof(MainViewOptions).GetAttribute<FileAttribute>();
            var filePath = $@"{DataFolderPath}\{file.Name}.{file.Extension}";

            BinarySerializer.Deserialize(filePath, out MainViewOptions options);
            Options = options ?? Link.ViewOptions.Create<MainViewOptions>();
            
            Options.Language.Set();
            Theme.LoadTheme(Options.Theme);
        });
        StartTasks.Add(true, "View", 
            () => MainViewModel = Link.ViewModel.Create<IMainViewModel>());
    }

    ///

    async void OnStarted(object sender, EventArgs e)
    {
        XToolTip.Initialize();
        OnLoaded(Environment.GetCommandLineArgs().Skip(1).ToArray());

        await StartTasks.SplashWindow.FadeOut();

        MainWindow = Link.View.Create<MainWindow>();
        MainWindow.Closing += OnMainWindowClosing;
        MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        MainWindow.Show();

        StartTasks.SplashWindow.Close();
    }

    bool skipDocumentAction = false;

    bool skipExitAction = false;

    async void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!e.Cancel && !skipExitAction)
        {
            e.Cancel = true;
            if (!skipDocumentAction)
            {
                if (MainViewModel is IDockViewModel x)
                {
                    if (x.Documents.Contains(i => i.IsModified))
                    {
                        if (Options.WarnOnCloseWithUnsavedDocuments)
                        {
                            var neverShow = new BooleanAccessor(() => !Options.WarnOnCloseWithUnsavedDocuments, i => Options.WarnOnCloseWithUnsavedDocuments = !i);
                            Dialog.ShowWarning(XAssembly.GetProperties(AssemblyType.Current).Title, new Warning($"One or more documents have unsaved changes. Close anyway?"), neverShow, i =>
                            {
                                if (i == 0)
                                {
                                    skipDocumentAction = true;
                                    MainWindow.Close();
                                }
                            },
                            Buttons.YesNo);
                            return;
                        }
                    }
                }
            }

            List<Error> errors = new();
            Action exitAction = new(() =>
            {
                Notifications
                    .Save().If<Error>(i => errors.Add(i));
                Options
                    .Save().If<Error>(i => errors.Add(i));
            });

            async void close()
            {
                if (Options.LogClearOnAppClose)
                {
                    Log.Clear();
                    await While.InvokeAsync(() => Log.Count > 0);
                }

                Try.Invoke(() => Log.Save());

                skipExitAction = true;
                MainWindow.Close();
            }

            void showLog()
            {
                if (MainViewModel is IDockMainViewModel x)
                {
                    if (x.Panels.FirstOrDefault<LogPanel>() is LogPanel logPanel)
                    {
                        if (!logPanel.IsVisible)
                            logPanel.IsVisible = true;

                        logPanel.IsSelected = true;
                    }
                }
                else if (MainViewModel is MainViewModel y)
                    y.LogCommand.Execute();
            }

            await Dialog.ShowProgress(ExitLabel, i => exitAction(), TimeSpan.FromSeconds(ExitDelay), true);
            if (errors.Count > 0)
            {
                if (Options.WarnOnClose)
                {
                    var neverShow = new BooleanAccessor(() => !Options.WarnOnClose, i => Options.WarnOnClose = !i);
                    Dialog.ShowError(XAssembly.GetProperties(AssemblyType.Current).Title, new Error($"An error occurred while saving. Close anyway?") { Inner = errors.First() }, neverShow, i =>
                    {
                        if (i == 1)
                        {
                            showLog();
                            return;
                        }
                        XWindow.SetDisableCancel(MainWindow, true);
                        close();
                    },
                    Buttons.YesNo);
                    return;
                }
            }
            close();
        }
    }

    void OnSplashWindowShown(object sender, EventArgs e)
    {
        if (sender is SplashWindow window)
        {
            window.Shown -= OnSplashWindowShown;
            _ = StartTasks.Invoke(window);
        }
    }

    ///

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var window = new SplashWindow();
        window.SetResourceReference(SplashWindow.StyleProperty, SplashWindowStyleKey);
        window.Shown += OnSplashWindowShown;
        window.Show();
    }

    protected virtual void OnLoaded(IList<string> arguments)
    {
        Loaded?.Invoke(this, EventArgs.Empty);
        MainViewModel.OnLoaded(arguments);
    }

    ///

    void OnExceptionUnhandled(object sender, System.UnhandledExceptionEventArgs e)
    {
        OnExceptionUnhandled(UnhandledExceptions.AppDomain, e.ExceptionObject as Exception);
    }

    void OnExceptionUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
#if DEBUG  
        e.Handled = false;
#else
        e.Handled = true;
#endif     
        OnExceptionUnhandled(UnhandledExceptions.Dispatcher, e.Exception);
    }

    void OnExceptionUnhandled(object sender, UnobservedTaskExceptionEventArgs e)
    {
#if DEBUG  

#else
        e.SetObserved();
#endif     
        OnExceptionUnhandled(UnhandledExceptions.TaskScheduler, e.Exception);
    }

    ///

    protected virtual void OnExceptionUnhandled(UnhandledExceptions type, Exception e)
    {
        var error = new Error(e);
        Log.Write<BaseApplication>(error, ResultLevel.High);
        ExceptionUnhandled?.Invoke(this, new UnhandledExceptionEventArgs(type, error));
    }

    ///

    public static string GetFolderPath(DataFolders folder)
    {
        switch (folder)
        {
            case DataFolders.Documents:
                return Environment.SpecialFolder.MyDocuments.GetPath();

            case DataFolders.Execution:
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            case DataFolders.Local:
                return Environment.SpecialFolder.LocalApplicationData.GetPath();

            case DataFolders.Roaming:
                return Environment.SpecialFolder.ApplicationData.GetPath();
        }
        throw new NotSupportedException();
    }
}