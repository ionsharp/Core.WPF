using Imagin.Core.Input;
using Imagin.Core.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imagin.Core.Models;

public abstract class TaskPanel : Panel
{
    protected readonly Method<object> task = null;
    [Hide]
    public Method<object> Task => task;

    protected abstract TaskManagement Execution { get; }

    [Hide]
    public TimeSpan Duration { get => Get(TimeSpan.Zero); private set => Set(value); }

    [Hide]
    public virtual DateTime? LastActive { get => Get<DateTime?>(null); private set => Set(value); }

    public TaskPanel() : base() => task = new(StartSync, StartAsync, false);

    /// <summary>Already on thread</summary>
    protected abstract void ExecuteSync(object parameter, CancellationToken token);

    /// <summary>Not yet on thread</summary>
    protected abstract Task ExecuteAsync(object parameter, CancellationToken token);

    /// <summary>Already on thread</summary>
    void StartSync(object parameter, CancellationToken token)
    {
        Dispatch.Invoke(() =>
        {
            IsBusy = true; IsLocked = true;
            Progress = 0; ProgressVisibility = false;
        });

        var watch = System.Diagnostics.Stopwatch.StartNew();
        ExecuteSync(parameter, token);
        watch.Stop();

        Dispatch.Invoke(() =>
        {
            Duration = watch.Elapsed;
            LastActive = DateTime.Now;

            Progress = 0; ProgressVisibility = false;
            IsLocked = false; IsBusy = false;
        });
    }

    /// <summary>Not yet on thread</summary>
    async protected Task StartAsync(object parameter, CancellationToken token)
    {
        IsBusy = true; IsLocked = true;
        Progress = 0; ProgressVisibility = false;

        var watch = System.Diagnostics.Stopwatch.StartNew();
        await ExecuteAsync(parameter, token);
        watch.Stop();

        Duration = watch.Elapsed;
        LastActive = DateTime.Now;

        Progress = 0; ProgressVisibility = false;
        IsLocked = false; IsBusy = false;
    }

    protected virtual void OnExecuted() { }

    ICommand startCommand;
    [Name("Start"), Image(SmallImages.Play), Index(0), Pin(Pin.BelowOrRight), Style(ButtonStyle.Default)]
    [VisibilityTrigger(nameof(IsBusy), false)]
    public virtual ICommand StartCommand => startCommand ??= new RelayCommand<object>(i => _ = task.Start(i, Execution), i => !task.Started);

    ICommand cancelCommand;
    [Name("Cancel"), Image(SmallImages.Block), Index(1), Pin(Pin.BelowOrRight), Style(ButtonStyle.Cancel)]
    [VisibilityTrigger(nameof(IsBusy), true)]
    public virtual ICommand CancelCommand => cancelCommand ??= new RelayCommand(() => task.Cancel(), () => task.Started);
}