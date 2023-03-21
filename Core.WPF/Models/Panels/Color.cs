using Imagin.Core.Models;

namespace Imagin.Core.Controls;

[Image(SmallImages.Color), Name("Color"), Explicit]
public class ColorPanel : Panel
{
    enum Category { Component }

    [Category(Category.Component), Name("Normalize"), Option, Show]
    public bool ComponentNormalize { get => Get(false); set => Set(value); }

    [Category(Category.Component), Name("Precision"), Option, Show]
    public int ComponentPrecision { get => Get(2); set => Set(value); }

    public ColorPanel() : base() { }
}