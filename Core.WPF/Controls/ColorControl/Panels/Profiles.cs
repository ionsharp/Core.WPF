using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Profiles"), Explicit]
[Serializable]
public class ColorProfilesPanel : GroupPanel<WorkingProfileModel>
{
    public override Uri Icon => Resources.InternalImage(Images.Channels);

    public override string ItemName => "profile";

    public override string Title => "Profiles";

    public ColorProfilesPanel(IGroupWriter input) : base(input) { }

    protected override WorkingProfileModel GetNewItem() => new("Untitled profile", "", WorkingProfile.Default);
}