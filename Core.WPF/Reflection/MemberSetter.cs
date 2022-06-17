using System;

namespace Imagin.Core.Reflection;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MemberSetterAttribute : Attribute
{
    public readonly string PropertyName;

    public readonly string Value;

    public MemberSetterAttribute(string propertyName, string value) : base()
    {
        PropertyName = propertyName; Value = value;
    }
}