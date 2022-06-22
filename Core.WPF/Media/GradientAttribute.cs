using System;

namespace Imagin.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GradientAttribute : Attribute
    {
        public readonly string[] Colors;

        public GradientAttribute(params string[] colors) : base() => Colors = colors;
    }
}