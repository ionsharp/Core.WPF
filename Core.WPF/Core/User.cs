using System.Runtime.InteropServices;

namespace Imagin.Core;

public static class User
{
    public static string Image => GetImage(Name);

    public static string Name => System.Security.Principal.WindowsIdentity.GetCurrent().Name;

    [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern void GetUserTilePath(string username, uint whatever, /*0x80000000*/ System.Text.StringBuilder picpath, int maxLength);

    public static string GetImage(string userName)
    {
        var sb = new System.Text.StringBuilder(1000);
        GetUserTilePath(userName, 0x80000000, sb, sb.Capacity);
        return sb.ToString();
    }
}