using Imagin.Core.Collections.Concurrent;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Threading;
using NLog.Time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace Imagin.Core.Controls;

[Explicit, Image(SmallImages.ColorWheel), Name("Harmony")]
public class ColorHarmonyPanel : ViewPanel
{
    enum Category { Components, Harmony, Model, Randomize, Step }

    [Flags]
    public enum TargetComponents { [Hide]None = 0, Y = 1, Z = 2, [Hide]Both = Y | Z }

    public enum Steps { Increase, Decrease, Both }

    public event DefaultEventHandler<Color> Picked;

    public event DefaultEventHandler<Color[]> Saved;

    [NonSerialized]
    public readonly ColorView Control;

    public ColorDocument ActiveDocument { get; private set; }

    public ConcurrentCollection<Color> Colors { get => Get(new ConcurrentCollection<Color>()); set => Set(value); }

    //
    [Category(Category.Components), Header, Index(-9), Show]
    [Style(EnumStyle.Flags)]
    public TargetComponents Components { get => Get(TargetComponents.Y); set => Set(value); }

    [Header, HideName, Index(-8), Name(nameof(Components)), Show, StringStyle(Format = Text.Formats.Raw)]
    public string ComponentNames
    {
        get
        {
            var result = "None";
            if (Models != null && ModelIndex >= 0 && ModelIndex < Models.Count)
            {
                var components = Models[ModelIndex].GetAttributes<ComponentAttribute>().Select(i => i.Info).Where(i => i.Name != "Hue").ToArray();
                if (Components == TargetComponents.Both)
                {
                    result = $"{components[0].Name}, {components[1].Name}";
                }
                else if (Components == TargetComponents.None)
                {
                }
                else if (Components.HasFlag(TargetComponents.Y))
                {
                    result = $"{components[0].Name}";
                }
                else if (Components.HasFlag(TargetComponents.Z))
                {
                    result = $"{components[1].Name}";
                }
            }
            return $"({result})";
        }
    }

    [Header, Range(2, 256, 1, Style = RangeStyle.UpDown), RightText("colors"), Show]
    public int Count { get => Get(7); set => Set(value); }

    [Category(Category.Harmony), Header, Index(-11), Show]
    public Harmony Harmony { get => Get(Harmony.Monochromatic); set => Set(value); }

    [Category(Category.Model), Header, Index(-10), Name("Model"), Show]
    [Int32Style(Int32Style.Index, nameof(Models), nameof(Type.Name))]
    public int ModelIndex { get => Get(0); set => Set(value); }

    ObservableCollection<Type> models = new ObservableCollection<Type>(XAssembly.GetDerivedTypes<ColorModel3>(AssemblyType.Color, true).Where(i => i.GetAttribute<ClassAttribute>()?.Class.HasFlag(Class.H) == true));
    [Hide]
    public ObservableCollection<Type> Models => models;

    [Category(Category.Randomize), Header, Index(-6), Show]
    public bool Randomize { get => Get(false); set => Set(value); }

    [Header, Range(0.0, 100.0, 1.0, Style = RangeStyle.UpDown), RightText("%"), Show]
    public double Range { get => Get(100.0); set => Set(value); }

    [Header, HideName, Image(SmallImages.Revert), Show, Style(BooleanStyle.Button)]
    public bool Reverse { get => Get(false); set => Set(value); }

    [Hide]
    public override IList SortNames => new StringCollection() { };

    [Category(Category.Step), Header, Index(-7), Show]
    public Steps Step { get => Get(Steps.Both); set => Set(value); }

    [Header, HideName, Image(SmallImages.Refresh), Show, Style(BooleanStyle.Button)]
    public bool Sync { get => Get(true); set => Set(value); }

    ///

    readonly Method update;

    public ColorHarmonyPanel() : base() => update = new(Update, true);

    ///

    void OnColorChanged(object sender, EventArgs<Color> e)
    {
        if (Sync)
            _ = update.Start();
    }

    void ApplySteps(Color oldColor, double x, int count)
    {
        if (!(ModelIndex >= 0 && ModelIndex < Models.Count))
            return;

        var colorProfile = WorkingProfile.Default;

        var colorModel = Models[ModelIndex];
        oldColor.Convert(out RGB rgb, colorProfile);

        ColorModel3 targetColor = (ColorModel3)Colour.New(colorModel);
        targetColor.From(rgb, colorProfile);
        
        var nColor = targetColor.Normalize();

        x = x < 0 ? 359 - Abs(x) : x;
        x = x > 359 ? x - 359 : x;
        x = x / 359.0;

        double y = nColor.Y, z = nColor.Z;

        Func<double, double> increment = Step switch
        {
            //[Original, 0]
            Steps.Decrease => i => -(i / count.Double()),
            //[Original, 1]
            Steps.Increase => i => (1.0 - i) / count.Double(),
            //[0, 1]
            Steps.Both => i => 1.0 / count.Double(),
        };

        double randomize(double i) => Randomize ? Step switch
        {
            //[Original, 0]
            Steps.Decrease => Numerics.Random.NextDouble() * y,
            //[Original, 1]
            Steps.Increase => 1 - Numerics.Random.NextDouble() * (1 - y),
            //[0, 1]
            Steps.Both => Numerics.Random.NextDouble(),
        } : i;

        double iY = increment(y);
        double iZ = increment(z);

        for (var i = 0; i < count; i++)
        {
            var rX = x;
            var rY = Components.HasFlag(TargetComponents.Y) ? randomize(y) : y;
            var rZ = Components.HasFlag(TargetComponents.Z) ? randomize(z) : z;

            var firstColor = Colour.New(colorModel, rX, rY, rZ).As<ColorModel3>().Denormalize();

            var newColor = Colour.New(colorModel, firstColor.X, firstColor.Y, firstColor.Z);
            newColor.To(out RGB result, WorkingProfile.Default);

            Colors.Add(XColor.Convert(result));

            if (Components.HasFlag(TargetComponents.Y))
                y += iY;

            if (Components.HasFlag(TargetComponents.Z))
                z += iZ;
        }
    }

