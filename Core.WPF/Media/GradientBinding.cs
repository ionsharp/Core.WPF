﻿using Imagin.Core.Conversion;
using Imagin.Core.Data;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

public class GradientBinding : MultiBind
{
    public GradientBinding() : this(".") { }

    public GradientBinding(string path) : base()
    {
        Converter = new MultiConverter<GradientBrush>(data =>
        {
            if (data.Values?.Length > 0)
            {
                if (data.Values[0] is Gradient gradient)
                    return gradient.LinearBrush();

                else if (data.Values[0] is GradientStepCollection collection)
                    return new Gradient(collection).LinearBrush();
            }
            return null;
        });

        void Add(string a, string b) => Bindings.Add(new Binding($"{a}{b}"));
        Add(string.Empty, path);

        var i = path == "." ? "" : $"{path}.";

        Add(i, $"{nameof(Gradient.Steps)}");
        Add(i, $"{nameof(Gradient.Steps)}.{nameof(Gradient.Steps.Count)}");
    }
}

public class GradientStepBinding : MultiBind
{
    public GradientStepBinding() : base()
    {
        Converter = new MultiConverter<GradientBrush>(i =>
        {
            if (i.Values?.Length > 0)
            {
                if (i.Values[0] is GradientStepCollection collection)
                    return new Gradient(collection).LinearBrush();
            }
            return null;
        });

        Bindings.Add(new Binding("."));
        Bindings.Add(new Binding($"{nameof(GradientStepCollection.Count)}"));
    }
}