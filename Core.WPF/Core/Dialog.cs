using Imagin.Core.Analytics;
using Imagin.Core.Colors;
using Imagin.Core.Controls;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imagin.Core;

public static class Dialog
{
    public class ColorModel : Base
    {
        public readonly ColorDocument Document;

        public ColorDocument ActiveDocument { get => Get<ColorDocument>(); set => Set(value); }
        
        public ColorViewOptions Options { get; private set; }

        public DockViewModel ViewModel { get; private set; }

        public ColorModel(Color color) : base()
        {
            var defaultOptions = new ColorViewOptions();
            defaultOptions.Load(out ColorViewOptions existingOptions);
            Options = existingOptions ?? defaultOptions;

            ViewModel = new(Options);            

            var document = new ColorDocument(color, typeof(HSB), Options.Profiles) { CanClose = false, CanFloat = false };
            document.NewColor = color;

            ViewModel.Documents.Add(document);
            Document = document;
        }

        internal void OnClosed() => Options.Save();
    }

    public class ProgressModel : Base
    {
        public bool IsIndeterminate { get => Get(false); set => Set(value); }

        public double Progress { get => Get(0.0); set => Set(value); }

        public string Text { get => Get(""); set => Set(value); }
    }

    public class MessageModel : Base
    {
        public string Caption { get => Get<string>(null); private set => Set(value); }

        public object Content { get => Get<object>(); private set => Set(value); }

        public Uri Image { get => Get<Uri>(); private set => Set(value); }

        public BooleanAccessor NeverShow { get => Get<BooleanAccessor>(); private set => Set(value); }

        public MessageModel(string caption, object content, Uri image, BooleanAccessor neverShow) : base()
        {
            Caption = caption; Content = content; Image = image; NeverShow = neverShow;
        }
    }

    ///

    public static readonly ResourceKey AboutTemplateKey = new();

    public static readonly ResourceKey ColorTemplateKey = new();

    public static readonly ResourceKey DownloadTemplateKey = new();

    public static readonly ResourceKey ObjectTemplateKey = new();

    public static readonly ResourceKey ProgressTemplateKey = new();

    public static readonly ResourceKey ResultTemplateKey = new();

    ///

    static Window MainWindow => Current.Get<IMainViewModel>().View;

    ///

    public static void Close(Window input, int result = 0) => input.GetChild(XWindow.DialogPresenterKey).Close(result);

    ///

    static void Launch(DialogReference dialog)
    {
        var options = Current.Get<MainViewOptions>();

        dialog.Background
            = options.DialogBackground;

        dialog.MaxHeight
            = options.DialogMaximumHeight;
        dialog.MaxWidth
            = options.DialogMaximumWidth;
        dialog.MinWidth
            = options.DialogMinimumWidth;

        MainWindow.GetChild(XWindow.DialogPresenterKey).Show(dialog);
    }

    ///

    public static void Show(string title, object content, LargeImages largeImage = LargeImages.Info, params Button[] buttons)
        => Show(title, content, null, largeImage, buttons);

    public static void Show(string title, object content, Uri smallImage, LargeImages? largeImage, params Button[] buttons)
        => Show(title, content, smallImage, largeImage, null, buttons);

    public static void Show(string title, object content, Uri smallImage, LargeImages? largeImage, Action<int> result, params Button[] buttons)
        => Show(title, content, smallImage, largeImage, null, result, buttons);

    public static void Show(string title, object content, SmallImages smallImage, LargeImages? largeImage, Action<int> result, params Button[] buttons)
        => Show(title, content, Resource.GetImageUri(smallImage), largeImage, null, result, buttons);

    public static void Show(string title, object content, Uri smallImage, LargeImages? largeImage, BooleanAccessor neverShow, Action<int> result, params Button[] buttons)
    {
        var dialog = new DialogReference(title, new MessageModel(null, content, largeImage?.IfGet(Markup.LargeImage.GetUri), neverShow), ResultTemplateKey, smallImage ?? Resource.GetImageUri(SmallImages.Info), largeImage ?? LargeImages.Info, buttons) { OnClosed = result };
        Launch(dialog);
    }

    ///

    public static void ShowAbout(params Button[] buttons)
    {
        var dialog = new DialogReference("About", null, AboutTemplateKey, SmallImages.Info, buttons);
        Launch(dialog);
    }

    public static void ShowColor(string title, Color oldColor, out Color newColor, Action<int> result)
    {
        var content = new ColorModel(oldColor);

        var dialog = new DialogReference(title ?? "Color", content, ColorTemplateKey, SmallImages.Color, Buttons.SaveCancel)
        {
            MaxHeight = 540, MaxWidth = 900, MinWidth = 720,
            OnClosed = result
        };
        Launch(dialog);

        content.OnClosed();
        newColor = content.Document?.NewColor ?? oldColor;
    }

    public static void ShowDownload(string title, params Button[] buttons)
    {
        var dialog = new DialogReference(title ?? "Download", null, DownloadTemplateKey, SmallImages.Download, buttons ?? Buttons.Done);
        Launch(dialog);
    }

    ///

    public static void ShowError(string title, Error error, params Button[] buttons)
        => ShowError(title, error, null, null, buttons);

    public static void ShowError(string title, Error error, BooleanAccessor neverShow, Action<int> result, params Button[] buttons)
        => Show(title, error, Resource.GetImageUri(SmallImages.XRound), LargeImages.Error, neverShow, result, buttons);

