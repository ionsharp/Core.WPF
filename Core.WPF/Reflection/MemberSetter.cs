using System;

namespace Imagin.Core.Reflection;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MemberSetterAttribute : Attribute
{
    public readonly string PropertyName;

    public readonly object Value;

    public MemberSetterAttribute(string propertyName, object value) : base()
    {
        PropertyName = propertyName; Value = value;
    }
}