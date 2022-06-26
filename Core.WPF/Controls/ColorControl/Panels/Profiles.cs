using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[Explicit, Serializable]
public class ColorProfilesPanel : GroupPanel<NamableProfile>
{
    public override Uri Icon => Resources.InternalImage(Images.Channels);

    public override string Title => "Profiles";

    public ColorProfilesPanel(IGroupWriter input) : base(input) { }

    protected override NamableProfile GetNewItem() => new("Untitled profile", WorkingProfile.Default);
}