using Imagin.Core.Collections.Generic;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

[DisplayName("Color"), Explicit, Serializable]
public partial class ColorDocument : Document
{
    readonly Handle handle = false;

    #region Events

    [field: NonSerialized]
    public event DefaultEventHandler<Color> ColorChanged;

    [field: NonSerialized]
    public event DefaultEventHandler<Color> ColorSaved;

    #endregion

    #region Fields

    public static readonly Color DefaultOldColor = System.Windows.Media.Colors.Black;

    public static readonly Color DefaultNewColor = System.Windows.Media.Colors.White;

    public static readonly Type DefaultModel = typeof(HSB);

    public static readonly NormalColorModel[] DefaultModels = new NormalColorModel[] { new NormalColorModel4(typeof(CMYK)), new NormalColorModel3(typeof(XYZ)) };

    #endregion

    #region Properties

    public Component ActualComponent => Components?.ElementAtOrDefault((int)Component);

    byte alpha = 255;
    public byte Alpha
    {
        get => alpha;
        set => this.Change(ref alpha, value);
    }

    NormalColorModel color = null;
    [Index(1), Visible]
    public NormalColorModel Color
    {
        get => color;
        set
        {
            var oldValue = color;
            var newValue = value;

            oldValue.If(i => i.ValueChanged -= OnValueChanged);
            newValue.If(i => i.ValueChanged += OnValueChanged);

            this.Change(ref color, value);
        }
    }

    ObservableCollection<NormalColorModel> colors = new(DefaultModels);
    [Index(4), Trigger(nameof(MemberModel.ItemClones), nameof(DefaultColors)), Visible]
    public ObservableCollection<NormalColorModel> Colors
    {
        get => colors;
        set => this.Change(ref colors, value);
    }

    [NonSerialized]
    ListCollectionView defaultColors;
    public ListCollectionView DefaultColors => defaultColors ??= GetDefaultColors();

    public Component4 Component => (Component4)Clamp(ComponentIndex, 3);

    [field: NonSerialized]
    public ObservableCollection<Component> Components { get; private set; } = new();

    int componentIndex = 0;
    public int ComponentIndex
    {
        get => componentIndex;
        set => this.Change(ref componentIndex, value);
    }

    double depth = 0;
    [Below, SliderUpDown, Localize(false), Range(0.0, 128.0, 1.0), Status, StringFormat("N0"), Visible, Width(128)]
    public double Depth
    {
        get => depth;
        set => this.Change(ref depth, value);
    }

    Dimensions dimension = Dimensions.One;
    [Above, Index(0), Label(false), Localize(false), Status, Visible]
    public Dimensions Dimension
    {
        get => dimension;
        set => this.Change(ref dimension, value);
    }

    public override object Icon => NewColor;

    public bool Dimension12 => !Dimension3;

    public bool Dimension3 => Dimension == Dimensions.Three;

    public bool Is4D => Model?.Inherits<ColorModel4>() == true;

    string modelName = DefaultModel.Name;
    public Type Model => GetModel(modelName ?? DefaultModel.Name)?.Value ?? DefaultModel;

    [NonSerialized]
    ListCollectionView models = GetDefaultModels();
    public ListCollectionView Models
    {
        get => models;
        private set => models = value;
    }

    [NonSerialized]
    bool normalize = false;
    public bool Normalize
    {
        get => normalize;
        set => this.Change(ref normalize, value);
    }

    //...

