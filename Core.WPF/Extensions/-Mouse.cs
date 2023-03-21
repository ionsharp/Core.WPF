using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Runtime.Serialization;

namespace Imagin.Core.Config;

[Description("Shows information about the mouse anywhere on the screen.")]
[Image(SmallImages.Arrow), Name("Mouse"), Serializable]
public class MouseExtension : PanelExtension
{
    public override string Author => nameof(Imagin);

    public override string Description => this.GetDescription();

    public override string Icon => this.GetAttribute<ImageAttribute>().SmallImage;

    public override string Name => this.GetName();

    public override string Uri => null;

    public override Version Version => new Version(1, 0, 0, 0);

    [NonSerialized]
    MousePanel panel;
    [Hide]
    public override Panel Panel => panel;

    public override IExtensionResources Resources => null;

    public MouseExtension() : base()
    {
        panel = new();
    }

    [OnDeserialized]
    void OnDeserialized(StreamingContext context)
    {
        panel = new();
    }
}