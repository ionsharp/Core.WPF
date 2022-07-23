using Imagin.Core.Collections.Serialization;
using Imagin.Core.Reflection;
using System.Windows;

namespace Imagin.Core.Models;

public class GroupIndexModel : Base
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

    public GroupIndexModel(IGroupWriter groups, int selectedIndex) : base()
    {
        Groups = groups; GroupIndex = selectedIndex;
    }
}