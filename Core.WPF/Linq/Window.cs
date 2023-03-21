using Imagin.Core.Analytics;
using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Imports;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Imagin.Core.Linq;

public static class XWindow
{
    public static readonly ReferenceKey<Border> BorderKey = new();

    public static readonly ReferenceKey<DialogPresenter> DialogPresenterKey = new();

    public static readonly ResourceKey GripTemplateKey = new();

    #region Fields

    const int SC_MOVE = 0xF010;

    ///

    const uint SWP_NOSIZE = 0x0001;

    const uint SWP_NOMOVE = 0x0002;

    const uint SWP_NOACTIVATE = 0x0010;

    const uint SWP_NOZORDER = 0x0004;

    ///

    const int WM_ACTIVATEAPP = 0x001C;

    const int WM_ACTIVATE = 0x0006;

    const int WM_SETFOCUS = 0x0007;

    const int WM_SYSCOMMAND = 0x0112;

    const int WM_WINDOWPOSCHANGING = 0x0046;

    #endregion

    #region Properties

    #region AutoCenter

    public static readonly DependencyProperty AutoCenterProperty = DependencyProperty.RegisterAttached("AutoCenter", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false, OnAutoCenterChanged));
    public static bool GetAutoCenter(Window i) => (bool)i.GetValue(AutoCenterProperty);
    public static void SetAutoCenter(Window i, bool input) => i.SetValue(AutoCenterProperty, input);
    static void OnAutoCenterChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
            window.RegisterHandlerAttached((bool)e.NewValue, AutoCenterProperty, i => i.SizeChanged += AutoCenter_SizeChanged, i => i.SizeChanged -= AutoCenter_SizeChanged);
    }

    static void AutoCenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is Window i)
        {
            if (e.HeightChanged)
                i.Top += (e.PreviousSize.Height - e.NewSize.Height) / 2;

            if (e.WidthChanged)
                i.Left += (e.PreviousSize.Width - e.NewSize.Width) / 2;
        }
    }

    #endregion

    #region ButtonStyle

    public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.RegisterAttached("ButtonStyle", typeof(Controls.ButtonStyle), typeof(XWindow), new FrameworkPropertyMetadata(Controls.ButtonStyle.Classic));
    public static Controls.ButtonStyle GetButtonStyle(Window i) => (Controls.ButtonStyle)i.GetValue(ButtonStyleProperty);
    public static void SetButtonStyle(Window i, Controls.ButtonStyle input) => i.SetValue(ButtonStyleProperty, input);

    #endregion

    #region ButtonVisibility

    public static readonly DependencyProperty ButtonVisibilityProperty = DependencyProperty.RegisterAttached("ButtonVisibility", typeof(Visibility), typeof(XWindow), new FrameworkPropertyMetadata(Visibility.Visible));
    public static Visibility GetButtonVisibility(Window i) => (Visibility)i.GetValue(ButtonVisibilityProperty);
    public static void SetButtonVisibility(Window i, Visibility input) => i.SetValue(ButtonVisibilityProperty, input);

    #endregion

    #region CanMaximize

    public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.RegisterAttached("CanMaximize", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(true));
    public static bool GetCanMaximize(Window i) => (bool)i.GetValue(CanMaximizeProperty);
    public static void SetCanMaximize(Window i, bool input) => i.SetValue(CanMaximizeProperty, input);

    #endregion

    #region CanMove

    public static readonly DependencyProperty CanMoveProperty = DependencyProperty.RegisterAttached("CanMove", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(true, OnCanMoveChanged));
    public static bool GetCanMove(Window i) => (bool)i.GetValue(CanMoveProperty);
    public static void SetCanMove(Window i, bool input) => i.SetValue(CanMoveProperty, input);
    static void OnCanMoveChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
        {
            window.SourceInitialized
                -= CanMove_SourceInitialized;

            if ((bool)e.OldValue)
            {
                window.SourceInitialized
                    += CanMove_SourceInitialized;
            }
        }
    }

    static void CanMove_SourceInitialized(object sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.SourceInitialized
                -= CanMove_SourceInitialized;

            HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).AddHook(CanMove_WndProc);
        }
    }

    static IntPtr CanMove_WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_SYSCOMMAND:
                int command = wParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                    handled = true;

                break;

            default: break;
        }
        return IntPtr.Zero;
    }

    #endregion

    #region CloseCommand

    public static readonly RoutedUICommand CloseCommand = new(nameof(CloseCommand), nameof(CloseCommand), typeof(XWindow));
    static void OnClose(object sender, ExecutedRoutedEventArgs e)
    {
        SetResult(sender as Window, e.Parameter?.Int32() ?? -1);
        SystemCommands.CloseWindow(sender as Window);
    }
    static void CanClose(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    #endregion

    #region MarkNotificationCommand

    public static readonly RoutedUICommand MarkNotificationCommand = new(nameof(MarkNotificationCommand), nameof(MarkNotificationCommand), typeof(XWindow));
    static void OnMarkNotificationCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is Window window)
            (e.Parameter as Notification).IsRead = true;
    }
    static void OnCanExecuteMarkNotificationCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is Window window)
            e.CanExecute = e.Parameter is Notification;
    }

    #endregion

    #region ContentStyle

    public static readonly DependencyProperty ContentStyleProperty = DependencyProperty.RegisterAttached("ContentStyle", typeof(Style), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static Style GetContentStyle(Window i) => (Style)i.GetValue(ContentStyleProperty);
    public static void SetContentStyle(Window i, Style input) => i.SetValue(ContentStyleProperty, input);

    #endregion

    #region (readonly) Dialogs

    static readonly DependencyPropertyKey DialogsKey = DependencyProperty.RegisterAttachedReadOnly("Dialogs", typeof(Stack<DialogReference>), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty DialogsProperty = DialogsKey.DependencyProperty;
    public static Stack<DialogReference> GetDialogs(Window i) => i.GetValueOrSetDefault(DialogsKey, () => new Stack<DialogReference>());

    #endregion

    #region DialogBlur

    public static readonly DependencyProperty DialogBlurProperty = DependencyProperty.RegisterAttached("DialogBlur", typeof(double), typeof(XWindow), new FrameworkPropertyMetadata(1000.0));
    public static double GetDialogBlur(Window i) => (double)i.GetValue(DialogBlurProperty);
    public static void SetDialogBlur(Window i, double input) => i.SetValue(DialogBlurProperty, input);

    #endregion

    #region DialogTemplate

    public static readonly DependencyProperty DialogTemplateProperty = DependencyProperty.RegisterAttached("DialogTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetDialogTemplate(Window i) => (DataTemplate)i.GetValue(DialogTemplateProperty);
    public static void SetDialogTemplate(Window i, DataTemplate input) => i.SetValue(DialogTemplateProperty, input);

    #endregion

    #region DialogTransition

    public static readonly DependencyProperty DialogTransitionProperty = DependencyProperty.RegisterAttached("DialogTransition", typeof(Transitions), typeof(XWindow), new FrameworkPropertyMetadata(Transitions.Default));
    public static Transitions GetDialogTransition(Window i) => (Transitions)i.GetValue(DialogTransitionProperty);
    public static void SetDialogTransition(Window i, Transitions input) => i.SetValue(DialogTransitionProperty, input);

    #endregion

    #region DisableCancel

    public static readonly DependencyProperty DisableCancelProperty = DependencyProperty.RegisterAttached("DisableCancel", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false));
    public static bool GetDisableCancel(Window i) => (bool)i.GetValue(DisableCancelProperty);
    public static void SetDisableCancel(Window i, bool input) => i.SetValue(DisableCancelProperty, input);

    #endregion

    #region Extend

    public static readonly DependencyProperty ExtendProperty = DependencyProperty.RegisterAttached("Extend", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false, OnExtendChanged));
    public static bool GetExtend(Window i) => (bool)i.GetValue(ExtendProperty);
    public static void SetExtend(Window i, bool input) => i.SetValue(ExtendProperty, input);
    static void OnExtendChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
        {
            window.SourceInitialized -= OnSourceInitialized;
            if ((bool)e.NewValue)
                window.SourceInitialized += OnSourceInitialized;
        }
    }

    #endregion

    #region Footer

    public static readonly DependencyProperty FooterProperty = DependencyProperty.RegisterAttached("Footer", typeof(object), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static object GetFooter(Window i) => (object)i.GetValue(FooterProperty);
    public static void SetFooter(Window i, object input) => i.SetValue(FooterProperty, input);

    #endregion

    #region FooterButtons

    public static readonly DependencyProperty FooterButtonsProperty = DependencyProperty.RegisterAttached("FooterButtons", typeof(Buttons), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static Buttons GetFooterButtons(Window i) => (Buttons)i.GetValue(FooterButtonsProperty);
    public static void SetFooterButtons(Window i, Buttons input) => i.SetValue(FooterButtonsProperty, input);

    #endregion

    #region FooterTemplate

    public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.RegisterAttached("FooterTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetFooterTemplate(Window i) => (DataTemplate)i.GetValue(FooterTemplateProperty);
    public static void SetFooterTemplate(Window i, DataTemplate input) => i.SetValue(FooterTemplateProperty, input);

    #endregion

    #region FooterTemplateSelector

    public static readonly DependencyProperty FooterTemplateSelectorProperty = DependencyProperty.RegisterAttached("FooterTemplateSelector", typeof(DataTemplateSelector), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplateSelector GetFooterTemplateSelector(Window i) => (DataTemplateSelector)i.GetValue(FooterTemplateSelectorProperty);
    public static void SetFooterTemplateSelector(Window i, DataTemplateSelector input) => i.SetValue(FooterTemplateSelectorProperty, input);

    #endregion

    #region FooterVisibility

    public static readonly DependencyProperty FooterVisibilityProperty = DependencyProperty.RegisterAttached("FooterVisibility", typeof(Visibility), typeof(XWindow), new FrameworkPropertyMetadata(Visibility.Collapsed));
    public static Visibility GetFooterVisibility(Window i) => (Visibility)i.GetValue(FooterVisibilityProperty);
    public static void SetFooterVisibility(Window i, Visibility input) => i.SetValue(FooterVisibilityProperty, input);

    #endregion

    #region HeaderBackground

    public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.RegisterAttached("HeaderBackground", typeof(Brush), typeof(XWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
    public static Brush GetHeaderBackground(Window i) => (Brush)i.GetValue(HeaderBackgroundProperty);
    public static void SetHeaderBackground(Window i, Brush input) => i.SetValue(HeaderBackgroundProperty, input);

    #endregion

    #region HeaderButtons

    public static readonly DependencyProperty HeaderButtonsProperty = DependencyProperty.RegisterAttached("HeaderButtons", typeof(WindowButtons), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static WindowButtons GetHeaderButtons(Window i) => (WindowButtons)i.GetValue(HeaderButtonsProperty);
    public static void SetHeaderButtons(Window i, WindowButtons input) => i.SetValue(HeaderButtonsProperty, input);

    #endregion

    #region HeaderButtonTemplate

    public static readonly DependencyProperty HeaderButtonTemplateProperty = DependencyProperty.RegisterAttached("HeaderButtonTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetHeaderButtonTemplate(Window i) => (DataTemplate)i.GetValue(HeaderButtonTemplateProperty);
    public static void SetHeaderButtonTemplate(Window i, DataTemplate input) => i.SetValue(HeaderButtonTemplateProperty, input);

    #endregion

    #region Icon

    public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static ImageSource GetIcon(Window i) => (ImageSource)i.GetValue(IconProperty);
    public static void SetIcon(Window i, ImageSource input) => i.SetValue(IconProperty, input);

    #endregion

    #region IconMenu

    public static readonly DependencyProperty IconMenuProperty = DependencyProperty.RegisterAttached("IconMenu", typeof(object), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static object GetIconMenu(Window i) => (object)i.GetValue(IconMenuProperty);
    public static void SetIconMenu(Window i, object input) => i.SetValue(IconMenuProperty, input);

    #endregion
    
    #region IconSize

    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.RegisterAttached("IconSize", typeof(DoubleSize), typeof(XWindow), new FrameworkPropertyMetadata(null));
    [TypeConverter(typeof(DoubleSizeTypeConverter))]
    public static DoubleSize GetIconSize(Window i) => (DoubleSize)i.GetValue(IconSizeProperty);
    public static void SetIconSize(Window i, DoubleSize input) => i.SetValue(IconSizeProperty, input);

    #endregion

    #region IconVisibility

    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.RegisterAttached("IconVisibility", typeof(Visibility), typeof(XWindow), new FrameworkPropertyMetadata(Visibility.Visible));
    public static Visibility GetIconVisibility(Window i) => (Visibility)i.GetValue(IconVisibilityProperty);
    public static void SetIconVisibility(Window i, Visibility input) => i.SetValue(IconVisibilityProperty, input);

    #endregion

    #region HeaderPlacement

    public static readonly DependencyProperty HeaderPlacementProperty = DependencyProperty.RegisterAttached("HeaderPlacement", typeof(TopBottom), typeof(XWindow), new FrameworkPropertyMetadata(TopBottom.Top));
    public static TopBottom GetHeaderPlacement(Window i) => (TopBottom)i.GetValue(HeaderPlacementProperty);
    public static void SetHeaderPlacement(Window i, TopBottom input) => i.SetValue(HeaderPlacementProperty, input);

    #endregion

    #region HeaderVisibility

    public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.RegisterAttached("HeaderVisibility", typeof(Visibility), typeof(XWindow), new FrameworkPropertyMetadata(Visibility.Visible));
    public static Visibility GetHeaderVisibility(Window i) => (Visibility)i.GetValue(HeaderVisibilityProperty);
    public static void SetHeaderVisibility(Window i, Visibility input) => i.SetValue(HeaderVisibilityProperty, input);

    #endregion

    #region IsAlwaysMaximized

    public static readonly DependencyProperty IsAlwaysMaximizedProperty = DependencyProperty.RegisterAttached("IsAlwaysMaximized", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false, OnIsAlwaysMaximizedChanged));
    public static bool GetIsAlwaysMaximized(Window i) => (bool)i.GetValue(IsAlwaysMaximizedProperty);
    public static void SetIsAlwaysMaximized(Window i, bool input) => i.SetValue(IsAlwaysMaximizedProperty, input);
    static void OnIsAlwaysMaximizedChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
            window.RegisterHandlerAttached((bool)e.NewValue, IsAlwaysMaximizedProperty, i => i.StateChanged += IsAlwaysMaximized_StateChanged, i => i.StateChanged -= IsAlwaysMaximized_StateChanged);
    }

    static void IsAlwaysMaximized_StateChanged(object sender, EventArgs e)
    {
        if (sender is Window window)
        {
            if (window.WindowState != WindowState.Maximized)
                window.WindowState = WindowState.Maximized;
        }
    }

    #endregion

    #region IsChild

    public static readonly DependencyProperty IsChildProperty = DependencyProperty.RegisterAttached("IsChild", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false, OnIsChildChanged));
    public static bool GetIsChild(Window i) => (bool)i.GetValue(IsChildProperty);
    public static void SetIsChild(Window i, bool input) => i.SetValue(IsChildProperty, input);
    static void OnIsChildChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window i)
        {
            i.Owner = null;
            if ((bool)e.NewValue)
                i.Owner = Application.Current.MainWindow;
        }
    }

    #endregion

    #region IsDialogShowing

    public static readonly DependencyProperty IsDialogShowingProperty = DependencyProperty.RegisterAttached("IsDialogShowing", typeof(bool), typeof(XWindow), new FrameworkPropertyMetadata(false));
    public static bool GetIsDialogShowing(Window i) => (bool)i.GetValue(IsDialogShowingProperty);
    public static void SetIsDialogShowing(Window i, bool input) => i.SetValue(IsDialogShowingProperty, input);

    #endregion
    
    #region MaximizeCommand

    public static readonly RoutedUICommand MaximizeCommand = new(nameof(MaximizeCommand), nameof(MaximizeCommand), typeof(XWindow));
    static void OnMaximize(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MaximizeWindow(sender as Window);
    static void OnCanMaximize(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanMaximize(sender as Window);

    #endregion

    #region Menu

    public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached("Menu", typeof(object), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static object GetMenu(Window i) => (object)i.GetValue(MenuProperty);
    public static void SetMenu(Window i, object input) => i.SetValue(MenuProperty, input);

    #endregion

    #region MenuOrientation

    public static readonly DependencyProperty MenuOrientationProperty = DependencyProperty.RegisterAttached("MenuOrientation", typeof(Orientation), typeof(XWindow), new FrameworkPropertyMetadata(Orientation.Horizontal));
    public static Orientation GetMenuOrientation(Window i) => (Orientation)i.GetValue(MenuOrientationProperty);
    public static void SetMenuOrientation(Window i, Orientation input) => i.SetValue(MenuOrientationProperty, input);

    #endregion

    #region MenuTemplate

    public static readonly DependencyProperty MenuTemplateProperty = DependencyProperty.RegisterAttached("MenuTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetMenuTemplate(Window i) => (DataTemplate)i.GetValue(MenuTemplateProperty);
    public static void SetMenuTemplate(Window i, DataTemplate input) => i.SetValue(MenuTemplateProperty, input);

    #endregion

    #region MinimizeCommand

    public static readonly RoutedUICommand MinimizeCommand = new(nameof(MinimizeCommand), nameof(MinimizeCommand), typeof(XWindow));
    static void OnMinimize(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(sender as Window);
    static void OnCanMinimize(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    #endregion
    
    #region Notifications

    static readonly DependencyPropertyKey NotificationsKey = DependencyProperty.RegisterAttachedReadOnly("Notifications", typeof(ICollectionChanged), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty NotificationsProperty = NotificationsKey.DependencyProperty;
    public static ICollectionChanged GetNotifications(Window i) => i.GetValueOrSetDefault(NotificationsKey, () => new ObservableCollection<Notification>());

    #endregion

    #region NotificationTemplate

    public static readonly DependencyProperty NotificationTemplateProperty = DependencyProperty.RegisterAttached("NotificationTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetNotificationTemplate(Window i) => (DataTemplate)i.GetValue(NotificationTemplateProperty);
    public static void SetNotificationTemplate(Window i, DataTemplate input) => i.SetValue(NotificationTemplateProperty, input);

    #endregion

    #region Placement

    public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(WindowPlacements), typeof(XWindow), new FrameworkPropertyMetadata(WindowPlacements.None, OnPlacementChanged));
    public static WindowPlacements GetPlacement(Window i) => (WindowPlacements)i.GetValue(PlacementProperty);
    public static void SetPlacement(Window i, WindowPlacements input) => i.SetValue(PlacementProperty, input);
    static void OnPlacementChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
        {
            switch ((WindowPlacements)e.OldValue)
            {
                case WindowPlacements.Bottom:
                    Bottom_Unsubscribe(window);
                    break;

                case WindowPlacements.Top:
                    window.Topmost = false;
                    break;
            }
            switch ((WindowPlacements)e.NewValue)
            {
                case WindowPlacements.Bottom:
                    Bottom_Subscribe(window);
                    break;

                case WindowPlacements.Top:
                    window.Topmost = true;
                    break;
            }
        }
    }

    ///

    static void Bottom_Subscribe(Window window)
    {
        SetBottomHandler(window, new(window));

        window.Closing
            += Bottom_Closing;
        window.Loaded
            += Bottom_Loaded;
    }

    static void Bottom_Unsubscribe(Window window)
    {
        window.Closing
            -= Bottom_Closing;
        window.Loaded
            -= Bottom_Loaded;

        if (GetBottomHandler(window) is InternalBottomHandler i)
        {
            HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).RemoveHook(new HwndSourceHook(i.WndProc));
            SetBottomHandler(window, null);
        }
    }

    ///

    static void Bottom_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (sender is Window window)
            Bottom_Unsubscribe(window);
    }

    static void Bottom_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Window window)
        {
            window.Loaded
                -= Bottom_Loaded;

            GetBottomHandler(window).Update();
            HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).AddHook(new HwndSourceHook(GetBottomHandler(window).WndProc));
        }
    }

    #region (internal) BottomHandler

    internal static readonly DependencyProperty BottomHandlerProperty = DependencyProperty.RegisterAttached("BottomHandler", typeof(InternalBottomHandler), typeof(XWindow), new FrameworkPropertyMetadata(null));
    internal static InternalBottomHandler GetBottomHandler(Window i) => (InternalBottomHandler)i.GetValue(BottomHandlerProperty);
    internal static void SetBottomHandler(Window i, InternalBottomHandler input) => i.SetValue(BottomHandlerProperty, input);

    #endregion

    #region (internal) class InternalBottomHandler

    internal class InternalBottomHandler
    {
        static readonly IntPtr HWND_BOTTOM = new(1);

        public readonly Window Window;

        public InternalBottomHandler(Window window) => Window = window;

        public void Update() => SetWindowPos(new WindowInteropHelper(Window).Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETFOCUS)
            {
                Update();
                handled = true;
            }
            return IntPtr.Zero;
        }
    }

    #endregion

    #endregion

    #region RestoreCommand

    public static readonly RoutedUICommand RestoreCommand = new(nameof(RestoreCommand), nameof(RestoreCommand), typeof(XWindow));
    static void OnRestore(object sender, ExecutedRoutedEventArgs e) => SystemCommands.RestoreWindow(sender as Window);
    static void OnCanRestore(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanRestore(sender as Window);

    #endregion

    #region Result

    public static readonly DependencyProperty ResultProperty = DependencyProperty.RegisterAttached("Result", typeof(int), typeof(XWindow), new FrameworkPropertyMetadata(-1));
    public static int GetResult(Window i) => (int)i.GetValue(ResultProperty);
    public static void SetResult(Window i, int input) => i.SetValue(ResultProperty, input);

    #endregion

    #region StartupLocation

    public static readonly DependencyProperty StartupLocationProperty = DependencyProperty.RegisterAttached("StartupLocation", typeof(WindowStartupLocation), typeof(XWindow), new FrameworkPropertyMetadata(WindowStartupLocation.Manual, OnStartupLocationChanged));
    public static WindowStartupLocation GetStartupLocation(Window i) => (WindowStartupLocation)i.GetValue(StartupLocationProperty);
    public static void SetStartupLocation(Window i, WindowStartupLocation input) => i.SetValue(StartupLocationProperty, input);
    static void OnStartupLocationChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window)
            window.WindowStartupLocation = (WindowStartupLocation)e.NewValue;
    }

    #endregion

    #region SystemMenuItems

    const uint MF_SEPARATOR = 0x800;

    const uint MF_BYCOMMAND = 0x0;

    const uint MF_BYPOSITION = 0x400;

    const uint MF_STRING = 0x0;

    const uint MF_ENABLED = 0x0;

    const uint MF_DISABLED = 0x2;
    
    ///

    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool InsertMenu(IntPtr hmenu, int position, uint flags, uint item_id, [MarshalAs(UnmanagedType.LPTStr)]string item_text);

    [DllImport("user32.dll")]
    static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    static Dictionary<Window, IntPtr> systemMenus;

    ///

    ///<summary>https://www.codeproject.com/Articles/70568/An-MVVM-friendly-approach-to-adding-system-menu-en</summary>
    public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.RegisterAttached("MenuItems", typeof(FreezableCollection<SystemMenuItem>), typeof(XWindow), new PropertyMetadata(OnMenuItemsChanged));
    public static FreezableCollection<SystemMenuItem> GetMenuItems(Window i) => i.GetValueOrSetDefault(MenuItemsProperty, () => new FreezableCollection<SystemMenuItem>());
    public static void SetMenuItems(Window i, FreezableCollection<SystemMenuItem> input) => i.SetValue(MenuItemsProperty, input);
    static void OnMenuItemsChanged(DependencyObject i, DependencyPropertyChangedEventArgs e)
    {
        if (i is Window window)
        {
            if (e.NewValue is FreezableCollection<SystemMenuItem> items)
                SetMenuItems(window, items);
        }
    }

    #endregion

    #region TaskbarIconTemplate

    public static readonly DependencyProperty TaskbarIconTemplateProperty = DependencyProperty.RegisterAttached("TaskbarIconTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetTaskbarIconTemplate(Window i) => (DataTemplate)i.GetValue(TaskbarIconTemplateProperty);
    public static void SetTaskbarIconTemplate(Window i, DataTemplate input) => i.SetValue(TaskbarIconTemplateProperty, input);

    #endregion

    #region TitleTemplate

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.RegisterAttached("TitleTemplate", typeof(DataTemplate), typeof(XWindow), new FrameworkPropertyMetadata(null));
    public static DataTemplate GetTitleTemplate(Window i) => (DataTemplate)i.GetValue(TitleTemplateProperty);
    public static void SetTitleTemplate(Window i, DataTemplate input) => i.SetValue(TitleTemplateProperty, input);

    #endregion

    #region Type

    public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached("Type", typeof(WindowTypes), typeof(XWindow), new FrameworkPropertyMetadata(WindowTypes.Default));
    public static WindowTypes GetType(Window i) => (WindowTypes)i.GetValue(TypeProperty);
    public static void SetType(Window i, WindowTypes input) => i.SetValue(TypeProperty, input);

    #endregion

    #endregion

    #region XWindow

    static void OnSourceInitialized(object sender, EventArgs e)
    {
        if (sender is Window window)
        {
            /*
            var helper = new WindowInteropHelper(window);

            var menu = GetSystemMenu(helper.Handle, false);
            systemMenus.Add(window, menu);

            if (GetMenuItems(window).Count > 0)
                InsertMenu(menu, -1, MF_BYPOSITION | MF_SEPARATOR, 0, String.Empty);

            foreach (SystemMenuItem item in GetMenuItems(window))
                InsertMenu(systemMenus[window], (int)item.Id, MF_BYCOMMAND | MF_STRING, (uint)item.Id, item.Header);

            var source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
            */

            ///

            window.SourceInitialized -= OnSourceInitialized;
            HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).AddHook(new HwndSourceHook(WndProc));

            window.AddOnce
                (new CommandBinding(CloseCommand,
                OnClose, CanClose));
            window.AddOnce
                (new CommandBinding(MarkNotificationCommand,
                OnMarkNotificationCommandExecuted, OnCanExecuteMarkNotificationCommand));
            window.AddOnce
                (new CommandBinding(MaximizeCommand,
                OnMaximize, OnCanMaximize));
            window.AddOnce
                (new CommandBinding(MinimizeCommand,
                OnMinimize, OnCanMinimize));
            window.AddOnce
                (new CommandBinding(RestoreCommand,
                OnRestore, OnCanRestore));
        }
    }

    ///

    static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        Window window = null;
        switch (msg)
        {
            case (int)WindowMessages.WM_SYSCOMMAND:
                break;
                var item = GetMenuItems(window).Where(i => i.Id == wParam.ToInt32()).FirstOrDefault();
                if (item != null)
                {
                    item.Command.Execute(item.CommandParameter);
                    handled = true;
                }

            case (int)WindowMessages.WM_INITMENUPOPUP:
                break;
                if (systemMenus[window] == wParam)
                {
                    foreach (var i in GetMenuItems(window))
                        EnableMenuItem(systemMenus[window], (uint)i.Id, i.Command.CanExecute(i.CommandParameter) ? MF_ENABLED : MF_DISABLED);

                    handled = true;
                }

            case (int)WindowMessages.WM_GETMINMAXINFO:
                GetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;
        }
        return IntPtr.Zero;
    }

    #endregion

    #region Methods

    [DllImport("user32.dll")]
    static extern IntPtr BeginDeferWindowPos(int nNumWindows);

    [DllImport("user32.dll")]
    static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

    [DllImport("user32.dll")]
    static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    ///

    static void GetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

        // Adjust the maximized size and position to fit the work area of the correct monitor
        var MONITOR_DEFAULTTONEAREST = 0x00000002;
        IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MONITORINFO();
            GetMonitorInfo(monitor, monitorInfo);

            var rcWorkArea 
                = monitorInfo.rcWork;
            var rcMonitorArea 
                = monitorInfo.rcMonitor;

            mmi.ptMaxPosition.x 
                = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
            mmi.ptMaxPosition.y 
                = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
            mmi.ptMaxSize.x 
                = Math.Abs(rcWorkArea.right - rcWorkArea.left);
            mmi.ptMaxSize.y 
                = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
        }
        Marshal.StructureToPtr(mmi, lParam, true);
    }

    ///

    public static double ActualLeft(this Window input)
    {
        if (input.WindowState == WindowState.Maximized)
        {
            var field = typeof(Window).GetField("_actualLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (double)field.GetValue(input);
        }
        else return input.Left;
    }

    public static double ActualTop(this Window input)
    {
        if (input.WindowState == WindowState.Maximized)
        {
            var field = typeof(Window).GetField("_actualTop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (double)field.GetValue(input);
        }
        else return input.Top;
    }

    ///

    public static bool CanMaximize(this Window window) => window != null && window.WindowStyle != WindowStyle.ToolWindow && window.WindowState != WindowState.Maximized && window.ResizeMode != ResizeMode.NoResize;

    public static void Center(this Window input)
    {
        input.Left 
            = (SystemParameters.PrimaryScreenWidth / 2.0) - (input.Width / 2.0);
        input.Top 
            = (SystemParameters.PrimaryScreenHeight / 2.0) - (input.Height / 2.0);
    }

    public static bool CanRestore(this Window window) => window != null && window.WindowStyle != WindowStyle.ToolWindow && window.WindowState != WindowState.Normal && window.ResizeMode != ResizeMode.NoResize;

    #endregion
}