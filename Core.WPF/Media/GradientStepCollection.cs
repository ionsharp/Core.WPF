using Imagin.Core.Collections.ObjectModel;
using System;

namespace Imagin.Core.Media;

[Description()]
[Name("Gradient preview")]
public sealed class GradientPreview 
{ 
    public GradientStepCollection Steps { get; private set; }

    public GradientPreview(GradientStepCollection steps) : base() => Steps = steps;
}

[Description()]
[Categorize(false), Name("Gradient steps"), Serializable]
public class GradientStepCollection : ChangeCollection<GradientStep> 
{
    [HideName]
    public GradientPreview Preview => new(this);
}