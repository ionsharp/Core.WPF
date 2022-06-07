using Imagin.Core.Collections.Generic;
using Imagin.Core.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls
{
    public class Buttons : ObservableCollection<Button>
    {
        static Button New(string label, int i, bool isDefault = false, bool isCancel = false)
        {
            var result = new Button() { Content = label, IsDefault = isDefault, IsCancel = isCancel };
            XButton.SetResult(result, i);
            return result;
        }

        //...

        public static Button[] AbortRetryIgnore 
            = XArray.New(New("Abort", 0), New("Retry", 1, true), New("Ignore", 2, false, true));

        public static Button[] Cancel 
            = XArray.New(New("Cancel", 0, false, true));

        public static Button[] Continue 
            = XArray.New(New("Continue", 0, true));

        public static Button[] ContinueCancel 
            = XArray.New(New("Continue", 0, true), New("Cancel", 1, false, true));

        public static Button[] Done 
            = XArray.New(New("Done", 0, true));

        public static Button[] Ok 
            = XArray.New(New("Ok", 0, true));

        public static Button[] OkCancel 
            = XArray.New(New("Ok", 0, true), New("Cancel", 1, false, true));

        public static Button[] SaveCancel 
            = XArray.New(New("Save", 0, true), New("Cancel", 1, false, true));

        public static Button[] YesCancel 
            = XArray.New(New("Yes", 0, true), New("Cancel", 1, false, true));

        public static Button[] YesNo 
            = XArray.New(New("Yes", 0, true), New("No", 1, false, true));

        public static Button[] YesNoCancel 
            = XArray.New(New("Yes", 0, true), New("No", 1), New("Cancel", 2, false, true));

        //...

        public Buttons() : base() { }

        public Buttons(Button[] input) : base(input) { }

        public Buttons(Window window, params Button[] input) : base(input) 
        {
            foreach (var i in this)
            {
                i.Command
                    = XWindow.CloseCommand;
                i.CommandParameter
                    = XButton.GetResult(i);
                i.CommandTarget
                    = window;
            }
        }
    }
}