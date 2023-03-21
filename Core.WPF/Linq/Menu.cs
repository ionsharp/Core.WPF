using Imagin.Core.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Imagin.Core.Linq;

public static class XMenuBase
{
    #region GeneratorSource

    public static readonly DependencyProperty GeneratorSourceProperty = DependencyProperty.RegisterAttached("GeneratorSource", typeof(object), typeof(XMenuBase), new FrameworkPropertyMetadata(null, OnGeneratorSourceChanged));
    public static object GetGeneratorSource(MenuBase i) => (object)i.GetValue(GeneratorSourceProperty);
    public static void SetGeneratorSource(MenuBase i, object input) => i.SetValue(GeneratorSourceProperty, input);
    static void OnGeneratorSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e)
    {
        if (i is MenuBase menu)
        {
            var generator = GetGenerator(menu);
            generator.Load(e.NewValue);
        }
    }

    #endregion

    #region (readonly) Generator

    static readonly DependencyPropertyKey GeneratorKey = DependencyProperty.RegisterAttachedReadOnly("Generator", typeof(MenuGenerator), typeof(XMenuBase), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty GeneratorProperty = GeneratorKey.DependencyProperty;
    public static MenuGenerator GetGenerator(MenuBase i) => i.GetValueOrSetDefault<MenuGenerator>(GeneratorKey, () => new(i));

    #endregion
}

public static class XMenu
{
    #region TopLevelIconVisibility

    public static readonly DependencyProperty TopLevelIconVisibilityProperty = DependencyProperty.RegisterAttached("TopLevelIconVisibility", typeof(Visibility), typeof(XMenu), new FrameworkPropertyMetadata(Visibility.Visible));
    public static Visibility GetTopLevelIconVisibility(Menu i) => (Visibility)i.GetValue(TopLevelIconVisibilityProperty);
    public static void SetTopLevelIconVisibility(Menu i, Visibility input) => i.SetValue(TopLevelIconVisibilityProperty, input);

    #endregion
}