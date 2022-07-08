using Imagin.Core.Analytics;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Controls;

[DisplayName("Options"), Serializable]
public class ColorControlOptions : BaseSavable, IColorControlOptions
{
    enum Category { Color, Documents, Window }

    enum LayoutNames { Analyze, Basic, Collect, Everything, Explore }

    public int DefaultLayout => (int)LayoutNames.Everything;

    public IEnumerable<Uri> GetDefaultLayouts()
    {
        yield return Resources.Uri(AssemblyProperties.Name, $"{nameof(Controls)}/{nameof(ColorControl)}/Layouts/{LayoutNames.Analyze}.xml");
        yield return Resources.Uri(AssemblyProperties.Name, $"{nameof(Controls)}/{nameof(ColorControl)}/Layouts/{LayoutNames.Basic}.xml");
        yield return Resources.Uri(AssemblyProperties.Name, $"{nameof(Controls)}/{nameof(ColorControl)}/Layouts/{LayoutNames.Collect}.xml");
        yield return Resources.Uri(AssemblyProperties.Name, $"{nameof(Controls)}/{nameof(ColorControl)}/Layouts/{LayoutNames.Everything}.xml");
        yield return Resources.Uri(AssemblyProperties.Name, $"{nameof(Controls)}/{nameof(ColorControl)}/Layouts/{LayoutNames.Explore}.xml");
    }

    #region Properties

    #region Other

    [Hidden]
    public ColorControl ColorControl { get; private set; }

    IGroupWriter IColorControlOptions.Colors => colors;
    [NonSerialized]
    GroupWriter<ByteVector4> colors;
    [Hidden]
    public GroupWriter<ByteVector4> Colors
    {
        get => colors;
        set => this.Change(ref colors, value);
    }

    IGroupWriter IColorControlOptions.Illuminants => illuminants;
    [NonSerialized]
    GroupWriter<ChromacityModel> illuminants;
    [Hidden]
    public GroupWriter<ChromacityModel> Illuminants
    {
        get => illuminants;
        set => this.Change(ref illuminants, value);
    }

    IGroupWriter IColorControlOptions.Matrices => matrices;
    [NonSerialized]
    GroupWriter<AdaptationModel> matrices;
    [Hidden]
    public GroupWriter<AdaptationModel> Matrices
    {
        get => matrices;
        set => this.Change(ref matrices, value);
    }

    IGroupWriter IColorControlOptions.Profiles => profiles;
    [NonSerialized]
    GroupWriter<WorkingProfileModel> profiles;
    [Hidden]
    public GroupWriter<WorkingProfileModel> Profiles
    {
        get => profiles;
        set => this.Change(ref profiles, value);
    }

    #endregion

    #region Documents

    bool autoSaveDocuments = true;
    [Category(Category.Documents)]
    [DisplayName("Auto save")]
    public bool AutoSaveDocuments
    {
        get => autoSaveDocuments;
        set => this.Change(ref autoSaveDocuments, value);
    }

    [Hidden]
    public bool RememberDocuments => true;

    [NonSerialized]
    IList Documents;
    [Hidden]
    IList IColorControlOptions.Documents => Documents;

    readonly List<Document> rememberedDocuments = new();

    #endregion

    #region Window

    bool autoSaveLayout = true;
    [Category(Category.Window)]
    [DisplayName("Auto save")]
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
    [Category(Category.Window)]
    [DisplayName("Layout")]
    public virtual Layouts Layouts
    {
        get => layouts;
        set => this.Change(ref layouts, value);
    }

    [Category(Category.Window)]
    [DisplayName("Panels")]
    public PanelCollection Panels => ColorControl?.Panels;

    [Hidden]
    IList IColorControlOptions.Panels => Panels;

    Dictionary<string, PanelOptions> PanelOptions = new();

    #endregion

    #endregion

    #region ColorControlOptions

    public ColorControlOptions() : base() { }

    public ColorControlOptions(string filePath, IList documents) : this()
    {
        FilePath = filePath;
        Documents = documents;
    }

    #endregion

    #region Methods

