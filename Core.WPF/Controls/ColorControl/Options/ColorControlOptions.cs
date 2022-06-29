using Imagin.Core.Analytics;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Controls;

[DisplayName("Options"), Serializable]
public class ColorControlOptions : BaseSavable, IColorControlOptions, ILayout
{
    enum Category { Window }

    public IEnumerable<Uri> GetDefaultLayouts()
    {
        yield return Resources.Uri(AssemblyProperties.Name, "Controls/ColorControl/Layouts/2.xml");
        yield return Resources.Uri(AssemblyProperties.Name, "Controls/ColorControl/Layouts/1.xml");
    }

    #region Properties

    #region Other

    [Hidden]
    [field: NonSerialized]
    public ColorControl ColorControl { get; private set; }

    IGroupWriter IColorControlOptions.Colors => colors;
    [field: NonSerialized]
    GroupWriter<ByteVector4> colors;
    [Hidden]
    public GroupWriter<ByteVector4> Colors
    {
        get => colors;
        set => this.Change(ref colors, value);
    }

    IGroupWriter IColorControlOptions.Illuminants => illuminants;
    [field: NonSerialized]
    GroupWriter<NamableIlluminant> illuminants;
    [Hidden]
    public GroupWriter<NamableIlluminant> Illuminants
    {
        get => illuminants;
        set => this.Change(ref illuminants, value);
    }

    IGroupWriter IColorControlOptions.Profiles => profiles;
    [field: NonSerialized]
    GroupWriter<NamableProfile> profiles;
    [Hidden]
    public GroupWriter<NamableProfile> Profiles
    {
        get => profiles;
        set => this.Change(ref profiles, value);
    }

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

    #endregion

    #endregion

    #region ColorControlOptions

    public ColorControlOptions() : base() { }

    public ColorControlOptions(string filePath) : this() => FilePath = filePath;

    #endregion

    #region Methods

    protected override void OnSaved()
    {
        Layout = Layouts.Layout;
        Colors.Save(); Illuminants.Save(); Profiles.Save();
    }

    public static Result Load(string filePath, out ColorControlOptions data)
    {
        var result = BinarySerializer.Deserialize(filePath, out object options);
        data = options as ColorControlOptions ?? new ColorControlOptions(filePath);
        return result;
    }

    public Result Deserialize(string filePath, out object data) => BinarySerializer.Deserialize(filePath, out data);

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

        Illuminants = new GroupWriter<NamableIlluminant>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Illuminants", "data", "illuminants", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        result = Illuminants.Load();

        if (!result)
        {
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Daylight (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Daylight (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
                
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Equal energy",
                typeof(Illuminant).GetProperties().Where(i => i.Name == "E").Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
                
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Flourescent (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Flourescent (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));

            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Incandescent (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("Incandescent (10°)",
                typeof(Illuminant10).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));

            Illuminants.Add(new Collections.ObjectModel.GroupCollection<NamableIlluminant>("LED (2°)",
                typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("LED")).Select(i => new NamableIlluminant(i.Name, (Vector2)i.GetValue(null)))));
        }

        Profiles = new GroupWriter<NamableProfile>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}", "Profiles", "data", "profiles", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
        result = Profiles.Load();

        if (!result)
        {
            Profiles.Add(new Collections.ObjectModel.GroupCollection<NamableProfile>("Default",
                typeof(WorkingProfiles).GetProperties().Select(i => new NamableProfile(i.GetDisplayName(), (WorkingProfile)i.GetValue(null)))));
        }

        Layouts = new Layouts($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\{nameof(ColorControl)}\Layouts", GetDefaultLayouts());
        Layouts.Update(layout);
        Layouts.Refresh();
    }

    #endregion
}