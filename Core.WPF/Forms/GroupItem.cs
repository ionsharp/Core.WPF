using Imagin.Core.Collections.Serialization;
using System;
using System.Collections;

namespace Imagin.Core.Models;

[Serializable, ViewSource(ShowHeader = false)]
public class GroupItemForm : Base
{
    public IList Groups { get => Get<IList>(null, false); set => Set(value, false); }

    public IList SelectedGroup => Groups != null && SelectedGroupIndex >= 0 && SelectedGroupIndex < Groups.Count ? (IList)Groups[SelectedGroupIndex] : null;

    public int SelectedGroupIndex { get => Get(0); set => Set(value); }

    public int SelectedIndex { get => Get(0); set => Set(value); }

    [HideName]
    public object SelectedItem => SelectedGroup != null && SelectedIndex >= 0 && SelectedIndex < SelectedGroup.Count ? SelectedGroup[SelectedIndex] : null;

    GroupItemForm() : base() { }

    public GroupItemForm(IGroupWriter groups, int selectedGroupIndex, int selectedIndex) : base()
    {
        Groups = groups; SelectedGroupIndex = selectedGroupIndex; SelectedIndex = selectedIndex;
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Groups):
            case nameof(SelectedGroupIndex):
                Update(() => SelectedGroup); 
                break;

            case nameof(SelectedIndex):
                Update(() => SelectedItem);
                break;
        }
    }
}