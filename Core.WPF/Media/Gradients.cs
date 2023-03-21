using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Imagin.Core.Media;

[Serializable]
public abstract class DefaultGradients : GroupCollection<Gradient>
{
    protected DefaultGradients(string name) : base(name) { }

    string GetName(Gradient gradient)
    {
        var name = "Untitled";
        foreach (var i in gradient.Steps)
        {
            if (name == "Untitled")
                name = "";

            var color = Colour.FindName(i.Color);

            if (i.Color.A == 0)
                name += $"{nameof(System.Drawing.Color.Transparent)}";

            else if (color != null)
                name += $"{color}";

            else if (color == null)
                name += i.Color.GetApproximateName();

            name += ", ";
        }

        if (name.EndsWith(", "))
            name = name.Substring(0, name.Length - 2);

        return name;
    }

    protected void Generate(Color[] colors, Color? require = null)
    {
        var generator = new GradientGenerator(colors, require);
        generator.Generate().ForEach(i => Add(GetName(i), i));
    }
}

[Serializable]
public sealed class NeutralGradients : DefaultGradients
{
    public NeutralGradients() : base("Neutral")
    {
        var colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors);
    }
}

[Serializable]
public sealed class PrimaryGradients : DefaultGradients
{
    public PrimaryGradients() : base("Primary")
    {
        var colors = new Color[] { System.Windows.Media.Colors.Red, System.Windows.Media.Colors.Green, System.Windows.Media.Colors.Blue};
        Generate(colors);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Red, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Red);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Green, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Green);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Blue, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Blue);
    }
}

[Serializable]
public sealed class SecondaryGradients : DefaultGradients
{
    public SecondaryGradients() : base("Secondary")
    {
        var colors = new Color[] { System.Windows.Media.Colors.Cyan, System.Windows.Media.Colors.Magenta, System.Windows.Media.Colors.Yellow };
        Generate(colors);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Cyan, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Cyan);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Magenta, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Magenta);

        colors = new Color[] { System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Yellow, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
        Generate(colors, System.Windows.Media.Colors.Yellow);
    }
}

[Serializable]
public sealed class TertiaryGradients : DefaultGradients
{
    public TertiaryGradients() : base("Tertiary")
    {
        var colors = XList.Select<FieldInfo, ByteVector4>(typeof(Colors3).GetFields(), i => new ByteVector4((string)i.GetValue(null))).Select(i => XColor.Convert(i));
        Generate(colors?.ToArray());

        Color[] moreColors = null;
        foreach (var i in colors)
        {
            moreColors = new Color[] { i, System.Windows.Media.Colors.Black, System.Windows.Media.Colors.Transparent, System.Windows.Media.Colors.White };
            Generate(moreColors, i);
        }
    }
}