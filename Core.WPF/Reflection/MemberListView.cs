using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;

namespace Imagin.Core.Controls;

public class MemberListView<T>
{
    public ObservableCollection<T> Source { get; private set; } = new();

    public ListCollectionView View { get; private set; }

    public MemberListView() : this(null) { }

    public MemberListView(IComparer input) : base()
    {
        View = new(Source) { CustomSort = input };
    }
}

public class MemberListView : MemberListView<MemberModel>
{
    public MemberListView(IComparer input) : base(input) { }
}

public class MemberTabView
{
    public MemberListView<string> Tabs { get; private set; } = new();

    public MemberListView Members { get; private set; }

    public MemberTabView(IComparer input) : base() => Members = new(input);

    public void UpdateTabs(IList<MemberModel> input)
    {
        Tabs.Source.Clear();

        var result = new List<string>();
        foreach (var i in input)
        {
            if (i.Tab != null)
            {
                if (!result.Contains(i.Tab))
                    result.Add(i.Tab);
            }
        }
        result.Sort();

        result.ForEach(i => Tabs.Source.Add(i));
    }

    public void UpdateMembers(IList<MemberModel> input, int index)
    {
        Members.Source.Clear();
        if (index >= 0 && index < Tabs.Source.Count)
        {
            foreach (var i in input)
            {
                if (i.Tab == Tabs.Source[index])
                    Members.Source.Add(i);
            }
        }
    }
}