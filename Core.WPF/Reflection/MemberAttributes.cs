using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Imagin.Core.Reflection;

public class MemberAttributeHandler : Dictionary<Type, Action<MemberModel, Attribute, IEnumerable<Attribute>>> { }

public sealed class MemberAttributes : Dictionary<Type, List<Attribute>>
{
    public bool Hidden => GetFirst<HiddenAttribute>()?.Hidden == true || GetFirst<System.ComponentModel.BrowsableAttribute>()?.Browsable == false || GetFirst<VisibleAttribute>()?.Visible == false;

    public MemberAttributes(MemberInfo member) : base()
    {
        if (member != null)
        {
            foreach (Attribute i in member.GetCustomAttributes(true))
            {
                var type = i.GetType();
                if (!ContainsKey(type))
                    Add(type, new());

                this[type].Add(i);
            }
        }
    }

    public IEnumerable<T> GetAll<T>() where T : Attribute => GetAll(typeof(T))?.Cast<T>();

    public IEnumerable<Attribute> GetAll(Type type) => this.ContainsKey(type) ? this[type] : default;

    public T GetFirst<T>() where T : Attribute => GetAll<T>()?.FirstOrDefault();

    public void Apply(MemberModel input, MemberAttributeHandler handler)
    {
        foreach (var i in handler)
        {
            var j = GetAll(i.Key);
            if (j?.Count() > 0)
                i.Value(input, j.FirstOrDefault(), j);
        }
    }
}