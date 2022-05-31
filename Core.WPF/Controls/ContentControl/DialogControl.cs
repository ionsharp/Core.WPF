using System.Windows.Media.Effects;

namespace Imagin.Core.Controls
{
    public class DialogControl : ContentControl<DialogReference>
    {
        public static readonly ResourceKey<DropShadowEffect> DropShadowEffectKey = new();

        public DialogControl() : base() { }
    }
}