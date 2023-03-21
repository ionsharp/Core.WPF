using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Models;

#region (enum) DockViewModelCategory

enum DockViewModelCategory { Content, Custom, Default, Documents, Group, Layouts, Manage, New, Open, Panels, Save }

#endregion

#region (enum) DockViewModelMenu

[Menu]
enum DockViewModelMenu 
{
    [MenuItem(Icon = SmallImages.File)]
    Document,
    [MenuItem(Icon = SmallImages.Layout)]
    Layout,
    [MenuItem(Icon = SmallImages.Panel)]
    Panel,
}

#endregion

#region (class) DockMainViewModel<View, Document>

[Menu(typeof(DockViewModelMenu))]
public abstract class DockMainViewModel<View, Document> : MainViewModel<View>, IDockMainViewModel where View : MainWindow where Document : Models.Document
{
    #region Properties

    [Hide]
    public Content ActiveContent => ViewModel.ActiveContent;

    [Hide]
    public Document ActiveDocument => (Document)ViewModel.ActiveDocument;

    Models.Document IDockMainViewModel.ActiveDocument => ActiveDocument;

    [Hide]
    public Panel ActivePanel => ViewModel.ActivePanel;

    ///

    [Hide]
    public DocumentCollection Documents => ViewModel.Documents;

    #endregion

    ///

    public IDockViewOptions Options { get; private set; }

    public DockViewModel ViewModel { get => Get<DockViewModel>(); private set => Set(value); }

    ///

    #region DockViewModel

    public DockMainViewModel(IDockViewOptions options) : base()
    {
        Options = options;

        ViewModel = GetModel(options);
        ViewModel.ActiveContentChanged 
            += OnActiveContentChanged;
        ViewModel.ActiveDocumentChanged 
            += OnActiveDocumentChanged;
        ViewModel.ActivePanelChanged 
            += OnActivePanelChanged;
        ViewModel.DocumentAdded 
            += OnDocumentAdded;
        ViewModel.DocumentRemoved 
            += OnDocumentRemoved;

        Documents.CollectionChanged 
            += OnDocumentsChanged;
        Documents.Removing
            += OnDocumentRemoving;

        GetDefaultPanels()
            .ForEach(Panels.Add);
        GetPanels()
            .ForEach(Panels.Add);

        if (options.RememberedDocuments.Count > 0)
        {
            options.RememberedDocuments.ForEach(i => OnRememberedDocumentAdded((Document)i));
            options.RememberedDocuments.Clear();
        }

        foreach (var i in Panels)
        {
            if (options.PanelOptions.ContainsKey(i.Name))
                options.PanelOptions[i.Name].Load(i);
        }
        options.PanelOptions.Clear();
    }

    private void OnActivePanelChanged(object sender, ChangedEventArgs<Panel> e)
        => OnActivePanelChanged(e.OldValue, e.NewValue);

    private void OnActiveDocumentChanged(object sender, ChangedEventArgs<Models.Document> e)
        => OnActiveDocumentChanged((Document)e.OldValue, (Document)e.NewValue);

    private void OnActiveContentChanged(object sender, ChangedEventArgs<Content> e)
        => OnActiveContentChanged(e.OldValue, e.NewValue);

    private void OnDocumentRemoved(object sender, EventArgs<Models.Document> e)
        => OnDocumentRemoved((Document)e.Value);

    private void OnDocumentAdded(object sender, EventArgs<Models.Document> e)
        => OnDocumentAdded((Document)e.Value);

    protected virtual void OnDocumentAdded(Document document) { }

    protected virtual void OnDocumentRemoved(Document document) { }

    protected virtual void OnActiveContentChanged(Content oldValue, Content newValue) { }

    protected virtual void OnActiveDocumentChanged(Document oldValue, Document newValue) { }

    protected virtual void OnActivePanelChanged(Panel oldValue, Panel newValue) { }

    #endregion

    #region Menu

