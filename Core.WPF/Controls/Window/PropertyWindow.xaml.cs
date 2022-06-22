using Imagin.Core.Linq;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls
{
    public partial class MemberWindow : Window
    {
        public static readonly ReferenceKey<MemberGrid> MemberGridKey = new();

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(MemberWindow), new FrameworkPropertyMetadata(null));
        public object Source
        {
            get => (object)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        //...

        public MemberWindow() : this(Buttons.Done) { }

        public MemberWindow(Button[] buttons) : base()
        {
            XWindow.SetFooterButtons(this, new Buttons(this, buttons));
            InitializeComponent();
        }

        //...

        public static T Show<T>(string title, T source, Action<MemberGrid> action = null)
        {
            var result = new MemberWindow();
            result.SetCurrentValue(SourceProperty, source);
            result.SetCurrentValue(TitleProperty, title.NullOrEmpty() ? source.GetType().Name : title);
            if (result.GetChild<MemberGrid>(MemberGridKey) is MemberGrid i)
                action?.Invoke(i);

            result.Show();
            return (T)result.Source;
        }

        public static T ShowDialog<T>(string title, T source, Action<MemberGrid> action = null)
            => ShowDialog(title, source, out int result, action);

        public static T ShowDialog<T>(string title, T source, out int result, Action<MemberGrid> action, params Button[] buttons)
        {
            var window = new MemberWindow(buttons ?? Buttons.Done);
            window.SetCurrentValue(SourceProperty, source);
            window.SetCurrentValue(TitleProperty, title.NullOrEmpty() ? source.GetType().Name : title);
            if (window.GetChild(MemberGridKey) is MemberGrid i)
                action?.Invoke(i);

            window.ShowDialog();
            result = XWindow.GetResult(window);
            return (T)window.Source;
        }
    }
}