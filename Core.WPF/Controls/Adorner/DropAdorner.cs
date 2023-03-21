using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls
{
    public class DropAdorner : Adorner<Control>
    {
        readonly ContentControl Target;

        public DropAdorner(Control control) : base(control)
        {
            Target = new()
            {
                Content = new(),
                IsHitTestVisible = false
            };
            Target.Bind(ContentControl.ContentTemplateProperty, 
                new PropertyPath("(0)", XControl.DropTemplateProperty), control);
            Target.Bind(ContentControl.VisibilityProperty,
                new PropertyPath("(0)", XControl.IsDraggingOverProperty), control, System.Windows.Data.BindingMode.OneWay, Converter.Get<Conversion.BooleanToVisibilityConverter>());

            Element.Bind(Control.OpacityProperty,
                new PropertyPath("(0)", XControl.IsDraggingOverProperty), control, System.Windows.Data.BindingMode.OneWay, new Conversion.ValueConverter<bool, double>(i => i ? 0 : 1));

            Children.Add(Target);
            SetCurrentValue(IsHitTestVisibleProperty, false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Target?.Arrange(new Rect(new Point(0, 0), new Size(Element.ActualWidth, Element.ActualHeight)));
            return finalSize;
        }
    }
}