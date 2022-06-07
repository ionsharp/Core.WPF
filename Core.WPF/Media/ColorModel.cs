﻿using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

/// <summary>A normalized <see cref="ColorVector"/> used to convert between color spaces via user interface.</summary>
[DisplayName("Color")]
public class ColorModel : ViewModel
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

    IAdapt adapt = new VonKriesAdaptation();
    [Assignable(typeof(VonKriesAdaptation))]
    [Category(Category.Profile), Index(-1), Style(ObjectStyle.Default), Visible]
    public IAdapt Adapt
    {
        get => adapt;
        set => this.Change(ref adapt, value);
    }

    Components component = Components.X;
    [Index(1), Label(false), Tool]
    public Components Component
    {
        get => component;
        set => this.Change(ref component, value);
    }

    ICompress compress = WorkingProfile.DefaultCompression;
    [Assignable(typeof(GammaCompression), typeof(LCompression), typeof(Rec601Compression), typeof(Rec709Compression), typeof(Rec2100Compression), typeof(Rec2020Compression), typeof(sRGBCompression))]
    [Category(Category.Profile), Index(0), Style(ObjectStyle.Default), Visible]
    public ICompress Compress
    {
        get => compress;
        set => this.Change(ref compress, value);
    }

    [Hidden]
    public IList Models
    {
        get
        {
            var result = new ObservableCollection<Type>();
            ColorVector.Type.ForEach(i => result.Add(i.Value));
            return result;
        }
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
    [Category(Category.Profile), DisplayName("Primary (R)"), Index(1), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryRed
    {
        get => primaryRed;
        set => this.Change(ref primaryRed, value);
    }

    Vector2 primaryGreen = WorkingProfile.DefaultPrimary.Y;
    [Category(Category.Profile), DisplayName("Primary (G)"), Index(2), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryGreen
    {
        get => primaryGreen;
        set => this.Change(ref primaryGreen, value);
    }

    Vector2 primaryBlue = WorkingProfile.DefaultPrimary.Z;
    [Category(Category.Profile), DisplayName("Primary (B)"), Index(3), Style(ObjectStyle.Default), Visible]
    public Vector2 PrimaryBlue
    {
        get => primaryBlue;
        set => this.Change(ref primaryBlue, value);
    }

    Vector2 white = WorkingProfile.DefaultWhite;
    [Category(Category.Profile), Index(4), Style(ObjectStyle.Default), Visible]
    public Vector2 White
    {
        get => white;
        set => this.Change(ref white, value);
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
    public string NameX => $"({ColorVector.GetComponent(ModelType, 0).Symbol}) {ColorVector.GetComponent(ModelType, 0).Name}";

    [Hidden]
    public string NameY => $"({ColorVector.GetComponent(ModelType, 1).Symbol}) {ColorVector.GetComponent(ModelType, 1).Name}";

    [Hidden]
    public string NameZ => $"({ColorVector.GetComponent(ModelType, 2).Symbol}) {ColorVector.GetComponent(ModelType, 2).Name}";

    [Hidden]
    public string UnitX => $"{ColorVector.GetComponent(ModelType, 0).Unit}";

    [Hidden]
    public string UnitY => $"{ColorVector.GetComponent(ModelType, 1).Unit}";

    [Hidden]
    public string UnitZ => $"{ColorVector.GetComponent(ModelType, 2).Unit}";

    //...

    [Hidden]
    public Vector Maximum => new(ColorVector.GetComponent(ModelType, 0).Maximum, ColorVector.GetComponent(ModelType, 1).Maximum, ColorVector.GetComponent(ModelType, 2).Maximum);

    [Hidden]
    public Vector Minimum => new(ColorVector.GetComponent(ModelType, 0).Minimum, ColorVector.GetComponent(ModelType, 1).Minimum, ColorVector.GetComponent(ModelType, 2).Minimum);

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

    public ColorModel(Color defaultColor, Action<Color> onChanged = null) : base()
    {
        ActualColor = defaultColor; OnChanged = onChanged;
    }

    #endregion

    #region Methods

    ColorVector GetColor()
    {
        Vector3<double> result = default;
        if (Component == Components.X)
            result = new(z, x, y);

        if (Component == Components.Y)
            result = new(x, z, y);

        if (Component == Components.Z)
            result = new(x, y, z);

        return ColorVector.New(ModelType, result);
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
        return aRange.Convert(bRange.Minimum, bRange.Maximum, result).Round(2).ToString();
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
            var rgb = Xrgb.Convert(ActualColor);

            var result = ColorVector.New(ModelType, rgb, WorkingProfile.Default.sRGB).Value;
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
                //ConvertFrom();
                break;

            case nameof(Compress):
            case nameof(PrimaryRed):
            case nameof(PrimaryGreen):
            case nameof(PrimaryBlue):
            case nameof(White):
                Profile = new WorkingProfile(PrimaryRed, PrimaryGreen, PrimaryBlue, White, Compress);
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