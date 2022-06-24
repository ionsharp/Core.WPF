using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Imagin.Core.Controls;

[MemberVisibility(Property: MemberVisibility.Explicit)]
public class ColorHarmonyPanel : Panel
{
    public override Uri Icon => Resources.InternalImage(Images.ColorWheel);

    public override string Title => "Harmony";

    public readonly ColorControl Control;

    public ColorDocument ActiveDocument { get; private set; }

    ObservableCollection<Color> colors = new();
    public ObservableCollection<Color> Colors
    {
        get => colors;
        set => this.Change(ref colors, value);
    }

    int count = 20;
    [Setter(nameof(MemberModel.Format), RangeFormat.Both), Setter(nameof(MemberModel.RightText), "colors"), Range(2, 256, 1), Tool, Visible]
    public int Count
    {
        get => count;
        set => this.Change(ref count, value);
    }

    Harmony harmony = Harmony.Monochromatic;
    [Featured, Label(false), Localize(false), Tool, Visible]
    public Harmony Harmony
    {
        get => harmony;
        set => this.Change(ref harmony, value);
    }

    double range = 0.1;
    [Setter(nameof(MemberModel.Format), RangeFormat.Slider), Setter(nameof(MemberModel.RightText), "%"), Range(0.0, 1.0, 0.01), Tool, Visible]
    public double Range
    {
        get => range;
        set => this.Change(ref range, value);
    }

    bool reverse = false;
    [Tool, Visible]
    public bool Reverse
    {
        get => reverse;
        set => this.Change(ref reverse, value);
    }

    public ColorHarmonyPanel(ColorControl control) : base() 
    {
        Control = control;
        Control.ActiveDocumentChanged += OnActiveDocumentChanged;
    }

    void Refresh()
    {
        Colors.Clear();
        if (ActiveDocument == null)
            return;

        var color = ActiveDocument.Color.ActualColor;
        double h = color.GetHue(), saturation = color.GetSaturation(), lightness = color.GetBrightness();
        switch (Harmony)
        {
            case Harmony.Analogous:
            case Harmony.WarmCold:

                double hx = h;
                double hy = range * 360;
                double hz = hy * 1.0 / (Count - 1.0);

                for (double i = 0; i < Count; i++)
                {
                    var hsl = Colour.New<HSL>(hx, saturation * 100, lightness * 100);
                    hsl.To(out RGB rgb, WorkingProfile.Default);

                    Colors.Add(XColor.Convert(rgb));
                    hx = reverse ? hx - hz : hx + hz;

                    if (Harmony == Harmony.Analogous)
                    {
                        hx = hx < 0 ? 359 : hx;
                        hx = hx > 359 ? 0 : hx;
                    }
                    if (Harmony == Harmony.WarmCold)
                    {
                        int w = 135, c = 315, m = 359;
                        bool warm = h < w || h > c; bool cool = !warm;

                        if (cool)
                            hx = hx < w ? c : hx > c ? w : hx;

                        if (warm)
                        {
                            hx = hx < 0 ? m : hx > m ? 0 : (reverse ? hx < c && hx > w ? w : hx : hx > w && hx < c ? c : hx );
                        }
                    }
                }
                break;

            case Harmony.Complementary:
                break;

            case Harmony.Monochromatic:
                //If not black/white add two additional colors to replace black/white later
                double count = Count.Double(); count = lightness > 0 && lightness < 1 ? count + 2 : count;

                for (double i = 0; i < count; i++)
                {
                    //If not black/white, ignore black/white
                    if (lightness > 0 && lightness < 1 && (i == 0 || i == count - 1))
                        continue;

                    double s = 100;
                    double l = i / (count - 1.0) * 100.0;

                    if (i > count / 2.0)
                        s = l;

                    var hsl = Colour.New<HSL>(h, s, l);
                    hsl.To(out RGB rgb, WorkingProfile.Default);

                    Colors.Add(XColor.Convert(rgb));
                }
                break;

            case Harmony.SplitComplementary:
                break;

            case Harmony.Square:
                break;

            case Harmony.Tetradic:
                break;

            case Harmony.Triadic:

                break;
        }
    }

    void OnActiveDocumentChanged(object sender, EventArgs<ColorDocument> e)
    {
        if (ActiveDocument != null)
            ActiveDocument.ColorChanged -= OnColorChanged;

        if (e.Value != null)
        {
            ActiveDocument = e.Value;
            ActiveDocument.ColorChanged += OnColorChanged;
        }

        Refresh();
    }

    void OnColorChanged(object sender, EventArgs<Color> e)
    {
        Refresh();
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Count):
            case nameof(Harmony):
            case nameof(Range):
            case nameof(Reverse):
                Refresh();
                break;
        }
    }
}