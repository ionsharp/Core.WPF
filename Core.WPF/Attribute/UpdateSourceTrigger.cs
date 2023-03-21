using System;
using System.Windows.Data;

namespace Imagin.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UpdateSourceTriggerAttribute : Attribute
    {
        public readonly UpdateSourceTrigger Value;

        public UpdateSourceTriggerAttribute(UpdateSourceTrigger input) : base() => Value = input;
    }
}