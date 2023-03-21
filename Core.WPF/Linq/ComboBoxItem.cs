using Imagin.Core.Conversion;
using Imagin.Core.Numerics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class XComboBoxItem
{
    public static readonly ResourceKey SeparatorStyleKey = new();

    #region Properties
        
    #region Icon

    public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(XComboBoxItem), new FrameworkPropertyMetadata(null));
    public static ImageSource GetIcon(ComboBoxItem i) => (ImageSource)i.GetValue(IconProperty);
    public static void SetIcon(ComboBoxItem i, ImageSource input) => i.SetValue(IconProperty, input);

    #endregion

    #region IconSize

    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.RegisterAttached("IconSize", typeof(DoubleSize), typeof(XComboBoxItem), new FrameworkPropertyMetadata(null));
    [TypeConverter(typeof(DoubleSizeTypeConverter))]
    public static DoubleSize GetIconSize(ComboBoxItem i) => (DoubleSize)i.GetValue(IconSizeProperty);
    public static void SetIconSize(ComboBoxItem i, DoubleSize input) => i.SetValue(IconSizeProperty, input);

    #endregion

    #region IsSelected

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(XComboBoxItem), new FrameworkPropertyMetadata(false, OnIsSelectedChanged));
    public static bool GetIsSelected(ComboBoxItem i) => (bool)i.GetValue(IsSelectedProperty);
    public static void SetIsSelected(ComboBoxItem i, bool input) => i.SetValue(IsSelectedProperty, input);
    static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ComboBoxItem item)
            item.GetParent().OnSelected(item);
    }

    #endregion

    #region (private) Parent

    static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached("Parent", typeof(ComboBox), typeof(XComboBoxItem), new FrameworkPropertyMetadata(null));
    static ComboBox GetParent(this ComboBoxItem i) => i.GetValueOrSetDefault(ParentProperty, () => i.FindParent<ComboBox>());

    #endregion

    #endregion

    #region Methods

    internal static bool isSelected(this ComboBoxItem input) => GetIsSelected(input);

    public static void Select(this ComboBoxItem i, bool input) => SetIsSelected(i, input);

    public static void SelectInverse(this ComboBoxItem i) => SetIsSelected(i, !GetIsSelected(i));

    #endregion
}