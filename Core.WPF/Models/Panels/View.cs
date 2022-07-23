using Imagin.Core.Controls;

namespace Imagin.Core.Models;

public abstract class ViewPanel : Panel
{
    enum Category { View }

    double itemSize = 32.0;
    [Category(Category.View), Option, Range(16.0, 512.0, 4.0), Slider, Visible]
    public double ItemSize
    {
        get => itemSize;
        set => this.Change(ref itemSize, value);
    }

    ViewControl.Views view = ViewControl.Views.Grid;
    [Category(Category.View), Option, Visible]
    public ViewControl.Views View
    {
        get => view;
        set => this.Change(ref view, value);
    }
}