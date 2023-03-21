using IWshRuntimeLibrary;
using Shell32;
using System;

namespace Imagin.Core.Storage;

[Name("Shortcut"), Serializable]
public sealed class Shortcut : File
{
    public ItemCollection Items { get => Get(new ItemCollection()); private set => Set(value); }

    public Shortcut(string Path) : base(Path)
    {
        Type = ItemType.Shortcut;
    }

    ///

    public static void Create(string name, string description, string targetPath, string folderPath)
    {
        WshShell WshShell = new();
        IWshShortcut Shortcut = WshShell.CreateShortcut(folderPath + @"\" + name + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
        Shortcut.Arguments = "";
        Shortcut.TargetPath = targetPath;
        Shortcut.WindowStyle = 1;
        Shortcut.Description = description;
        Shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(targetPath);
        Shortcut.Save();
    }

    ///

    public static string TargetPath(string path)
    {
        var parent = System.IO.Path.GetDirectoryName(path);
        var name = System.IO.Path.GetFileName(path);

        var shell = new Shell();
        var folder = shell.NameSpace(parent);
        var folderItem = folder.ParseName(name);

        var result = string.Empty;
        Try.Invoke(() => result = folderItem != null ? ((ShellLinkObject)folderItem.GetLink).Path : result);
        return result;
    }

    ///

    public static bool Is(string path) => System.IO.Path.GetExtension(path).TrimStart('.').ToLower() == "lnk";

    ///

    public static bool TargetsFile(string path)
    {
        if (File.Long.Exists(TargetPath(path)))
        {
            return true;
        }
        return false;
    }

    public static bool TargetsFolder(string path)
    {
        if (Folder.Long.Exists(TargetPath(path)))
        {
            return true;
        }
        return false;
    }
}