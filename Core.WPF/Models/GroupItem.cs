using Imagin.Core.Collections.Serialization;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Imagin.Core.Models;

[Serializable]
public class GroupItemModel : Base
{
    [NonSerialized]
    IList groups = null;
    public IList Groups
    {
        get => groups;
        set => this.Change(ref groups, value);
    }

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

    GroupItemModel() : base() { }

    public GroupItemModel(IGroupWriter groups, int selectedGroupIndex, int selectedIndex) : base()
    {
        Groups = groups; SelectedGroupIndex = selectedGroupIndex; SelectedIndex = selectedIndex;
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Groups):
            case nameof(SelectedGroupIndex):
                this.Changed(() => SelectedGroup); 
                break;

            case nameof(SelectedIndex):
                this.Changed(() => SelectedItem);
                break;
        }
    }
}