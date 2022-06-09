using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

/// <summary>A normalized <see cref="ColorModel"/> used to convert between color spaces via user interface.</summary>
[DisplayName("Color")]
public class ColorViewModel : ViewModel
{
    enum Category { Component, Profile }

    #region Properties

    readonly Handle handle = false;

    readonly Action<Color> OnChanged;

    //...

    Color actualColor = System.Windows.Media.Colors.White;
    [Format(ColorFormat.TextBox), DisplayName("Color")]
    [Featured, Label(false), Visible]
    public Color ActualColor
    {
        get => actualColor;
        set => this.Change(ref actualColor, value);
    }

    Components component = Components.X;
    [Index(1), Label(false), Tool]
    public Components Component
    {
        get => component;
        set => this.Change(ref component, value);
    }

    int componentPrecision = 2;
    [Hidden]
    public int ComponentPrecision
    {
        get => componentPrecision;
        set => this.Change(ref componentPrecision, value);
    }

    int model = 0;
    [Label(false)]
    [Localize(false)]
    [Index(0)]
    [Source(nameof(Models), nameof(Type.Name))]
    [Style(Int32Style.Index)]
    [Tool]
    public virtual int Model
    {
        get => model;
        set => this.Change(ref model, value);
    }

    [Hidden]
    public IList Models => ColorModel.Types;

    [Hidden]
    public Type ModelType
    {
        get => Model >= 0 ? (Type)Models[Model] : null;
        set
        {
            if (value != null)
            {
                var i = 0;
                foreach (var j in Models)
                {
                    if (j.Equals(value))
                    {
                        Model = i;
                        break;
                    }
                    i++;
                }
            }
        }
    }

    WorkingProfile profile = WorkingProfile.Default.sRGB;
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

