using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Config;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Text;
using System;
using System.Collections.Generic;

namespace Imagin.Core.Controls;

[File(DefaultName, DefaultExtension), Name(DefaultName), Serializable]
public class ColorViewOptions : DataSavable, IFileDockViewOptions
{
    enum Category { Color, Documents, Format, Layout, Window }

    enum LayoutNames { All, Analyze, Collect, Explore, Select }

    ///

    public const string DefaultExtension = "data";

    public const string DefaultName = "Options";

    ///

    readonly Dictionary<string, MemberDictionary> panelOptions = new();
    [Hide]
    public Dictionary<string, MemberDictionary> PanelOptions => panelOptions;

    ///

    protected override string FileExtension => DefaultExtension;

    protected override string FileName => DefaultName;

    protected override string FolderPath => Current.Get<SingleApplication>().RootFolderPath + $@"\ColorView";

    [Category(Category.Format)]
    public Encoding Encoding { get => Get(Encoding.ASCII); set => Set(value); }

    [Hide]
    public ObservableCollection<string> RecentFiles { get => Get(new ObservableCollection<string>()); set => Set(value); }

    #region Properties

    #region Data

    [Hide]
    public GroupWriter<ByteVector4> Colors { get => Get<GroupWriter<ByteVector4>>(null, false); set => Set(value, false); }

    [Hide]
    public GroupWriter<Vector2> Illuminants { get => Get<GroupWriter<Vector2>>(null, false); set => Set(value, false); }

    [Hide]
    public GroupWriter<DoubleMatrix> Matrices { get => Get<GroupWriter<DoubleMatrix>>(null, false); set => Set(value, false); }

    [Hide]
    public GroupWriter<WorkingProfile> Profiles { get => Get<GroupWriter<WorkingProfile>>(null, false); set => Set(value, false); }

    #endregion

    #region Documents

    [Category(nameof(Category.Documents))]
    [Name("AutoSave")]
    public bool AutoSaveDocuments { get => Get(false); set => Set(value); }

    public virtual bool RememberDocuments => true;

    [Hide]
    public virtual List<Document> RememberedDocuments { get => Get(new List<Document>()); set => Set(value); }

    [Category(Category.Documents)]
    public int DefaultDepth { get => Get(0); set => Set(value); }

    [Category(Category.Documents)]
    public Dimensions DefaultDimension { get => Get(Dimensions.Two); set => Set(value); }

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

    #region ColorViewOptions

    public ColorViewOptions() : base() { }

    #endregion

    #region Methods

    protected override IEnumerable<IWriter> GetData()
    {
        Colors ??= new GroupWriter<ByteVector4>
            (FolderPath, nameof(Colors),
            "data", "colors", new(250));

        Illuminants ??= new GroupWriter<Vector2>
            (FolderPath, nameof(Illuminants),
            "data", "illuminants", new(250));

        Matrices ??= new GroupWriter<DoubleMatrix>
            (FolderPath, nameof(Matrices),
            "data", "matrices", new(250));

        Profiles ??= new GroupWriter<WorkingProfile>
            (FolderPath, nameof(Profiles),
            "data", "profiles", new(250));

        yield return Colors; yield return Illuminants; yield return Matrices; yield return Profiles;
    }

    ///

    protected override void OnLoaded()
    {
        base.OnLoaded();
        Layouts = new Layouts($@"{FolderPath}\Layouts", GetDefaultLayouts(), GetDefaultLayout());
        Layouts.Subscribe();
        Layouts.Refresh();
    }

    ///

    public int GetDefaultLayout() => (int)LayoutNames.All;

    public IEnumerable<Uri> GetDefaultLayouts()
    {
        yield return Resource.GetUri(XAssembly.Name, $"{nameof(Controls)}/{nameof(ColorView)}/Layouts/{LayoutNames.All}.xml");
        yield return Resource.GetUri(XAssembly.Name, $"{nameof(Controls)}/{nameof(ColorView)}/Layouts/{LayoutNames.Analyze}.xml");
        yield return Resource.GetUri(XAssembly.Name, $"{nameof(Controls)}/{nameof(ColorView)}/Layouts/{LayoutNames.Collect}.xml");
        yield return Resource.GetUri(XAssembly.Name, $"{nameof(Controls)}/{nameof(ColorView)}/Layouts/{LayoutNames.Explore}.xml");
        yield return Resource.GetUri(XAssembly.Name, $"{nameof(Controls)}/{nameof(ColorView)}/Layouts/{LayoutNames.Select}.xml");
    }

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(ViewModel))
        {
            if (e.NewValue is IDockMainViewModel newViewModel)
            {
                if (RememberedDocuments.Count > 0)
                {
                    RememberedDocuments.ForEach(newViewModel.Documents.Add);
                    RememberedDocuments.Clear();
                }

                foreach (var i in newViewModel.Panels)
                {
                    if (PanelOptions.ContainsKey(i.Name))
                        PanelOptions[i.Name].Load(i);
                }
                PanelOptions.Clear();
            }
        }
    }

    #endregion
}