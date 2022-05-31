using System;

namespace Imagin.Core.Storage
{
    [Serializable]
    public enum Attributes
    {
        [Hidden]
        None = 0,
        Hidden = 1,
        ReadOnly = 2,
        [Hidden]
        All = Hidden | ReadOnly
    }
}