    Vector2 primaryRed = WorkingProfile.DefaultPrimary.X;
    [Assignable(nameof(RedPrimaries))]
    [Category(Category.Profile), DisplayName("Primary (R)"), Index(0), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryRed
    {
        get => primaryRed;
        set => this.Change(ref primaryRed, value);
    }

    [Hidden]
    public object RedPrimaries
    {
        get
        {
            var result = new ObservableCollection<Namable<Vector2>>();
            typeof(WorkingProfile.Default).GetProperties(BindingFlags.Public | BindingFlags.Static).ForEach(i => result.Add(new(i.GetDisplayName(), i.GetValue(null).As<WorkingProfile>().Primary.R)));
            return result;
        }
    }

    Vector2 primaryGreen = WorkingProfile.DefaultPrimary.Y;
    [Assignable(nameof(GreenPrimaries))]
    [Category(Category.Profile), DisplayName("Primary (G)"), Index(1), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryGreen
    {
        get => primaryGreen;
        set => this.Change(ref primaryGreen, value);
    }

    [Hidden]
    public object GreenPrimaries
    {
        get
        {
            var result = new ObservableCollection<Namable<Vector2>>();
            typeof(WorkingProfile.Default).GetProperties(BindingFlags.Public | BindingFlags.Static).ForEach(i => result.Add(new(i.GetDisplayName(), i.GetValue(null).As<WorkingProfile>().Primary.G)));
            return result;
        }
    }

    Vector2 primaryBlue = WorkingProfile.DefaultPrimary.Z;
    [Assignable(nameof(BluePrimaries))]
    [Category(Category.Profile), DisplayName("Primary (B)"), Index(2), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryBlue
    {
        get => primaryBlue;
        set => this.Change(ref primaryBlue, value);
    }

    [Hidden]
    public object BluePrimaries
    {
        get
        {
            var result = new ObservableCollection<Namable<Vector2>>();
            typeof(WorkingProfile.Default).GetProperties(BindingFlags.Public | BindingFlags.Static).ForEach(i => result.Add(new(i.GetDisplayName(), i.GetValue(null).As<WorkingProfile>().Primary.B)));
            return result;
        }
    }

    ITransfer transfer = WorkingProfile.DefaultTransfer;
    [Assignable(typeof(GammaTransfer), typeof(LGammaTransfer), typeof(LinearTransfer), typeof(PQTransfer), typeof(Rec709Transfer), typeof(Rec2020Transfer), typeof(sRGBTransfer))]
    [Category(Category.Profile), Index(3), Style(ObjectStyle.Default), Visible]
    public ITransfer Transfer
    {
        get => transfer;
        set => this.Change(ref transfer, value);
    }

    DoubleMatrix transform = new(LMS.Transform.Bradford.As<IMatrix>().Values);
    [Assignable(nameof(Transforms))]
    [Category(Category.Profile), Index(4), Visible]
    public DoubleMatrix Transform
    {
        get => transform;
        set => this.Change(ref transform, value);
    }

    [Hidden]
    public object Transforms
    {
        get
        {
            var result = new ObservableCollection<Namable<DoubleMatrix>>();
            foreach (var i in typeof(LMS.Transform).GetProperties(BindingFlags.Public | BindingFlags.Static))
                result.Add(new(i.Name, new DoubleMatrix(i.GetValue(null).As<IMatrix>().Values)));

            return result;
        }
    }

    Vector2 white = WorkingProfile.DefaultWhite;
    [Assignable(nameof(Whites))]
    [Category(Category.Profile), DisplayName("Illuminant"), Index(-1), Style(ObjectStyle.Default), Visible]
    public Vector2 White
    {
        get => white;
        set => this.Change(ref white, value);
    }

    [Hidden]
    public object Whites
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

    //...

    [PropertyTrigger(nameof(Controls.MemberModel.RightText), nameof(UnitX))]
    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameX))]
    [Category(Category.Component), Clear(false), Index(0), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayX
    {
        get => GetDisplayValue(0);
        set => SetDisplayValue(value, 0);
    }

    [PropertyTrigger(nameof(Controls.MemberModel.RightText), nameof(UnitY))]
    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameY))]
    [Category(Category.Component), Clear(false), Index(1), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayY
    {
        get => GetDisplayValue(1);
        set => SetDisplayValue(value, 1);
    }

    [PropertyTrigger(nameof(Controls.MemberModel.RightText), nameof(UnitZ))]
    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameZ))]
    [Category(Category.Component), Clear(false), Index(2), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayZ
    {
        get => GetDisplayValue(2);
        set => SetDisplayValue(value, 2);
    }

    //...

    [Hidden]
    public string NameX => $"({ColorModel.GetComponent(ModelType, 0).Symbol}) {ColorModel.GetComponent(ModelType, 0).Name}";

    [Hidden]
    public string NameY => $"({ColorModel.GetComponent(ModelType, 1).Symbol}) {ColorModel.GetComponent(ModelType, 1).Name}";

    [Hidden]
    public string NameZ => $"({ColorModel.GetComponent(ModelType, 2).Symbol}) {ColorModel.GetComponent(ModelType, 2).Name}";

    [Hidden]
    public string UnitX => $"{ColorModel.GetComponent(ModelType, 0).Unit}";

    [Hidden]
    public string UnitY => $"{ColorModel.GetComponent(ModelType, 1).Unit}";

    [Hidden]
    public string UnitZ => $"{ColorModel.GetComponent(ModelType, 2).Unit}";

    //...

    [Hidden]
    public Vector Maximum => new(ColorModel.GetComponent(ModelType, 0).Maximum, ColorModel.GetComponent(ModelType, 1).Maximum, ColorModel.GetComponent(ModelType, 2).Maximum);

    [Hidden]
    public Vector Minimum => new(ColorModel.GetComponent(ModelType, 0).Minimum, ColorModel.GetComponent(ModelType, 1).Minimum, ColorModel.GetComponent(ModelType, 2).Minimum);

    [Hidden]
    public Vector Value => new(x, y, z);

    //...

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

    #endregion

    #region ColorModel

    public ColorViewModel(Color defaultColor, Action<Color> onChanged = null) : base()
    {
        ActualColor = defaultColor; OnChanged = onChanged;
    }

    #endregion

    #region Methods

    ColorModel GetColor()
    {
        Vector3 result = default;
        if (Component == Components.X)
            result = new(z, x, y);

        if (Component == Components.Y)
            result = new(x, z, y);

        if (Component == Components.Z)
            result = new(x, y, z);

        return ColorModel.New(ModelType, result);
    }

    string GetDisplayValue(int index)
    {
        One result = default;
        switch (Component)
        {
            case Components.X:
                var a = new One[] { z, x, y };
                result = a[index];
                break;
            case Components.Y:
                var b = new One[] { x, z, y };
                result = b[index];
                break;
            case Components.Z:
                var c = new One[] { x, y, z };
                result = c[index];
                break;
        }

        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);
        return aRange.Convert(bRange.Minimum, bRange.Maximum, result).Round(ComponentPrecision).ToString();
    }

    void SetDisplayValue(string input, int index)
    {
        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);

        var result = (One)bRange.Convert(aRange.Minimum, aRange.Maximum, input?.Double() ?? 0);
        switch (Component)
        {
            case Components.X:
                switch (index)
                {
                    case 0: Z = result; break;
                    case 1: X = result; break;
                    case 2: Y = result; break;
                }
                break;
            case Components.Y:
                switch (index)
                {
                    case 0: X = result; break;
                    case 1: Z = result; break;
                    case 2: Y = result; break;
                }
                break;
            case Components.Z:
                switch (index)
                {
                    case 0: X = result; break;
                    case 1: Y = result; break;
                    case 2: Z = result; break;
                }
                break;
        }

        switch (index)
        {
            case 0: this.Changed(() => DisplayX); break;
            case 1: this.Changed(() => DisplayY); break;
            case 2: this.Changed(() => DisplayZ); break;
        }
    }

    //... ActualColor <Binding> [X, Y, Z]

    /// <summary>
    /// Converts from <see cref="Model"/> to <see cref="RGB"/> based on <see cref="Component"/>.
    /// </summary>
    void ConvertTo()
    {
        handle.SafeInvoke(() =>
        {
            var xyz = GetColor();
            var rgb = xyz.ToRGB(profile);

            ActualColor = XColor.Convert(rgb);
        });
    }

    /// <summary>
    /// Converts from <see cref="RGB"/> to <see cref="Model"/> based on <see cref="Component"/>.
    /// </summary>
    void ConvertFrom()
    {
        handle.SafeInvoke((Action)(() =>
        {
            return;
            var rgb = Xrgb.Convert(ActualColor);

            var result = ColorModel.New(ModelType, rgb, WorkingProfile.Default.sRGB).Value;
            var xyz = new Vector3<double>(result[(int)0] / 255, result[(int)1] / 255, result[(int)2] / 255);

            switch (Component)
            {
                case Components.X: X = xyz.Y; Y = xyz.Z; Z = xyz.X; break;
                case Components.Y: X = xyz.X; Y = xyz.Z; Z = xyz.Y; break;
                case Components.Z: X = xyz.X; Y = xyz.Y; Z = xyz.Z; break;
            }
        }));
    }

    //...

    void OnProfileChanged(Value<WorkingProfile> input)
    {
        if (input.Old != input.New)
        {
            var xyz = GetColor();
            xyz.Adapt(input.Old, input.New);

            //Now what...?
        }
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(ActualColor):
                //OnChanged?.Invoke(ActualColor);
                ConvertFrom();
                break;

            case nameof(ComponentPrecision):
                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);
                break;

            case nameof(PrimaryRed):
            case nameof(PrimaryGreen):
            case nameof(PrimaryBlue):
            case nameof(Transfer):
            case nameof(Transform):
            case nameof(White):
                Profile = new WorkingProfile(PrimaryRed, PrimaryGreen, PrimaryBlue, White, Transfer, new(Transform));
                break;

            case nameof(Component):
                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);

                this.Changed(() => NameX);
                this.Changed(() => NameY);
                this.Changed(() => NameZ);

                this.Changed(() => UnitX);
                this.Changed(() => UnitY);
                this.Changed(() => UnitZ);
                break;

            case nameof(Model):
                this.Changed(() => ModelType);
                goto case nameof(Component);

            case nameof(Profile):
                break;

            case nameof(X):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayY); break;
                    case Components.Y: this.Changed(() => DisplayX); break;
                    case Components.Z: this.Changed(() => DisplayX); break;
                }
                ConvertTo();
                break;

            case nameof(Y):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayZ); break;
                    case Components.Y: this.Changed(() => DisplayZ); break;
                    case Components.Z: this.Changed(() => DisplayY); break;
                }
                ConvertTo();
                break;

            case nameof(Z):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayX); break;
                    case Components.Y: this.Changed(() => DisplayY); break;
                    case Components.Z: this.Changed(() => DisplayZ); break;
                }
                ConvertTo();
                break;
        }
    }

    #endregion
}