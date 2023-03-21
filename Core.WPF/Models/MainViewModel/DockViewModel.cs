using ImageMagick;
using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Serialization;
using Imagin.Core.Storage;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imagin.Core.Models;

#region DockViewModel

public class DockViewModel : ViewModel, IDockViewModel
{
    public event ChangedEventHandler<Content> ActiveContentChanged;
    event ChangedEventHandler<Content> IDockViewModel.ActiveContentChanged { add => ActiveContentChanged += value; remove => ActiveContentChanged -= value; }

    public event ChangedEventHandler<Document> ActiveDocumentChanged;
    event ChangedEventHandler<Document> IDockViewModel.ActiveDocumentChanged { add => ActiveDocumentChanged += value; remove => ActiveDocumentChanged -= value; }

    public event ChangedEventHandler<Panel> ActivePanelChanged;
    event ChangedEventHandler<Panel> IDockViewModel.ActivePanelChanged { add => ActivePanelChanged += value; remove => ActivePanelChanged -= value; }

    public event DefaultEventHandler<Document> DocumentAdded;
    event DefaultEventHandler<Document> IDockViewModel.DocumentAdded { add => DocumentAdded += value; remove => DocumentAdded -= value; }

    public event DefaultEventHandler<Document> DocumentRemoved;
    event DefaultEventHandler<Document> IDockViewModel.DocumentRemoved { add => DocumentRemoved += value; remove => DocumentRemoved -= value; }

    ///

    [Hide]
    public Content ActiveContent
    {
        get => Get<Content>();
        set
        {
            var oldValue = Get<Content>();

            if (Set(value))
                OnActiveContentChanged(oldValue, value);
        }
    }

    [Hide]
    public Document ActiveDocument
    {
        get => Get<Document>();
        set
        {
            var oldValue = Get<Document>();

            if (Set(value))
                OnActiveDocumentChanged(oldValue, value);
        }
    }

    [Hide]
    public Panel ActivePanel
    {
        get => Get<Panel>();
        set
        {
            var oldValue = Get<Panel>();

            if (Set(value))
                OnActivePanelChanged(oldValue, value);
        }
    }

    ///

    readonly DocumentCollection documents = new();
    [Hide]
    public DocumentCollection Documents => documents;

    readonly PanelCollection panels;
    [Hide]
    public PanelCollection Panels => panels;

    ///

    public readonly IDockViewOptions Options;

    ///

    public DockViewModel(IDockViewOptions options) : base()
    {
        Options = options;

        panels = new(this);
        documents.CollectionChanged += OnDocumentsChanged;

        if (options.RememberedDocuments.Count > 0)
        {
            options.RememberedDocuments.ForEach(Documents.Add);
            options.RememberedDocuments.Clear();
        }

        foreach (var i in Panels)
        {
            if (options.PanelOptions.ContainsKey(i.Name))
                options.PanelOptions[i.Name].Load(i);
        }
        options.PanelOptions.Clear();
    }

    ///

    void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.OldItems?.ForEach<Document>(i => { OnDocumentAdded(i); i.Modified -= OnDocumentModified; i.Modified += OnDocumentModified; });
                break;

