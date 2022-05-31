using System;

namespace Imagin.Core.Controls
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StatusAttribute : Attribute
    {
        public StatusAttribute() { }
    }
}