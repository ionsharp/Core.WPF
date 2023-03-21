using Imagin.Core.Analytics;
using Imagin.Core.Collections.Generic;
using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Imagin.Core.Reflection;

public static class XMemberDictionary
{
    /// <summary>If the input is a collection, this stores the items.</summary>
    public const string CollectionKey = "this";

    public static readonly Type[] IgnoreNonSerializableTypes = new[]
    {
        //System
        typeof(bool),
        typeof(decimal), typeof(double),
        typeof(short), typeof(int), typeof(long),
        typeof(ushort), typeof(uint), typeof(ulong),
        typeof(DateTime), typeof(TimeSpan), typeof(Type),
        //System.Reflection
        typeof(Assembly)
    };

    ///

    /// <remarks><b>Recursive</b></remarks>
    public static T LoadMembers<T>(this T input, ObjectDictionary result, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public, MemberTypes types = MemberTypes.Field | MemberTypes.Property)
    {
        if (input == null) return default;

        var type = input.GetType();
        foreach (var i in type.GetMembers(flags, types, null, true, true))
        {
            if (result.ContainsKey(i.Name))
            {
                var value = result[i.Name];
                if (value is MemberDictionary.InternalDictionary j)
                {
                    Try.Invoke(() => value = j.Type.Create<object>(), e => Log.Write<object>(e));
                    value?.LoadMembers(j, flags, types);
                }

                Try.Invoke(() => input.SetMemberValue(i, value), e => Log.Write<object>(e));
            }
        }

        if (input is IList a)
        {
            if (result.ContainsKey(CollectionKey) && result[CollectionKey] is IList b)
            {
                b.ForEach(i =>
                {
                    if (i is MemberDictionary.InternalDictionary j)
                    {
                        Try.Invoke(() => i = j.Type.Create<object>(), e => Log.Write<object>(e));
                        i?.LoadMembers(j, flags, types);
                    }

                    if (i != null)
                        a.Add(i);
                });
            }
        }
        return input;
    }

    /// <remarks><b>Recursive</b></remarks>
    static void SaveMembers<T>(this T input, ObjectDictionary result, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public, MemberTypes types = MemberTypes.Field | MemberTypes.Property)
    {
        if (input == null) return;

        var type = input.GetType();
        foreach (var i in type.GetMembers(flags, types, null, true, true))
        {
            object value = null;
            Try.Invoke(() => value = input.GetMemberValue(i), e => Log.Write<object>(e));

            var memberType = value?.GetType();
            if (value != null && !IgnoreNonSerializableTypes.Contains(memberType) && !value.HasAttribute<SerializableAttribute>())
            {
                Log.Write<object>(new NotSerializableWarning(value));

                var j = new MemberDictionary.InternalDictionary(memberType);
                value.SaveMembers(j, flags, types);
                value = j;
            }

            result.Add(i.Name, value);
        }

        if (input is IList list)
        {
            var items = new List<object>();
            result.Add(CollectionKey, items);

            list.ForEach(i =>
            {
                object value = i;
                if (value != null && !IgnoreNonSerializableTypes.Contains(value.GetType()) && !value.HasAttribute<SerializableAttribute>())
                {
                    Log.Write<object>(new NotSerializableWarning(value));

                    var j = new MemberDictionary.InternalDictionary(value.GetType());
                    value.SaveMembers(j);
                    value = j;
                }

                if (value != null)
                    items.Add(value);
            });
        }
    }

    public static ObjectDictionary SaveMembers<T>(this T input, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public, MemberTypes types = MemberTypes.Field | MemberTypes.Property)
    {
        var result = new ObjectDictionary();
        input.SaveMembers(result, flags, types); //This throws
        return result;
    }
}

[Serializable]
public class MemberDictionary : ObjectDictionary
{
    [Serializable]
    internal class InternalDictionary : ObjectDictionary 
    { 
        public Type Type; 
        
        public InternalDictionary() : base() { } 
        
        public InternalDictionary(Type type) : this() => Type = type; 
    }

    public MemberDictionary() : base() { }

    public void Load(object input) => input.LoadMembers(this);

    public void Save(object input)
    {
        Clear();
        input.SaveMembers().ForEach(i => Add(i.Key, i.Value));
    }
}