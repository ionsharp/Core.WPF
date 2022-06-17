using System;
using System.Windows.Data;

namespace Imagin.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UpdateSourceTriggerAttribute : Attribute
    {
        public readonly UpdateSourceTrigger UpdateSourceTrigger;

        public UpdateSourceTriggerAttribute(UpdateSourceTrigger input) : base() => UpdateSourceTrigger = input;
    }
}