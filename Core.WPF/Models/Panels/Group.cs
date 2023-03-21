using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Models;

public interface IGroupPanel { object SelectedItem { get; } }

#region GroupPanel

[Serializable]
public abstract class GroupPanel : ViewPanel
{
    enum Category { [Index(-1)]Edit, Grid }

    [Category(Category.Grid), Name("Collapse"), Option, Show]
    public bool GridCollapsed { get => Get(true); set => Set(value); }

    [Category(Category.Grid), Name("Length (1)"), Option, Range(0.0, double.MaxValue, 0.01), Show]
    public GridLength GridLength1 { get => Get(new GridLength(6, GridUnitType.Star)); set => Set(value); }

    [Category(Category.Grid), Name("Length (2)"), Option, Range(0.0, double.MaxValue, 0.01), Show]
    public GridLength GridLength2 { get => Get(new GridLength(4, GridUnitType.Star)); set => Set(value); }

    [Category(Category.Grid), Name("Orientation"), Option, Show]
    public Orientation GridOrientation { get => Get(Orientation.Vertical); set => Set(value); }

    [Category(Category.Grid), Name("Reverse"), Option, Show]
    public bool GridReverse { get => Get(false); set => Set(value); }

    [Category(Category.Edit), Header, HideName, Image(SmallImages.Pencil), Name("Edit"), Show, Style(BooleanStyle.Button)]
    public bool IsEditing { get => Get(false); set => Set(value); }
}

#endregion

#region GroupPanel<T>

[Serializable]
public abstract class GroupPanel<T> : GroupPanel, IGroupPanel
{
    enum Category { AddRemove, Clipboard, Edit, Export, Import, Move, View }

    #region Properties

    [Description("The default name of new groups."), Option, Reserve, Show]
    public string DefaultGroupName { get => Get("Untitled group"); set => Set(value); }

    [Description("The suffix to append to the name of cloned items."), Option, Reserve, Show]
    public string DefaultCloneSuffix { get => Get(" (Clone)"); set => Set(value); }

    [Description("The default name of new items."), Option, Reserve, Show]
    public string DefaultItemName { get => Get("Untitled"); set => Set(value); }

    [Hide]
    public GroupWriter<T> Groups { get => Get<GroupWriter<T>>(); set => Set(value); }

    [Category(nameof(Category.AddRemove)), CollectionStyle(ItemCommand = nameof(AddCommand)), Header, HideName, Image(SmallImages.Plus), Index(0), Name("Add"), Show, VisibilityTrigger(nameof(ItemTypeCount), 1, Operators.Greater)]
    public ListCollectionView ItemTypes => new(new ObservableCollection<Type>(GetItemTypes()));

    [Hide]
    public int ItemTypeCount => GetItemTypes().Count();

    [Hide]
    public GroupCollection<T> SelectedGroup => SelectedGroupIndex >= 0 && SelectedGroupIndex < Groups?.Count ? Groups[SelectedGroupIndex] : default;

    object IGroupPanel.SelectedItem => SelectedItem;
    [Hide]
    public T SelectedItem => SelectedIndex >= 0 && SelectedIndex < SelectedGroup?.Count ? SelectedGroup[SelectedIndex].Value : default;

    [Hide]
    public GroupItem<T> SelectedItemContainer => SelectedIndex >= 0 && SelectedIndex < SelectedGroup?.Count ? SelectedGroup[SelectedIndex] : default;

    [Description("The selected group.")]
    [Pin(Pin.AboveOrLeft), Header, HideName, Index(0), Modify, Name("Selected group"), Placeholder("Select a group"), Show]
    [Int32Style(Int32Style.Index, nameof(Groups), nameof(GroupCollection<T>.Name))]
    public int SelectedGroupIndex { get => Get(-1); set => Set(value); }

    [Hide]
    public override IList SortNames => new Collections.ObjectModel.StringCollection() { nameof(GroupItem<T>.Created), nameof(GroupItem<T>.Name) };

