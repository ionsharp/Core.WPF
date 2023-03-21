using System;

namespace Imagin.Core.Models;

[Name("Properties"), Image(SmallImages.Properties), Serializable]
public class PropertiesPanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Properties);

    [Hide, NonSerializable]
    public virtual int SelectedIndex { get => Get(-1, false); set => Set(value, false); }

    [HeaderItem, HideName, Image(SmallImages.Info), Name("Show description"), Reserve, Show, Style(BooleanStyle.Button)]
    public bool ShowDescription { get => Get(false); set => Set(value); }

    [Hide, NonSerializable]
    public virtual object Source { get => Get<object>(null, false); set => Set(value, false); }

    public PropertiesPanel() : base() { }
}