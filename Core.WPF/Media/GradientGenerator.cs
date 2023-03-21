using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Imagin.Core.Media;

public class GradientGenerator : Base
{
    enum Category { Gradient }

    [Caption("The colors to include in a gradient."), Category(Category.Gradient)]
    [CollectionStyle(AddItems = nameof(DefaultColors))]
    public ObservableCollection<Color> Colors { get => Get<ObservableCollection<Color>>(new()); set => Set(value); }

    [Hide]
    public object DefaultColors => new ObservableCollection<Color>()
    {
        System.Windows.Media.Colors.Black,
        System.Windows.Media.Colors.Blue,
        System.Windows.Media.Colors.Cyan,
        System.Windows.Media.Colors.Green,
        System.Windows.Media.Colors.Magenta,
        System.Windows.Media.Colors.Red,
        System.Windows.Media.Colors.Transparent,
        System.Windows.Media.Colors.White,
        System.Windows.Media.Colors.Yellow
    };

    [Caption("The maximum number of colors in a gradient."), Category(Category.Gradient), Range(2, 10, 1, Style = RangeStyle.Both)]
    public int Combinations { get => Get(3); set => Set(value); }

    [Hide]
    public Color? Require { get => Get<Color?>(); set => Set(value); }

    [Caption("Add a copy of each gradient with colors in reverse order."), Category(Category.Gradient)]
    public bool Reverse { get => Get(false); set => Set(value); }

    public GradientGenerator(IEnumerable<Color> colors, Color? require = null) : base()
    {
        Colors = colors?.Count() > 0 ? new(colors) : new(); Require = require;
    }

    public IEnumerable<Gradient> Generate()
    {
        if (Colors == null) yield break;

        var n = Colors.Count;
        for (int i = 0; i < n; i++)
        {
            var a = Colors[i];
            for (int j = i + 1; j < n; j++)
            {
                var b = Colors[j];

                if (Require == null || Require == a || Require == b)
                {
                    yield return new Gradient(new GradientStep(0, a), new GradientStep(1, b));
                    if (Reverse)
                        yield return new Gradient(new GradientStep(0, b), new GradientStep(1, a));
                }

                for (int k = j + 1; k < n; k++)
                {
                    var c = Colors[k];
                    if (Require == null || Require == a || Require == b || Require == c)
                    {
                        yield return new Gradient(new GradientStep(0, a), new GradientStep(0.5, b), new(1, c));
                        yield return new Gradient(new GradientStep(0, a), new GradientStep(0.5, c), new(1, b));
                        yield return new Gradient(new GradientStep(0, b), new GradientStep(0.5, a), new(1, c));
                        if (Reverse)
                        {
                            yield return new Gradient(new GradientStep(0, c), new GradientStep(0.5, b), new(1, a));
                            yield return new Gradient(new GradientStep(0, b), new GradientStep(0.5, c), new(1, a));
                            yield return new Gradient(new GradientStep(0, c), new GradientStep(0.5, a), new(1, b));
                        }
                    }
                }
            }
        }
    }
}