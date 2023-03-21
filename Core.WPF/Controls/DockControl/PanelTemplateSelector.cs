using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Imagin.Core.Controls;

public class PanelTemplateSelector : TypeTemplateSelector, IDockSelector
{
    public sealed override bool Strict => true;

    public sealed override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var result = base.SelectTemplate(item, container);
        if (ReferenceEquals(result, Default))
        {
            var control = container.FindParent<Popup>() is Popup popup
                ? popup.PlacementTarget.FindVisualParent<DockRootControl>()?.DockControl
                : container.FindParent<DockRootControl>()?.DockControl;

            return control?.DefaultPanelTemplate;
        }
        return result;
    }
}