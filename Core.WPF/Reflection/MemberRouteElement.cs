using System;

namespace Imagin.Core.Reflection;

public abstract class MemberRouteElement
{
    public virtual string Name { get; }

    public virtual object Value { get; }

    public MemberRouteElement() { }
}

public class MemberRouteChild : MemberRouteElement
{
    public override string Name => Member.Name;

    public override object Value => Member.Value;

    public MemberModel Member { get; private set; }

    public MemberRouteChild(MemberModel member) : base() => Member = member;
}

public class MemberRouteItem : MemberRouteChild
{
    //public override string Name => $"{(Member as ListItemModel).OtherParent.Name}[]";

    public MemberRouteItem(MemberModel member) : base(member) { }
}

public class MemberRouteSource : MemberRouteElement
{
    public Type Type => Value?.GetType();

    readonly object value;
    public override object Value => value;

    public MemberRouteSource(object i) : base() => value = i;
}