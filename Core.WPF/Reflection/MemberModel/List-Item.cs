using Imagin.Core.Linq;
using System;
using System.Reflection;

namespace Imagin.Core.Reflection;
    
public class ListItemModel : MemberModel
{
    public MemberModel ParentMember { get; private set; }

    public override bool CanWrite => !ParentMember.IsReadOnly;

    public override bool IsReadOnly => !CanWrite;

    public override MemberInfo Member => ParentMember.Member;

    public int CollectionIndex => ParentMember.Items.IndexOf(this);
        
    public override Type Type => ParentMember?.ItemType;

    public ListItemModel(MemberModel parent, MemberData data) : base(data, 0) => ParentMember = parent;

    protected override void SetValue(object source, object value) 
        => CollectionIndex.If(i => i > -1 && ParentMember.SourceCollection?.Count > i, i => ParentMember.SourceCollection[i] = value);

    protected override object GetValue(object input)
    {
        object result = null;
        CollectionIndex.If(i => i > -1 && ParentMember.SourceCollection?.Count > i, i => result = ParentMember.SourceCollection[i]);
        return result;
    }
}