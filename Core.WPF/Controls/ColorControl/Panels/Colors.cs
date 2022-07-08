using Imagin.Core.Collections.Serialization;
using Imagin.Core.Input;
using Imagin.Core.Numerics;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[DisplayName("Colors"), Explicit]
[Serializable]
public class ColorsPanel : GroupPanel<ByteVector4>
{
    [field: NonSerialized]
    public event EventHandler<EventArgs<ByteVector4>> Selected;

    //...

    public override Uri Icon => Resources.InternalImage(Images.Colors);

    public override string ItemName => "color";

    public override string Title => "Colors";

    //...

    public ColorsPanel(IGroupWriter input) : base(input) { }

    protected virtual void OnSelected(ByteVector4 input) => Selected?.Invoke(this, new EventArgs<ByteVector4>(input));

    ICommand selectColorCommand;
    public ICommand SelectColorCommand => selectColorCommand ??= new RelayCommand<ByteVector4>(i => OnSelected(i), i => i != null);
}