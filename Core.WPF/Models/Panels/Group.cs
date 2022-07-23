using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Data;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Models;

public interface IGroupPanel { object SelectedItem { get; } }

public abstract class GroupPanel<T> : Panel, IGroupPanel where T : new()
{
    enum Category { Add, Clipboard, Edit, Export, Import, View }

    #region Properties

    string defaultGroupName = "Untitled group";
    [Option, Visible]
    public string DefaultGroupName
    {
        get => defaultGroupName;
        set => this.Change(ref defaultGroupName, value);
    }

    GroupWriter<T> groups = null;
    [Hidden]
    public GroupWriter<T> Groups
    {
        get => groups;
        private set => this.Change(ref groups, value);
    }

    public virtual string ItemName => "item";

    double itemSize = 128.0;
    [Category(nameof(Category.View)), Option, Range(32.0, 512.0, 4.0), Slider, Visible]
    public virtual double ItemSize
    {
        get => itemSize;
        set => this.Change(ref itemSize, value);
    }

    [Hidden]
    public GroupCollection<T> SelectedGroup => SelectedGroupIndex >= 0 && SelectedGroupIndex < Groups?.Count ? Groups[SelectedGroupIndex] : default;

    object IGroupPanel.SelectedItem => SelectedItem;
    [Hidden]
    public T SelectedItem => SelectedIndex >= 0 && SelectedIndex < SelectedGroup?.Count ? SelectedGroup[SelectedIndex] : default;

    int selectedGroupIndex = -1;
    [Above, Index(2), Label(false), SelectedIndex, Setter(nameof(MemberModel.ItemPath), nameof(GroupCollection<T>.Name)), Trigger(nameof(MemberModel.ItemSource), nameof(Groups)), Tool, Visible]
    public int SelectedGroupIndex
    {
        get => selectedGroupIndex;
        set
        {
            this.Change(ref selectedGroupIndex, value);
            this.Changed(() => SelectedGroup);
        }
    }

