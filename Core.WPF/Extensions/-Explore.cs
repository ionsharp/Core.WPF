using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Runtime.Serialization;

namespace Imagin.Core.Config;

[Description("File system explorer.")]
[Image(SmallImages.Folder), Name("Explore"), Serializable]
public class ExplorerExtension : PanelExtension
{
    public override string Author => nameof(Imagin);

    public override string Description => this.GetDescription();

    public override string Icon => this.GetAttribute<ImageAttribute>().SmallImage;

    public override string Name => this.GetName();

    public override string Uri => null;

    public override Version Version => new Version(1, 0, 0, 0);

    [NonSerialized]
    ExplorePanel panel;
    [Hide]
    public override Panel Panel => panel;

    public override IExtensionResources Resources => null;

    public ExplorerExtension() : base()
    {
        panel = new();
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        panel = new();
    }
}