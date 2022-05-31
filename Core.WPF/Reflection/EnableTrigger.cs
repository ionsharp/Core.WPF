using System;

namespace Imagin.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnableTriggerAttribute : Attribute
    {
        public readonly string Property;

        public readonly object Value;

        public EnableTriggerAttribute(string property, object value) : base()
        {
            Property = property;
            Value = value;
        }
    }
}