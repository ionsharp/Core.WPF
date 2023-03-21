using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using System;
using System.Windows.Media;

namespace Imagin.Core.Models;

[Categorize(false), Name("Mouse"), Image(SmallImages.Arrow), Serializable, ViewSource(ShowHeader = false)]
public class MousePanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    MouseHookListener listener;

    [Header, ReadOnly, Reserve]
    public int X { get => Get(0); set => Set(value); }

    [Header, ReadOnly, Reserve]
    public int Y { get => Get(0); set => Set(value); }

    [ReadOnly]
    public Color Color { get => Get<Color>(); set => Set(value); }

    public MousePanel() : base() { }

    void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        Display.Color().Convert(out Color color);
        Color = color;

        X = e.X; Y = e.Y;
    }

    public override void Subscribe()
    {
        base.Subscribe();
        listener = new(new Input.WinApi.GlobalHooker());
        listener.Enabled = true;
        listener.MouseMove += OnMouseMove;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        if (listener != null)
        {
            listener.Enabled = false;
            listener.MouseMove -= OnMouseMove;
            listener.Dispose();
            listener = null;
        }
    }
}