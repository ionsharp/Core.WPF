using Imagin.Core.Collections.Serialization;
using Imagin.Core.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imagin.Core.Models;

public class GroupItemModel : Base
{
    public IList Groups { get; private set; }

    public IList SelectedGroup => Groups != null && SelectedGroupIndex >= 0 && SelectedGroupIndex < Groups.Count ? (IList)Groups[SelectedGroupIndex] : null;

    int selectedGroupIndex;
    public int SelectedGroupIndex
    {
        get => selectedGroupIndex;
        set => this.Change(ref selectedGroupIndex, value);
    }

    int selectedIndex;
    public int SelectedIndex
    {
        get => selectedIndex;
        set => this.Change(ref selectedIndex, value);
    }

    public object SelectedItem => SelectedGroup != null && SelectedIndex >= 0 && SelectedIndex < SelectedGroup.Count ? SelectedGroup[SelectedIndex] : null;

    public GroupItemModel(IGroupWriter groups, int selectedGroupIndex, int selectedIndex) : base()
    {
        Groups = groups; SelectedGroupIndex = selectedGroupIndex; SelectedIndex = selectedIndex;
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(SelectedGroupIndex):
                this.Changed(() => SelectedGroup); 
                break;

            case nameof(SelectedIndex):
                this.Changed(() => SelectedItem);
                break;
        }
    }
}

internal class GroupItemWithNoValue : Base
{
    int groupIndex;
    [DisplayName("Group"), SelectedIndex, Setter(nameof(MemberModel.ItemPath), "Name"), Trigger(nameof(MemberModel.ItemSource), nameof(Groups))]
    public int GroupIndex
    {
        get => groupIndex;
        set => this.Change(ref groupIndex, value);
    }

    [Hidden]
    public IGroupWriter Groups { get; private set; }

    public GroupItemWithNoValue(IGroupWriter groups, int selectedIndex) : base()
    {
        Groups = groups; GroupIndex = selectedIndex;
    }
}

internal class GroupItemWithValue : Base<object>
{
    int groupIndex;
    [DisplayName("Group"), Feature, SelectedIndex, Setter(nameof(MemberModel.ItemPath), "Name"), Trigger(nameof(MemberModel.ItemSource), nameof(Groups))]
    public int GroupIndex
    {
        get => groupIndex;
        set => this.Change(ref groupIndex, value);
    }

    [Hidden]
    public IGroupWriter Groups { get; private set; }

    public GroupItemWithValue(IGroupWriter groups, object value, int selectedIndex) : base(value)
    {
        Groups = groups; GroupIndex = selectedIndex;
    }
}