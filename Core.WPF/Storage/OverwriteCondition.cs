using System;

namespace Imagin.Core.Storage
{
    [Serializable]
    public enum OverwriteCondition
    {
        IfNewer,
        IfSizeDifferent,
        IfNewerOrSizeDifferent,
        Always
    }
}