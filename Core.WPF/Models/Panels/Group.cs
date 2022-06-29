using Imagin.Core.Collections.ObjectModel;
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
    enum Category { Export, Import }

    #region (class) InternalItem

    internal class InternalItem : Base<T>
    {
        int groupIndex;
        [Feature]
        [DisplayName("Group"), SelectedIndex]
        [Setter(nameof(MemberModel.ItemPath), nameof(GroupCollection<T>.Name))]
        [Trigger(nameof(MemberModel.ItemSource), nameof(Groups))]
        public int GroupIndex
        {
            get => groupIndex;
            set => this.Change(ref groupIndex, value);
        }

        [Hidden]
        public GroupWriter<T> Groups { get; private set; }

        public InternalItem(GroupWriter<T> groups, T value) : base(value) => Groups = groups;
    }

    #endregion

    #region Properties

    string defaultGroupName = "Untitled group";
    [Option, Visible]
    public string DefaultGroupName
    {
        get => defaultGroupName;
        set => this.Change(ref defaultGroupName, value);
    }

    [Hidden]
    public GroupCollection<T> SelectedGroup => SelectedGroupIndex >= 0 && SelectedGroupIndex < Groups?.Count ? Groups[SelectedGroupIndex] : default;

    object IGroupPanel.SelectedItem => SelectedItem;
    [Hidden]
    public T SelectedItem => SelectedIndex >= 0 && SelectedIndex < SelectedGroup?.Count ? SelectedGroup[SelectedIndex] : default;

    int selectedGroupIndex = -1;
    [Feature, Index(2), SelectedIndex]
    [Label(false)]
    [Setter(nameof(MemberModel.ItemPath), nameof(GroupCollection<T>.Name))]
    [Trigger(nameof(MemberModel.ItemSource), nameof(Groups))]
    [Tool, Visible]
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

    GroupWriter<T> groups = null;
    [Hidden]
    public GroupWriter<T> Groups
    {
        get => groups;
        private set => this.Change(ref groups, value);
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
    [DisplayName("Add")]
    [Image(Images.Plus)]
    [Index(0)]
    [Tool, Visible]
    public ICommand AddCommand => addCommand ??= new RelayCommand(() =>
    {
        var oldItem = GetNewItem();
        MemberWindow.ShowDialog("New item", oldItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
            SelectedGroup.As<GroupCollection<T>>().Add(oldItem);
    },
    () => SelectedGroup != null);

    ICommand addToCommand;
    [Hidden]
    public ICommand AddToCommand => addToCommand ??= new RelayCommand(() =>
    {
        var oldItem = GetNewItem();
        var newItem = new InternalItem(Groups, oldItem);

        MemberWindow.ShowDialog("New item", newItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
        {
            if (newItem.GroupIndex == -1)
            {

            }
            else Groups[newItem.GroupIndex].Add(oldItem);
        }
    });

    ICommand cloneCommand;
    [DisplayName("Clone")]
    [Image(Images.Clone)]
    [Index(2)]
    [Tool, Visible]
    public ICommand CloneCommand => cloneCommand ??= new RelayCommand(() => SelectedGroup.Insert(SelectedGroup.IndexOf(SelectedItem), (T)SelectedItem.As<ICloneable>().Clone()), () => SelectedGroup != null && SelectedItem is ICloneable);

    ICommand deleteCommand;
    [DisplayName("Delete")]
    [Image(Images.Trash)]
    [Index(3)]
    [Tool, Visible]
    public ICommand DeleteCommand => deleteCommand ??= new RelayCommand(() => SelectedGroup.Remove(SelectedItem), () => SelectedGroup?.Contains(SelectedItem) == true);

    ICommand editCommand;
    [DisplayName("Edit")]
    [Index(1)]
    [Image(Images.Pencil)]
    [Tool, Visible]
    public ICommand EditCommand => editCommand ??= new RelayCommand(() =>
    {
        MemberWindow.ShowDialog("Edit item", SelectedItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.Done);
    }, 
    () => SelectedItem != null);

    ICommand addGroupCommand;
    [DisplayName("Add group")]
    [Feature, Index(0)]
    [Image(Images.FolderAdd)]
    [Tool, Visible]
    public ICommand AddGroupCommand => addGroupCommand ??= new RelayCommand(() =>
    {
        var group = MemberWindow.ShowDialog("New group", new GroupCollection<T>(DefaultGroupName), out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
            Groups.Add(group);
    },
    () => Groups != null);

    ICommand editGroupCommand;
    [DisplayName("Edit group")]
    [Feature, Index(1)]
    [Image(Images.FolderEdit)]
    [Tool, Visible]
    public ICommand EditGroupCommand => editGroupCommand ??= new RelayCommand(() => MemberWindow.ShowDialog("Edit group", SelectedGroup, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.Done), () => Groups != null);

    ICommand deleteGroupCommand;
    [DisplayName("Delete group")]
    [Feature, Index(2)]
    [Image(Images.FolderDelete)]
    [Tool, Visible]
    public ICommand DeleteGroupCommand => deleteGroupCommand ??= new RelayCommand(() => Groups.Remove(SelectedGroup), () => Groups?.Contains(SelectedGroup) == true);

    ICommand exportCommand;
    [Category(nameof(Category.Export))]
    [DisplayName("Export")]
    [Image(Images.Export)]
    [Tool, Visible]
    public ICommand ExportCommand
        => exportCommand ??= new RelayCommand(() => _ = Groups.Export(SelectedGroup), () => SelectedGroup != null);

    ICommand exportAllCommand;
    [Category(nameof(Category.Export))]
    [DisplayName("ExportAll")]
    [Image(Images.ExportAll)]
    [Tool, Visible]
    public ICommand ExportAllCommand
        => exportAllCommand ??= new RelayCommand(() => _ = Groups.Export());

    ICommand importCommand;
    [Category(nameof(Category.Import))]
    [DisplayName("Import")]
    [Image(Images.Import)]
    [Tool, Visible]
    public ICommand ImportCommand
        => importCommand ??= new RelayCommand(() => Groups.Import());

    #endregion
}