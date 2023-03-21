using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Imagin.Core.Reflection;

internal abstract class MemberAttribute 
{
    public abstract Type Type { get; }

    public abstract void Execute(MemberModel input, IEnumerable<Attribute> attributes);
}

internal class MemberAttribute<T> : MemberAttribute where T : Attribute
{
    public readonly Action<MemberModel, IEnumerable<T>> Action;

    public override Type Type => typeof(T);

    public MemberAttribute(Action<MemberModel, IEnumerable<T>> action) => Action = action;

    public override void Execute(MemberModel input, IEnumerable<Attribute> attributes) => Action(input, attributes.Cast<T>());
}

///

internal class MemberAttributeHandler : List<MemberAttribute> { }

///

public sealed class MemberAttributes : Dictionary<Type, List<Attribute>>
{
    public readonly MemberInfo Member;

    ///

    public MemberAttributes(MemberInfo member) : base()
    {
        Member = member;
        foreach (Attribute i in member.GetCustomAttributes(true))
        {
            var type = i.GetType();
            if (!ContainsKey(type))
                Add(type, new());

            this[type].Add(i);
        }
    }

    ///

    public IEnumerable<T> GetAll<T>() where T : Attribute => GetAll(typeof(T))?.Cast<T>();

    public IEnumerable<Attribute> GetAll(Type type) => this.ContainsKey(type) ? this[type] : default;

    ///

    public T GetFirst<T>() where T : Attribute => GetAll<T>()?.FirstOrDefault();

    public Attribute GetFirst(Type type) => GetAll(type)?.FirstOrDefault();
}