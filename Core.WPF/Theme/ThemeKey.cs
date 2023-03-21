using System.Windows;

namespace Imagin.Core.Controls;

public class ThemeKey : DynamicResourceExtension
{
    public const string KeyFormat = "Theme/Default/{0}.xaml";

    public ThemeKeys Key { set => ResourceKey = $"{value}"; }

    public ThemeKey() : base() { }

    public ThemeKey(ThemeKeys key) : this() => Key = key;
}