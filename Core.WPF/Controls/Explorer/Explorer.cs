using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Controls;

public class Explorer : Control, IExplorer
{
    public static readonly ReferenceKey<AddressBox> AddressBoxKey = new();

    public static readonly ReferenceKey<Browser> BrowserKey = new();

    ///

    public static readonly Limit DefaultLimit = new(50);

    public static string DefaultPath => StoragePath.Root;

    ///

    public const double HiddenOpacity = 0.6;

    ///

    public event EventHandler<EventArgs<string>> FileOpened;

    public event DefaultEventHandler<IEnumerable<string>> FilesOpened;

    public event CollectionChangedEventHandler SelectionChanged;

    ///

    public static readonly DependencyProperty AddressVisibilityProperty = DependencyProperty.Register(nameof(AddressVisibility), typeof(Visibility), typeof(Explorer), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility AddressVisibility
    {
        get => (Visibility)GetValue(AddressVisibilityProperty);
        set => SetValue(AddressVisibilityProperty, value);
    }

    public static readonly DependencyProperty FileOpenedCommandProperty = DependencyProperty.Register(nameof(FileOpenedCommand), typeof(ICommand), typeof(Explorer), new FrameworkPropertyMetadata(null));
    public ICommand FileOpenedCommand
    {
        get => (ICommand)GetValue(FileOpenedCommandProperty);
        set => SetValue(FileOpenedCommandProperty, value);
    }

    public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(StringHistory), typeof(Explorer), new FrameworkPropertyMetadata(null));
    public StringHistory History
    {
        get => (StringHistory)GetValue(HistoryProperty);
        set => SetValue(HistoryProperty, value);
    }

    public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(nameof(Options), typeof(ExplorerOptions), typeof(Explorer), new FrameworkPropertyMetadata(null));
    public ExplorerOptions Options
    {
        get => (ExplorerOptions)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public string Path
    {
        get => XExplorer.GetPath(this);
        set => XExplorer.SetPath(this, value);
    }

    static readonly DependencyPropertyKey SelectionKey = DependencyProperty.RegisterReadOnly(nameof(Selection), typeof(ICollectionChanged), typeof(Explorer), new FrameworkPropertyMetadata(null, OnSelectionChanged));
    public static readonly DependencyProperty SelectionProperty = SelectionKey.DependencyProperty;
    public ICollectionChanged Selection
    {
        get => (ICollectionChanged)GetValue(SelectionProperty);
        private set => SetValue(SelectionKey, value);
    }
    static void OnSelectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<Explorer>().OnSelectionChanged(e.NewValue as ICollectionChanged);

    public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand), typeof(Explorer), new FrameworkPropertyMetadata(null));
    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(Explorer), new FrameworkPropertyMetadata(SelectionMode.Multiple));
    public SelectionMode SelectionMode
    {
        get => (SelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    ///

    public Explorer()
    {
        SetCurrentValue(HistoryProperty,
            new StringHistory(DefaultLimit));
        SetCurrentValue(OptionsProperty,
            new ExplorerOptions());

        this.RegisterHandler(i =>
        {
            this.GetChild(BrowserKey).FilesOpened
                += OnFilesOpened;
            this.GetChild(BrowserKey).SelectionChanged
                += OnSelectionChanged;
        },
        i =>
        {
            this.GetChild(BrowserKey).FilesOpened
                -= OnFilesOpened;
            this.GetChild(BrowserKey).SelectionChanged
                -= OnSelectionChanged;
        });
    }

    ///

    void OnFileOpened(object sender, EventArgs<string> e)
    {
        FileOpened?.Invoke(sender, e);
        FileOpenedCommand?.Execute(e.Value);
    }

    void OnFilesOpened(object sender, EventArgs<IEnumerable<string>> e)
    {
        OnFileOpened(this, new(e.Value.First()));
        FilesOpened?.Invoke(sender, e);
    }

    void OnRefreshed(object sender, RoutedEventArgs e) => _ = this.GetChild(BrowserKey).Items.RefreshAsync();

    void OnSelectionChanged(object sender, EventArgs<ICollectionChanged> e) => Selection ??= e.Value;

    ///

    protected virtual void OnSelectionChanged(ICollectionChanged input)
    {
        SelectionChanged?.Invoke(this, new(input));
        SelectionChangedCommand?.Execute(input.FirstOrDefault<Item>());
    }

    ///

    ICommand groupCommand;
    public ICommand GroupCommand => groupCommand ??= new RelayCommand<ItemProperty>
        (i => Options.CurrentFolderOptions.GroupName = i, 
        i => Options?.CurrentFolderOptions != null);

    ICommand groupDirectionCommand;
    public ICommand GroupDirectionCommand => groupDirectionCommand ??= new RelayCommand<ListSortDirection>
        (i => Options.CurrentFolderOptions.GroupDirection = i.Convert(), 
        i => Options?.CurrentFolderOptions != null);

    ICommand refreshCommand;
    public ICommand RefreshCommand => refreshCommand ??= new RelayCommand
        (() => _ = this.GetChild(BrowserKey).Items.RefreshAsync());

    ICommand sortCommand;
    public ICommand SortCommand => sortCommand ??= new RelayCommand<ItemProperty>
        (i => Options.CurrentFolderOptions.SortName = i, 
        i => Options?.CurrentFolderOptions != null);

    ICommand sortDirectionCommand;
    public ICommand SortDirectionCommand => sortDirectionCommand ??= new RelayCommand<ListSortDirection>
        (i => Options.CurrentFolderOptions.SortDirection = i.Convert(), 
        i => Options?.CurrentFolderOptions != null);
}