    [Hide]
    public sealed override string Title => SelectedGroup?.Count > 0 ? TitleKey.Translate() + $" ({SelectedGroup.Count})" : TitleKey.Translate();

    [Hide]
    public virtual string TitleKey => base.Title;

    [Hide]
    public sealed override bool TitleLocalized => false;

    #endregion

    #region GroupPanel

    public GroupPanel() : base() { }

    public GroupPanel(IGroupWriter groups) : this()
    {
        Groups = (GroupWriter<T>)groups;
    }

    #endregion

    #region Methods

    void OnSelectedGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Update(() => Title);
        OnSelectedGroupChanged(e);
    }

    ///

    protected abstract IEnumerable<GroupCollection<T>> GetDefaultGroups();

    protected virtual T GetDefaultItem() => default;

    protected virtual IEnumerable<Type> GetItemTypes()
    {
        yield return typeof(T);
    }

    protected virtual Dictionary<Type, Func<T>> ItemHandlers { get; }

    protected virtual void OnSelectedGroupChanged(NotifyCollectionChangedEventArgs e) { }

    ///

    public override void OnModified(ModifiedEventArgs e)
    {
        base.OnModified(e);
        switch (e.PropertyName)
        {
            case nameof(SelectedGroupIndex):
                e.OldValue.If<int>(i => i >= 0 && i < Groups.Count, i => Groups[i].CollectionChanged -= OnSelectedGroupChanged);
                e.NewValue.If<int>(i => i >= 0 && i < Groups.Count, i => Groups[i].CollectionChanged += OnSelectedGroupChanged);
                break;
        }
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Groups):
                Groups.If(i => i.Count == 0, i => GetDefaultGroups().ForEach(j => i.Add(j)));
                //SelectedGroupIndex = SelectedGroupIndex == -1 ? 0 : SelectedGroupIndex;
                break;

            case nameof(SelectedGroup):
                Update(() => Title);
                break;

            case nameof(SelectedGroupIndex):
                Update(() => SelectedGroup);
                break;

            case nameof(SelectedIndex):
                Update(() => SelectedItem);
                Update(() => SelectedItemContainer);
                break;
        }
    }

    public void SetGroups(IGroupWriter input) => Groups = input as GroupWriter<T>;

    #endregion

    #region Commands

    ///Items

    bool AnySelected => Try.Return(() => SelectedItems?.Length > 0 == true);

    ICommand addCommand;
    [Category(nameof(Category.AddRemove)), Image(SmallImages.Plus), Index(0), Header, Name("Add"), Show, VisibilityTrigger(nameof(ItemTypeCount), 1)]
    public ICommand AddCommand => addCommand ??= new RelayCommand<Type>(i =>
    {
        T value = i is Type type ? (ItemHandlers?.ContainsKey(type) == true ? ItemHandlers[type]() : type.Create<T>()) : GetDefaultItem();

        var oldItem = new GroupItem<T>(DefaultItemName, value);
        var newItem = new GroupValueForm<T>(Groups, oldItem, SelectedGroupIndex);

        Dialog.ShowObject($"New {ItemName}", newItem, Resource.GetImageUri(SmallImages.Plus), i =>
        {
            if (i == 0)
            {
                newItem.GroupIndex = newItem.GroupIndex == -1 ? SelectedGroupIndex : newItem.GroupIndex;
                Groups[newItem.GroupIndex].Add((GroupItem<T>)newItem.Value);
            }
        },
        Buttons.SaveCancel);
    },
    i => SelectedGroup != null);

    ICommand cloneCommand;
    [Category(nameof(Category.Clipboard)), Image(SmallImages.Clone), Index(2), Header, Name("Clone"), Show]
    public ICommand CloneCommand => cloneCommand ??= new RelayCommand(() =>
    {
        SelectedItems.Select<GroupItem<T>>(i => (GroupItem<T>)i).ForEach(i =>
        {
            var j = i.DeepClone(new CloneHandler());
            j.Name += DefaultCloneSuffix;

            j.If(i => SelectedGroup.Add(j));
        });
    },
    () => SelectedGroup != null && AnySelected && !typeof(T).HasAttribute<NonCloneableAttribute>());

    ICommand copyCommand;
    [Category(nameof(Category.Clipboard)), Header, Image(SmallImages.Copy), Index(1), Name("Copy"), Show]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
    {
        var result = XList.Select(SelectedItems, i => i.As<GroupItem<T>>().DeepClone(new CloneHandler()));
        Copy.Set(result);
    },
    () => SelectedGroup != null && AnySelected && !typeof(T).HasAttribute<NonCloneableAttribute>());

    ICommand cutCommand;
    [Category(nameof(Category.Clipboard)), Header, Image(SmallImages.Cut), Index(0), Name("Cut"), Show]
    public ICommand CutCommand => cutCommand ??= new RelayCommand(() =>
    {
        var result = XList.Select(SelectedItems, i => i.As<GroupItem<T>>().DeepClone(new CloneHandler()));
        Copy.Set(result);

        RemoveCommand.Execute();
    },
    () => SelectedGroup != null && AnySelected && !typeof(T).HasAttribute<NonCloneableAttribute>());

    ICommand editCommand;
    [Hide]
    public ICommand EditCommand => editCommand ??= new RelayCommand<object>(i =>
    {
        IsEditing = true;
        return;

        var newItem = new GroupValueForm<T>(Groups, i ?? SelectedGroup[SelectedIndex], SelectedGroupIndex);
        Dialog.ShowObject($"Edit {ItemName.ToLower()}", newItem, Resource.GetImageUri(SmallImages.Pencil), i => 
        {
            if (newItem.GroupIndex != SelectedGroupIndex)
            {
                SelectedGroup.RemoveAt(SelectedIndex);
                Groups[newItem.GroupIndex].Add((GroupItem<T>)newItem.Value);
            }
        },
        Buttons.Done);
    },
    i => i != null || (SelectedGroup != null && SelectedIndex >= 0 && SelectedIndex < SelectedGroup.Count));

    ICommand moveCommand;
    [Category(nameof(Category.Move)), Header, Image(SmallImages.FolderMove), Name("Move"), Show]
    public ICommand MoveCommand => moveCommand ??= new RelayCommand(() => 
    {
        var result = new GroupIndexForm(Groups, SelectedGroupIndex);
        Dialog.ShowObject($"Move {ItemName.ToLower()}", result, Resource.GetImageUri(SmallImages.Pencil), i =>
        {
            if (result.GroupIndex != SelectedGroupIndex)
            {
                for (var j = SelectedGroup.Count - 1; j >= 0; j--)
                {
                    var item = SelectedGroup[j];
                    if (item.IsSelected)
                    {
                        SelectedGroup.RemoveAt(j);
                        Groups[result.GroupIndex].Add(item);
                    }
                }
            }
        },
        Buttons.Done);
    },
    () => SelectedGroup != null && AnySelected && !typeof(T).HasAttribute<NonCloneableAttribute>());

    ICommand pasteCommand;
    [Category(nameof(Category.Clipboard)), Header, Index(3), Image(SmallImages.Paste), Name("Paste"), Show]
    public ICommand PasteCommand => pasteCommand ??= new RelayCommand(() =>
    {
        if (Copy.Contains<GroupItem<T>>())
            SelectedGroup.Add(Copy.Get<GroupItem<T>>().DeepClone(new CloneHandler()));

        else if (Copy.Contains<IEnumerable<GroupItem<T>>>())
            Copy.Get<IEnumerable<GroupItem<T>>>().ForEach(i => SelectedGroup.Add(i.DeepClone(new CloneHandler())));
    },
    () => SelectedGroup != null && (Copy.Contains<GroupItem<T>>() || Copy.Contains<IEnumerable<GroupItem<T>>>()));

    ICommand removeCommand;
    [Category(nameof(Category.AddRemove)), Header, Image(SmallImages.Minus), Index(1), Name("Remove"), Show]
    public ICommand RemoveCommand => removeCommand ??= new RelayCommand(() =>
    {
        var itemName = Converter.Get<PluralConverter>().ConvertTo(ItemName.ToLower());
        Dialog.ShowWarning($"Remove {itemName}", new Warning($"Are you sure you want to remove the selected {itemName}?"), i =>
        {
            if (i == 0)
            {
                for (var j = SelectedGroup.Count - 1; j >= 0; j--)
                {
                    if (SelectedGroup[j].IsSelected)
                        SelectedGroup.RemoveAt(j);
                }
            }
        },
        Buttons.YesNo);
    },
    () => SelectedGroup != null && AnySelected);

    ///Groups

    ICommand addGroupCommand;
    [HeaderOption, Image(SmallImages.FolderAdd), Index(2), Name("Add group"), Show]
    public ICommand AddGroupCommand => addGroupCommand ??= new RelayCommand(() =>
    {
        var result = new NameForm(DefaultGroupName);
        Dialog.ShowObject("Add group", result, Resource.GetImageUri(SmallImages.FolderAdd), i => 
        {
            if (i == 0)
                Groups.Add(new GroupCollection<T>(result.Name));
        }, 
        Buttons.SaveCancel);
    },
    () => Groups != null);

    ICommand editGroupCommand;
    [HeaderOption, Image(SmallImages.FolderEdit), Index(1), Name("Edit group"), Show]
    public ICommand EditGroupCommand => editGroupCommand ??= new RelayCommand(() =>
    {
        var result = new NameForm(SelectedGroup.Name);
        Dialog.ShowObject("Edit group", result, Resource.GetImageUri(SmallImages.FolderEdit), i =>
        {
            if (i == 0)
                SelectedGroup.Name = result.Name;
        },
        Buttons.Done);
    }, 
    () => SelectedGroup != null);

    ICommand removeGroupCommand;
    [HeaderOption, Image(SmallImages.FolderDelete), Index(3), Name("Delete group"), Show]
    public ICommand RemoveGroupCommand => removeGroupCommand ??= new RelayCommand(() =>
    {
        Dialog.ShowWarning("Remove group", new Warning("Are you sure you want to remove the selected group?"), i =>
        {
            if (i == 0)
                Groups.Remove(SelectedGroup);
        },
        Buttons.YesNo);
    },
    () => Groups?.Contains(SelectedGroup) == true);

    ICommand resetGroupsCommand;
    [HeaderOption, Image(SmallImages.Reset), Index(4), Name("Reset groups"), Show]
    public ICommand ResetGroupsCommand => resetGroupsCommand ??= new RelayCommand(() =>
    {
        Dialog.ShowWarning("Reset groups", new Warning("Are you sure you want to reset all groups?"), i => 
        {
            if (i == 0)
            {
                Groups.Clear();
                GetDefaultGroups().ForEach(i => Groups.Add(i));
            }
        }, 
        Buttons.YesNo);
    });

    ///Other

    ICommand exportCommand;
    [Category(nameof(Category.Export)), HeaderOption, Image(SmallImages.Export), Name("Export"), Show]
    public ICommand ExportCommand
        => exportCommand ??= new RelayCommand(() => _ = Groups.Export(SelectedGroup), () => SelectedGroup != null);

    ICommand exportAllCommand;
    [Category(nameof(Category.Export)), HeaderOption, Image(SmallImages.ExportAll), Name("ExportAll"), Show]
    public ICommand ExportAllCommand
        => exportAllCommand ??= new RelayCommand(() => _ = Groups.Export());

    ICommand importCommand;
    [Category(nameof(Category.Import)), HeaderOption, Image(SmallImages.Import), Name("Import"), Show]
    public ICommand ImportCommand
        => importCommand ??= new RelayCommand(() => Groups.Import());

    #endregion
}

#endregion