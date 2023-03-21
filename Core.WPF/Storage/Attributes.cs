using System;

namespace Imagin.Core.Storage;

[Serializable]
public enum Attributes
{
    [Hide]
    None = 0,
    Hidden = 1,
    ReadOnly = 2,
    [Hide]
    All = Hidden | ReadOnly
}