    #region Document

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 2, Header = "Close", Icon = SmallImages.Close)]
    public ICommand CloseCommand => ViewModel.CloseCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 2, Header = "CloseAll", Icon = SmallImages.CloseAll)]
    public ICommand CloseAllCommand => ViewModel.CloseAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 2, Header = "CloseAllButThis", Icon = SmallImages.CloseAllButThis)]
    public ICommand CloseAllButThisCommand => ViewModel.CloseAllButThisCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 0, Header = "Minimize", Icon = SmallImages.Minimize)]
    public ICommand MinimizeCommand => ViewModel.MinimizeCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 0, Header = "MinimizeAll", Icon = SmallImages.MinimizeAll)]
    public ICommand MinimizeAllCommand => ViewModel.MinimizeAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 0,
        Header = "RestoreAll", Icon = SmallImages.RestoreAll)]
    public ICommand RestoreAllCommand => ViewModel.RestoreAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 1,
        Header = "Dock", Icon = SmallImages.Dock)]
    public ICommand DockDocumentCommand => ViewModel.DockAllDocumentsCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 1,
        Header = "DockAll", Icon = SmallImages.DockAll)]
    public ICommand DockAllDocumentsCommand => ViewModel.DockAllDocumentsCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 1,
        Header = "Float", Icon = SmallImages.Float)]
    public ICommand FloatDocumentCommand => ViewModel.FloatCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 1,
        Header = "FloatAll", Icon = SmallImages.FloatAll)]
    public ICommand FloatAllDocumentsCommand => ViewModel.FloatAllDocumentsCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 3,
        Header = "NewHorizontalGroup", Icon = SmallImages.GroupHorizontal)]
    public ICommand NewDocumentHorizontalGroupCommand => ViewModel.NewDocumentHorizontalGroupCommand;

    [MenuItem(Parent = DockViewModelMenu.Document, SubCategory = 3,
        Header = "NewVerticalGroup", Icon = SmallImages.GroupVertical)]
    public ICommand NewDocumentVerticalGroupCommand => ViewModel.NewDocumentVerticalGroupCommand;

    #endregion

    #region Layout

    ICommand applyLayoutCommand;
    public ICommand ApplyLayoutCommand => applyLayoutCommand ??= new RelayCommand<object>(i => Options.Layouts.Layout = i, i => i != null);

    [MenuItemCollection(Parent = DockViewModelMenu.Layout, Category = DockViewModelCategory.Default,
        ItemCommandName = nameof(ApplyLayoutCommand),
        ItemCommandParameterPath = ".",
        ItemHeaderPath = ".",
        ItemHeaderConverter = typeof(UriFileNameConverter),
        ItemIcon = SmallImages.SmallPeriod,
        ItemType = typeof(Uri))]
    public object DefaultLayouts => Options.Layouts.DefaultLayouts;

    [MenuItemCollection(Parent = DockViewModelMenu.Layout, Category = DockViewModelCategory.Custom,
        IsInline = true,

        ItemCommandName = nameof(ApplyLayoutCommand),
        ItemCommandParameterPath = ".",
        ItemHeaderConverter = typeof(FileNameConverter),
        ItemHeaderPath = ".",
        ItemIcon = SmallImages.SmallPeriod,
        ItemType = typeof(string))]
    public object CustomLayouts => Options.Layouts;

    [MenuItem(Parent = DockViewModelMenu.Layout, SubCategory = 1,
        Header = "Export",
        Icon = SmallImages.Export)]
    public ICommand ExportLayoutCommand => Options.Layouts.ExportCommand;

    [MenuItem(Parent = DockViewModelMenu.Layout, SubCategory = 1,
        Header = "Import",
        Icon = SmallImages.Import)]
    public ICommand ImportLayoutCommand => Options.Layouts.ImportCommand;

    ICommand manageLayoutsCommand;
    [MenuItem(Parent = DockViewModelMenu.Layout, SubCategory = 2,
        Header = "Manage",
        Icon = SmallImages.Options)]
    public ICommand ManageLayoutsCommand => manageLayoutsCommand ??= new RelayCommand(() => Dialog.ShowObject("Manage layouts", Options.Layouts, Resource.GetImageUri(SmallImages.Options)),
    () => Options.Layouts.Count > 0);

    ICommand saveLayoutCommand;
    [MenuItem(Parent = DockViewModelMenu.Layout, SubCategory = 0,
        Header = "Save",
        Icon = SmallImages.Save)]
    public ICommand SaveLayoutCommand => saveLayoutCommand ??= new RelayCommand(() =>
    {
        var file = new Namable("Untitled");
        Dialog.ShowObject($"Save layout", file, Resource.GetImageUri(SmallImages.Save), i =>
        {
            if (i == 0)
                Options.Layouts.Save(file.Name);
        },
        Buttons.SaveCancel);
    });

    #endregion

    #region Panel

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 0, Header = "Hide", Icon = SmallImages.Hide)]
    public ICommand HideActivePanelCommand => ViewModel.HideCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 0, Header = "HideAll", Icon = SmallImages.HideAll)]
    public ICommand HideAllCommand => ViewModel.HideAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 0, Header = "ShowAll", Icon = SmallImages.ShowAll)]
    public ICommand ShowAllCommand => ViewModel.ShowAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 1, Header = "Pin", Icon = SmallImages.Pin)]
    public ICommand PinCommand => ViewModel.PinCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 1, Header = "PinAll", Icon = SmallImages.PinAll)]
    public ICommand PinAllCommand => ViewModel.PinAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 1, Header = "UnpinAll", Icon = SmallImages.UnpinAll)]
    public ICommand UnpinAllCommand => ViewModel.UnpinAllCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 2, Header = "Dock", Icon = SmallImages.Dock)]
    public ICommand DockPanelCommand => ViewModel.DockPanelCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 2, Header = "DockAll", Icon = SmallImages.DockAll)]
    public ICommand DockAllPanelsCommand => ViewModel.DockAllPanelsCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 2,
        Header = "Float", Icon = SmallImages.Float)]
    public ICommand FloatPanelCommand => ViewModel.FloatPanelCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 2,
        Header = "FloatAll", Icon = SmallImages.FloatAll)]
    public ICommand FloatAllPanelsCommand => ViewModel.FloatAllPanelsCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 3,
        Header = "NewHorizontalGroup", Icon = SmallImages.GroupHorizontal)]
    public ICommand NewPanelHorizontalGroupCommand => ViewModel.NewPanelHorizontalGroupCommand;

    [MenuItem(Parent = DockViewModelMenu.Panel, SubCategory = 3,
        Header = "NewVerticalGroup", Icon = SmallImages.GroupVertical)]
    public ICommand NewPanelVerticalGroupCommand => ViewModel.NewPanelVerticalGroupCommand;

    [MenuItemCollection(Parent = DockViewModelMenu.Panel, SubCategory = 4, Header = nameof(Panel.Title), Icon = nameof(Panel.Icon),
        ItemCheckable = true,
        ItemCheckableMode = BindingMode.TwoWay,
        ItemCheckablePath = nameof(Panel.IsVisible),
        ItemHeaderPath = nameof(Panel.Title),
        ItemIconPath = nameof(Panel.Icon),
        ItemToolTipPath = ".",
        ItemToolTipTemplateSource = typeof(MemberGrid),
        ItemToolTipTemplateKey = nameof(MemberGrid.ObjectToolTipKey),
        ItemType = typeof(Panel),
        SortDirection = System.ComponentModel.ListSortDirection.Ascending,
        SortName = nameof(Panel.Title))]
    public PanelCollection Panels => ViewModel.Panels;

    #endregion

    #endregion

    #region Methods

    void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems?.ForEach<Document>(i =>
                {
                    i.Modified -= OnDocumentModified;
                    i.Modified += OnDocumentModified;

                    OnDocumentAdded(i);
                });
                break;

            case NotifyCollectionChangedAction.Remove:
                e.OldItems?.ForEach<Document>(i => { i.Modified -= OnDocumentModified; OnDocumentRemoved(i); });
                break;
        }
    }

    void OnDocumentModified(object sender, ModifiedEventArgs e)
    {
        if (Options.AutoSaveDocuments)
            sender.As<Document>().Save();
    }

    void OnDocumentRemoving(object sender, CancelEventArgs<object> e)
    {
        if (!e.Cancel)
        {
            if (e.Value.As<Models.Document>().CanClose)
            {
                OnDocumentRemoving(e.Value as Document);
                if (e.Value.As<Models.Document>().IsModified)
                    Dialog.ShowWarning("Close", new Warning("Are you sure you want to close?"), i => e.Cancel = i == 1, Buttons.YesNo);
            }
            else e.Cancel = true;
        }
    }

    ///

    public override void OnLoaded(IList<string> arguments) => _ = ViewModel.As<FileDockViewModel>()?.Open(arguments);

    public override void OnReloaded(IList<string> arguments) => _ = ViewModel.As<FileDockViewModel>()?.Open(arguments);

    ///

    protected virtual DockViewModel GetModel(IDockViewOptions options) => new DockViewModel(options);

    protected virtual void OnRememberedDocumentAdded(Document document)
        => Documents.Add(document);

    protected virtual void OnDocumentRemoving(Document document) { }

    ///

    protected override void OnOptionsSaving(MainViewOptions input) 
    {
        base.OnOptionsSaving(input);
        var options = (DockMainViewOptions)input;
        if (options.RememberDocuments)
        {
            options.RememberedDocuments.Clear();
            Documents.ForEach(options.RememberedDocuments.Add);
        }

        options.PanelOptions.Clear();
        foreach (var i in Panels)
        {
            options.PanelOptions.Add(i.Name, new());
            options.PanelOptions[i.Name].Save(i);
        }
    }

    ///

    public virtual IEnumerable<Panel> GetDefaultPanels()
    {
        yield return LogPanel; yield return NotificationPanel; yield return new OptionsPanel(); yield return new ThemePanel();
    }

    public virtual IEnumerable<Panel> GetPanels() => Enumerable.Empty<Panel>();

    ///

    #endregion

    #region Commands

    ICommand saveAllCommand;
    [Hide]
    public virtual ICommand SaveAllCommand => saveAllCommand ??= new RelayCommand(() => Documents.ForEach(i => i.Save()), () => Documents.Count > 0);

    #endregion
}

#endregion