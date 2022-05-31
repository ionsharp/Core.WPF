using System;

namespace Imagin.Core.Controls
{
    [Flags]
    [Serializable]
    public enum MemberFilter
    {
        [Hidden]
        None = 0,
        Entry = 1,
        Field = 2,
        Property = 4,
        [Hidden]
        All = Entry | Field | Property
    }
}