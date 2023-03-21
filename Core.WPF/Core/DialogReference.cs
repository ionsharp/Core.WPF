using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows.Controls;

namespace Imagin.Core;

public class DialogReference : Base
{
    public ByteVector4 Background { get; set; }

    public Button[] Buttons { get; private set; }

    public object Content { get; set; }
    
    public object ContentTemplate { get; set; }

    public Uri Image { get; internal set; }

    public DoubleSize ImageSize { get; private set; } = new(64);

    public Uri LargeImage { get; internal set; }

    public double MaxHeight { get; set; } = 720;

    public double MaxWidth { get; set; } = 900;

    public double MinWidth { get; set; } = 360;

    public int Result { get; set; }

    public string Title { get; private set; }

    ///

    internal Action<int> OnClosed { get; set; }

    ///

    public DialogReference(string title, object content, object contentTemplate, Uri smallImage, LargeImages largeImage, Button[] buttons) 
        : this(title, content, contentTemplate, smallImage, buttons)
        => LargeImage = largeImage.IfGet(Markup.LargeImage.GetUri);

    public DialogReference(string title, object content, object contentTemplate, SmallImages smallImage, LargeImages largeImage, Button[] buttons)
        : this(title, content, contentTemplate, Resource.GetImageUri(smallImage), largeImage, buttons) { }

    public DialogReference(string title, object content, object contentTemplate, Uri smallImage, Action<int> result, Button[] buttons) 
        : this(title, content, contentTemplate, smallImage, buttons) => OnClosed = result;

    public DialogReference(string title, object content, object contentTemplate, SmallImages smallImage, Action<int> result, Button[] buttons)
        : this(title, content, contentTemplate, smallImage, buttons) => OnClosed = result;

    public DialogReference(string title, object content, object contentTemplate, Uri smallImage, Button[] buttons)
    {
        Title = title; Content = content; ContentTemplate = contentTemplate; Image = smallImage; Buttons = buttons;

        Buttons = buttons?.Length == 0 ? null : buttons;
        Buttons ??= Controls.Buttons.Done;
    }

    public DialogReference(string title, object content, object contentTemplate, SmallImages smallImage, Button[] buttons)
        : this(title, content, contentTemplate, Resource.GetImageUri(smallImage), buttons) { }
}