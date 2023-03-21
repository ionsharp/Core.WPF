using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public class AddressBox : ComboBox, IExplorer
{
    public static readonly ReferenceKey<TextBox> TextBoxKey = new();

    public static readonly ReferenceKey<ToolBar> ToolBarKey = new();

    #region Fields

    readonly Handle handle = false;

    #endregion

    #region Properties

    new public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(AddressBox), new FrameworkPropertyMetadata(null, null, OnBackgroundCoerced));
    new public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
    static object OnBackgroundCoerced(DependencyObject i, object value) => value ?? Brushes.Transparent;

    public static readonly DependencyProperty CrumbsProperty = DependencyProperty.Register(nameof(Crumbs), typeof(StringCollection), typeof(AddressBox), new FrameworkPropertyMetadata(default(StringCollection)));
    public StringCollection Crumbs
    {
        get => (StringCollection)GetValue(CrumbsProperty);
        set => SetValue(CrumbsProperty, value);
    }

    public AddressBoxDropHandler DropHandler { get; private set; } = null;

    public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(StringHistory), typeof(AddressBox), new FrameworkPropertyMetadata(null));
    public StringHistory History
    {
        get => (StringHistory)GetValue(HistoryProperty);
        set => SetValue(HistoryProperty, value);
    }

    public string Path
    {
        get => XExplorer.GetPath(this);
        set => XExplorer.SetPath(this, value);
    }
        
    public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(AddressBox), new FrameworkPropertyMetadata(null));
    public ICommand RefreshCommand
    {
        get => (ICommand)GetValue(RefreshCommandProperty);
        set => SetValue(RefreshCommandProperty, value);
    }

    #endregion

    #region AddressBox

    public AddressBox() : base()
    {
        DropHandler 
            = new AddressBoxDropHandler(this);

        SetCurrentValue(CrumbsProperty, 
            new StringCollection());
        SetCurrentValue(HistoryProperty, 
            new StringHistory(Explorer.DefaultLimit));

        this.RegisterHandler(i =>
        {
            Update();
            this.GetChild<ToolBar>(ToolBarKey).If(j => j is not null, j => j.PreviewMouseDown += OnPreviewMouseDown);
            this.AddPathChanged(OnPathChanged);
        }, i => 
        {
            this.GetChild<ToolBar>(ToolBarKey).If(j => j is not null, j => j.PreviewMouseDown -= OnPreviewMouseDown);
            this.RemovePathChanged(OnPathChanged);
        });
    }

    #endregion

    #region Methods

    void Update()
    {
        Crumbs.Clear();
        Try.Invoke(() =>
        {
            var i = Path;
            while (!i.NullOrEmpty())
            {
                Crumbs.Insert(0, i);
                i = System.IO.Path.GetDirectoryName(i);
            }
        });
    }

    ///

    void OnPathChanged(object sender, PathChangedEventArgs e)
    {
        Update();
        handle.SafeInvoke(() => History?.Add(e.Path));
    }

    void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource.FindParent<ButtonBase>() is null)
        {
            SetCurrentValue(IsEditableProperty, true);
            this.GetChild<TextBox>(TextBoxKey)?.Focus();
        }
    }

    ///

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        Path = SelectedItem.ToString();
    }

    ///

    ICommand backCommand;
    public ICommand BackCommand => backCommand ??= new RelayCommand<object>(i => History.Undo(j => handle.Invoke(() => Path = j)), i => History?.CanUndo() == true);

    ICommand clearHistoryCommand;
    public ICommand ClearHistoryCommand => clearHistoryCommand ??= new RelayCommand<object>(i => History.Clear(), i => History?.Count > 0);

    ICommand enterCommand;
    public ICommand EnterCommand => enterCommand ??= new RelayCommand(() => SetCurrentValue(IsEditableProperty, false));

    ICommand forwardCommand;
    public ICommand ForwardCommand => forwardCommand ??= new RelayCommand<object>(i => History.Redo(j => handle.Invoke(() => Path = j)), i => History?.CanRedo() == true);

    ICommand goCommand;
    public ICommand GoCommand => goCommand ??= new RelayCommand<object>(i => SetCurrentValue(IsEditableProperty, false), i => true);

    ICommand goUpCommand;
    public ICommand GoUpCommand => goUpCommand ??= new RelayCommand(() => Try.Invoke(() => Path = Folder.Long.Parent(Path), e => Log.Write<AddressBox>(e)), () => Path != StoragePath.Root);

    ICommand setPathCommand;
    public ICommand SetPathCommand => setPathCommand ??= new RelayCommand<string>(i => Path = i, i => i is not null);

    #endregion
}