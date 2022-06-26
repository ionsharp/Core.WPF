using System;

namespace Imagin.Core.Reflection;

public abstract class MemberPathElement
{
    public virtual string Name { get; }

    public virtual object Value { get; }

    public MemberPathElement() { }
}

public class MemberPathChild : MemberPathElement
{
    public override string Name => Member.Name;

    public override object Value => Member.Value;

    public MemberModel Member { get; private set; }

    public MemberPathChild(MemberModel member) : base() => Member = member;
}

public class MemberPathItem : MemberPathChild
{
    //public override string Name => $"{(Member as ListItemModel).OtherParent.Name}[]";

    public MemberPathItem(MemberModel member) : base(member) { }
}

public class MemberPathSource : MemberPathElement
{
    public Type Type => Value?.GetType();

    readonly object value;
    public override object Value => value;

    public MemberPathSource(object i) : base() => value = i;
}