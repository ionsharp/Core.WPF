using Imagin.Core.Linq;
using System.Windows;

namespace Imagin.Core.Controls
{
    public partial class AboutWindow : Window
    {
        public AboutWindow() : base()
        {
            XWindow.SetFooterButtons(this, new Buttons(this, Buttons.Done));
            InitializeComponent();
        }
    }
}