    int selectedIndex = -1;
    [Hidden]
    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            this.Change(ref selectedIndex, value);
            this.Changed(() => SelectedItem);
        }
    }

    GroupView view = GroupView.Grid;
    [Category(nameof(Category.View)), Option, Visible]
    public GroupView View
    {
        get => view;
        set => this.Change(ref view, value);
    }

    #endregion

    #region GroupPanel

    GroupPanel() : base() { }

    public GroupPanel(IGroupWriter groups) : this() => Groups = (GroupWriter<T>)groups;

    #endregion

    #region Methods

    protected virtual T GetNewItem() => new T();

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Groups))
            SelectedGroupIndex = SelectedGroupIndex == -1 ? 0 : SelectedGroupIndex;
    }
        
    public void Update(IGroupWriter input) => Groups = input as GroupWriter<T>;

    #endregion

    #region Commands

    ICommand addCommand;
    [Category(nameof(Category.Add)), DisplayName("Add")]
    [Image(Images.Plus)]
    [Index(0), Tool, Visible]
    public ICommand AddCommand => addCommand ??= new RelayCommand(() =>
    {
        var oldItem = GetNewItem();
        var newItem = new GroupValueModel(Groups, oldItem, SelectedGroupIndex);

        MemberWindow.ShowDialog($"New {ItemName}", newItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
        {
            if (newItem.GroupIndex == -1)
            {

            }
            else Groups[newItem.GroupIndex].Add((T)newItem.Value);
        }
    });

    ICommand addGroupCommand;
    [DisplayName("Add group")]
    [Above, Index(0)]
    [Image(Images.FolderAdd), Tool, Visible]
    public ICommand AddGroupCommand => addGroupCommand ??= new RelayCommand(() =>
    {
        var group = MemberWindow.ShowDialog("New group", new GroupCollection<T>(DefaultGroupName), out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
            Groups.Add(group);
    },
    () => Groups != null);

    ICommand cloneCommand;
    [Category(nameof(Category.Edit)), DisplayName("Clone")]
    [Image(Images.Clone)]
    [Index(2), Tool, Visible]
    public ICommand CloneCommand => cloneCommand ??= new RelayCommand(() => SelectedGroup.Insert(SelectedGroup.IndexOf(SelectedItem), (T)SelectedItem.As<ICloneable>().Clone()), () => SelectedGroup != null && SelectedItem is ICloneable);

    ICommand copyCommand;
    [Category(nameof(Category.Clipboard)), DisplayName("Copy"), Index(1), Image(Images.Copy), Tool, Visible]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand<T>(i => XClipboard.Copy(i.SmartClone<object>()), i => i != null);

    ICommand cutCommand;
    [Category(nameof(Category.Clipboard)), DisplayName("Cut"), Index(0), Image(Images.Cut), Tool, Visible]
    public ICommand CutCommand => cutCommand ??= new RelayCommand<T>(i =>
    {
        CopyCommand.Execute(i);
        SelectedGroup.RemoveAt(SelectedIndex);
    }, 
    i => i != null);

    ICommand deleteCommand;
    [Category(nameof(Category.Edit)), DisplayName("Delete")]
    [Image(Images.Trash)]
    [Index(3), Tool, Visible]
    public ICommand DeleteCommand => deleteCommand ??= new RelayCommand(() =>
    {
        if (Dialog.Show($"Delete {ItemName}", "Are you sure?", DialogImage.Warning, Buttons.YesNo) == 0)
            SelectedGroup.RemoveAt(SelectedIndex);
    },
    () => SelectedGroup != null && SelectedIndex >= 0 && SelectedIndex < SelectedGroup.Count);

    ICommand deleteGroupCommand;
    [DisplayName("Delete group")]
    [Above, Index(2)]
    [Image(Images.FolderDelete), Tool, Visible]
    public ICommand DeleteGroupCommand => deleteGroupCommand ??= new RelayCommand(() =>
    {
        if (Dialog.Show("Delete group", "Are you sure?", DialogImage.Warning, Buttons.YesNo) == 0)
            Groups.Remove(SelectedGroup);
    },
    () => Groups?.Contains(SelectedGroup) == true);

    ICommand editCommand;
    [Category(nameof(Category.Edit)), DisplayName("Edit")]
    [Index(1)]
    [Image(Images.Pencil), Tool, Visible]
    public ICommand EditCommand => editCommand ??= new RelayCommand<object>(i =>
    {
        var newItem = new GroupValueModel(Groups, i ?? SelectedGroup[SelectedIndex], SelectedGroupIndex);
        MemberWindow.ShowDialog($"Edit {ItemName}", newItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.Done);
        if (newItem.GroupIndex != SelectedGroupIndex)
        {
            SelectedGroup.RemoveAt(SelectedIndex);
            Groups[newItem.GroupIndex].Add((T)newItem.Value);
        }
        else
        {
            Groups[SelectedGroupIndex][SelectedIndex] = (T)newItem.Value;
        }
    }, 
    i => i != null || (SelectedGroup != null && SelectedIndex >= 0 && SelectedIndex < SelectedGroup.Count));

    ICommand editGroupCommand;
    [DisplayName("Edit group")]
    [Above, Index(1)]
    [Image(Images.FolderEdit), Tool, Visible]
    public ICommand EditGroupCommand => editGroupCommand ??= new RelayCommand(() => MemberWindow.ShowDialog("Edit group", SelectedGroup, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.Done), () => Groups != null);

    ICommand exportCommand;
    [Category(nameof(Category.Export))]
    [DisplayName("Export")]
    [Image(Images.Export), Tool, Visible]
    public ICommand ExportCommand
        => exportCommand ??= new RelayCommand(() => _ = Groups.Export(SelectedGroup), () => SelectedGroup != null);

    ICommand exportAllCommand;
    [Category(nameof(Category.Export))]
    [DisplayName("ExportAll")]
    [Image(Images.ExportAll), Tool, Visible]
    public ICommand ExportAllCommand
        => exportAllCommand ??= new RelayCommand(() => _ = Groups.Export());

    ICommand importCommand;
    [Category(nameof(Category.Import))]
    [DisplayName("Import")]
    [Image(Images.Import), Tool, Visible]
    public ICommand ImportCommand
        => importCommand ??= new RelayCommand(() => Groups.Import());

    ICommand pasteCommand;
    [Category(nameof(Category.Clipboard)), DisplayName("Paste"), Index(2), Image(Images.Paste), Tool, Visible]
    public ICommand PasteCommand => pasteCommand ??= new RelayCommand<T>(i => SelectedGroup.Add((T)XClipboard.Paste(typeof(T))), i => i != null && SelectedGroup != null && XClipboard.Contains(typeof(T)));

    #endregion
}