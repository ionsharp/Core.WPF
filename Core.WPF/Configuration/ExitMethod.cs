using System;

namespace Imagin.Core.Configuration
{
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
}