    public override Result Load(out BaseSavable output)
    {
        var result = base.Load(out output);
        if (output is ColorControlOptions options && options.rememberedDocuments.Count > 0)
        {
            options.Documents = Documents;
            options.rememberedDocuments.ForEach(i => Documents?.Add(i));
            options.rememberedDocuments.Clear();
        }

        if (Panels != null)
        {
            foreach (var i in Panels)
            {
                if (PanelOptions.ContainsKey(i.Name))
                    PanelOptions[i.Name].Load(i);
            }
        }
        return result;
    }

    protected override void OnSaved()
    {
        Layout = Layouts.Layout is string layout ? layout : Layout;
        Colors.Save(); Illuminants.Save(); Profiles.Save();
    }

    protected override void OnSaving()
    {
        base.OnSaving();
        if (RememberDocuments)
        {
            rememberedDocuments.Clear();
            Documents?.ForEach(i => rememberedDocuments.Add(i as Document));
        }

        PanelOptions.Clear();
        foreach (var i in Panels)
        {
            PanelOptions.Add(i.Name, new());
            PanelOptions[i.Name].Save(i);
        }
    }

    //...

    public void OnLoaded(ColorControl colorPicker)
    {
        ColorControl = colorPicker;

        Colors = new GroupWriter<ByteVector4>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Colors", "data", "colors", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        var result = Colors.Load();

        if (!result)
        {
            Colors.Add(new PrimaryColors());
            Colors.Add(new SecondaryColors());
            Colors.Add(new TertiaryColors());
            Colors.Add(new QuaternaryColors());
            Colors.Add(new QuinaryColors());

            Colors.Add(new Collections.ObjectModel.GroupCollection<ByteVector4>("Basic", 
                typeof(BasicColors).GetFields().Select(i => new ByteVector4((string)i.GetValue(null)))));
            Colors.Add(new Collections.ObjectModel.GroupCollection<ByteVector4>("Web (CSS)",
                typeof(CSSColors).GetFields().Select(i => new ByteVector4((string)i.GetValue(null)))));
            Colors.Add(new Collections.ObjectModel.GroupCollection<ByteVector4>("Web (Safe)", 
                SafeWebColors.Colors.Select(i => new ByteVector4(i))));
            Colors.Add(new Collections.ObjectModel.GroupCollection<ByteVector4>("Web (Safest)",
                typeof(SafestWebColors).GetFields().Select(i => new ByteVector4((string)i.GetValue(null)))));
        }

        Illuminants = new GroupWriter<ChromacityModel>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Illuminants", "data", "illuminants", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        result = Illuminants.Load();

        if (!result)
        {
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Daylight (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Daylight (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
                
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Equal energy",
                typeof(Illuminant).GetProperties().Where(i => i.Name == "E").Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
                
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Flourescent (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Flourescent (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));

            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Incandescent (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("Incandescent (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));

            Illuminants.Add(new Collections.ObjectModel.GroupCollection<ChromacityModel>("LED (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("LED")).Select(i => new ChromacityModel(i.Name, i.GetDescription(), (Vector2)i.GetValue(null)))));
        }

        Matrices = new GroupWriter<AdaptationModel>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Matrices", "data", "matrices", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        result = Matrices.Load();

        if (!result)
        {
            Matrices.Add(new Collections.ObjectModel.GroupCollection<AdaptationModel>("LMS", 
                typeof(ChromaticAdaptationTransform).GetProperties().Where(i => i.Name != nameof(ChromaticAdaptationTransform.Default)).Select(i => new AdaptationModel(i.GetDisplayName(), i.GetDescription(), (Matrix)i.GetValue(null)))));
        }

        Profiles = new GroupWriter<WorkingProfileModel>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Profiles", "data", "profiles", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        result = Profiles.Load();

        if (!result)
        {
            typeof(WorkingProfiles).GetProperties()
                .GroupBy(i => i.GetCategory()).ForEach(i => Profiles.Add(new Collections.ObjectModel.GroupCollection<WorkingProfileModel>(i.Key, i.Select(j => new WorkingProfileModel(j.GetDisplayName(), j.GetDescription(), (WorkingProfile)j.GetValue(null))))));
        }

        Layouts = new Layouts($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}\Layouts", GetDefaultLayouts(), DefaultLayout);
        Layouts.Update(layout);
        Layouts.Refresh();
    }

    #endregion
}