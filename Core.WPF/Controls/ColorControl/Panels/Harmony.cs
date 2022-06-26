using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using static System.Math;

namespace Imagin.Core.Controls;

[Explicit]
public class ColorHarmonyPanel : Panel
{
    public enum Steps
    {
        Darkness,
        Lightness,
        Saturation,
        Desaturation,
        Random
    }

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

    int count = 4;
    [Range(2, 256, 1), Setter(nameof(MemberModel.RightText), "colors"), SliderUpDown, Tool, Visible]
    public int Count
    {
        get => count;
        set => this.Change(ref count, value);
    }

    Harmony harmony = Harmony.Monochromatic;
    [Feature, Label(false), Localize(false), Tool, Visible]
    public Harmony Harmony
    {
        get => harmony;
        set => this.Change(ref harmony, value);
    }

    double range = 1;
    [Range(0.0, 1.0, 0.01), SliderUpDown, Tool, Visible]
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

    Steps step = Steps.Random;
    [Tool, Visible]
    public Steps Step
    {
        get => step;
        set => this.Change(ref step, value);
    }

    //...

    public ColorHarmonyPanel(ColorControl control) : base() 
    {
        Control = control;
        Control.ActiveDocumentChanged += OnActiveDocumentChanged;
    }

    //...

    void ApplySteps(double h, double s, double l, int count)
    {
        h = h < 0 ? 359 - Abs(h) : h;
        h = h > 359 ? h - 359 : h;

        var ss = (1 - s) / count;
        var sd = s / count;

        var ll = (1 - l) / count;
        var ld = l / count;

        for (var i = 0; i < count; i++)
        {
            double j(double k) => (Step == Steps.Random ? Numerics.Random.NextDouble() : k) * 100;

            var hsl = Colour.New<HSL>(h, j(s), j(l));
            hsl.To(out RGB rgb, WorkingProfile.Default);
            Colors.Add(XColor.Convert(rgb));

            switch (Step)
            {
                case Steps.Darkness:
                    l -= ld; break;
                case Steps.Lightness:
                    l += ll; break;
                case Steps.Saturation:
                    s += ss; break;
                case Steps.Desaturation:
                    s -= sd; break;
            }
        }
    }

    void Update()
    {
        Colors.Clear();
        if (ActiveDocument == null)
            return;

        var color = ActiveDocument.Color.ActualColor;
        double h = color.GetHue(), saturation = color.GetSaturation(), lightness = color.GetBrightness();

        double[] hues = null;
        switch (Harmony)
        {
            case Harmony.Analogous:
                hues = reverse ? new double[] { h, h - range * 30, h - range * 60 } : new double[] { h, h + range * 30, h + range * 60 };
                break;

            case Harmony.Diad:
                hues = reverse ? new double[] { h, h + range * 180 } : new double[] { h, h + range * 180 };
                break;

            case Harmony.Monochromatic:
                hues = reverse ? new double[] { h } : new double[] { h };
                break;

            case Harmony.DoubleComplementary:
                hues = reverse ? new double[] { h, h - range * 30, h - range * 180, h - range * 210 } : new double[] { h, h + range * 30, h + range * 180, h + range * 210 };
                break;

            case Harmony.SplitComplementary:
                hues = reverse ? new double[] { h, h + range * 150, h - range * 150 } : new double[] { h, h + range * 150, h - range * 150 };
                break;

            case Harmony.Square:
                hues = reverse ? new double[] { h, h - range * 90, h - range * 180, h - range * 270 } : new double[] { h, h + range * 90, h + range * 180, h + range * 270 };
                break;

            case Harmony.Tetrad:
                hues = reverse ? new double[] { h, h - range * 60, h - range * 180, h - range * 240 } : new double[] { h, h + range * 60, h + range * 180, h + range * 240 };
                break;

            case Harmony.Triad:
                hues = reverse ? new double[] { h, h - range * 120, h - range * 240 } : new double[] { h, h + range * 120, h + range * 240 };
                break;

            case Harmony.CoolWarm:

                double hx = h;
                double hy = range * 360; hy = hy > Count ? Count : hy;
                double hz = hy / Count;

                List<double> other = new();
                for (double i = 0; i < Count; i++)
                {
                    other.Add(hx);
                    hx = reverse ? hx - hz : hx + hz;

                    int w = 135, c = 315, m = 359;
                    bool warm = h < w || h > c; bool cool = !warm;

                    if (cool)
                        hx = hx < w ? c : hx > c ? w : hx;

                    if (warm)
                    {
                        hx = hx < 0 ? m : hx > m ? 0 : (reverse ? hx < c && hx > w ? w : hx : hx > w && hx < c ? c : hx);
                    }
                }
                hues = other.ToArray();
                break;
        }

        var portion = count / hues.Length;

        foreach (var i in hues)
            ApplySteps(i, saturation, lightness, portion);
    }

    //...

    void OnActiveDocumentChanged(object sender, EventArgs<ColorDocument> e)
    {
        if (ActiveDocument != null)
            ActiveDocument.ColorChanged -= OnColorChanged;

        if (e.Value != null)
        {
            ActiveDocument = e.Value;
            ActiveDocument.ColorChanged += OnColorChanged;
        }

        Update();
    }

    void OnColorChanged(object sender, EventArgs<Color> e)
    {
        Update();
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Count):
            case nameof(Harmony):
            case nameof(Reverse):
            case nameof(Step):
                Update();
                break;
        }
    }
}