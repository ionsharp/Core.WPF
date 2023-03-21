using System;

namespace Imagin.Core.Config;

[Serializable]
public enum ExitMethod
{
    None,
    Exit,
    Hibernate,
    Lock,
    LogOff,
    Restart,
    Shutdown,
    Sleep
}