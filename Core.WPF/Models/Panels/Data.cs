using Imagin.Core.Collections;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Serializable]
public abstract class DataPanel : Panel, IElementReference
{
    public static readonly ReferenceKey<DataGrid> DataGridReferenceKey = new();

    enum Category { General, Group, Sort }

    #region Events

    public event EventHandler<EventArgs<IList>> SelectionChanged;

    #endregion

    #region Properties

    [Category(Category.General), Option]
    public Bullets Bullet { get => Get(Bullets.NumberParenthesis); set => Set(value); }

    [Hide]
    public List<bool> Columns { get => Get(new List<bool>()); set => Set(value); }

    [Hide]
    public virtual int Count => Data?.Count ?? 0;

    [Hide]
    [NonSerializable]
    public ICollectionChanged Data { get => Get<ICollectionChanged>(); set => Set(value); }

    [Hide]
    public DataGrid DataGrid { get; private set; }

    [Category(Category.Group)]
    [Name("Direction")]
    [Option]
    public ListSortDirection GroupDirection { get => Get(ListSortDirection.Ascending); set => Set(value); }

    [Hide]
    public string GroupName => GroupNames?.Count > GroupNameIndex && GroupNameIndex >= 0 ? (string)GroupNames[GroupNameIndex] : null;

    [Category(Category.Group), Name("Name"), Option]
    [Int32Style(Int32Style.Index, nameof(GroupNames))]
    public virtual int GroupNameIndex { get => Get(0); set => Set(value); }

    [Hide]
    public virtual IList GroupNames { get; }

    [Hide]
    public object SelectedItem { get => Get<object>(); set => Set(value); }

    ICollectionChanged selectedItems;
    [Hide]
    public ICollectionChanged SelectedItems
    {
        get => selectedItems;
        private set
        {
            if (selectedItems != null)
                selectedItems.CollectionChanged -= OnSelectionChanged;

            selectedItems = value;
            if (selectedItems != null)
                selectedItems.CollectionChanged += OnSelectionChanged;
        }
    }

    [Category(Category.Sort), Name("Direction"), Option]
    public ListSortDirection SortDirection { get => Get(ListSortDirection.Descending); set => Set(value); }

    [Hide]
    public string SortName => SortNames?.Count > SortNameIndex && SortNameIndex >= 0 ? (string)SortNames[SortNameIndex] : null;

    [Category(Category.Sort), Name("Name"), Option]
    [Int32Style(Int32Style.Index, nameof(SortNames))]
    public virtual int SortNameIndex { get => Get(0); set => Set(value); }

    [Hide]
    public virtual IList SortNames { get; }

    [Hide]
    public sealed override string Title
    {
        get
        {
            var result = TitleKey.Translate();
            if (TitleCount == 0)
                return result;

            return $"{result}{TitleSuffix} ({TitleCount})";
        }
    }

    [Hide]
    public virtual int TitleCount => Data?.Count ?? 0;

    /// <summary>A key used to localize the title.</summary>
    [Hide]
    public abstract string TitleKey { get; }

    [Hide]
    public sealed override bool TitleLocalized => false;

    /// <summary>A suffix to append to the title after localization.</summary>
    [Hide]
    public virtual string TitleSuffix { get; }

    #endregion

    #region DataPanel

    public DataPanel() : base()
    {
        Current.Get<MainViewOptions>().LanguageChanged += OnLanguageChanged;
    }

    public DataPanel(ICollectionChanged input) : this()
    {
        Data = input;
    }

    void IElementReference.SetReference(IElementKey key, FrameworkElement element)
    {
        if (key == DataGridReferenceKey)
        {
            DataGrid = element as DataGrid;
            DataGrid.If(i => i.Unloaded += OnDataGridUnloaded);

            SelectedItems = XSelector.GetSelectedItems(DataGrid);
        }
    }

    #endregion

    #region Methods

    void OnDataGridUnloaded(object sender, RoutedEventArgs e)
    {
        DataGrid.Unloaded -= OnDataGridUnloaded;
        DataGrid = null;

        SelectedItems = null;
    }

    void OnLanguageChanged(object sender, EventArgs e)
    {
        Update(() => Title);
    }

    void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Update(() => Count);
        Update(() => Title);

        OnItemsChanged();
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnItemAdded(e.NewItems[0]);
                break;

            case NotifyCollectionChangedAction.Remove:
                OnItemRemoved(e.OldItems[0]);
                break;
        }
    }

    void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var result = new List<object>();
        (sender as ICollectionChanged).ForEach<object>(i => result.Add(i));
        SelectionChanged?.Invoke(this, new EventArgs<IList>(result));
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Data):
                Update(() => Count);
                Update(() => Title);

                if (e.OldValue != null)
                    e.OldValue.As<ICollectionChanged>().CollectionChanged -= OnItemsChanged;

                if (e.NewValue != null)
                {
                    e.NewValue.As<ICollectionChanged>().CollectionChanged
                        -= OnItemsChanged;
                    e.NewValue.As<ICollectionChanged>().CollectionChanged
                        += OnItemsChanged;
                }
                break;

            case nameof(GroupNameIndex):
                Update(() => GroupName);
                break;

            case nameof(SortNameIndex):
                Update(() => SortName);
                break;
        }
    }

    protected virtual void OnItemsChanged() { }

    protected virtual void OnItemRemoved(object input) { }

    protected virtual void OnItemAdded(object input) { }

    ICommand clearCommand;
    [Name("Clear")]
    [Image(SmallImages.Trash)]
    [Header]
    public virtual ICommand ClearCommand => clearCommand ??= new RelayCommand(() => Data.Clear(), () => Data?.Count > 0);

    ICommand refreshCommand;
    [Name("Refresh")]
    [Pin(Pin.BelowOrRight)]
    [Image(SmallImages.Refresh)]
    [Index(int.MaxValue)]
    [Header]
    public virtual ICommand RefreshCommand => refreshCommand ??= new RelayCommand(() => DataGrid.ItemsSource.As<ListCollectionView>().Refresh(), () => DataGrid?.ItemsSource is ListCollectionView);

    ICommand removeCommand;
    [Hide]
    public ICommand RemoveCommand => removeCommand ??= new RelayCommand<object>(i => Data.Remove(i), i => i != null && Data?.Contains(i) == true);

    #endregion
}