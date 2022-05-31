using System;

namespace Imagin.Core
{
    public class InvalidAncestor<Target> : Exception
    {
        public InvalidAncestor() : base($"'{typeof(Target).FullName}' must be an ancestor.") { }
    }
}