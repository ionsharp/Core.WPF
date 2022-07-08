using Imagin.Core.Linq;
using System;
using System.Reflection;

namespace Imagin.Core.Reflection;
    
public class ListItemModel : MemberModel
{
    public override bool CanWrite => !ParentMember.IsReadOnly;

    public override bool IsReadOnly => !CanWrite;

    public override MemberInfo Member => ParentMember.Member;

    public int CollectionIndex => Collection?.IndexOf(Value) ?? -1;
        
    public override Type Type => ParentMember?.ItemType;

    public ListItemModel(MemberModel parent, MemberData data) : base(parent, data, 0) { }

    protected override void SetValue(object source, object value) 
        => CollectionIndex.If(i => i > -1 && ParentMember.Collection?.Count > i, i => ParentMember.Collection[i] = value);

    protected override object GetValue(object input)
    {
        object result = null;
        CollectionIndex.If(i => i > -1 && ParentMember.Collection?.Count > i, i => result = ParentMember.Collection[i]);
        return result;
    }
}