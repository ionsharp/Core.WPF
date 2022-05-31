using System.Windows.Input;

namespace Imagin.Core.Linq
{
    public static class XModifierKeys
    {
        public static bool Pressed(this ModifierKeys Value) => (Keyboard.Modifiers & Value) != 0;
    }
}