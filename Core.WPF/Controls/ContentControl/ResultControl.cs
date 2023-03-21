using Imagin.Core.Analytics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls
{
    public class ResultControl : ContentControl<Result>
    {
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(ResultControl), new FrameworkPropertyMetadata(null));
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ResultControl), new FrameworkPropertyMetadata(null));
        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageTemplateProperty = DependencyProperty.Register(nameof(ImageTemplate), typeof(DataTemplate), typeof(ResultControl), new FrameworkPropertyMetadata(null));
        public DataTemplate ImageTemplate
        {
            get => (DataTemplate)GetValue(ImageTemplateProperty);
            set => SetValue(ImageTemplateProperty, value);
        }

        public static readonly DependencyProperty ImageTemplateSelectorProperty = DependencyProperty.Register(nameof(ImageTemplateSelector), typeof(DataTemplateSelector), typeof(ResultControl), new FrameworkPropertyMetadata(null));
        public DataTemplateSelector ImageTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ImageTemplateSelectorProperty);
            set => SetValue(ImageTemplateSelectorProperty, value);
        }

        public ResultControl() : base() { }
    }
}