using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Controls;

[Description("Custom defined working profiles.")]
[Name("Profiles"), Explicit, Image(SmallImages.Channels), Serializable]
public class ColorProfilesPanel : GroupPanel<WorkingProfile>
{
    public static ResourceKey TemplateKey = new();

    public override string ItemName => "Profile";

    [Hide]
    public override IList SortNames => new StringCollection() { nameof(GroupItem<WorkingProfile>.Created), nameof(GroupItem<WorkingProfile>.Name), nameof(WorkingProfile.Compression), nameof(WorkingProfile.Primary), nameof(WorkingProfile.White) };

    public ColorProfilesPanel() : base() { }

    public ColorProfilesPanel(IGroupWriter input) : base(input) { }

    protected override IEnumerable<GroupCollection<WorkingProfile>> GetDefaultGroups()
    {
        var profiles = new List<GroupCollection<WorkingProfile>>();
        typeof(WorkingProfiles).GetProperties().GroupBy(i => i.GetCategory()).ForEach(i => profiles.Add(new GroupCollection<WorkingProfile>(i.Key, i.Select(j => new GroupItem<WorkingProfile>(j.GetDisplayName(), j.GetDescription(), (WorkingProfile)j.GetValue(null))))));
        foreach (var i in profiles)
            yield return i;
    }

    protected override WorkingProfile GetDefaultItem() => WorkingProfile.Default;
}