using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

/// <summary>A normalized <see cref="ColorModel"/> used to perform conversion via user interface.</summary>
[DisplayName("Color")]
public class ColorViewModel : ViewModel
{
    enum Category { Adaptation, Chromacity, Component, Compression, Primary, View }

    enum Views { Components, Profile }

    //...

    public event DefaultEventHandler<Color> ColorChanged;

    #region Properties

    readonly Handle handle = false;

    //...

    Color actualColor = System.Windows.Media.Colors.White;
    [MemberSetter(nameof(MemberModel.Format), ColorFormat.TextBox), DisplayName("Color")]
    [Featured, Label(false), Visible]
    public Color ActualColor
    {
        get => actualColor;
        set => this.Change(ref actualColor, value);
    }

    DoubleMatrix adapt = new(LMS.Transform.Bradford.As<IMatrix>().Values);
    [Assignable(nameof(Adaptations))]
    [Category(Category.Adaptation), DisplayName("Transform"), Index(2), View(Views.Profile), Visible]
    public DoubleMatrix Adapt
    {
        get => adapt;
        set => this.Change(ref adapt, value);
    }

    [Hidden]
    public object Adaptations
    {
        get
        {
            var result = new ObservableCollection<Namable<DoubleMatrix>>();
            foreach (var i in typeof(LMS.Transform).GetProperties(BindingFlags.Public | BindingFlags.Static))
                result.Add(new(i.Name, new DoubleMatrix(i.GetValue(null).As<IMatrix>().Values)));

            return result;
        }
    }

    Vector2 chromacity = WorkingProfile.Default.Chromacity;
    [Assignable(nameof(DefaultChromacity))]
    [Category(Category.Chromacity), Index(-2), Object(ObjectLayout.Horizontal), View(Views.Profile), Visible]
    public Vector2 Chromacity
    {
        get => chromacity;
        set => this.Change(ref chromacity, value);
    }

    [Hidden]
    public object DefaultChromacity
    {
        get
        {
            var result = new ObservableCollection<Namable<Vector2>>();
            foreach (var i in typeof(Illuminant2).GetProperties(BindingFlags.Public | BindingFlags.Static))
                result.Add(new(i.Name + " (2°)", (Vector2)i.GetValue(null)));

            foreach (var i in typeof(Illuminant10).GetProperties(BindingFlags.Public | BindingFlags.Static))
                result.Add(new(i.Name + " (10°)", (Vector2)i.GetValue(null)));

            return result;
        }
    }

    [Hidden]
    public Colors.Component ActualComponent => componentIndex >= 0 && componentIndex < Components.Count ? Components[componentIndex] : null;
    
    Component4 component = Component4.X;
    [Hidden]
    public Component4 Component
    {
        get => component;
        private set => this.Change(ref component, value);
    }

    int componentIndex = 0;
    [Hidden]
    public int ComponentIndex
    {
        get => componentIndex;
        set => this.Change(ref componentIndex, value);
    }

    [Hidden]
    public ObservableCollection<Colors.Component> Components { get; private set; } = new();

    ICompress compress = WorkingProfile.Default.Compress;
    [Assignable(typeof(GammaCompression), typeof(LogGammaCompression), typeof(LCompression), typeof(PQCompression), typeof(Rec709Compression), typeof(Rec2020Compression), typeof(sRGBCompression))]
    [Category(Category.Compression), DisplayName("Method"), Index(1), Object, View(Views.Profile), Visible]
    public ICompress Compress
    {
        get => compress;
        set => this.Change(ref compress, value);
    }

