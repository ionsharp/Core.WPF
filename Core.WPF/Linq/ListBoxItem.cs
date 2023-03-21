using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Linq;

public static class XListBoxItem
{
    public static readonly ResourceKey TemplateKey = new();

    #region LastSelected

    public static readonly DependencyProperty LastSelectedProperty = DependencyProperty.RegisterAttached("LastSelected", typeof(bool), typeof(XListBoxItem), new FrameworkPropertyMetadata(false));
    public static bool GetLastSelected(ListBoxItem i) => (bool)i.GetValue(LastSelectedProperty);
    public static void SetLastSelected(ListBoxItem i, bool value) => i.SetValue(LastSelectedProperty, value);

    #endregion
}

public static class XListViewItem
{
    #region ParentHasColumns

    public static readonly DependencyProperty ParentHasColumnsProperty = DependencyProperty.RegisterAttached("ParentHasColumns", typeof(bool), typeof(XListViewItem), new FrameworkPropertyMetadata(false));
    public static bool GetParentHasColumns(ListViewItem i) => (bool)i.GetValue(ParentHasColumnsProperty);
    public static void SetParentHasColumns(ListViewItem i, bool value) => i.SetValue(ParentHasColumnsProperty, value);

    #endregion
}