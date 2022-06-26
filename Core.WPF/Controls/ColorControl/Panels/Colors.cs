using Imagin.Core.Collections.Serialization;
using Imagin.Core.Input;
using Imagin.Core.Media;
using System;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Explicit, Serializable]
public class ColorsPanel : GroupPanel<StringColor>
{
    public static readonly ResourceKey TemplateKey = new();

    [field: NonSerialized]
    public event EventHandler<EventArgs<StringColor>> Selected;

    //...

    public override Uri Icon => Resources.InternalImage(Images.Colors);

    public override string Title => "Colors";

    //...

    public ColorsPanel(IGroupWriter input) : base(input) { }

    protected virtual void OnSelected(StringColor input) => Selected?.Invoke(this, new EventArgs<StringColor>(input));

    ICommand selectColorCommand;
    public ICommand SelectColorCommand => selectColorCommand ??= new RelayCommand<StringColor>(i => OnSelected(i), i => i != null);
}