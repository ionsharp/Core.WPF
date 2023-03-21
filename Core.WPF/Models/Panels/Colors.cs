using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Description("Custom defined colors.")]
[Explicit, Image(SmallImages.Colors), Name("Colors"), Serializable]
public class ColorsPanel : GroupPanel<ByteVector4>
{
    public static ResourceKey TemplateKey = new();

    [field: NonSerialized]
    public event EventHandler<EventArgs<ByteVector4>> Selected;

    ///

    public override string ItemName => "Color";

    ///

    public ColorsPanel() : base() { }

    public ColorsPanel(IGroupWriter input) : base(input) { }

    protected override IEnumerable<GroupCollection<ByteVector4>> GetDefaultGroups()
    {
        yield return new PrimaryColors();
        yield return new SecondaryColors();
        yield return new TertiaryColors();
        yield return new QuaternaryColors();
        yield return new QuinaryColors();

        yield return new ColorGroupCollection("Basic", 
            typeof(BasicColors));
        yield return new ColorGroupCollection("Web (CSS)", 
            typeof(CSSColors));
        yield return new ColorGroupCollection("Web (Safe)", 
            SafeWebColors.Colors.Select(i => new GroupItem<ByteVector4>(new ByteVector4(i).GetName(), new ByteVector4(i))));
        yield return new ColorGroupCollection("Web (Safest)", 
            typeof(SafestWebColors));
    }

    protected virtual void OnSelected(ByteVector4 input) => Selected?.Invoke(this, new EventArgs<ByteVector4>(input));

    ICommand selectColorCommand;
    public ICommand SelectColorCommand => selectColorCommand ??= new RelayCommand<ByteVector4>(i => OnSelected(i), i => i != null);
}