using Imagin.Core.Collections.Serialization;

namespace Imagin.Core.Models;

[Categorize(false), ViewSource(ShowHeader = false)]
public class GroupIndexForm : Base
{
    [Name("Group")]
    [Int32Style(Int32Style.Index, nameof(Groups), "Name")]
    public int GroupIndex { get => Get(0); set => Set(value); }

    [Hide]
    public IGroupWriter Groups { get; private set; }

    public GroupIndexForm(IGroupWriter groups, int selectedIndex) : base()
    {
        Groups = groups; GroupIndex = selectedIndex;
    }
}