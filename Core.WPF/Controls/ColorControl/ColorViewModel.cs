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
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

/// <summary>A normalized <see cref="ColorModel"/> used to perform conversion via user interface.</summary>
[DisplayName("Color")]
public class ColorViewModel : ViewModel
{
    enum Category { Component, Profile }

    public event DefaultEventHandler<Color> ColorChanged;

    #region Properties

    readonly Handle handle = false;

    //...

    Color actualColor = System.Windows.Media.Colors.White;
    [DisplayName("Color"), Feature, Hexadecimal, Label(false), Visible]
    public Color ActualColor
    {
        get => actualColor;
        set => this.Change(ref actualColor, value);
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

    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameX))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitX))]
    [Category(Category.Component), Index(0), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayX
    {
        get => GetDisplayValue(0);
        set => SetDisplayValue(value, 0);
    }

    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameY))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitY))]
    [Category(Category.Component), Index(1), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayY
    {
        get => GetDisplayValue(1);
        set => SetDisplayValue(value, 1);
    }

    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameZ))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitZ))]
    [Category(Category.Component), Index(2), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayZ
    {
        get => GetDisplayValue(2);
        set => SetDisplayValue(value, 2);
    }

    [Setter(nameof(MemberModel.ClearText), false)]
    [Trigger(nameof(MemberModel.DisplayName), nameof(NameW))]
    [Trigger(nameof(MemberModel.IsVisible), nameof(WVisibility))]
    [Trigger(nameof(MemberModel.RightText), nameof(UnitW))]
    [Category(Category.Component), Index(3), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayW
    {
        get => WVisibility ? GetDisplayValue(3) : "0";
        set => WVisibility.If(true, () => SetDisplayValue(value, 3));
    }

    [Hidden]
    public bool WVisibility => ModelType?.Inherits<ColorModel4>() == true;

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

    int profileIndex = 0;
    [Index(1), Label(false), SelectedIndex, Tool, Visible]
    [Setter(nameof(MemberModel.ItemPath), "Name")]
    [Trigger(nameof(MemberModel.ItemSource), nameof(SelectedProfileGroup))]
    public int ProfileIndex
    {
        get => profileIndex;
        set => this.Change(ref profileIndex, value);
    }

    int profileGroupIndex = 0;
    [DisplayName("Profile"), Index(0), SelectedIndex, Tool, Visible]
    [Setter(nameof(MemberModel.ItemPath), "Name")]
    [Trigger(nameof(MemberModel.ItemSource), nameof(ProfileGroups))]
    public int ProfileGroupIndex
    {
        get => profileGroupIndex;
        set => this.Change(ref profileGroupIndex, value);
    }

    [Hidden]
    public object SelectedProfileGroup
    {
        get
        {
            if (Options != null)
            {
                if (ProfileGroupIndex >= 0 && ProfileGroupIndex < Options.Profiles.Count)
                    return Options.Profiles[ProfileGroupIndex];
            }
            return null;
        }
    }

    [Hidden]
    public object ProfileGroups => Options?.Profiles;

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

    #region ColorViewModel

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
        handle.SafeInvoke(() =>
        {
            ActualColor.Convert(out RGB rgb);

            var result = Colour.New(ModelType, rgb, Profile);

            if (result is ColorModel3 result3)
            {
                var color3 = result3.Normalize();
                X = color3.X; Y = color3.Y; Z = color3.Z;
            }

            else if (result is ColorModel4 result4)
            {
                var color4 = result4.Normalize();
                X = color4.X; Y = color4.Y; Z = color4.Z; W = color4.W;
            }

            OnColorChanged(ActualColor);
        });
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
            #region ActualColor
            case nameof(ActualColor):
                FromColor();
                break;
            #endregion

            #region Component
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
            #endregion

            #region ComponentIndex
            case nameof(ComponentIndex):
                Component = ComponentIndex < 0 ? Component4.X : (Component4)componentIndex;
                break;
            #endregion

            #region Model
            case nameof(Model):
                this.Changed(() => ModelType);
                this.Changed(() => WVisibility);
                goto case nameof(Component);
            #endregion

            #region ModelType
            case nameof(ModelType):
                var cIndex = componentIndex;

                Components.Clear();
                Colour.Components[ModelType].Each((i, j) => { Components.Add(j); return j; });

                ComponentIndex = !WVisibility && ComponentIndex == 3 ? 2 : cIndex;
                break;
            #endregion

            #region ProfileGroupIndex
            case nameof(ProfileGroupIndex):
                this.Changed(() => SelectedProfileGroup);
                break;
            #endregion

            #region ProfileIndex
            case nameof(ProfileIndex):
                if (SelectedProfileGroup is IList selectedProfileGroup)
                {
                    if (ProfileIndex >= 0 && ProfileIndex < selectedProfileGroup.Count)
                    {
                        if (selectedProfileGroup[ProfileIndex] is NamableProfile selectedProfile)
                            Profile = selectedProfile.Value;
                    }
                }
                break;
            #endregion

            #region X, Y, Z, w
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
            #endregion
        }
    }

    #endregion
}