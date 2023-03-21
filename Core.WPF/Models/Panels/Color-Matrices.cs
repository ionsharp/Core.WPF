using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Controls;

[Description("Custom defined color matrices.")]
[Explicit, Image(SmallImages.Matrix), Name("Matrices"), Serializable]
public class ColorMatricesPanel : GroupPanel<DoubleMatrix>
{
    public static ResourceKey TemplateKey = new();

    enum Category { AddRemove }

    public override string ItemName => "Matrix";

    public ColorMatricesPanel() : base() { }

    public ColorMatricesPanel(IGroupWriter input) : base(input) { }

    protected override IEnumerable<GroupCollection<DoubleMatrix>> GetDefaultGroups()
    {
        yield return new GroupCollection<DoubleMatrix>("LMS",
            typeof(ChromaticAdaptationTransform).GetProperties().Where(i => i.Name != nameof(ChromaticAdaptationTransform.Default)).Select(i => new GroupItem<DoubleMatrix>(i.GetDisplayName(), i.GetDescription(), new((double[][])(Matrix)i.GetValue(null)))));
    }

    protected override DoubleMatrix GetDefaultItem() => new((double[][])Matrix.Zero3x3);

    protected override IEnumerable<Type> GetItemTypes()
    {
        foreach (var i in base.GetItemTypes())
            yield return i;

        yield return typeof(ChromacityMatrix);
    }

    protected override Dictionary<Type, Func<DoubleMatrix>> ItemHandlers => new()
    {
        { typeof(ChromacityMatrix), 
            () => new ChromacityMatrix(Matrix.Zero3x3) },
        { typeof(DoubleMatrix), 
            () => new DoubleMatrix(Matrix.Zero3x3) }
    };
}