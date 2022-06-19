using Imagin.Core.Collections.Generic;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;

namespace Imagin.Core.Reflection;

public class MemberPath : ObservableCollection<MemberPathElement>
{
    internal MemberPath() : base() { }

    internal MemberPathElement New(object oldValue, object newValue)
    {
        if (newValue == null || ReferenceEquals(oldValue, newValue))
            return null;

        if (newValue is ListObjectModel a)
            newValue = new MemberPathItem(a);

        else if (newValue is MemberModel b)
            newValue = new MemberPathChild(b);

        if (newValue is MemberPathElement c)
        {
            if (c is MemberPathChild d)
            {
                //This should theoretically never occur!
                if (c.Value == oldValue)
                    return null;

                return d;
            }
            //Safely assume this already exists and start from scratch
            else if (c is MemberPathSource e)
                return e;

            throw new NotSupportedException();
        }
        //Safely assume we are starting from scratch
        else return new MemberPathSource(newValue);
    }

    internal void Append(MemberPathElement input)
    {
        if (input == null || input is MemberPathSource)
            Clear();

        var append = new List<MemberPathElement>();
        if (input is MemberPathChild child)
        {
            //If it already exists in the route, we must navigate to it!
            if (Contains(input))
            {
                var index = IndexOf(input);
                //Remove every child (including this one)
                for (var i = Count - 1; i >= index; i--)
                    RemoveAt(i);

                //The child gets added back at the end
            }
            //If it doesn't yet exist, we need to add it, but first, check if anything should come in between!
            //This happens when something deep in the hierarchy is edited directly. This only applies because part of
            //the hierarchy already exists in the root. Doing this allows all of the hierarchy to be edited from the route
            //regardless of what part is directly edited.
            //
            //Normal hierarchy looks like this:
            //(root) MemberCollection -> MemberModel -> (child) MemberCollection -> MemberModel -> (child) MemberCollection ...
            //
            //Hierarchy with a list member looks like this:
            //(root) MemberCollection -> MemberModel -> (child) MemberCollection -> ListMemberModel -> ListItemModel -> (child) MemberCollection ...
            //
            //The goal is to enumerate the hierarchy backwards and stop when we reach the root.
            //
            //This needs tested...
            else
            {
                goto skip;
                MemberModel parentMember = child.Member;
                while (parentMember != null)
                {
                    if (parentMember is ListObjectModel listItem)
                    {
                        parentMember = listItem.Parent;
                    }
                    else if (child.Member.Parent.Parent == null)
                    {
                        break;
                    }
                    else
                    {
                        parentMember = child.Member.Parent.Parent;
                    }

                    if (!this.Contains<MemberPathChild>(i => ReferenceEquals(i.Member, parentMember)))
                    {
                        MemberPathChild result = null;
                        if (parentMember is ListObjectModel)
                            result = new MemberPathItem(parentMember);

                        else result = new MemberPathChild(parentMember);
                        append.Insert(0, result);
                    }
                    else break;
                }
            }
        }
        skip: append.Add(input);
        append.ForEach(i => Add(i));
    }
}