using Imagin.Core.Analytics;
using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Storage;
using Imagin.Core.Text;
using Imagin.Core.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

#region (enum) CopyDirection

[Serializable]
public enum CopyDirection
{
    Left,
    Right,
    Both
}

#endregion

#region (enum) TaskStatus

[Serializable]
public enum TaskStatus { Active, Inactive, Monitoring }

#endregion

///

#region TaskLog

public class TaskLog : ConcurrentCollection<LogEntry>, ILog
{
    public bool Enabled => true;

    public readonly BaseTask Task;

    public TaskLog(BaseTask task)
    {
        Task = task;
    }

    public Result Save() => new Success();
}

#endregion

#region BaseTask

[Serializable, View(Reflection.MemberView.Tab, typeof(Tab))]
public abstract class BaseTask : Lockable, IMethod, ISubscribe, IUnsubscribe
{
    enum Category { Activity, Log }

    enum Tab { }

    [XmlIgnore]
    protected Watcher DefaultWatcher
    {
        get
        {
            var result = new Watcher()
            {
                IncludeChildren = true,
                Filter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Security
            };
            result.Failed += OnWatchingFailed;
            return result;
        }
    }

    [Category(Category.Activity)]
    [Description("When the task was created.")]
    [StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public DateTime Created { get => Get(DateTime.Now); set => Set(value); }

    [Hide]
    public bool IsActive => Status == TaskStatus.Active;

    [Hide]
    public bool IsEnabled
    {
        get => Get(false);
        set
        {
            //If the task is already enabled
            if (Get<bool>())
            {
                //If the task should be disabled
                if (!value)
                    Disable();
            }
            //If the task is already disabled
            else
            {
                //If the task should be enabled
                if (value)
                    Enable();
            }
        }
    }

    [Category(Category.Activity)]
    [Description("When the task was last active.")]
    [StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public DateTime? LastActive { get => Get<DateTime?>(); set => Set(value); }

    [Category(Category.Activity)]
    [Description("When the task was last enabled.")]
    [StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public DateTime? LastEnabled { get => Get<DateTime?>(); set => Set(value); }

    [Category(Category.Activity)]
    [Description("When the task was last disabled.")]
    [StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public DateTime? LastDisabled { get => Get<DateTime?>(); set => Set(value); }

    [Category(Category.Activity)]
    [Description("When the task was last modified.")]
    [StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public DateTime? LastModified { get => Get<DateTime?>(); set => Set(value); }

    [field: NonSerialized]
    TaskLog log;
    [Hide]
    [XmlIgnore]
    public TaskLog Log => log ??= new TaskLog(this);

    [Category(Category.Log)]
    [Description("Whether or not errors are logged.")]
    [Name("Errors")]
    [Lock]
    public bool LogErrors { get => Get(true); set => Set(value); }

    [Category(Category.Log)]
    [Description("Whether or not messages are logged.")]
    [Name("Messages")]
    [Lock]
    public bool LogMessages { get => Get(true); set => Set(value); }

    [Category(Category.Log)]
    [Description("Whether or not success is logged.")]
    [Name("Success")]
    [Lock]
    public bool LogSuccess { get => Get(true); set => Set(value); }

    [Category(Category.Log)]
    [Description("Whether or not warnings are logged.")]
    [Name("Warnings")]
    [Lock]
    public bool LogWarnings { get => Get(true); set => Set(value); }

    [Hide, XmlIgnore]
    public double Progress { get => Get(.0, false); set => Set(value, false); }

    [Description("The status of the task.")]
    [Float(Float.Below), ReadOnly, XmlIgnore]
    public TaskStatus Status { get => Get(TaskStatus.Inactive, false); set => Set(value, false); }

    protected readonly List<Watcher> Monitors = new();

    protected readonly Method internalTask;

    public BaseTask() : base() => internalTask = new(Run, false);

    void OnWatchingFailed(object sender, EventArgs<Error> e) => Write(e.Value);

    protected abstract Task Run(CancellationToken token);

    protected virtual bool IsMonitor { get; }

    protected virtual void OnMonitoring() { }

    protected virtual async void Enable()
    {
        if (IsEnabled) return;

        Set(() => IsEnabled, true);
        LastEnabled = DateTime.Now;

        IsLocked = true;
        Status = TaskStatus.Active;

        await internalTask.Start();

        if (IsMonitor)
        {
            Status = TaskStatus.Monitoring;
            OnMonitoring();
        }
        else Disable();
    }

    protected virtual void Disable()
    {
        if (!IsEnabled) return;

        Status = TaskStatus.Inactive;
        IsLocked = false;

        LastDisabled = DateTime.Now;
        Set(() => IsEnabled, false);
    }

    protected void Write(object message, ResultLevel level = ResultLevel.Normal, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0) => Write(true, message, level, member, line);

    protected void Write(bool log, object message, ResultLevel level = ResultLevel.Normal, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0) => log.If(true, () =>
    {
        var n = message is Result m ? m : new Message(message);

        if (n is Error && !LogErrors) return;
        if (n is Message && !LogMessages) return;
        if (n is Success && !LogSuccess) return;
        if (n is Warning && !LogWarnings) return;

        if (!log) return;
        Log.Write<BaseTask>(n, level, member, line);
    });

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        LastModified = DateTime.Now;
        switch (e.PropertyName)
        {
            case nameof(Status): Update(() => IsActive); break;
        }
    }

    public void SimulateProgress(Action<double> action, double increment = 0.25, int delay = 100)
    {
        double c = 0;
        while (c <= 1)
        {
            Dispatch.Invoke(() => action(c));
            c += increment;

            Thread.Sleep(delay);
        }
    }

    public virtual void Subscribe()
    {
        foreach (var i in Monitors)
        {
            i.Failed -= OnWatchingFailed; i.Failed += OnWatchingFailed;
            i.Subscribe();
        }
    }

    public virtual void Unsubscribe()
    {
        foreach (var i in Monitors)
        {
            i.Failed -= OnWatchingFailed;
            i.Unsubscribe();
        }
    }
}

#endregion

#region CopyTask

[Name(DefaultTitle), Serializable]
[XmlType(TypeName = nameof(Task))]
public class CopyTask : BaseTask
{
    enum Categories { General, Last, Source, Target }

    public const string DefaultTitle = "Copy";

    #region Events

    public delegate void SynchronizedEventHandler();

    [field: NonSerialized]
    public event SynchronizedEventHandler Synchronized;

    #endregion

    //...

    #region Messages

    class Messages
    {
        public const string Browse = "Browsing '{0}'...";

        public const string Create = "Creating '{0}'";

        public const string Delete = "Deleting '{0}'";

        public const string Skip = "Skipping '{0}'";

        public const string Synchronize = "Synchronizing '{0}'";
    }

    #endregion

    #region Tasks

    [field: NonSerialized]
    Method analyzeTask;
    [Hide, XmlIgnore]
    public Method AnalyzeTask => analyzeTask;

    [field: NonSerialized]
    Method synchronizeTask;
    [Hide]
    [XmlIgnore]
    public Method SynchronizeTask => synchronizeTask;

    #endregion

    //...

    #region Properties

    [Category(Categories.General), Lock]
    [Description("How to copy files.")]
    public Imagin.Core.Text.TextConverter.Action Action { get => Get(new TextConverter.Action()); set => Set(value); }

    [Float(Float.Above), Index(1)]
    [Int32Style(Int32Style.Index, nameof(CategorySource))]
    public int Category { get => Get(-1); set => Set(value); }

    [Category(Categories.General)]
    public CopyDirection Direction { get => Get(CopyDirection.Right); set => Set(value); }

    [Hide]
    public string DirectionDescription => Direction == CopyDirection.Left ? "From target to source" : Direction == CopyDirection.Right ? "From source to target" : "From source to target (and back)";

    [Hide, XmlIgnore]
    public IList CategorySource => Current.Get<IDockMainViewModel>().Panels.FirstOrDefault<CopyPanel>().Categories;

    [Hide, XmlIgnore]
    public string CategoryName => CategorySource?.Count > Category && Category >= 0 ? (string)CategorySource[Category] : "General";

    [Category(Categories.General), Lock]
    public Encoding Encoding { get => Get(Encoding.Unicode); set => Set(value); }

    [Category(Categories.Source)]
    [ReadOnly]
    [XmlIgnore]
    public uint Files { get => Get((uint)0, false); set => Set(value, false); }

    [field: NonSerialized]
    Threading.Queue queue;
    [Hide]
    [XmlIgnore]
    public Threading.Queue Queue
    {
        get
        {
            if (queue != null)
                return queue;

            queue = new Threading.Queue(this);
            return queue;
        }
    }

    [Category(Categories.Source)]
    [StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    [XmlIgnore]
    public long Size { get => Get(0L, false); set => Set(value, false); }

    [Category(Categories.Source)]
    [Description("The path to the folder where stuff should be written from.")]
    [Index(0)]
    [Lock]
    [StringStyle(StringStyle.FolderPath)]
    public string Source { get => Get(""); set => Set(value); }

    [Category(Categories.Source)]
    [Description("The file/folder extensions to include or exclude at the source.")]
    [Name("Extensions")]
    [Lock]
    public FilterExtensions SourceExtensions { get => Get(new FilterExtensions()); set => Set(value); }

    [Category(Categories.Source)]
    [Description("Whether or not to include or exclude files with the given attributes at the source.")]
    [Name("FileAttributes")]
    [Lock]
    [Style(EnumStyle.Flags)]
    public Attributes SourceFileAttributes { get => Get(Attributes.All); set => Set(value); }

    [Category(Categories.Source)]
    [Description("Whether or not to include or exclude folders with the given attributes at the source.")]
    [Name("FolderAttributes")]
    [Lock]
    [Style(EnumStyle.Flags)]
    public Attributes SourceFolderAttributes { get => Get(Attributes.All); set => Set(value); }

    [Category(Categories.Source)]
    [Description("When source files should be overwritten.")]
    [Lock]
    public OverwriteCondition SourceOverwriteFiles { get => Get(OverwriteCondition.Always); set => Set(value); }

    string target = string.Empty;
    [Category(Categories.Target)]
    [Description("The path to the folder where stuff should be written to.")]
    [Index(1)]
    [Lock]
    [StringStyle(StringStyle.FolderPath)]
    [Validate(typeof(TargetValidator))]
    public string Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value.TrimEnd('\\');
            Update(() => Target);
        }
    }

    [Category(Categories.Target)]
    [Description("The file/folder extensions to include or exclude at the target.")]
    [Name("Extensions")]
    [Lock]
    public FilterExtensions TargetExtensions { get => Get(new FilterExtensions()); set => Set(value); }

    [Category(Categories.Target)]
    [Description("Whether or not to include or exclude files with the given attributes at the target.")]
    [Name("FileAttributes")]
    [Lock]
    [Style(EnumStyle.Flags)]
    public Attributes TargetFileAttributes { get => Get(Attributes.All); set => Set(value); }

    [Category(Categories.Target)]
    [Description("Whether or not to include or exclude folders with the given attributes at the target.")]
    [Name("FolderAttributes")]
    [Lock]
    [Style(EnumStyle.Flags)]
    public Attributes TargetFolderAttributes { get => Get(Attributes.All); set => Set(value); }

    [Category(Categories.Source)]
    [Description("When target files should be overwritten.")]
    [Lock]
    public OverwriteCondition TargetOverwriteFiles { get => Get(OverwriteCondition.Always); set => Set(value); }

    [Hide]
    [XmlIgnore]
    public IValidate TargetValidator => new TargetValidator();

    //...

    [Category(Categories.Source)]
    [Description("The types of changes to watch the source folder for.")]
    [Name("Watch"), Lock]
    public MonitorProfile SourceMonitorProfile { get => Get(new MonitorProfile()); set => Set(value); }

    [field: NonSerialized]
    Watcher sourceMonitor;
    [XmlIgnore]
    Watcher SourceMonitor => sourceMonitor ??= DefaultWatcher;

    [Category(Categories.Target)]
    [Description("The types of changes to watch the target folder for.")]
    [Name("Watch"), Lock]
    public MonitorProfile TargetMonitorProfile { get => Get(new MonitorProfile()); set => Set(value); }

    [field: NonSerialized]
    Watcher targetMonitor;
    [XmlIgnore]
    Watcher TargetMonitor => targetMonitor ??= DefaultWatcher;

    protected override bool IsMonitor => true;

    #endregion

    #region CopyTask

    public CopyTask() : base()
    {
        analyzeTask = new Method(AnalyzeSize, true);
    }

    #endregion

    #region Methods

    [field: NonSerialized]
    ICommand deleteCommand;
    [Hide]
    [XmlIgnore]
    public ICommand DeleteCommand => deleteCommand ??= new RelayCommand(() =>
    {
        Dialog.ShowWarning("Delete", new Warning("Are you sure you want to delete this?"), i =>
        {
            if (i == 0)
                Current.Get<IDockMainViewModel>().Panels.FirstOrDefault<CopyPanel>().Tasks.Remove(this);
        },
        Buttons.YesNo);
    },
    () => true);

    //...

    void Browse(string source, CancellationToken token, Action<string, string> createdFile = null, Action<string, string> createdFolder = null, Action<string> deletedFile = null, Action<string> deletedFolder = null, bool log = true)
    {
        if (token.IsCancellationRequested)
            return;

        Result result = Try.Invoke(() =>
        {
            if (!Folder.Long.Exists(source))
                throw new DirectoryNotFoundException($"'{source}' does not exist.");
        },
        e => Write(log, e));
        if (!result)
            return;

        Write(log, Messages.Browse.F(source));

        List<string> sItems = new(), dItems = new();

        result = Try.Invoke(() => sItems = Folder.Long.GetItems(source).ToList(), e => Write(log, e));
        if (!result)
            return;

        var target = source.Replace(Source, Target);
        //Try.Invoke(() => target = Converter.FormatUri(Target, target.Replace(Target, string.Empty), ItemType.Folder, action), e => Write(log, e));

        if (!result)
            return;

        result = Try.Invoke(() => dItems = Folder.Long.GetItems(target).ToList(), e => Write(log, e));
        foreach (var i in sItems)
        {
            if (token.IsCancellationRequested)
                return;

            ItemType itemType = default;
            result = Try.Invoke(() => itemType = Computer.GetType(i), e => Write(log, e));

            if (result)
            {
                var dOld = i.Replace(Source, Target);
                var dNew = string.Empty;

                Try.Invoke(() => dNew = dOld /*Converter.FormatUri(Target, dOld.Replace(Target, string.Empty), itemType, action)*/, e => Write(log, e));
                if (ApplySource(log, itemType, i, dNew))
                {
                    switch (itemType)
                    {
                        case ItemType.File:
                            createdFile?.Invoke(i, dNew);
                            break;

                        case ItemType.Folder:
                            createdFolder?.Invoke(i, dNew);
                            Browse(i, token, createdFile, createdFolder, deletedFile, deletedFolder, log);
                            break;
                    }
                    dItems.Remove(dNew);
                }
            }
        }

        if (token.IsCancellationRequested)
            return;

        dItems.ForEach(i =>
        {
            ItemType dType = default;
            result = Try.Invoke(() => dType = Computer.GetType(i), e => Write(log, e));

            if (result)
            {
                if (ApplyTarget(log, dType, i))
                {
                    switch (dType)
                    {
                        case ItemType.File:
                            deletedFile?.Invoke(i);
                            break;
                        case ItemType.Folder:
                            deletedFolder?.Invoke(i);
                            break;
                    }

                }
            }
        });
    }

    //...

    void CreateFile(bool log, string source, string target, CancellationToken token)
    {
        Write(log, Messages.Create.F(target));
        return;

        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        Try.Invoke(() =>
        {
            var callback = new Imagin.Core.Text.TextConverter.Callback((sizeRead, size) =>
            {
                Queue.Current.Duration = stopWatch.Elapsed;
                Queue.Current.SizeRead = sizeRead.Int64();
                Queue.Current.Progress = sizeRead / size;
                return token.IsCancellationRequested;
            });
            Imagin.Core.Text.TextConverter.CopyFile(source, target, Encoding, true, callback);
        }, e => Write(log, e));

        stopWatch.Stop();
        Queue.Remove(Queue.Current);
    }

    void CreateFolder(bool log, string target)
    {
        Write(log, Messages.Create.F(target));
        return;

        Try.Invoke(() => Folder.Long.Create(target), e => Write(log, e));
    }

    //...

    void Delete(bool log, string i)
    {
        Write(log, Messages.Delete.F(i));
        return;

        Try.Invoke(() =>
        {
            //This is where everything gets deleted. Test before going live!
            return;
            ItemType itemType = Computer.GetType(i);
            switch (itemType)
            {
                case ItemType.File:
                    Imagin.Core.Storage.File.Long.Delete(i);
                    break;

                case ItemType.Folder:
                    Folder.Long.Delete(i, true);
                    break;
            }
        }, e => Write(log, e));
    }

    //...

    bool ApplyAttributes(ItemType type, string input, Attributes fileAttributes, Attributes folderAttributes)
    {
        FileAttributes attributes = Imagin.Core.Storage.File.Long.Attributes(input);
        var h = attributes.HasAnyFlags(FileAttributes.Hidden);
        var r = attributes.HasAnyFlags(FileAttributes.ReadOnly);

        switch (type)
        {
            case ItemType.File:
                if (!fileAttributes.HasAnyFlags(Attributes.Hidden))
                    return !h;

                if (!fileAttributes.HasAnyFlags(Attributes.ReadOnly))
                    return !r;

                break;

            case ItemType.Folder:
                if (!folderAttributes.HasAnyFlags(Attributes.Hidden))
                    return !h;

                if (!folderAttributes.HasAnyFlags(Attributes.ReadOnly))
                    return !r;

                break;
        }
        return true;
    }

    bool ApplyExtensions(string input, FilterExtensions extensions)
    {
        if (extensions == null || extensions.Value.NullOrEmpty())
            return true;

        var fileExtension = Path.GetExtension(input).TrimExtension();
        if (extensions.Value.ToLower().Contains(fileExtension))
            return extensions.Filter == Exclusivity.Include;

        return false;
    }

    bool ApplyOverwrite(string source, string target, OverwriteCondition condition)
    {
        if (condition == OverwriteCondition.Always)
            return true;

        try
        {
            var a = new FileInfo(source);
            var b = new FileInfo(target);

            return condition switch
            {
                OverwriteCondition.IfNewer => a.LastWriteTime > b.LastWriteTime,
                OverwriteCondition.IfNewerOrSizeDifferent => (a.LastWriteTime > b.LastWriteTime) || (a.Length != b.Length),
                OverwriteCondition.IfSizeDifferent => a.Length != b.Length,
                _ => false,
            };
        }
        catch
        {
            return true;
        }
    }

    //...

    bool ApplyTarget(bool log, ItemType type, string path)
    {
        var result = false;
        Try.Invoke(() =>
        {
            switch (type)
            {
                case ItemType.File:
                case ItemType.Folder:
                    result = ApplyAttributes(type, path, TargetFileAttributes, TargetFolderAttributes) && ApplyExtensions(path, TargetExtensions);
                    break;
            }
            result = false;
        }, e => Write(log, e));

        if (!result)
            Write(log, Messages.Skip.F(Source));

        return result;
    }

    bool ApplySource(bool log, ItemType type, string source, string target)
    {
        var result = false;
        Try.Invoke(() =>
        {
            switch (type)
            {
                case ItemType.File:
                    result = ApplyAttributes(type, source, SourceFileAttributes, SourceFolderAttributes) && ApplyExtensions(source, SourceExtensions) && ApplyOverwrite(source, target, TargetOverwriteFiles);
                    return;
                case ItemType.Folder:
                    result = ApplyAttributes(type, source, SourceFileAttributes, SourceFolderAttributes) && ApplyExtensions(source, SourceExtensions);
                    return;
            }
            result = false;
        }, e => Write(log, e));

        if (!result)
            Write(log, Messages.Skip.F(source));

        return result;
    }

    //...

    void OnTargetItemChanged(object sender, FileSystemEventArgs e)
    {
    }

    void OnTargetItemCreated(object sender, FileSystemEventArgs e)
    {
    }

    void OnTargetItemDeleted(object sender, FileSystemEventArgs e)
    {
    }

    void OnTargetItemRenamed(object sender, RenamedEventArgs e)
    {
    }

    //...

    void OnSourceItemChanged(object sender, FileSystemEventArgs e)
    {
        var itemType = Computer.GetType(e.FullPath);
        if (itemType == ItemType.File)
        {
            var dOld = e.FullPath.Replace(Source, Target);
            var dNew = dOld; //Converter.FormatUri(Target, dOld.Replace(Target, string.Empty), ItemType.File, action);

            if (ApplySource(true, itemType, e.FullPath, dNew))
            {
                if (!analyzeTask.Started)
                {
                    Size -= new FileInfo(dNew).Length;
                    Size += new FileInfo(e.FullPath).Length;
                }
                else _ = analyzeTask.Start();

                if (IsEnabled)
                {
                    if (!synchronizeTask.Started)
                        Queue.Add(OperationType.Create, e.FullPath, dNew, token => Try.Invoke(() => CreateFile(true, e.FullPath, dNew, token), e => Log.Write<CopyTask>(e)));
                }
            }
        }
        if (itemType == ItemType.Folder)
        {
            //To do: Handle changed attributes!
        }
    }

    void OnSourceItemCreated(object sender, FileSystemEventArgs e)
    {
        var itemType = Computer.GetType(e.FullPath);

        var oldTarget = e.FullPath.Replace(Source, Target);
        var newTarget = oldTarget; //Converter.FormatUri(Target, oldTarget.Replace(Target, string.Empty), itemType, action);

        if (ApplySource(true, itemType, e.FullPath, newTarget))
        {
            if (itemType == ItemType.File)
            {
                if (!analyzeTask.Started)
                {
                    Files++;
                    Size += new FileInfo(e.FullPath).Length;
                }
                else _ = analyzeTask.Start();

                if (IsEnabled)
                {
                    if (!synchronizeTask.Started)
                        Queue.Add(OperationType.Create, e.FullPath, newTarget, token => Try.Invoke(() => CreateFile(true, e.FullPath, newTarget, token), e => Log.Write<CopyTask>(e)));
                }
            }
            else if (itemType == ItemType.Folder)
            {
                if (IsEnabled)
                {
                    if (!synchronizeTask.Started)
                        Queue.Add(OperationType.Create, e.FullPath, newTarget, token => Try.Invoke(() => CreateFolder(true, newTarget), e => Log.Write<CopyTask>(e)));
                }
            }
        }
    }

    void OnSourceItemDeleted(object sender, FileSystemEventArgs e)
    {
        /*
        var head = Target;
        var tail = e.FullPath.Replace(Source, Target).Replace(Target, string.Empty);

        var filePath = Converter.FormatUri(head, tail, ItemType.File, action);
        var folderPath = Converter.FormatUri(head, tail, ItemType.Folder, action);

        ItemType? itemType = null;

        var result = Try.Invoke(() => itemType = Computer.GetType(filePath), e => Log.Write<CopyTask>(e));
        result = !result ? Try.Invoke(() => itemType = Computer.GetType(folderPath), e => Log.Write<CopyTask>(e)) : result;

        if (itemType == ItemType.File)
        {
            if (!analyzeSizeTask.Started)
            {
                Files--;
                Size -= new FileInfo(filePath).Length;
            }
            else _ = analyzeSizeTask.Start();

            if (enable)
            {
                if (!synchronizeTask.Started)
                    Queue.Add(Operation.Types.Delete, itemType.Value, filePath, string.Empty, token => Delete(true, filePath));
            }
        }
        else if (itemType == ItemType.Folder)
        {
            if (enable)
            {
                if (!synchronizeTask.Started)
                    Queue.Add(Operation.Types.Delete, itemType.Value, folderPath, string.Empty, token => Delete(true, folderPath));
            }
        }
        */
    }

    void OnSourceItemRenamed(object sender, RenamedEventArgs e)
    {
        /*
        if (!enable || synchronizeTask.Started)
            return;

        var source = e.FullPath;
        var target = e.FullPath.Replace(Source, Target);

        var oldPath = e.OldFullPath;
        var newPath = e.FullPath;

        string a = string.Empty, b = string.Empty;
        //We know this will always be in target scope
        a = oldPath.Replace(Source, Target);
        //We won't know that this will always be in target scope
        b = newPath.Replace(Source, Target);

        var aTail = a.Replace(Target, string.Empty);
        var bTail = b.Replace(Target, string.Empty);

        if (b.StartsWith(Target))
        {
            var itemType = Computer.GetType(newPath);
            if (itemType == ItemType.Folder)
            {
                var dA = Converter.FormatUri(Target, aTail, ItemType.Folder, action);
                var dB = Converter.FormatUri(Target, bTail, ItemType.Folder, action);
                Queue.Add(Operation.Types.Move, itemType, dA, dB, token => Try.Invoke(() => Folder.Long.Move(dA, dB), e => Log.Write<CopyTask>(e)));
            }
            else if (itemType == ItemType.File)
            {
                var dA = Converter.FormatUri(Target, aTail, ItemType.File, action);
                var dB = Converter.FormatUri(Target, bTail, ItemType.File, action);
                Queue.Add(Operation.Types.Move, itemType, dA, dB, token => Try.Invoke(() => Imagin.Core.Storage.File.Long.Move(dA, dB), e => Log.Write<CopyTask>(e)));
            }
        }
        //Delete object at path, a: Since the original object has moved outside scope of target folder, it must no longer be present there!
        else
        {
            var dA = Converter.FormatUri(Target, aTail, Computer.GetType(a), action);
            Queue.Add(Operation.Types.Delete, Computer.GetType(dA), dA, string.Empty, token => Try.Invoke(() => { /-Delete(dA)-/ }, e => Log.Write<CopyTask>(e)));
        }
        */
    }

    //...

    void AnalyzeSize(CancellationToken token)
    {
        uint files = 0;
        long size = 0;

        Browse(Source, token, (i, j) =>
        {
            Try.Invoke(() =>
            {
                var file = new Core.Storage.File(i);
                file.Refresh();

                files++;
                size += file.Size;
            });
        },
        null, null, null, false);
        Dispatch.Invoke(() =>
        {
            Files = files;
            Size = size;
        });
    }

    //...

    /*
    async Task TryCompressAsync()
    {
        await Task.Run(() =>
        {
            var outName = compressionOptions.OutputName;

            if (outName.IsNullOrEmpty())
                outName = Source.GetFileName();

            var outExtension = string.Empty;
            switch (compressionOptions.Type)
            {
                case CompressionFormat.BZip2:
                    outExtension = "tar.bz2";
                    break;
                case CompressionFormat.GZip:
                    outExtension = "tar.gz";
                    break;
                case CompressionFormat.Tar:
                    outExtension = "tar";
                    break;
                case CompressionFormat.Zip:
                    outExtension = "zip";
                    break;
            }

            var outPath = @"{0}\{1}.{2}".F(Target, outName, outExtension);

            try
            {
                var fileStream = Imagin.Core.Storage.File.Long.Create(outPath);

                var stream = default(Stream);
                switch (compressionOptions.Type)
                {
                    case CompressionFormat.BZip2:
                        stream = new BZip2OutputStream(fileStream);
                        break;
                    case CompressionFormat.GZip:
                        stream = new GZipOutputStream(fileStream);
                        break;
                    case CompressionFormat.Tar:
                        stream = new TarOutputStream(fileStream);
                        break;
                    case CompressionFormat.Zip:
                        stream = new ZipOutputStream(fileStream);

                        var zipStream = stream as ZipOutputStream;
                        zipStream.SetLevel(compressionOptions.Level);
                        zipStream.Password = compressionOptions.Password;
                        break;
                }

                switch (compressionOptions.Type)
                {
                    case CompressionFormat.BZip2:
                    case CompressionFormat.GZip:
                    case CompressionFormat.Tar:
                        var tarArchive = TarArchive.CreateOutputTarArchive(stream);

                        tarArchive.RootPath = Target.Replace('\\', '/');
                        if (tarArchive.RootPath.EndsWith("/"))
                            tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

                        var tarEntry = TarEntry.CreateEntryFromFile(Target);
                        tarArchive.WriteEntry(tarEntry, false);

                        CompressTar(tarArchive, Target, outPath);
                        break;
                    case CompressionFormat.Zip:
                        int folderOffset = Target.Length + (Target.EndsWith("\\") ? 0 : 1);
                        CompressZip(stream as ZipOutputStream, Target, folderOffset, outPath);
                        break;
                }

                switch (compressionOptions.Type)
                {
                    case CompressionFormat.BZip2:
                        var bZip2Stream = stream as BZip2OutputStream;
                        bZip2Stream.IsStreamOwner = true;
                        bZip2Stream.Close();
                        break;
                    case CompressionFormat.GZip:
                        var gZipStream = stream as GZipOutputStream;
                        gZipStream.IsStreamOwner = true;
                        gZipStream.Close();
                        break;
                    case CompressionFormat.Tar:
                        var tarStream = stream as TarOutputStream;
                        tarStream.IsStreamOwner = true;
                        tarStream.Close();
                        break;
                    case CompressionFormat.Zip:
                        var zipStream = stream as ZipOutputStream;
                        zipStream.IsStreamOwner = true;
                        zipStream.Close();
                        break;
                }

                fileStream?.Close();
                fileStream?.Dispose();
            }
            catch
            {

            }
        });
    }

    void CompressFile(ZipOutputStream ZipStream, string Path, int FolderOffset)
    {
        var FileInfo = new FileInfo(Path);

        //Makes the name in zip based on the folder
        var EntryName = Path.Substring(FolderOffset);
        //Removes drive from name and fixes slash direction 
        EntryName = ZipEntry.CleanName(EntryName);

        var NewEntry = new ZipEntry(EntryName);
        NewEntry.DateTime = FileInfo.LastWriteTime;
        NewEntry.Size = FileInfo.Length;

        ZipStream.PutNextEntry(NewEntry);

        var Buffer = new byte[4096];
        using (var StreamReader = Imagin.Core.Storage.File.Long.OpenRead(Path))
            StreamUtils.Copy(StreamReader, ZipStream, Buffer);

        ZipStream.CloseEntry();
    }

    void CompressFile(TarArchive Archive, string Path)
    {
        var tarEntry = TarEntry.CreateEntryFromFile(Path);
        Archive.WriteEntry(tarEntry, true);
    }

    void CompressTar(TarArchive archive, string folderPath, string outPath)
    {
        var Files = Enumerable.Empty<string>();
        try
        {
            Files = Folder.GetFiles(folderPath);
        }
        catch (Exception e)
        {
            Write(e.Message, LogEntryType.Error);
        }

        foreach (var i in Files)
        {
            if (i != outPath)
                CompressFile(archive, i);
        }

        var Folders = Enumerable.Empty<string>();
        try
        {
            Folders = Folder.GetFolders(Target);
        }
        catch (Exception e)
        {
            Write(e.Message, LogEntryType.Error);
        }

        foreach (var i in Folders)
            CompressTar(archive, i, outPath);
    }

    void CompressZip(ZipOutputStream stream, string folderPath, int folderOffset, string outPath)
    {
        var Files = Enumerable.Empty<string>();
        try
        {
            Files = Folder.GetFiles(folderPath);
        }
        catch (Exception e)
        {
            Write(e.Message, LogEntryType.Error);
        }

        foreach (var i in Files)
        {
            if (i != outPath)
            {
                try
                {
                    CompressFile(stream, i, folderOffset);
                    Imagin.Core.Storage.File.Long.Delete(i);
                }
                catch (Exception e)
                {
                    Write(e.Message, LogEntryType.Error);
                }
            }
        }

        var Folders = Enumerable.Empty<string>();
        try
        {
            Folders = Folder.GetFolders(Target);
        }
        catch (Exception e)
        {
            Write(e.Message, LogEntryType.Error);
        }

        foreach (var i in Folders)
        {
            CompressZip(stream, i, folderOffset, outPath);

            try
            {
                Folder.Delete(i);
            }
            catch (Exception e)
            {
                Write(e.Message, LogEntryType.Error);
            }
        }
    }
    */

    //...

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        analyzeTask = new Method(AnalyzeSize, true);
        _ = analyzeTask.Start();
    }

    //...

    protected override void OnMonitoring()
    {
        Try.Invoke(() => SourceMonitor.Enable(Source), e => Write(LogErrors, new Error(e)));
        Try.Invoke(() => TargetMonitor.Enable(Target), e => Write(LogErrors, new Error(e)));
    }

    async protected override Task Run(CancellationToken token)
    {
        if (Current.Get<IDockMainViewModel>().Panels.FirstOrDefault<CopyPanel>().ShowWarningBeforeEnablingTask)
        {
            //if (Dialog.ShowWarning(nameof(IsEnabled), new Warning($"Are you sure you want to synchronize '{Source}' with '{target}'?"), Buttons.ContinueCancel) != 0)
                //return;
        }

        bool result = Try.Invoke(() =>
        {
            if (Target == Source || Target.StartsWith($@"{Source}\"))
                throw new InvalidDataException($"'{nameof(Target)}' cannot equal or derive from '{nameof(Source)}'.");

            if (!Folder.Long.Exists(Target))
                throw new DirectoryNotFoundException($"'{nameof(Target)}' does not exist.");

            if (!TargetValidator.Validate(ItemType.Folder, Target))
                throw new InvalidDataException($"'{nameof(Target)}' must start with (but not equal) '{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}'.");

            //Warn if source/target is

            //- drive
            foreach (var i in Computer.Drives)
            {
                if (Source == i.Name)
                    Write(new Warning($"Source folder '{Source}' is drive path."));

                if (Target == i.Name)
                    Write(new Warning($"Target folder '{Target}' is drive path."));
            }

            //- folder located outside local user folder
            if (!Source.StartsWith(Environment.SpecialFolder.UserProfile.GetPath()))
                Write(new Warning($"Source folder '{Source}' is located outside of '{Environment.SpecialFolder.UserProfile.GetPath()}'."));

            if (!Target.StartsWith(Environment.SpecialFolder.UserProfile.GetPath()))
                Write(new Warning($"Target folder '{Target}' is located outside of '{Environment.SpecialFolder.UserProfile.GetPath()}'."));

        }, e => Write(new Error(e)));

        if (!result)
        {
            Disable();
            return;
        }

        Write(Messages.Synchronize.F(Source));
        await Task.Run(() => Browse
        (
            Source, token,
            (i, j) => Queue.Add(OperationType.Create, i, j, k => CreateFile(true, i, j, k)),
            (i, j) => Queue.Add(OperationType.Create, i, j, k => CreateFolder(true, j)),
            i => Queue.Add(OperationType.Delete, i, string.Empty, k => Delete(true, i)),
            i => Queue.Add(OperationType.Delete, i, string.Empty, k => Delete(true, i)),
            true
        ), token);
    }

    protected override void Disable()
    {
        if (Current.Get<IDockMainViewModel>().Panels.FirstOrDefault<CopyPanel>().ShowWarningBeforeDisablingTask)
        {
            //if (Dialog.ShowWarning(nameof(Disable), new Warning($"Are you sure you want to stop synchronizing '{Source}' with '{target}'?"), Buttons.ContinueCancel) != 0)
                //return;
        }

        Queue.Clear();
        base.Disable();
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Category):
                Update(() => CategoryName);
                break;

            case nameof(Direction):
                Update(() => DirectionDescription);
                break;

            case nameof(Target):
            case nameof(TargetExtensions):
            case nameof(TargetFileAttributes):
            case nameof(TargetFolderAttributes):

            case nameof(SourceOverwriteFiles):

            case nameof(Source):
            case nameof(SourceExtensions):
            case nameof(SourceFileAttributes):
            case nameof(SourceFolderAttributes):
                _ = analyzeTask?.Start(); break;
        }
    }

    public override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);
        if (e.PropertyName == nameof(Source))
            e.NewValue = e.NewValue?.ToString().TrimEnd('\\');
    }

