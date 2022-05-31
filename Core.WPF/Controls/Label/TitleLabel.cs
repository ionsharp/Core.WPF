using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls
{
    public class TitleLabel : Label
    {
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(int), typeof(TitleLabel), new FrameworkPropertyMetadata(1));
        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }
    }
}