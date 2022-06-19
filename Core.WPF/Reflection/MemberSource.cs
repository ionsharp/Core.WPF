using System;
using System.Collections.Generic;

namespace Imagin.Core.Reflection;

public class MemberSource
{
    public Types DataType => Type.IsValueType ? Types.Value : Types.Reference;

    public object Instance { get; internal set; }

    public readonly Type Type;

    //...

    public MemberSource(MemberPathElement input)
    {
        Instance = input.Value; Type = Instance.GetType();
    }

    //...

    Type GetSharedType(IEnumerable<Type> types)
    {
        if (types == null)
            return null;

        Type a = null;
        foreach (var i in types)
        {
            if (a == null)
            {
                a = i;
                continue;
            }

            var b = i;

            //Compare a and b
            while (a != typeof(object))
            {
                while (b != typeof(object))
                {
                    if (a == b)
                        goto next;

                    b = b.BaseType;
                }
                a = a.BaseType;
                b = i;
            }

            return null;
            next: continue;
        }
        return a;
    }
}