    ByteVector4 oldColor = new(DefaultOldColor.R, DefaultOldColor.G, DefaultOldColor.B, DefaultOldColor.A);
    public Color OldColor
    {
        get => System.Windows.Media.Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, oldColor.B);
        set => this.Change(ref oldColor, new ByteVector4(value.R, value.G, value.B, value.A));
    }

    ByteVector4 newColor = ByteVector4.White;
    [Above, DisplayName("Color"), Hexadecimal, Label(false), Visible]
    public Color NewColor
    {
        get => XColor.Convert(newColor);
        set
        {
            value.Convert(out ByteVector4 result);
            this.Change(ref newColor, result);
        }
    }

    //...

    string path = "";
    public string Path
    {
        get => path;
        set => this.Change(ref path, value);
    }

    [NonSerialized]
    int precision = 2;
    public int Precision
    {
        get => precision;
        set => this.Change(ref precision, value);
    }

    WorkingProfile profile = WorkingProfile.Default;
    [Index(0)]
    public WorkingProfile Profile
    {
        get => profile;
        set
        {
            var oldProfile = profile;
            var newProfile = value;

            this.Change(ref profile, newProfile);
            OnProfileChanged(new(oldProfile, newProfile));
        }
    }

    [NonSerialized]
    IGroupWriter profiles = null;
    public IGroupWriter Profiles
    {
        get => profiles;
        set
        {
            this.Change(ref profiles, value);
            SelectedProfile = profiles != null ? new(profiles, 0, 0) : null;
        }
    }

    double rotateX = 45;
    [DisplayName("X°"), SliderUpDown, Index(0), Localize(false), Range(0.0, 360.0, 1.0), Status, StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateX
    {
        get => rotateX;
        set => this.Change(ref rotateX, value);
    }

    double rotateY = 45;
    [DisplayName("Y°"), SliderUpDown, Index(1), Localize(false), Range(0.0, 360.0, 1.0), Status, StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateY
    {
        get => rotateY;
        set => this.Change(ref rotateY, value);
    }

    double rotateZ = 0;
    [DisplayName("Z°"), SliderUpDown, Index(2), Localize(false), Range(0.0, 360.0, 1.0), Status, StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateZ
    {
        get => rotateZ;
        set => this.Change(ref rotateZ, value);
    }

    [NonSerialized]
    object selectedModel = null;
    public object SelectedModel
    {
        get => selectedModel;
        set => this.Change(ref selectedModel, value);
    }

    [NonSerialized]
    GroupItemModel selectedProfile = null;
    [DisplayName("Profile"), Tool, Visible]
    public GroupItemModel SelectedProfile
    {
        get => selectedProfile;
        set
        {
            var oldValue = selectedProfile;
            var newValue = value;

            oldValue.If(i => i.PropertyChanged -= OnSelectedProfileChanged);
            newValue.If(i => i.PropertyChanged += OnSelectedProfileChanged);

            this.Change(ref selectedProfile, value);
        }
    }

    Shapes2 shape = Shapes2.Square;
    [Above, Index(1), Label(false), Localize(false), Status, Visible]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension12))]
    public Shapes2 Shape
    {
        get => shape;
        set => this.Change(ref shape, value);
    }

    public override string Title
    {
        get
        {
            NewColor.Convert(out ByteVector4 color);
            return $"#{color.ToString(false)}";
        }
    }

    public override object ToolTip => NewColor;

    double zoom = 1.8;
    [Index(3), Range(0.0, 5.0, 0.01), SliderUpDown, Status, StringFormat("P0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double Zoom
    {
        get => zoom;
        set => this.Change(ref zoom, value);
    }

    #endregion

    #region ColorDocument

    public ColorDocument() : this(DefaultNewColor, DefaultModel, null) { }

    public ColorDocument(IGroupWriter profiles) : this(DefaultNewColor, DefaultModel, profiles) { }

    public ColorDocument(Color color, IGroupWriter profiles) : this(color, DefaultModel, profiles) { }

    public ColorDocument(Color color, Type model, IGroupWriter profiles) : base()
    {
        NewColor = color; Profiles = profiles; SelectedModel = GetModel(model);
    }

    #endregion

    #region Methods

    void OnColorChanged(Color color) => ColorChanged?.Invoke(this, new(color));

    void OnProfileChanged(Value<WorkingProfile> input) { if (input.Old != input.New) { } }

    void OnSelectedProfileChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is GroupItemModel result)
            Profile = result.SelectedItem.As<WorkingProfileModel>().Value;
    }

    void OnValueChanged(object sender, EventArgs e)
    {
        ToColor(Color);
        UpdateColors();
    }

    //...

    public static ListCollectionView GetDefaultColors()
    {
        ObservableCollection<NormalColorModel> models = new();
        foreach (var i in Colour.Types)
        {
            NormalColorModel j = null;
            if (i.Inherits<ColorModel4>())
                j = new NormalColorModel4(i);
            
            else if (i.Inherits<ColorModel3>())
                j = new NormalColorModel3(i);

            models.Add(j);
        }

        var result = new ListCollectionView(models);
        result.GroupDescriptions.Add(new PropertyGroupDescription(nameof(NormalColorModel.Category)));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(NormalColorModel.Category), System.ComponentModel.ListSortDirection.Ascending));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(NormalColorModel.Name), System.ComponentModel.ListSortDirection.Ascending));
        return result;
    }

    public static ListCollectionView GetDefaultModels(string group = null)
    {
        group ??= "Name";

        var models = new ObservableCollection<NormalColorModelWrapper>();
        Colour.Types.ForEach(i => models.Add(new(i)));

        var result = new ListCollectionView(models);
        result.GroupDescriptions.Add(new PropertyGroupDescription(group));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        return result;
    }

    //...

    NormalColorModelWrapper GetModel(Type model)
        => GetModel(model.Name);

    NormalColorModelWrapper GetModel(string modelName)
        => Models?.SourceCollection.As<IList>().FirstOrDefault<NormalColorModelWrapper>(i => i.Value.Name == (modelName ?? DefaultModel.Name));

    //...

    /// <summary>Converts from <see cref="Model"/> to <see cref="RGB"/> based on <see cref="Component"/>.</summary>
    void ToColor(NormalColorModel input)
    {
        handle.SafeInvoke(() =>
        {
            var color = input.GetColor();
            color.To(out RGB rgb, profile);

            NewColor = XColor.Convert(rgb);
            OnColorChanged(NewColor);
        });
    }

    /// <summary>Converts from <see cref="RGB"/> to <see cref="Model"/> based on <see cref="Component"/>.</summary>
    void FromColor(NormalColorModel input)
    {
        handle.SafeInvoke(() =>
        {
            NewColor.Convert(out RGB rgb);

            var result = Colour.New(Model, rgb, Profile);

            if (result is ColorModel3 result3)
            {
                var color3 = result3.Normalize();
                input.If<NormalColorModel3>(i => { i.X = color3.X; i.Y = color3.Y; i.Z = color3.Z; });
            }

            else if (result is ColorModel4 result4)
            {
                var color4 = result4.Normalize();
                input.If<NormalColorModel4>(i => { i.X = color4.X; i.Y = color4.Y; i.Z = color4.Z; i.W = color4.W; });
            }

            OnColorChanged(NewColor);
        });
    }

    //...

    void UpdateColors()
    {
        NewColor.Convert(out RGB rgb);
        foreach (var i in Colors)
        {
            var j = rgb.To(i.Value, WorkingProfile.Default);
            if (i is NormalColorModel3 i3)
            {
                if (j is ColorModel3 j3)
                {
                    if (Normalize)
                    {
                        i3.X = new DoubleRange(i3.Minimum[0], i3.Maximum[0]).Convert(0, 1, j3.X);
                        i3.Y = new DoubleRange(i3.Minimum[1], i3.Maximum[1]).Convert(0, 1, j3.Y);
                        i3.Z = new DoubleRange(i3.Minimum[2], i3.Maximum[2]).Convert(0, 1, j3.Z);
                    }
                    else
                    {
                        i3.DisplayX = $"{j3.X}";
                        i3.DisplayY = $"{j3.Y}";
                        i3.DisplayZ = $"{j3.Z}";
                    }
                }
            }
            if (i is NormalColorModel4 i4)
            {
                if (j is ColorModel4 j4)
                {
                    if (Normalize)
                    {
                        i4.X = new DoubleRange(i4.Minimum[0], i4.Maximum[0]).Convert(0, 1, j4.X);
                        i4.Y = new DoubleRange(i4.Minimum[1], i4.Maximum[1]).Convert(0, 1, j4.Y);
                        i4.Z = new DoubleRange(i4.Minimum[2], i4.Maximum[2]).Convert(0, 1, j4.Z);
                        i4.W = new DoubleRange(i4.Minimum[3], i4.Maximum[3]).Convert(0, 1, j4.W);
                    }
                    else
                    {
                        i4.DisplayX = $"{j4.X}";
                        i4.DisplayY = $"{j4.Y}";
                        i4.DisplayZ = $"{j4.Z}";
                        i4.DisplayW = $"{j4.W}";
                    }
                }
            }
        }
    }

    //...

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext input)
    {
        Components
            ??= new();
        Models
            ??= GetDefaultModels();
        SelectedModel 
            = GetModel(modelName ?? DefaultModel.Name);
    }

    //...

    public override Document Clone() => new ColorDocument(NewColor, Model, Profiles);

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Alpha):
                NewColor = NewColor.A(Alpha);
                break;

            case nameof(Component):
                this.Changed(() => ActualComponent);
                break;

            case nameof(ComponentIndex):
                this.Changed(() => Component);
                break;

            case nameof(Dimension):
                this.Changed(() => Dimension12);
                this.Changed(() => Dimension3);
                break;

            case nameof(Model):
                var cIndex = componentIndex;

                Components.Clear();
                Colour.Components[Model].Each((i, j) => { Components.Add(j); return j; });

                ComponentIndex = !Is4D && ComponentIndex == 3 ? 2 : cIndex;
                break;

            case nameof(NewColor):
                this.Changed(() => Title);
                this.Changed(() => ToolTip);

                FromColor(Color);
                UpdateColors();
                break;

            case nameof(Normalize):
            case nameof(Precision):
                Color.Normalize = Normalize;
                Color.Precision = Precision;

                Colors.ForEach(i => { i.Normalize = Normalize; i.Precision = Precision; });
                break;

            case nameof(SelectedModel):
                modelName = SelectedModel?.As<NormalColorModelWrapper>().Value.Name ?? DefaultModel.Name;

                this.Changed(() => Model);
                this.Changed(() => Is4D);

                Color = (NormalColorModel)DefaultColors.SourceCollection.As<ObservableCollection<NormalColorModel>>().First(i => i.Value.Name == modelName).Clone();
                goto case nameof(Component);
        }
    }

    public override void Save() { }

    #endregion

    #region Commands

    [field: NonSerialized]
    ICommand pickCommand;
    public ICommand PickCommand => pickCommand ??= new RelayCommand<Color>(i => NewColor = i);

    [field: NonSerialized]
    ICommand revertCommand;
    public ICommand RevertCommand => revertCommand ??= new RelayCommand(() =>
    {
        var oldColor = OldColor;
        OldColor = NewColor;
        NewColor = oldColor;
    },
    () => true);

    [field: NonSerialized]
    ICommand selectCommand;
    public ICommand SelectCommand => selectCommand ??= new RelayCommand(() => OldColor = NewColor, () => true);

    [field: NonSerialized]
    ICommand saveColorCommand;
    public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(() => ColorSaved?.Invoke(this, new(NewColor)), () => true);

    #endregion
}