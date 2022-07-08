using Imagin.Core.Collections.Serialization;
using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Matrix"), Explicit, Serializable]
public class ColorMatrixPanel : GroupPanel<AdaptationModel>
{
    public override Uri Icon => Resources.InternalImage(Images.Grid);

    public override string ItemName => "matrix";

    public override string Title => "Matrix";

    public ColorMatrixPanel(IGroupWriter input) : base(input) { }

    protected override AdaptationModel GetNewItem() => new("Untitled matrix", "", new Matrix(new[] { new[] { .0, 0, 0 }, new[] { .0, 0, 0 }, new[] { .0, 0, 0 } }));
}