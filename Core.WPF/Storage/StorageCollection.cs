﻿using Imagin.Core.Analytics;
using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Imagin.Core.Storage;

public abstract class StorageCollection<T> : ConcurrentCollection<T>, ISubscribe, IUnsubscribe
{
    enum Category { Import }

    public static Watcher DefaultWatcher 
        => new(NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Security);

    class Automator : List<DispatcherOperation>
    {
        void OnCompleted(object sender, EventArgs e)
        {
            var i = (DispatcherOperation)sender;
            i.Completed -= OnCompleted;
            Remove(i);
        }

        new public void Add(DispatcherOperation i)
        {
            base.Add(i);
            i.Completed += OnCompleted;
        }

        new public void Clear()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                this[i].Completed -= OnCompleted;
                this[i].Abort();
                RemoveAt(i);
            }
        }
    }

    public delegate void StorageCollectionEventHandler(StorageCollection<T> sender);

    public event StorageCollectionEventHandler Refreshing;

    public event StorageCollectionEventHandler Refreshed;

    #region Properties

    readonly Automator automator = new();

    Watcher watcher = null;

    ///

    readonly Method<string> refreshTask;

    ///

    protected bool subscribed = false;

    [Hide]
    public virtual string ItemName => "Item";

    [Hide]
    public bool IsRefreshing { get => this.Get(false); private set => this.Set(value); }

    Filter filter = Filter.Default;
    [Hide]
    public Filter Filter
    {
        get => filter;
        set => filter = value ?? Filter.Default;
    }

    [Hide]
    public string Path { get => this.Get(""); set => this.Set(value); }

    [Hide]
    public double Progress { get => this.Get(.0); set => this.Set(value); }

    #endregion

    #region StorageCollection

    public StorageCollection() : this(string.Empty, null) { }

    public StorageCollection(Filter filter) : this(string.Empty, filter) { }

    public StorageCollection(string path, Filter filter) : base()
    {
        refreshTask
            = new(null, RefreshAsync, true);

        Path
            = path;
        Filter
            = filter;
    }

    ///

    protected abstract T this[string path] { get; }

    #endregion

    #region Methods

    IEnumerable<T> Query(string path, Filter filter)
    {
        if (filter.Types.HasFlag(ItemType.Drive))
        {
            if (path.NullOrEmpty() || path == StoragePath.Root)
            {
                Try.Invoke(out IEnumerable<DriveInfo> drives, () => Computer.Drives);
                if (drives is not null)
                {
                    foreach (var i in drives)
                        yield return ToDrive(i);
                }
            }
        }
        if (filter.Types.HasFlag(ItemType.Folder))
        {
            Try.Invoke(out IEnumerable<string> folders, () => Folder.Long.GetFolders(path).Where(j => j != ".."));
            if (folders is not null)
            {
                foreach (var i in folders)
                    yield return ToFolder(i);
            }
        }
        if (filter.Types.HasFlag(ItemType.File))
        {
            Try.Invoke(out IEnumerable<string> files, () => Folder.Long.GetFiles(path));
            if (files is not null)
            {
                foreach (var i in files)
                {
                    if (filter.Evaluate(i, ItemType.File))
                        yield return ToFile(i);
                }
            }
        }
    }

    ///

    void OnItemChanged(object sender, FileSystemEventArgs e)
    {
        var item = this[e.FullPath];
        if (item != null)
        {
            var changedProperty = GetChangedProperty(item);
            OnItemChanged(item, changedProperty);
        }
    }

    void OnItemCreated(object sender, FileSystemEventArgs e)
    {
        T i = default;

        var a = File.Long.Exists(e.FullPath);
        var b = Filter?.Evaluate(e.FullPath, ItemType.File) != false;

        if (a && b)
        {
            i = ToFile(e.FullPath);
        }
        else
        {
            a = Folder.Long.Exists(e.FullPath);
            b = Filter?.Evaluate(e.FullPath, ItemType.Folder) != false;

            if (a && b)
                i = ToFolder(e.FullPath);
        }

        if (i != null)
        {
            OnItemCreated(i);
            Add(i);
        }
    }

    void OnItemDeleted(object sender, FileSystemEventArgs e)
    {
        var item = this[e.FullPath];
        if (item != null)
        {
            OnItemDeleted(item);
            Remove(item);
        }
    }

    void OnItemRenamed(object sender, RenamedEventArgs e) => OnItemRenamed(e);

    ///

    protected abstract ItemProperty GetChangedProperty(T input);

    ///

    protected abstract T ToDrive(DriveInfo input);

    protected abstract T ToFile(string input);

    protected abstract T ToFolder(string input);

    protected virtual void OnExported(string path) { }

    ///

    protected virtual void OnItemChanged(T i, ItemProperty property) { }

    protected virtual void OnItemCreated(T i) { }

    protected virtual void OnItemDeleted(T i) { }

    protected virtual void OnItemRenamed(RenamedEventArgs e) { }

    ///

    protected virtual void OnRefreshing() => Refreshing?.Invoke(this);

    protected virtual void OnRefreshed() 
    {
        if (subscribed)
            watcher.Enable(Path);

        Refreshed?.Invoke(this);
    }

    ///

    public override void Clear()
    {
        refreshTask.Cancel();
        InternalClear();
    }

    void InternalClear()
    {
        automator
            .Clear();
        base
            .Clear();
    }

    async Task InternalClearAsync()
    {
        InternalClear();
        await While.InvokeAsync(() => automator, i => 0 < i.Count && 0 < Count);
    }

    ///

    void Refresh(string path, Filter filter, CancellationToken token)
    {
        IEnumerable<T> items = null;
        Try.Invoke(() => items = Query(path, filter), e => Log.Write<ItemCollection>(new Error(e)));

        if (items is not null)
        {
            foreach (var i in items)
            {
                if (token.IsCancellationRequested)
                    return;

                automator.Add(Dispatch.InvokeReturn(() => Add(i)));
            }
        }
    }

    ///

    async Task RefreshAsync(string path, CancellationToken token)
    {
        IsRefreshing = true;

        Path = path;
        await InternalClearAsync();

        OnRefreshing();
        var filter = Filter;
        await Task.Run(() => Refresh(path, filter, token));
        OnRefreshed();

        IsRefreshing = false;
    }

    void RefreshSync(string path)
    {
        IsRefreshing = true;

        Path = path;
        InternalClear();

        OnRefreshing();
        Refresh(path, Filter, new(false));
        OnRefreshed();

        IsRefreshing = false;
    }

    ///

    public void Refresh() => Refresh(Path);

    public void Refresh(string path) => RefreshSync(path);

    ///

    public async Task RefreshAsync() => await RefreshAsync(Path);

    public async Task RefreshAsync(string path) => await refreshTask.Start(path);

    ///

    public virtual void Subscribe()
    {
        if (!subscribed)
        {
            subscribed = true;

            watcher = DefaultWatcher;
            watcher.Subscribe();

            watcher.Changed += OnItemChanged;
            watcher.Created += OnItemCreated;
            watcher.Deleted += OnItemDeleted;
            watcher.Renamed += OnItemRenamed;

            if (!IsRefreshing)
                watcher.Enable(Path);
        }
    }

    public virtual void Unsubscribe()
    {
        if (watcher != null)
        {
            watcher.Unsubscribe();

            watcher.Changed -= OnItemChanged;
            watcher.Created -= OnItemCreated;
            watcher.Deleted -= OnItemDeleted;
            watcher.Renamed -= OnItemRenamed;

            watcher.Disable();
            watcher.Dispose();

            watcher = null;
        }
        subscribed = false;
    }

    ///

    [field: NonSerialized]
    ICommand exportCommand;
    [Float(Float.Above), Category(nameof(Category.Import)), Image(SmallImages.Export), Name("Export")]
    public virtual ICommand ExportCommand => exportCommand ??= new RelayCommand(() =>
    {
        if (StorageDialog.Show(out string destination, $"Export {ItemName.ToLower()}(s)", StorageDialogMode.OpenFolder, null, Path))
        {
            var j = 0;
            foreach (var i in this)
            {
                string source = "";
                if (i is string path)
                {
                    source = path;
                }
                else if (i is File file)
                    source = file.Path;

                if (Try.Invoke(() => System.IO.File.Copy(source, $@"{destination}\{System.IO.Path.GetFileName(source)}"), e => Log.Write<StorageCollection<T>>(e)))
                    j++;
            }

            if (j > 0)
            {
                Notifications.Add($"Export {ItemName.ToLower()}", new Success($"Exported {ItemName.ToLower()}!"));
            }
        }
    });

    [field: NonSerialized]
    ICommand importCommand;
    [Float(Float.Above), Category(nameof(Category.Import)), Image(SmallImages.Import), Name("Import")]
    public virtual ICommand ImportCommand => importCommand ??= new RelayCommand(() =>
    {
        if (StorageDialog.Show(out string[] paths, $"Import {ItemName.ToLower()}(s)", StorageDialogMode.OpenFile, Filter?.Extensions, Path))
        {
            if (paths?.Length > 0)
            {
                var j = 0;
                foreach (var i in paths)
                {
                    if (Try.Invoke(() => System.IO.File.Copy(i, $@"{Path}\{System.IO.Path.GetFileName(i)}"), e => Log.Write<StorageCollection<T>>(e)))
                        j++;
                }

                if (j > 0)
                {
                    Notifications.Add($"Import {ItemName.ToLower()}", new Success($"Imported {ItemName.ToLower()}!"));
                }
            }
        }
    },
    () => true);

    ICommand refreshCommand;
    [Hide]
    public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(Refresh);

    ICommand refreshAsyncCommand;
    [Hide]
    public ICommand RefreshAsyncCommand => refreshAsyncCommand ??= new RelayCommand(async () => await RefreshAsync());

    ICommand renameCommand;
    [Float(Float.Above), Image(SmallImages.Rename), Name("Rename")]
    public ICommand RenameCommand => renameCommand ??= new RelayCommand<string>(i =>
    {
        var x = new Namable(System.IO.Path.GetFileNameWithoutExtension(i));
        Dialog.ShowObject($"Rename", x, Resource.GetImageUri(SmallImages.Rename), j =>
        {
            if (j == 0)
            {
                var newFilePath = $@"{System.IO.Path.GetDirectoryName(i)}\{x.Name}{System.IO.Path.GetExtension(i)}";
                Try.Invoke(() => File.Long.Move(i, newFilePath), e => Log.Write<StorageCollection<T>>(e));
            }
        }, 
        Buttons.SaveCancel);
    },
    i => Try.Return(() => File.Long.Exists(i)));

    ICommand deleteCommand;
    [Float(Float.Above), Image(SmallImages.Trash), Name("Delete")]
    public ICommand DeleteCommand => deleteCommand ??= new RelayCommand<string>(i => Dialog.ShowMessage("Delete", new($"Are you sure you want to delete '{i}'?"), j => { if (j == 0) { Computer.Recycle(i); } }, Buttons.YesNo),
    i => Try.Return(() => File.Long.Exists(i)));

    #endregion
}