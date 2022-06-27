using Imagin.Core.Colors;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imagin.Core.Controls;

[DisplayName("Chromacity"), Explicit]
[Serializable]
public class ColorChromacityPanel : Panel
{
    readonly Handle handle = false;

    Vector2 chromacity = WorkingProfile.DefaultWhite;
    [Assignable(nameof(DefaultChromacity)), Horizontal, Index(0), Visible]
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

    public override Uri Icon => Resources.InternalImage(Images.Thermometer);

    double illuminant = CCT.GetTemperature((xy)WorkingProfile.Default.Chromacity);
    [Feature, Gradient("ff1c00", "FFF", "bbd0ff"), Range(2000.0, 12000.0, 100.0), StringFormat("N0"), Visible]
    [Setter(nameof(MemberModel.RightText), "K"), SliderUpDown]
    public double Illuminant
    {
        get => illuminant;
        set => this.Change(ref illuminant, value);
    }

    public override string Title => "Chromacity";

    Vector3 white = Vector3.One;
    [Horizontal, Index(1), Visible]
    public Vector3 White
    {
        get => white;
        set => this.Change(ref white, value);
    }

    public ColorChromacityPanel() : base() { }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Chromacity):
                handle.SafeInvoke(() =>
                {
                    Illuminant = CCT.GetTemperature((xy)chromacity);
                    White = (XYZ)(xyY)(xy)chromacity;
                });
                break;

            case nameof(Illuminant):
                handle.SafeInvoke(() =>
                {
                    Chromacity = CCT.GetChromacity(illuminant);
                    White = (XYZ)(xyY)(xy)chromacity;
                });
                break;

            case nameof(White):
                handle.SafeInvoke(() =>
                {
                    Chromacity = (xy)(xyY)(XYZ)white;
                    Illuminant = CCT.GetTemperature((xy)chromacity);
                });
                break;
        }
    }
}