    [MemberSetter(nameof(MemberModel.ClearText), false)]
    [MemberTrigger(nameof(MemberModel.DisplayName), nameof(NameX))]
    [MemberTrigger(nameof(MemberModel.RightText), nameof(UnitX))]
    [Category(Category.Component), Index(0), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayX
    {
        get => GetDisplayValue(0);
        set => SetDisplayValue(value, 0);
    }

    [MemberSetter(nameof(MemberModel.ClearText), false)]
    [MemberTrigger(nameof(MemberModel.DisplayName), nameof(NameY))]
    [MemberTrigger(nameof(MemberModel.RightText), nameof(UnitY))]
    [Category(Category.Component), Index(1), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayY
    {
        get => GetDisplayValue(1);
        set => SetDisplayValue(value, 1);
    }

    [MemberSetter(nameof(MemberModel.ClearText), false)]
    [MemberTrigger(nameof(MemberModel.DisplayName), nameof(NameZ))]
    [MemberTrigger(nameof(MemberModel.RightText), nameof(UnitZ))]
    [Category(Category.Component), Index(2), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayZ
    {
        get => GetDisplayValue(2);
        set => SetDisplayValue(value, 2);
    }

    [MemberSetter(nameof(MemberModel.ClearText), false)]
    [MemberTrigger(nameof(MemberModel.DisplayName), nameof(NameW))]
    [MemberTrigger(nameof(MemberModel.IsVisible), nameof(WVisibility))]
    [MemberTrigger(nameof(MemberModel.RightText), nameof(UnitW))]
    [Category(Category.Component), Index(3), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayW
    {
        get => WVisibility ? GetDisplayValue(3) : "0";
        set => WVisibility.If(true, () => SetDisplayValue(value, 3));
    }

    [Hidden]
    public bool WVisibility => ModelType?.Inherits<ColorModel4>() == true;

    double illuminant = CCT.GetTemperature((xy)WorkingProfile.Default.Chromacity);
    [Featured, MemberSetter(nameof(MemberModel.Format), Reflection.RangeFormat.Both), Index(-3), Range(2000.0, 12000.0, 100.0), StringFormat("N0"), View(Views.Profile), Visible]
    [Gradient("ff1c00", "FFF", "bbd0ff")]
    [MemberTrigger(nameof(MemberModel.RightText), nameof(IlluminantUnit))]
    public double Illuminant
    {
        get => illuminant;
        set => this.Change(ref illuminant, value);
    }

    [Hidden]
    public string IlluminantUnit => "K";

    NamableCategory<Type> model = null;
    [Hidden]
    public virtual NamableCategory<Type> Model
    {
        get => model;
        set => this.Change(ref model, value);
    }

    ListCollectionView models = ColorControl.GetModels();
    [Hidden]
    public ListCollectionView Models
    {
        get => models;
        private set => models = value;
    }

    [Hidden]
    public Type ModelType => Model?.Value ?? typeof(RGB);

    [Hidden]
    public Vector Maximum => WVisibility ? new(Colour.Components[ModelType][0].Maximum, Colour.Components[ModelType][1].Maximum, Colour.Components[ModelType][2].Maximum, Colour.Components[ModelType][3].Maximum) : new(Colour.Components[ModelType][0].Maximum, Colour.Components[ModelType][1].Maximum, Colour.Components[ModelType][2].Maximum);

    [Hidden]
    public Vector Minimum => WVisibility ? new(Colour.Components[ModelType][0].Minimum, Colour.Components[ModelType][1].Minimum, Colour.Components[ModelType][2].Minimum, Colour.Components[ModelType][3].Minimum) : new (Colour.Components[ModelType][0].Minimum, Colour.Components[ModelType][1].Minimum, Colour.Components[ModelType][2].Minimum);

    [Hidden]
    public string NameX => $"({Colour.Components[ModelType][0].Symbol}) {Colour.Components[ModelType][0].Name}";

    [Hidden]
    public string NameY => $"({Colour.Components[ModelType][1].Symbol}) {Colour.Components[ModelType][1].Name}";

    [Hidden]
    public string NameZ => $"({Colour.Components[ModelType][2].Symbol}) {Colour.Components[ModelType][2].Name}";

    [Hidden]
    public string NameW => WVisibility ? $"({Colour.Components[ModelType][3].Symbol}) {Colour.Components[ModelType][3].Name}" : "";

    [Hidden]
    public readonly ColorControlOptions Options;

    Primary3 primary = WorkingProfile.Default.Primary;
    [Assignable(nameof(DefaultPrimary))]
    [Category(Category.Primary), Index(0), Object(ObjectLevel.Low, ObjectLayout.Horizontal), View(Views.Profile), Visible]
    public Primary3 Primary
    {
        get => primary;
        set => this.Change(ref primary, value);
    }

    [Hidden]
    public object DefaultPrimary
    {
        get
        {
            var result = new ObservableCollection<Namable<Primary3>>();
            typeof(WorkingProfiles).GetProperties(BindingFlags.Public | BindingFlags.Static).ForEach(i => result.Add(new(i.GetDisplayName(), i.GetValue(null).As<WorkingProfile>().Primary)));
            return result;
        }
    }

    WorkingProfile profile = WorkingProfile.Default;
    [Hidden]
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

    [Hidden]
    public string UnitX => $"{Colour.Components[ModelType][0].Unit}";

    [Hidden]
    public string UnitY => $"{Colour.Components[ModelType][1].Unit}";

    [Hidden]
    public string UnitZ => $"{Colour.Components[ModelType][2].Unit}";

    [Hidden]
    public string UnitW => WVisibility ? $"{Colour.Components[ModelType][3].Unit}" : "";

    [Hidden]
    public Vector Value => new(x, y, z);

    CAM02.ViewingConditions viewingConditions = WorkingProfile.DefaultViewingConditions;
    [Category(Category.View), DisplayName("Conditions"), View(Views.Profile), Visible]
    [Object(ObjectLayout.Vertical)]
    public CAM02.ViewingConditions ViewingConditions
    {
        get => viewingConditions;
        set => this.Change(ref viewingConditions, value);
    }

    Vector3 white = (XYZ)(xyY)(xy)WorkingProfile.Default.Chromacity;
    [Category(Category.Chromacity), Index(-1), View(Views.Profile), Visible]
    [Object(ObjectLayout.Horizontal)]
    public Vector3 White
    {
        get => white;
        set => this.Change(ref white, value);
    }

    One x = default;
    [Hidden]
    public One X
    {
        get => x;
        set => this.Change(ref x, value);
    }

    One y = default;
    [Hidden]
    public One Y
    {
        get => y;
        set => this.Change(ref y, value);
    }

    One z = default;
    [Hidden]
    public One Z
    {
        get => z;
        set => this.Change(ref z, value);
    }

    One w = default;
    [Hidden]
    public One W
    {
        get => w;
        set => this.Change(ref w, value);
    }

    #endregion

    #region ColorModel

    public ColorViewModel(Color defaultColor, ColorControlOptions options) : base()
    {
        ActualColor = defaultColor;

        Options = options;
        Options.If(i => i.PropertyChanged += OnOptionsChanged);
    }

    #endregion

    #region Methods

    ColorModel GetColor()
    {
        if (WVisibility)
        {
            var result4 = M.Denormalize(new Vector4(x, y, z, w), Minimum, Maximum);
            return Colour.New(ModelType, result4[0], result4[1], result4[2], result4[3]);
        }

        var result3 = M.Denormalize(new Vector3(x, y, z), Minimum, Maximum);
        return Colour.New(ModelType, result3[0], result3[1], result3[2]);
    }

    //...

    string GetDisplayValue(int index)
    {
        One result = default;
        switch (index)
        {
            case 0:
                result = x;
                break;
            case 1:
                result = y;
                break;
            case 2:
                result = z;
                break;
            case 3:
                result = w;
                break;
        }

        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);
        return (Options?.ComponentNormalize == true ? (double)result : aRange.Convert(bRange.Minimum, bRange.Maximum, result)).Round(Options?.ComponentPrecision ?? 2).ToString();
    }

    void SetDisplayValue(string input, int index)
    {
        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);

        var result = Options?.ComponentNormalize == true ? (One)(input?.Double() ?? 0) : (One)bRange.Convert(aRange.Minimum, aRange.Maximum, input?.Double() ?? 0);
        switch (index)
        {
            case 0: X = result; break;
            case 1: Y = result; break;
            case 2: Z = result; break;
            case 3: W = result; break;
        }
    }

    //...

    /// <summary>
    /// Converts from <see cref="Model"/> to <see cref="RGB"/> based on <see cref="Component"/>.
    /// </summary>
    void ToColor()
    {
        handle.SafeInvoke(() =>
        {
            var color = GetColor();
            color.To(out RGB rgb, profile);

            ActualColor = XColor.Convert(rgb);
            OnColorChanged(ActualColor);
        });
    }

    /// <summary>
    /// Converts from <see cref="RGB"/> to <see cref="Model"/> based on <see cref="Component"/>.
    /// </summary>
    void FromColor()
    {
        handle.SafeInvoke((Action)(() =>
        {
            return;
            //ActualColor.Convert(out RGB a);
            //var b = Colour.New(ModelType, a, WorkingProfile.Default).Value / 255.0;
            //var c = new Vector3(b[0], b[1], b[2]);
            //X = c.X; Y = c.Y; Z = c.Z;
        }));
    }

    //...

    void OnColorChanged(Color color) => ColorChanged?.Invoke(this, new(color));

    void OnOptionsChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ColorControlOptions.ComponentNormalize):
            case nameof(ColorControlOptions.ComponentPrecision):
                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);
                this.Changed(() => DisplayW);
                break;
        }
    }

    void OnProfileChanged(Value<WorkingProfile> input)
    {
        if (input.Old != input.New)
        {
            var color = GetColor();
            Try.Invoke(() => color.Adapt(input.Old, input.New), e => Analytics.Log.Write<ColorViewModel>(e));
            //Now what...?
        }
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(ActualColor):
                FromColor();
                break;

            case nameof(Adapt):
            case nameof(Compress):
            case nameof(Primary):
            case nameof(ViewingConditions):
                Profile = new WorkingProfile(Primary, Chromacity, Compress, new(Adapt), ViewingConditions);
                break;

            case nameof(Chromacity):
                handle.SafeInvoke(() =>
                {
                    Illuminant = CCT.GetTemperature((xy)chromacity);
                    White = (XYZ)(xyY)(xy)chromacity;
                });
                goto case nameof(Compress);

            case nameof(Component):
                this.Changed(() => ActualComponent);

                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);
                this.Changed(() => DisplayW);

                this.Changed(() => NameX);
                this.Changed(() => NameY);
                this.Changed(() => NameZ);
                this.Changed(() => NameW);

                this.Changed(() => UnitX);
                this.Changed(() => UnitY);
                this.Changed(() => UnitZ);
                this.Changed(() => UnitW);
                break;

            case nameof(ComponentIndex):
                Component = ComponentIndex < 0 ? Component4.X : (Component4)componentIndex;
                break;

            case nameof(Illuminant):
                handle.SafeInvoke(() =>
                {
                    Chromacity = CCT.GetChromacity(illuminant);
                    White = (XYZ)(xyY)(xy)chromacity;
                });
                goto case nameof(Compress);

            case nameof(Model):
                this.Changed(() => ModelType);
                this.Changed(() => WVisibility);
                goto case nameof(Component);

            case nameof(ModelType):
                var cIndex = componentIndex;

                Components.Clear();
                Colour.Components[ModelType].Each((i, j) => { Components.Add(j); return j; });

                ComponentIndex = !WVisibility && ComponentIndex == 3 ? 2 : cIndex;
                break;

            case nameof(Profile):
                break;

            case nameof(White):
                handle.SafeInvoke(() =>
                {
                    Chromacity = (xy)(xyY)(XYZ)white;
                    Illuminant = CCT.GetTemperature((xy)chromacity);
                });
                goto case nameof(Primary);

            case nameof(X):
                this.Changed(() => DisplayX);
                ToColor();
                break;
            case nameof(Y):
                this.Changed(() => DisplayY);
                ToColor();
                break;
            case nameof(Z):
                this.Changed(() => DisplayZ);
                ToColor();
                break;
            case nameof(W):
                this.Changed(() => DisplayW);
                ToColor();
                break;
        }
    }

    #endregion
}