using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Illuminants"), Explicit]
[Serializable]
public class ColorIlluminantsPanel : GroupPanel<NamableIlluminant>
{
    public override Uri Icon => Resources.InternalImage(Images.LightBulb);

    public override string Title => "Illuminants";

    public ColorIlluminantsPanel(IGroupWriter input) : base(input) { }

    protected override NamableIlluminant GetNewItem() => new("Untitled illuminant", Illuminant.E);
}