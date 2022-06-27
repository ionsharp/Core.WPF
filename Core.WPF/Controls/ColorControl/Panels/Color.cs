using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls;

[DisplayName("Color"), Explicit]
public class ColorPanel : Panel
{
    enum Category { Component }

    public override Uri Icon => Resources.InternalImage(Images.Color);

    public override string Title => "Color";

    bool componentNormalize = false;
    [Category(Category.Component), DisplayName("Normalize"), Option, Visible]
    public bool ComponentNormalize
    {
        get => componentNormalize;
        set => this.Change(ref componentNormalize, value);
    }

    int componentPrecision = 2;
    [Category(Category.Component), DisplayName("Precision"), Option, Visible]
    public int ComponentPrecision
    {
        get => componentPrecision;
        set => this.Change(ref componentPrecision, value);
    }

    public ColorPanel() : base() { }
}