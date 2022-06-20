using Imagin.Core.Media;

namespace Imagin.Core.Controls
{
    public partial class GradientWindow : PickerWindow
    {
        public GradientWindow() : base()
        {
            SetCurrentValue(ValueProperty, Gradient.Default);
            InitializeComponent();
        }

        public GradientWindow(string title, Gradient gradient) : this()
        {
            SetCurrentValue(TitleProperty, 
                title);
            SetCurrentValue(ValueProperty, 
                gradient);
        }
    }
}