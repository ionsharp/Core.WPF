using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Illuminants"), Explicit]
[Serializable]
public class ColorIlluminantsPanel : GroupPanel<ChromacityModel>
{
    public override Uri Icon => Resources.InternalImage(Images.LightBulb);

    public override string ItemName => "illuminant";

    public override string Title => "Illuminants";

    public ColorIlluminantsPanel(IGroupWriter input) : base(input) { }

    protected override ChromacityModel GetNewItem() => new("Untitled illuminant", "", Illuminant.E);
}