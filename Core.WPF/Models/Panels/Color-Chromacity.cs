using Imagin.Core.Colors;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imagin.Core.Controls;

[Name("Chromacity"), Explicit]
[Serializable]
public class ColorChromacityPanel : Panel
{
    readonly Handle handle = false;

    [Assign(nameof(DefaultChromacity)), Editable, Horizontal, Index(0), Show]
    public Vector2 Chromacity { get => Get(WorkingProfile.DefaultWhite); set => Set(value); }

    [Hide]
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

    public override Uri Icon => Resource.GetImageUri(SmallImages.Thermometer);

    [Pin(Pin.AboveOrLeft), Range(2000.0, 12000.0, 100.0, Style = RangeStyle.Both), RangeGradient("ff1c00", "FFF", "bbd0ff"), RightText("K"), StringFormat("N0"), Show]
    public double Illuminant { get => Get(CCT.GetTemperature((xy)WorkingProfile.Default.Chromacity)); set => Set(value); }

    public override string Title => "Chromacity";

    [Editable, Horizontal, Index(1), Show]
    public Vector3 White { get => Get(Vector3.One); set => Set(value); }

    public ColorChromacityPanel() : base() { }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Chromacity):
                handle.SafeInvoke(() =>
                {
                    Illuminant = CCT.GetTemperature((xy)Chromacity);
                    White = (XYZ)(xyY)(xy)Chromacity;
                });
                break;

            case nameof(Illuminant):
                handle.SafeInvoke(() =>
                {
                    Chromacity = CCT.GetChromacity(Illuminant);
                    White = (XYZ)(xyY)(xy)Chromacity;
                });
                break;

            case nameof(White):
                handle.SafeInvoke(() =>
                {
                    Chromacity = (xy)(xyY)(XYZ)White;
                    Illuminant = CCT.GetTemperature((xy)Chromacity);
                });
                break;
        }
    }
}