    async Task Update(CancellationToken token)
    {
        await Task.Run(() =>
        {
            Try.Invoke(() =>
            {
                Colors.Clear();
                if (ActiveDocument == null)
                    return;

                var color = ActiveDocument.NewColor;
                double h = color.GetHue();

                var range = Range / 100.0;

                double[] hues = null;
                switch (Harmony)
                {
                    case Harmony.Analogous:
                        hues = Reverse ? new double[] { h, h - range * 30, h - range * 60 } : new double[] { h, h + range * 30, h + range * 60 };
                        break;

                    case Harmony.Diad:
                        hues = Reverse ? new double[] { h, h + range * 180 } : new double[] { h, h + range * 180 };
                        break;

                    case Harmony.Monochromatic:
                        hues = Reverse ? new double[] { h } : new double[] { h };
                        break;

                    case Harmony.DoubleComplementary:
                        hues = Reverse ? new double[] { h, h - range * 30, h - range * 180, h - range * 210 } : new double[] { h, h + range * 30, h + range * 180, h + range * 210 };
                        break;

                    case Harmony.SplitComplementary:
                        hues = Reverse ? new double[] { h, h + range * 150, h - range * 150 } : new double[] { h, h + range * 150, h - range * 150 };
                        break;

                    case Harmony.Square:
                        hues = Reverse ? new double[] { h, h - range * 90, h - range * 180, h - range * 270 } : new double[] { h, h + range * 90, h + range * 180, h + range * 270 };
                        break;

                    case Harmony.Tetrad:
                        hues = Reverse ? new double[] { h, h - range * 60, h - range * 180, h - range * 240 } : new double[] { h, h + range * 60, h + range * 180, h + range * 240 };
                        break;

                    case Harmony.Triad:
                        hues = Reverse ? new double[] { h, h - range * 120, h - range * 240 } : new double[] { h, h + range * 120, h + range * 240 };
                        break;

                    case Harmony.CoolWarm:

                        double hx = h;
                        double hy = range * 360; hy = hy > Count ? Count : hy;
                        double hz = hy / Count;

                        List<double> other = new();
                        for (double i = 0; i < Count; i++)
                        {
                            other.Add(hx);
                            hx = Reverse ? hx - hz : hx + hz;

                            int w = 135, c = 315, m = 359;
                            bool warm = h < w || h > c; bool cool = !warm;

                            if (cool)
                                hx = hx < w ? c : hx > c ? w : hx;

                            if (warm)
                            {
                                hx = hx < 0 ? m : hx > m ? 0 : (Reverse ? hx < c && hx > w ? w : hx : hx > w && hx < c ? c : hx);
                            }
                        }
                        hues = other.ToArray();
                        break;
                }

                var portion = Count / hues.Length;

                foreach (var i in hues)
                    ApplySteps(color, i, portion);
            });
        }, 
        token);
    }

    ///

    protected override void OnActiveDocumentChanged(Document input)
    {
        base.OnActiveDocumentChanged(input);
        ActiveDocument.If(i => i.ColorChanged -= OnColorChanged);

        if (input != null)
        {
            ActiveDocument = (ColorDocument)input;
            ActiveDocument.ColorChanged += OnColorChanged;
        }

        _ = update.Start();
    }

    protected virtual void OnPicked(Color input) => Picked?.Invoke(this, new(input));

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Components):
            case nameof(ModelIndex):
                Update(() => ComponentNames);
                _ = update.Start();
                break;

            case nameof(Count):
            case nameof(Harmony):
            case nameof(Randomize):
            case nameof(Range):
            case nameof(Reverse):
            case nameof(Step):
            case nameof(Sync):
                _ = update.Start();
                break;
        }
    }

    ///

    ICommand copyCommand;
    [HeaderOption, Image(SmallImages.Copy), Name("Copy"), Show]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() => Copy.Set(Colors.ToArray<Color>().Select(i => new GroupItem<ByteVector4>("Untitled", "", new(i.R, i.G, i.B, i.A)))), () => Colors.Count > 0);

    ICommand pickCommand;
    [Hide]
    public ICommand PickCommand => pickCommand ??= new RelayCommand<Color>(OnPicked, i => i != null);

    ICommand saveCommand;
    [HeaderOption, Image(SmallImages.Save), Name("Save"), Show]
    public ICommand SaveCommand => saveCommand ??= new RelayCommand(() => Saved?.Invoke(this, new(Colors.ToArray<Color>())), () => Colors.Count > 0);
}