    //...

    public override void Subscribe()
    {
        base.Subscribe();
        if (SourceMonitor is Watcher i)
        {
            i.Changed -= OnSourceItemChanged; i.Created -= OnSourceItemCreated; i.Deleted -= OnSourceItemDeleted; i.Renamed -= OnSourceItemRenamed;
            i.Changed += OnSourceItemChanged; i.Created += OnSourceItemCreated; i.Deleted += OnSourceItemDeleted; i.Renamed += OnSourceItemRenamed;
        }
        if (TargetMonitor is Watcher j)
        {
            j.Changed -= OnTargetItemChanged; j.Created -= OnTargetItemCreated; j.Deleted -= OnTargetItemDeleted; j.Renamed -= OnTargetItemRenamed;
            j.Changed += OnTargetItemChanged; j.Created += OnTargetItemCreated; j.Deleted += OnTargetItemDeleted; j.Renamed += OnTargetItemRenamed;
        }
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        if (SourceMonitor is Watcher i)
        {
            i.Changed -= OnSourceItemChanged; i.Created -= OnSourceItemCreated; i.Deleted -= OnSourceItemDeleted; i.Renamed -= OnSourceItemRenamed;
        }
        if (TargetMonitor is Watcher j)
        {
            j.Changed -= OnSourceItemChanged; j.Created -= OnSourceItemCreated; j.Deleted -= OnSourceItemDeleted; j.Renamed -= OnSourceItemRenamed;
        }
    }

