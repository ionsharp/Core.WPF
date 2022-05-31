using System;

namespace Imagin.Core.Linq
{
    public static class XSpecialFolder
    {
        public static string Path(this Environment.SpecialFolder Value) => Environment.GetFolderPath(Value);
    }
}