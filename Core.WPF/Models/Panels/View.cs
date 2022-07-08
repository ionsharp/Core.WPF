using Imagin.Core.Controls;

namespace Imagin.Core.Models;

public abstract class ViewPanel : Panel
{
    double itemSize = 128.0;
    [Below, Index(0), Label(false), Range(32.0, 512.0, 4.0), Slider, Tool, Visible]
    public double ItemSize
    {
        get => itemSize;
        set => this.Change(ref itemSize, value);
    }

    ViewControl.Views view = ViewControl.Views.List;
    [Below, Index(1), Label(false), Tool, Visible]
    public ViewControl.Views View
    {
        get => view;
        set => this.Change(ref view, value);
    }
}