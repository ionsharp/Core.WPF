using Imagin.Core.Controls;
using Imagin.Core.Linq;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core
{
    public static class Dialog
    {
        public static int Show(string title, object message, Uri image = null, params Button[] buttons)
            => Show(out DialogWindow _, title, message, image ?? DialogImage.Information, buttons);

        public static int Show(out DialogWindow dialog, string title, object message, Uri image, params Button[] buttons)
        {
            dialog = new DialogWindow();
            dialog.SetCurrentValue(DialogWindow.ReferenceProperty, new DialogReference(title, message, image, buttons));

            XWindow.SetFooterButtons(dialog, new(dialog, buttons));
            dialog.ShowDialog();

            return XWindow.GetResult(dialog);
        }

        async public static Task<int> ShowAsync(string title, object message, Uri image, DialogOpenedHandler onOpened, DialogClosedHandler onClosed, params Button[] buttons)
        {
            var dialog = new DialogWindow(onOpened);
            dialog.SetCurrentValue(DialogWindow.ReferenceProperty, new DialogReference(title, message, image, buttons));

            XWindow.SetFooterButtons(dialog, new(dialog, buttons));
            dialog.ShowDialog();

            var result = XWindow.GetResult(dialog);
            if (onClosed != null)
                await onClosed(result);

            return result;
        }

        //...

        public static DialogReference Show(Window window, string title, object message, Uri image, params Button[] buttons)
        {
            var result = new DialogReference(title, message, image, buttons);
            window.GetChild(XWindow.DialogPresenterKey).Show(result);
            return result;
        }
    }
}