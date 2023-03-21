using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Imagin.Core.Linq;

public static class XToolTip
{
    public static readonly ResourceKey HeaderPatternKey = new();

    #region Header

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(object), typeof(XToolTip), new FrameworkPropertyMetadata(null));
    public static object GetHeader(FrameworkElement i) => i.GetValue(HeaderProperty);
    public static void SetHeader(FrameworkElement i, object input) => i.SetValue(HeaderProperty, input);

    #endregion

    #region HeaderIcon

    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.RegisterAttached("HeaderIcon", typeof(object), typeof(XToolTip), new FrameworkPropertyMetadata(null));
    public static object GetHeaderIcon(FrameworkElement i) => (object)i.GetValue(HeaderIconProperty);
    public static void SetHeaderIcon(FrameworkElement i, object input) => i.SetValue(HeaderIconProperty, input);

    #endregion

    #region HeaderIconTemplate

    public static readonly DependencyProperty HeaderIconTemplateProperty = DependencyProperty.RegisterAttached("HeaderIconTemplate", typeof(DataTemplate), typeof(XToolTip), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetHeaderIconTemplate(FrameworkElement i) => (DataTemplate)i.GetValue(HeaderIconTemplateProperty);
    public static void SetHeaderIconTemplate(FrameworkElement i, DataTemplate input) => i.SetValue(HeaderIconTemplateProperty, input);

    #endregion
    
    #region HeaderTemplate

    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(XToolTip), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetHeaderTemplate(FrameworkElement i) => (DataTemplate)i.GetValue(HeaderTemplateProperty);
    public static void SetHeaderTemplate(FrameworkElement i, DataTemplate input) => i.SetValue(HeaderTemplateProperty, input);

    #endregion

    #region HeaderTemplateSelector

    public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.RegisterAttached("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(XToolTip), new FrameworkPropertyMetadata(null));
    public static DataTemplateSelector GetHeaderTemplateSelector(FrameworkElement i) => (DataTemplateSelector)i.GetValue(HeaderTemplateProperty);
    public static void SetHeaderTemplateSelector(FrameworkElement i, DataTemplateSelector input) => i.SetValue(HeaderTemplateProperty, input);

    #endregion

    #region MaximumWidth

    public static readonly DependencyProperty MaximumWidthProperty = DependencyProperty.RegisterAttached("MaximumWidth", typeof(double), typeof(XToolTip), new FrameworkPropertyMetadata(720.0));
    public static double GetMaximumWidth(FrameworkElement i) => (double)i.GetValue(MaximumWidthProperty);
    public static void SetMaximumWidth(FrameworkElement i, double input) => i.SetValue(MaximumWidthProperty, input);

    #endregion

    #region MinimumWidth

    public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.RegisterAttached("MinimumWidth", typeof(double), typeof(XToolTip), new FrameworkPropertyMetadata(double.NaN));
    public static double GetMinimumWidth(FrameworkElement i) => (double)i.GetValue(MinimumWidthProperty);
    public static void SetMinimumWidth(FrameworkElement i, double input) => i.SetValue(MinimumWidthProperty, input);

    #endregion

    #region Width

    public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(double), typeof(XToolTip), new FrameworkPropertyMetadata(double.NaN));
    public static double GetWidth(FrameworkElement i) => (double)i.GetValue(WidthProperty);
    public static void SetWidth(FrameworkElement i, double input) => i.SetValue(WidthProperty, input);

    #endregion

    static XToolTip()
    {
        EventManager.RegisterClassHandler(typeof(ToolTip), ToolTip.OpenedEvent, 
            new RoutedEventHandler(OnOpened), true);

        ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
    }

    internal static void Initialize() { }

    /// <summary>https://stackoverflow.com/questions/9674508/wpf-tooltip-positioning/17050664#17050664</summary>
    static void OnOpened(object sender, EventArgs e)
    {
        if (sender is ToolTip i)
        {
            if (i.PlacementTarget is null)
                return;

            var point = i.PlacementTarget.TranslatePoint(new Point(0, 0), i);
            if (point.Y > 0)
                i.Placement = PlacementMode.Top;

            i.Tag = new Thickness(point.X, 0, 0, 0);
        }
    }
}