using System;

namespace Imagin.Core.Models;

[Name("Options"), Image(SmallImages.Options), Serializable]
public class OptionsPanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Options);

    [Hide]
    public override string Title => "Options";

    public OptionsPanel() : base() { }
}