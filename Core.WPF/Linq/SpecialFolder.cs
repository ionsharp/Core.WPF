using System;

namespace Imagin.Core.Linq;

public static class XSpecialFolder
{
    public static string GetPath(this Environment.SpecialFolder Value) => Environment.GetFolderPath(Value);
}