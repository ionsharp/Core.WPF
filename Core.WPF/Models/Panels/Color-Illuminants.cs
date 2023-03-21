using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Controls;

[Description("Custom defined illuminants.")]
[Name("Illuminants"), Explicit, Image(SmallImages.LightBulb), Serializable]
public class ColorIlluminantsPanel : GroupPanel<Vector2>
{
    public static ResourceKey TemplateKey = new();

    public override string ItemName => "Illuminant";

    [Hide]
    public override IList SortNames => new StringCollection() { nameof(GroupItem<Vector2>.Created), nameof(GroupItem<Vector2>.Name), nameof(Vector2.X), nameof(Vector2.Y) };

    public ColorIlluminantsPanel() : base() { }

    public ColorIlluminantsPanel(IGroupWriter input) : base(input) { }

    protected override IEnumerable<GroupCollection<Vector2>> GetDefaultGroups()
    {
        yield return new GroupCollection<Vector2>("Daylight (2°)",
            typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));
        yield return new GroupCollection<Vector2>("Daylight (10°)",
            typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("D")).Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));

        yield return new GroupCollection<Vector2>("Equal energy",
            typeof(Illuminant).GetProperties().Where(i => i.Name == "E").Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));

        yield return new GroupCollection<Vector2>("Flourescent (2°)",
            typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));
        yield return new GroupCollection<Vector2>("Flourescent (10°)",
            typeof(Illuminant10).GetProperties().Where(i => i.Name.StartsWith("F")).Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));

        yield return new GroupCollection<Vector2>("Incandescent (2°)",
            typeof(Illuminant2).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));
        yield return new GroupCollection<Vector2>("Incandescent (10°)",
            typeof(Illuminant10).GetProperties().Where(i => i.Name == "A" || i.Name == "B" || i.Name == "C").Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));

        yield return new GroupCollection<Vector2>("LED (2°)",
            typeof(Illuminant2).GetProperties().Where(i => i.Name.StartsWith("LED")).Select(i => new GroupItem<Vector2>(i.Name, i.GetDescription(), (Vector2)i.GetValue(null))));
    }

    protected override Vector2 GetDefaultItem() => Illuminant.E;
}