    #endregion
}

#endregion

///

#region MonitorProfile

[Name("Monitor"), Serializable, XmlType(TypeName = "Monitor")]
public class MonitorProfile : Base
{
    public bool Changed { get => Get(true); set => Set(value); }

    public bool Created { get => Get(true); set => Set(value); }

    public bool Deleted { get => Get(true); set => Set(value); }

    public bool Renamed { get => Get(true); set => Set(value); }

    public MonitorProfile() : base() { }
}

#endregion

#region TargetValidator

/// <summary>
/// A target path is valid if:
/// 1a) it resides on the main drive, 1b) starts with the current user's folder path, but doesn't equal it (for security reasons), and 1c) exists 
/// or 
/// 2a) doesn't reside on the main drive and 2b) exists.
/// </summary>
public class TargetValidator : LocalValidator
{
    public override bool Validate(ItemType target, string path)
    {
        var result = base.Validate(target, path);
        var a = Environment.GetFolderPath(Environment.SpecialFolder.System);
        if (path.StartsWith(Path.GetPathRoot(a)))
        {
            var b = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return path.StartsWith(b) && !path.Equals(b) && result;
        }
        return result;
    }
}

#endregion

///

#region TaskPanel<T>

[Serializable]
public abstract class TaskPanel<T> : DataPanel where T : BaseTask, new()
{
    enum Category { Disable, Enable, Pause, Remove, Warnings }