            case NotifyCollectionChangedAction.Remove:
                e.NewItems?.ForEach<Document>(i => { OnDocumentRemoved(i); i.Modified -= OnDocumentModified; });
                break;
        }
    }

    void OnDocumentModified(object sender, ModifiedEventArgs e)
    {
        if (Options.AutoSaveDocuments)
            sender.As<Document>().Save();
    }

    ///

    protected virtual void OnActiveContentChanged(Content oldValue, Content newValue)
        => ActiveContentChanged?.Invoke(this, new(oldValue, newValue));

    protected virtual void OnActiveDocumentChanged(Document oldValue, Document newValue)
        => ActiveDocumentChanged?.Invoke(this, new(oldValue, newValue));

    protected virtual void OnActivePanelChanged(Panel oldValue, Panel newValue)
        => ActivePanelChanged?.Invoke(this, new(oldValue, newValue));

    protected virtual void OnDocumentAdded(Document document) => DocumentAdded?.Invoke(this, new(document));

    protected virtual void OnDocumentRemoved(Document document) => DocumentRemoved?.Invoke(this, new(document));

    ///

    public virtual void OnLoaded(IList<string> arguments) { }

    public virtual void OnReloaded(IList<string> arguments) { }

    ///

    ICommand closeCommand;
    [Hide]
    public ICommand CloseCommand => closeCommand ??= new RelayCommand(() => Documents.Remove(ActiveDocument), () => ActiveDocument != null);

    ICommand closeAllCommand;
    [Hide]
    public ICommand CloseAllCommand => closeAllCommand ??= new RelayCommand(Documents.Clear, () => Documents.Count > 0);

    ICommand closeAllButThisCommand;
    [Hide]
    public ICommand CloseAllButThisCommand => closeAllButThisCommand ??= new RelayCommand(() =>
    {
        for (var j = Documents.Count - 1; j >= 0; j--)
        {
            if (!ActiveDocument.Equals(Documents[j]))
                Documents.RemoveAt(j);
        }
    },
    () => Documents.Count > 0 && ActiveDocument != null);

    ICommand showAllCommand;
    [Hide]
    public ICommand ShowAllCommand => showAllCommand ??= new RelayCommand(() => Panels.ForEach(i => i.IsVisible = true), () => Panels.Contains(i => !i.IsVisible));

    ICommand hideCommand;
    [Hide]
    public ICommand HideCommand => hideCommand ??= new RelayCommand(() => ActivePanel.IsVisible = false, () => ActivePanel?.IsVisible == true);

    ICommand hideAllCommand;
    [Hide]
    public ICommand HideAllCommand => hideAllCommand ??= new RelayCommand(() => Panels.ForEach(i => i.IsVisible = false), () => Panels.Contains(i => i.IsVisible));

    ICommand unpinAllCommand;
    [Hide]
    public ICommand UnpinAllCommand => unpinAllCommand ??= new RelayCommand(() => { }, () => true);

    ICommand pinCommand;
    [Hide]
    public ICommand PinCommand => pinCommand ??= new RelayCommand(() => { }, () => ActivePanel != null);

    ICommand pinAllCommand;
    [Hide]
    public ICommand PinAllCommand => pinAllCommand ??= new RelayCommand(() => { }, () => true);

    ICommand floatCommand;
    [Hide]
    public ICommand FloatCommand => floatCommand ??= new RelayCommand(() => { }, () => ActiveContent != null);

    ICommand floatAllDocumentsCommand;
    [Hide]
    public ICommand FloatAllDocumentsCommand => floatAllDocumentsCommand ??= new RelayCommand(() => { }, () => true);

    ICommand floatAllPanelsCommand;
    [Hide]
    public ICommand FloatAllPanelsCommand => floatAllPanelsCommand ??= new RelayCommand(() => { }, () => true);

    ICommand floatPanelCommand;
    [Hide]
    public ICommand FloatPanelCommand => floatPanelCommand ??= new RelayCommand(() => { }, () => true);

    ICommand dockDocumentCommand;
    [Hide]
    public ICommand DockDocumentCommand => dockDocumentCommand ??= new RelayCommand(() => { }, () => true);

    ICommand dockAllDocumentsCommand;
    [Hide]
    public ICommand DockAllDocumentsCommand => dockAllDocumentsCommand ??= new RelayCommand(() => { }, () => true);

    ICommand dockPanelCommand;
    [Hide]
    public ICommand DockPanelCommand => dockPanelCommand ??= new RelayCommand(() => { }, () => true);

    ICommand dockAllPanelsCommand;
    [Hide]
    public ICommand DockAllPanelsCommand => dockAllPanelsCommand ??= new RelayCommand(() => { }, () => true);

    ICommand restoreAllCommand;
    [Hide]
    public ICommand RestoreAllCommand => restoreAllCommand ??= new RelayCommand(() => Documents.ForEach(i => i.IsMinimized = false), () => Documents.Contains(i => i.IsMinimized));

    ICommand minimizeCommand;
    [Hide]
    public ICommand MinimizeCommand => minimizeCommand ??= new RelayCommand(() => ActiveDocument.IsMinimized = true, () => ActiveDocument?.IsMinimized == false);

    ICommand minimizeAllCommand;
    [Hide]
    public ICommand MinimizeAllCommand => minimizeAllCommand ??= new RelayCommand(() => Documents.ForEach(i => i.IsMinimized = true), () => Documents.Contains(i => !i.IsMinimized));

    ICommand newDocumentHorizontalGroupCommand;
    [Hide]
    public ICommand NewDocumentHorizontalGroupCommand => newDocumentHorizontalGroupCommand ??= new RelayCommand(() => { });

    ICommand newDocumentVerticalGroupCommand;
    [Hide]
    public ICommand NewDocumentVerticalGroupCommand => newDocumentVerticalGroupCommand ??= new RelayCommand(() => { });

    ICommand newPanelHorizontalGroupCommand;
    [Hide]
    public ICommand NewPanelHorizontalGroupCommand => newPanelHorizontalGroupCommand ??= new RelayCommand(() => { });

    ICommand newPanelVerticalGroupCommand;
    [Hide]
    public ICommand NewPanelVerticalGroupCommand => newPanelVerticalGroupCommand ??= new RelayCommand(() => { });
}

#endregion

#region FileDockViewModel : DockViewModel

public abstract class FileDockViewModel : DockViewModel
{
    public const string OpenTitle = "Open";

    new public IFileDockViewOptions Options => (IFileDockViewOptions)base.Options;

    public abstract IEnumerable<string> ReadableFileExtensions { get; }

    ///

    public FileDockViewModel(IDockViewOptions options) : base(options) { }

    ///

