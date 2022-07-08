using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Serializable]
public abstract class DockViewOptions : MainViewOptions
{
    enum Category { Documents }

    bool autoSaveDocuments = false;
    [Category(nameof(Category.Documents))]
    [DisplayName("AutoSave")]
    public bool AutoSaveDocuments
    {
        get => autoSaveDocuments;
        set => this.Change(ref autoSaveDocuments, value);
    }
}

[Serializable]
public abstract class DockViewOptions<T> : DockViewOptions where T : class, IDockViewModel
{
    enum Category { Documents, Window }

    #region Properties

    #region Documents

    protected virtual bool RememberDocuments => true;

    List<Document> documents = new();
    [Hidden]
    public virtual List<Document> Documents
    {
        get => documents;
        set => this.Change(ref documents, value);
    }

    #endregion

    #region Window

    bool autoSaveLayout = true;
    [Category(nameof(Category.Window)), DisplayName("AutoSave")]
    public bool AutoSaveLayout
    {
        get => autoSaveLayout;
        set => this.Change(ref autoSaveLayout, value);
    }

    string layout = string.Empty;
    [Hidden]
    public virtual string Layout
    {
        get => layout;
        set => this.Change(ref layout, value);
    }

    [NonSerialized]
    Layouts layouts = null;
    [Category(nameof(Category.Window)), DisplayName("Layout")]
    public virtual Layouts Layouts
    {
        get => layouts;
        set => this.Change(ref layouts, value);
    }

    [Category(nameof(Category.Window)), DisplayName("Panels")]
    public virtual PanelCollection Panels => Get.Where<T>().Panels;

    Dictionary<string, PanelOptions> PanelOptions = new();

    #endregion

    #endregion

    #region DockViewOptions

    public DockViewOptions() : base() { }

    #endregion

    #region Methods

    protected override void OnLoaded()
    {
        base.OnLoaded();
        Layouts = new Layouts($@"{Get.Where<IApplication>().Properties.FolderPath}\Layouts", GetDefaultLayouts(), 0);
        Layouts.Update(layout);
        Layouts.Refresh();
    }

    protected override void OnSaving()
    {
        base.OnSaving();
        Layout = Layouts?.Layout is string layout ? layout : Layout;

        if (RememberDocuments)
        {
            Documents.Clear();
            Get.Where<T>()?.Documents.ForEach(i => Documents.Add(i));
        }

        PanelOptions.Clear();
        foreach (var i in Panels)
        {
            PanelOptions.Add(i.Name, new());
            PanelOptions[i.Name].Save(i);
        }
    }

    public abstract IEnumerable<Uri> GetDefaultLayouts();

    public override void OnApplicationReady()
    {
        base.OnApplicationReady();
        if (Documents.Count > 0)
        {
            Documents.ForEach(i => Get.Where<T>().Documents.Add(i));
            Documents.Clear();
        }
        foreach (var i in Panels)
        {
            if (PanelOptions.ContainsKey(i.Name))
                PanelOptions[i.Name].Load(i);
        }
    }

    #endregion
}