    [Description("The exit method to perform when all tasks are complete.")]
    [Float(Float.Above), Option]
    public ExitMethod AutoExit { get => Get(ExitMethod.None); set => Set(value); }

    [Category(nameof(Category.Warnings)), Name("Before removing task"), Option]
    public bool ShowWarningBeforeRemovingTask { get => Get(true); set => Set(value); }

    [Category(nameof(Category.Warnings)), Name("Before removing all tasks"), Option]
    public bool ShowWarningBeforeRemovingAllTasks { get => Get(true); set => Set(value); }

    [Category(nameof(Category.Warnings)), Name("Before disabling task"), Option]
    public bool ShowWarningBeforeDisablingTask { get => Get(false); set => Set(value); }

    [Category(nameof(Category.Warnings)), Name("Before enabling task"), Option]
    public bool ShowWarningBeforeEnablingTask { get => Get(true); set => Set(value); }

    [Hide]
    public ObservableCollection<T> Tasks { get => Get<ObservableCollection<T>>(); set => Set(value); }

    //...

    public TaskPanel() : base() => Tasks = new();

    //...

    void OnTaskChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BaseTask.IsActive))
        {
            if (sender is BaseTask i)
                IsBusy = i.IsActive;
        }
    }

    protected override void OnItemAdded(object input)
    {
        base.OnItemRemoved(input);
        if (input is T i)
        {
            i.Unsubscribe(); i.Subscribe();
            i.PropertyChanged -= OnTaskChanged; i.PropertyChanged += OnTaskChanged;
        }
    }

    protected override void OnItemRemoved(object input)
    {
        base.OnItemRemoved(input);
        if (input is T i)
        {
            i.Unsubscribe();
            i.PropertyChanged -= OnTaskChanged;
        }
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(SelectedItem):
                if (SelectedItem is BaseTask task)
                {
                    Get<LogPanel>().If(i => i.Data = task.Log);
                }
                else Get<LogPanel>().If(i => i.Data = Current.Get<ILog>() as Core.Collections.ICollectionChanged);
                Get<PropertiesPanel>().If(i => i.Source = SelectedItem);
                break;

            case nameof(Tasks):
                Data = Tasks; break;
        }
    }

    //...

    public override void Subscribe()
    {
        foreach (var i in Tasks)
        {
            i.Unsubscribe(); i.Subscribe();
            i.PropertyChanged -= OnTaskChanged; i.PropertyChanged += OnTaskChanged;
        }
    }

    public override void Unsubscribe()
    {
        foreach (var i in Tasks)
        {
            i.Unsubscribe();
            i.PropertyChanged -= OnTaskChanged;
        }
    }

    //...

    ICommand addCommand;
    [Float(Float.Above), Name("Add"), Image(SmallImages.Plus)]
    [Index(0), Header]
    public ICommand AddCommand => addCommand ??= new RelayCommand(() =>
    {
        Tasks.Add(new T());
        Current.Get<BaseApplication>().Notifications.Add(new Notification("You added a task.", new Message("A task was added."), TimeSpan.Zero));
    },
    () => true);

    ICommand removeCommand;
    [Category(nameof(Category.Remove)), Name("Remove"), Image(SmallImages.Minus)]
    [Index(0), Header]
    new public ICommand RemoveCommand => removeCommand ??= new RelayCommand(() =>
    {
        /*
        if (!ShowWarningBeforeRemovingTask || Dialog.ShowWarning("Remove", new Warning(SelectedItems.Count == 1 ? $"Are you sure you want to remove this?" : $"Are you sure you want to remove the {SelectedItems.Count} selected tasks? This can't be undone."), Buttons.YesNo) == 0)
        {
            for (var i = Tasks.Count - 1; i >= 0; i--)
            {
                if (SelectedItems.Contains(Tasks[i]))
                    Tasks.RemoveAt(i);
            }
        }
        */
    },
    () => SelectedItems?.Count<CopyTask>(i => !i.IsEnabled) > 0);

    ICommand removeAllCommand;
    [Category(nameof(Category.Remove)), Name("Remove all"), Image(SmallImages.MinusRound)]
    [Index(1), Header]
    public ICommand RemoveAllCommand => removeAllCommand ??= new RelayCommand(() =>
    {
        /*
        if (!ShowWarningBeforeRemovingAllTasks || Dialog.ShowWarning("Remove all", new Warning($"Are you sure you want to remove everything? This can't be undone."), Buttons.YesNo) == 0)
        {
            for (var i = Tasks.Count - 1; i >= 0; i--)
                Tasks.RemoveAt(i);
        }
        */
    },
    () => Tasks.Count > 0);

    [Hide]
    new public ICommand ClearCommand => base.ClearCommand;

    ICommand disableCommand;
    [Category(nameof(Category.Disable)), Name("Disable"), Image(SmallImages.Stop)]
    [Index(0), Header]
    public ICommand DisableCommand => disableCommand ??= new RelayCommand(() => { });

    ICommand disableAllCommand;
    [Category(nameof(Category.Disable)), Name("DisableAll"), Image(SmallImages.StopRound)]
    [Index(1), Header]
    public ICommand DisableAllCommand => disableAllCommand ??= new RelayCommand(() => Tasks.ForEach(j => j.IsEnabled = false), () => Tasks.Count > 0);

    ICommand enableCommand;
    [Category(nameof(Category.Enable)), Name("Enable"), Image(SmallImages.Play)]
    [Index(0), Header]
    public ICommand EnableCommand => enableCommand ??= new RelayCommand(() => { });

    ICommand enableAllCommand;
    [Category(nameof(Category.Enable)), Name("EnableAll"), Image(SmallImages.PlayRound)]
    [Index(1), Header]
    public ICommand EnableAllCommand => enableAllCommand ??= new RelayCommand(() => Tasks.ForEach(j => j.IsEnabled = true), () => Tasks.Count > 0);
}

#endregion

#region CopyPanel

[Description("Sync file contents of multiple folders."), Name(DefaultTitle), Serializable]
public class CopyPanel : TaskPanel<CopyTask>
{
    public static readonly ResourceKey TemplateKey = new();

    enum Category { Category }

    public const string DefaultTitle = "Copy";

    [Category(Category.Category), Option]
    [CollectionStyle(AddType = typeof(string))]
    public StringCollection Categories { get => Get(new StringCollection()); set => Set(value); }

    [Hide]
    public override IList GroupNames => new Core.Collections.ObjectModel.StringCollection()
    {
        nameof(CopyTask.CategoryName),
        nameof(CopyTask.Source),
        nameof(CopyTask.Target),
        nameof(CopyTask.LastActive),
        nameof(CopyTask.Status)
    };

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Copy);

    [Hide]
    public override IList SortNames => new Core.Collections.ObjectModel.StringCollection()
    {
        nameof(CopyTask.Source),
        nameof(CopyTask.Target),
        nameof(CopyTask.LastActive),
        nameof(CopyTask.Status)
    };

    [Hide]
    public override string TitleKey => DefaultTitle;

    public CopyPanel() : base() { }
}

#endregion