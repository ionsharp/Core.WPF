using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Controls;
using Imagin.Core.Reflection;
using Imagin.Core.Text;
using System;
using System.Collections.Generic;

namespace Imagin.Core.Models;

[Serializable]
public abstract class DockMainViewOptions : MainViewOptions, IDockViewOptions
{
    enum Category { Documents, Layout, Window }

    readonly Dictionary<string, MemberDictionary> panelOptions = new();
    [Hide]
    public Dictionary<string, MemberDictionary> PanelOptions => panelOptions;

    #region Properties

    #region Documents

    [Category(nameof(Category.Documents))]
    [Name("AutoSave")]
    public bool AutoSaveDocuments { get => Get(false); set => Set(value); }

    public virtual bool RememberDocuments => true;

    [Hide]
    public virtual List<Document> RememberedDocuments { get => Get(new List<Document>()); set => Set(value); }

    #endregion

    #region Layout

    [Category(nameof(Category.Layout)), Name("AutoSave")]
    public bool AutoSaveLayout { get => Get(true); set => Set(value); }

    [Hide]
    public virtual Layouts Layouts { get => Get<Layouts>(null, false); set => Set(value, false); }

    //[Category(nameof(Category.Layout)), Name("Panels")]
    //public PanelCollection Panels => ViewModel.Panels;

    #endregion

    #endregion

    #region DockViewOptions

    public DockMainViewOptions() : base() { }

    #endregion

    #region Methods

    protected override void OnLoaded()
    {
        base.OnLoaded();
        Layouts = new Layouts($@"{FolderPath}\Layouts", GetDefaultLayouts(), GetDefaultLayout());
        Layouts.Subscribe();
        Layouts.Refresh();
    }

    ///

    public abstract int GetDefaultLayout();

    public abstract IEnumerable<Uri> GetDefaultLayouts();

    #endregion
}

[Serializable]
public abstract class FileDockMainViewOptions : DockMainViewOptions, IFileDockViewOptions
{
    enum Category { Format }

    #region Properties

    [Category(Category.Format)]
    public Encoding Encoding { get => Get(Encoding.ASCII); set => Set(value); }

    [Hide]
    public ObservableCollection<string> RecentFiles { get => Get(new ObservableCollection<string>()); set => Set(value); }

    #endregion
}