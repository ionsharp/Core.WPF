using System;

namespace Imagin.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct)]
public class ImageColorAttribute : Attribute
{
    public readonly string Color;

    public ImageColorAttribute(ThemeKeys color = ThemeKeys.ImageElement) : base() => Color = $"{color}";
}