using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using NLog.Time;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

[Categorize(false), Explicit, Name("Color"), Serializable, ViewSource(ShowHeader = false), View(MemberView.All)]
public partial class ColorDocument : FileDocument
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

    public byte Alpha { get => Get((byte)255); set => Set(value); }

    [HideName, Index(1), Options, Show]
    public NormalColorModel Color { get => Get<NormalColorModel>(); set => Set(value); }

    [HideName, Index(4), Options, Trigger(nameof(MemberModel.ItemAddItems), nameof(DefaultColors)), Show]
    public ObservableCollection<NormalColorModel> Colors { get => Get(new ObservableCollection<NormalColorModel>(DefaultModels)); set => Set(value); }

    [NonSerialized]
    ListCollectionView defaultColors;
    public ListCollectionView DefaultColors => defaultColors ??= GetDefaultColors();

    public Component4 Component => (Component4)Clamp(ComponentIndex, 3);

    [field: NonSerialized]
    public ObservableCollection<Component> Components { get; private set; } = new();

    [Description("The color component.")]
    [Float(Float.Above), HideName, Index(1), Name("Component"), Show]
    [Int32Style(Int32Style.Index, nameof(Components), "Name")]
    [ToolTip(nameof(ActualComponent), typeof(XColor), nameof(XColor.ComponentToolTipTemplateKey))]
    public int ComponentIndex { get => Get(0); set => Set(value); }

    [Description("The color model.")]
    [Float(Float.Above), HideName, Index(0), Name("Model")]
    [Show]
    [Int32Style(Int32Style.Index, nameof(Models), "Value.Name", nameof(SelectedModel))]
    [ToolTip(nameof(Model), typeof(XColor), nameof(XColor.ModelToolTipTemplateKey))]
    public int ModelIndex { get => Get(0); set => Set(value); }

    double depth = 0;
    [Pin(Pin.BelowOrRight), Range(0.0, 128.0, 1.0, Style = RangeStyle.Both), Footer, StringFormat("N0"), Show, Width(128)]
    public double Depth { get => Get(.0); set => Set(value); }

    [Pin(Pin.AboveOrLeft), Index(0), HideName, Footer, Show]
    public Dimensions Dimension { get => Get(Dimensions.Two); set => Set(value); }

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

    public bool Normalize { get => Get(false, false); set => Set(value, false); }

    ///

    public static IConvert<Color, string> ColorToStringConverter = new ValueConverter<Color, string>(i => { i.Convert(out ByteVector4 j); return j.ToString(); }, i => XColor.Convert(new ByteVector4(i)));

    public Color OldColor { get => GetFrom(DefaultOldColor, ColorToStringConverter); set => SetFrom(value, ColorToStringConverter); }

    [ColorStyle(ColorStyle.String), HideName, Name("Color"), Pin(Pin.AboveOrLeft), Options, Show]
    public Color NewColor { get => GetFrom(DefaultNewColor, ColorToStringConverter); set => SetFrom(value, ColorToStringConverter); }

    ///

    public int Precision { get => Get(2, false); set => Set(value, false); }

    public WorkingProfile Profile { get => Get(WorkingProfile.Default); set => Set(value); }

    public IGroupWriter Profiles { get => Get<IGroupWriter>(null, false); set => Set(value, false); }

    [Name("X°"), Index(0), Range(0.0, 360.0, 1.0, Style = RangeStyle.Both), Footer, StringFormat("N0"), Show, Width(86)]
    [VisibilityTrigger(nameof(Dimension3), true)]
    public double RotateX { get => Get(45.0); set => Set(value); }

    [Name("Y°"), Index(1), Range(0.0, 360.0, 1.0, Style = RangeStyle.Both), Footer, StringFormat("N0"), Show, Width(86)]
    [VisibilityTrigger(nameof(Dimension3), true)]
    public double RotateY { get => Get(45.0); set => Set(value); }

    [Name("Z°"), Index(2), Range(0.0, 360.0, 1.0, Style = RangeStyle.Both), Footer, StringFormat("N0"), Show, Width(86)]
    [VisibilityTrigger(nameof(Dimension3), true)]
    public double RotateZ { get => Get(.0); set => Set(value); }

    public object SelectedModel { get => Get<object>(null, false); set => Set(value, false); }

    [Index(-1), Name("Profile"), Options, Show]
    public GroupItemForm SelectedProfile { get => Get<GroupItemForm>(null, false); set => Set(value, false); }

    [Pin(Pin.AboveOrLeft), Index(1), HideName, Footer, Show]
    [VisibilityTrigger(nameof(Dimension12), true)]
    public Shapes2 Shape { get => Get(Shapes2.Square); set => Set(value); }

    public override string Title
    {
        get
        {
            NewColor.Convert(out ByteVector4 color);
            return $"#{color.ToString(false)}";
        }
    }

    public override object ToolTip => NewColor;

    public override string[] WritableExtensions => new string[] { "color" };

    [Index(3), Range(0.0, 5.0, 0.01, Style = RangeStyle.Both), Footer, StringFormat("P0"), Show, Width(86)]
    [VisibilityTrigger(nameof(Dimension3), true)]
    public double Zoom { get => Get(1.8); set => Set(value); }

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

    void OnProfileChanged(ReadOnlyValue<WorkingProfile> input) { if (input.Old != input.New) { } }

    void OnSelectedProfileChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is GroupItemForm result)
            Profile = result.SelectedItem?.As<GroupItem<WorkingProfile>>().Value ?? Profile;
    }

    void OnValueChanged(object sender, EventArgs e)
    {
        ToColor(Color);
        UpdateColors();
    }

    ///

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
        group ??= nameof(NormalColorViewModel.Category);

        var models = new ObservableCollection<NormalColorViewModel>();
        Colour.Types.ForEach(i => models.Add(new(i)));

        var result = new ListCollectionView(models);
        result.GroupDescriptions.Add(new PropertyGroupDescription(group));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(NormalColorViewModel.Category), System.ComponentModel.ListSortDirection.Ascending));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(NormalColorViewModel.Name), System.ComponentModel.ListSortDirection.Ascending));
        result.Refresh();
        return result;
    }

    ///

    NormalColorViewModel GetModel(Type model)
        => GetModel(model.Name);

    NormalColorViewModel GetModel(string modelName)
        => Models?.SourceCollection.As<IList>().FirstOrDefault<NormalColorViewModel>(i => i.Value.Name == (modelName ?? DefaultModel.Name));

    ///

    /// <summary>Converts from <see cref="Model"/> to <see cref="RGB"/> based on <see cref="Component"/>.</summary>
    void ToColor(NormalColorModel input)
    {
        handle.SafeInvoke(() =>
        {
            var color = input.GetColor();
            color.To(out RGB rgb, Profile);

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

    ///

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

    ///

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

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Alpha):
                NewColor = NewColor.A(Alpha);
                break;

            case nameof(Color):
                e.OldValue.If<NormalColorModel>(i => i.ValueChanged -= OnValueChanged);
                e.NewValue.If<NormalColorModel>(i => i.ValueChanged += OnValueChanged);
                break;

            case nameof(Component):
                Update(() => ActualComponent);
                break;

            case nameof(ComponentIndex):
                Update(() => Component);
                break;

            case nameof(Dimension):
                Update(() => Dimension12);
                Update(() => Dimension3);
                break;

            case nameof(Model):
                var cIndex = ComponentIndex >= 0 ? ComponentIndex : 0;

                Components.Clear();
                Colour.Components[Model].Each((i, j) => { Components.Add(j); return j; });

                ComponentIndex = !Is4D && ComponentIndex == 3 ? 2 : cIndex;
                break;

            case nameof(NewColor):
                Update(() => Title);
                Update(() => ToolTip);

                FromColor(Color);
                UpdateColors();
                break;

            case nameof(Normalize):
            case nameof(Precision):
                Color.Normalize = Normalize;
                Color.Precision = Precision;

                Colors.ForEach(i => { i.Normalize = Normalize; i.Precision = Precision; });
                break;

            case nameof(Profile):
                OnProfileChanged(new((WorkingProfile)e.OldValue, (WorkingProfile)e.NewValue));
                break;

            case nameof(Profiles):
                SelectedProfile = Profiles != null ? new(Profiles, 0, 0) : null;
                break;

            case nameof(SelectedModel):
                modelName = SelectedModel?.As<NormalColorViewModel>().Value.Name ?? DefaultModel.Name;

                Update(() => Model);
                Update(() => Is4D);

                Color = (NormalColorModel)DefaultColors.SourceCollection.As<ObservableCollection<NormalColorModel>>().First(i => i.Value.Name == modelName).Clone();
                goto case nameof(Component);

            case nameof(SelectedProfile):
                e.OldValue.If<GroupItemForm>(i => i.PropertyChanged -= OnSelectedProfileChanged);
                e.NewValue.If<GroupItemForm>(i => i.PropertyChanged += OnSelectedProfileChanged);
                break;
        }
    }

    protected override Task<bool> SaveAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Commands

    [field: NonSerialized]
    ICommand pickCommand;
    public ICommand PickCommand => pickCommand ??= new RelayCommand<Color>(i => NewColor = i);

    [field: NonSerialized]
    ICommand revertCommand;
    [Content("Revert"), Float(Float.Below), Image(SmallImages.Revert), Index(1), Name("Revert"), Show, Style(Core.ButtonStyle.Cancel)]
    public ICommand RevertCommand => revertCommand ??= new RelayCommand(() =>
    {
        var oldColor = OldColor;
        OldColor = NewColor;
        NewColor = oldColor;
    },
    () => true);

    [field: NonSerialized]
    ICommand selectCommand;
    [Content("Select"), Float(Float.Below), Image(SmallImages.Checkmark), Index(0), Name("Select"), Show, Style(Core.ButtonStyle.Default)]
    public ICommand SelectCommand => selectCommand ??= new RelayCommand(() => OldColor = NewColor, () => true);

    [field: NonSerialized]
    ICommand saveColorCommand;
    public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(() => ColorSaved?.Invoke(this, new(NewColor)), () => true);

    #endregion
}