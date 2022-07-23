using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Reflection;

namespace Imagin.Core.Reflection;

public class ItemModel : MemberModel
{
    public override IList Collection => Parent?.Value as IList;

    public override bool IsWritable => !Parent.IsReadOnly;

    public override bool IsReadOnly => !IsWritable;

    public override MemberInfo Member => Parent.Member;

    public int CollectionIndex => Collection?.IndexOf(Value) ?? -1;
        
    public override Type Type => Parent?.ItemType;

    public ItemModel(MemberModel parent, object source) : base(parent, new(source), null, null, 0) { }

    protected override void SetValue(object source, object value) 
        => CollectionIndex.If(i => i > -1 && Collection?.Count > i, i => Collection[i] = value);

    public override object GetValue() => Source.Instance;

    protected override object GetValue(object input) => Source.Instance;
}