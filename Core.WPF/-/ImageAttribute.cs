using System;

namespace Imagin.Core
{
    public class ImageAttribute : Attribute
    {
        public readonly string Color;

        public readonly string Icon;

        public ImageAttribute(string icon)
        {
            Icon = icon;
        }

        public ImageAttribute(Images icon) : this($"{AssemblyProperties.AbsoluteImagePath}{icon}.png") { }

        public ImageAttribute(Images icon, ThemeKeys color) : this(icon)
        {
            Color = $"{color}";
        }
    }
}