    ///

    public static void ShowMessage(string title, Message message, params Button[] buttons)
        => ShowMessage(title, message, null, buttons);

    public static void ShowMessage(string title, Message message, Action<int> result, params Button[] buttons)
        => Show(title, message, Resource.GetImageUri(SmallImages.Info), LargeImages.Info, result, buttons);

    ///

    public static void ShowSuccess(string title, Success success, params Button[] buttons)
        => ShowSuccess(title, success, null, buttons);

    public static void ShowSuccess(string title, Success success, Action<int> result, params Button[] buttons)
        => Show(title, success, Resource.GetImageUri(SmallImages.CheckmarkRound), LargeImages.Success, result, buttons);

    ///

    public static void ShowWarning(string title, Warning warning, params Button[] buttons)
        => ShowWarning(title, warning, null, buttons);

    public static void ShowWarning(string title, Warning warning, Action<int> result, params Button[] buttons)
        => Show(title, warning, Resource.GetImageUri(SmallImages.Warning), LargeImages.Warning, result, buttons);

    public static void ShowWarning(string title, Warning warning, BooleanAccessor neverShow, Action<int> onClosed, params Button[] buttons)
        => Show(title, warning, Resource.GetImageUri(SmallImages.Warning), LargeImages.Warning, neverShow, onClosed, buttons);

    ///

    public static void ShowObject<T>(string title, T source, Uri smallImage = null, params Button[] buttons)
        => ShowObject(title, source, smallImage, null, buttons);

    public static void ShowObject<T>(string title, T source, Uri smallImage, Action<int> result, params Button[] buttons)
    {
        var dialog = new DialogReference(title ?? "Edit", source, ObjectTemplateKey, smallImage ?? new Uri(source.GetSmallImage()) ?? Resource.GetImageUri(SmallImages.Pencil), buttons) { OnClosed = result };
        Launch(dialog);
    }

    public static void ShowPanel<T>(T panel, Action<int> result = null, params Button[] buttons) where T : Models.Panel
    {
        var dialog = new DialogReference(panel.Title, panel, DockControl.PanelBodyTemplateKey, panel.Icon, result, buttons);
        Launch(dialog);
    }

    ///

    public static async Task<int> ShowProgress(string title, Action<ProgressModel> action, TimeSpan delay = default, bool isIndeterminate = true)
    {
        var model = new ProgressModel() { IsIndeterminate = isIndeterminate };

        var dialog = new DialogReference(title ?? "Progress", model, ProgressTemplateKey, SmallImages.HourGlass, Buttons.Cancel);
        Launch(dialog);

        if (delay > TimeSpan.Zero)
            await delay.TrySleep();

        await Task.Run(() => action?.Invoke(model));
        Close(MainWindow);

        return dialog.Result;
    }

    ///

    /*
    public partial class DownloadWindow : Window
    {
        public static readonly ReferenceKey<DownloadControl> DownloadControlKey = new();

        public static readonly DependencyProperty AutoCloseProperty = DependencyProperty.Register(nameof(AutoClose), typeof(bool), typeof(DownloadWindow), new FrameworkPropertyMetadata(false));
        public bool AutoClose
        {
            get => (bool)GetValue(AutoCloseProperty);
            set => SetValue(AutoCloseProperty, value);
        }

        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(DownloadWindow), new FrameworkPropertyMetadata(false));
        public bool AutoStart
        {
            get => (bool)GetValue(AutoStartProperty);
            set => SetValue(AutoStartProperty, value);
        }

        static readonly DependencyPropertyKey DestinationKey = DependencyProperty.RegisterReadOnly(nameof(Destination), typeof(string), typeof(DownloadWindow), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty DestinationProperty = DestinationKey.DependencyProperty;
        public string Destination
        {
            get => (string)GetValue(DestinationProperty);
            private set => SetValue(DestinationKey, value);
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(DownloadWindow), new FrameworkPropertyMetadata(null));
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageTemplateProperty = DependencyProperty.Register(nameof(MessageTemplate), typeof(DataTemplate), typeof(DownloadWindow), new FrameworkPropertyMetadata(null));
        public DataTemplate MessageTemplate
        {
            get => (DataTemplate)GetValue(MessageTemplateProperty);
            set => SetValue(MessageTemplateProperty, value);
        }

        static readonly DependencyPropertyKey SourceKey = DependencyProperty.RegisterReadOnly(nameof(Source), typeof(string), typeof(DownloadWindow), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SourceProperty = SourceKey.DependencyProperty;
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            private set => SetValue(SourceKey, value);
        }

        DownloadWindow() : base()
        {
            this.RegisterHandler(null, i => i.GetChild<DownloadControl>(DownloadControlKey).Downloaded -= OnDownloaded);
            InitializeComponent();

            this.GetChild<DownloadControl>(DownloadControlKey).Downloaded += OnDownloaded;
        }

        public DownloadWindow(string title, string message, string source, string destination) : this()
        {
            Source 
                = source;
            Destination
                = destination;

            SetCurrentValue(MessageProperty,
                message);
            SetCurrentValue(TitleProperty,
                title);
        }

        void OnDownloaded(object sender, EventArgs<Result> e)
        {
            if (AutoClose)
                Close();
        }
    }
    */
}