    protected abstract SerializationType GetSerializationType(string fileExtension);

    protected abstract Type GetDocumentType(string fileExtension);

    ///

    protected virtual void OnErrorOpening(string filePath, Exception e)
    {
        Dialog.ShowError(nameof(Open), new Error(e), Buttons.Ok);
    }

    protected virtual void OnOpened(Document document)
    {
        Documents.Add(document);
    }

    ///

    public static string GetFileText(string filePath, System.Text.Encoding encoding)
    {
        try
        {
            filePath = new System.IO.FileInfo(filePath).FullName;
            return File.Long.ReadAllText(filePath, encoding) ?? string.Empty;
        }
        catch (Exception e)
        {
            Log.Write<FileDocument>(e);
            return null;
        }
    }

    ///

    async public Task Open()
    {
        if (StorageDialog.Show(out string[] paths, $"{OpenTitle}...", StorageDialogMode.OpenFile, ReadableFileExtensions, ActiveDocument?.As<FileDocument>()?.Path))
            await Open(paths);
    }

    async public Task Open(IList<string> filePaths)
    {
        if (filePaths?.Count > 0)
        {
            foreach (var i in filePaths)
                await Open(i);
        }
    }

    async public Task Open(string filePath)
    {
        var fileExtension = System.IO.Path.GetExtension(filePath).Substring(1);
        var fileText = "";

        //If the file is already open, activate it
        if (Documents.FirstOrDefault(i => i.As<FileDocument>().Path?.ToLower() == filePath.ToLower()) is Document existingDocument)
        {
            ActiveContent = existingDocument;
            return;
        }

        Document document = null;
        if (!ReadableFileExtensions.Contains(fileExtension))
        {
            OnErrorOpening(filePath, new FileNotSupported(filePath));
            return;
        }

        var documentType = GetDocumentType(fileExtension);

        var serializationType = GetSerializationType(fileExtension);
        switch (serializationType)
        {
            case SerializationType.Binary:
                BinarySerializer.Deserialize(filePath, out document);
                break;

            case SerializationType.Image:
                document = documentType.Create<Document>();

                if (document is ImageFileDocument imageFile)
                {
                    System.Drawing.Bitmap bitmap = null;
                    Try.Invoke(() => bitmap = new MagickImage(filePath).ToBitmap(), e => Log.Write<Document>(e));

                    imageFile.Source = bitmap;
                }
                break;

            case SerializationType.Text:
                document = documentType.Create<Document>();
                if (document is TextFileDocument textFile)
                {
                    fileText = GetFileText(filePath, Options.Encoding.Convert());
                    textFile.Set(fileText, true, true, nameof(TextFileDocument.Text));
                }
                break;

            case SerializationType.Xml:

                fileText = GetFileText(filePath, Options.Encoding.Convert());

                Try.Invoke(() => document = (Document)documentType.GetAttribute<XmlSerializerAttribute>().Serializer.Create<XmlCallbackSerializer>()
                    .Deserialize(new System.IO.StringReader(fileText)), e => Log.Write<Document>(e));

                document.If<TextFileDocument>(i => i.Set(fileText, true, true, nameof(TextFileDocument.Text)));
                break;
        }

        if (document == null)
        {
            OnErrorOpening(filePath, new InvalidFileException(filePath));
            return;
        }

        if (document is FileDocument fileDocument)
        {
            fileDocument.Path = filePath;
            OnOpened(fileDocument);

            Options.RecentFiles.Insert(0, fileDocument.Path);
        }
    }

    ///

    ICommand deleteFileCommand;
    [Hide]
    public ICommand DeleteFileCommand => deleteFileCommand ??= new RelayCommand<string>(i =>
    {
        Dialog.ShowWarning("Delete", new Warning($"Are you sure you want to delete '{i}'?"), j =>
        {
            if (j == 0)
                Try.Invoke(() => Computer.Recycle(i, RecycleOption.DeletePermanently));
        },
        Buttons.YesNo);
    }, File.Long.Exists);

    ICommand recycleFileCommand;
    [Hide]
    public ICommand RecycleFileCommand => recycleFileCommand ??= new RelayCommand<string>(i =>
    {
        Dialog.ShowWarning("Recycle", new Warning($"Are you sure you want to recycle '{i}'?"), j =>
        {
            if (j == 0)
                Try.Invoke(() => Computer.Recycle(i, RecycleOption.SendToRecycleBin));
        },
        Buttons.YesNo);
    }, File.Long.Exists);
}

#endregion

#region ColorFileDockViewModel : FileDockViewModel

public class ColorFileDockViewModel : FileDockViewModel
{
    public override IEnumerable<string> ReadableFileExtensions => new string[] { "color" };

    protected override Type GetDocumentType(string fileExtension) => typeof(ColorDocument);

    protected override SerializationType GetSerializationType(string fileExtension) => SerializationType.Binary;

    public ColorFileDockViewModel(IFileDockViewOptions options) : base(options) { }
}

#endregion