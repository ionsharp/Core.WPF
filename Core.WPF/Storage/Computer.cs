using Imagin.Core.Analytics;
using Imagin.Core.Imports;
using Imagin.Core.Linq;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Storage;

public class Computer
{
    #region Properties

    public static IEnumerable<DriveInfo> Drives
    {
        get
        {
            foreach (var i in DriveInfo.GetDrives())
            {
                if (i.IsReady)
                {
                    yield return i;
                }
            }
        }
    }

    public static IEnumerable<string> RemovableDrives
    {
        get
        {
            var drives = DriveInfo.GetDrives();
            if (drives?.Length > 0)
            {
                foreach (var i in drives)
                {
                    if (i.DriveType == DriveType.Removable)
                        yield return i.Name;
                }
            }
        }
    }

    #endregion

    #region Methods

    public static string FriendlyDescription(string path)
    {
        var result = "";
        if (path.EndsWith(@":\"))
        {
            foreach (var i in Drives)
            {
                if (i.Name.ToLower() == path.ToLower())
                {
                    result = $"{new FileSize(i.AvailableFreeSpace).ToString(FileSizeFormat.BinaryUsingSI)} free of {new FileSize(i.TotalSize).ToString(FileSizeFormat.BinaryUsingSI)}";
                    break;
                }
            }
        }
        else
        {
            result = Folder.Long.Exists(path) 
                ? "Folder" 
                : !path.NullOrEmpty() 
                ? File.Long.Description(path) 
                : path;
        }

        return result.NullOrEmpty() ? Path.GetFileNameWithoutExtension(path) : result;
    }

    public static string FriendlyName(string path)
    {
        if (path == null)
            return string.Empty;

        if (path == StoragePath.Root)
            return StoragePath.RootName;

        if (path.EndsWith(@":\"))
        {
            var volumeLabel = Drives.FirstOrDefault(i => i.Name == path)?.VolumeLabel;
            return volumeLabel != null ? $"{volumeLabel} ({path.TrimEnd('\\')})" : path;
        }

        var result = Path.GetFileName(path);
        return result.NullOrEmpty() ? path : result;
    }

    ///

    public static Item GetItem(string path, bool refresh = false)
    {
        Item result = default;
        switch (GetType(path))
        {
            case ItemType.Drive:
                result = new Drive(path);
                break;
            case ItemType.File:
                result = new File(path);
                break;
            case ItemType.Folder:
                result = new Folder(path);
                break;
            case ItemType.Shortcut:
                result = new Shortcut(path);
                break;
        }
        if (refresh)
            result.Refresh();

        return result;
    }

    public static ItemType GetType(string path)
    {
        if (path == StoragePath.Root)
            return ItemType.Root;

        if (path?.EndsWith(@":") == true || path?.EndsWith(@":\") == true)
            return ItemType.Drive;

        if (Folder.Long.Exists(path))
            return ItemType.Folder;

        if (File.Long.Exists(path))
        {
            if (Shortcut.Is(path))
                return ItemType.Shortcut;

            return ItemType.File;
        }
        return ItemType.Nothing;
    }

    ///

    public static bool Hidden(string itemPath)
    {
        foreach (var i in Drives)
        {
            if (i.Name == itemPath)
                return false;
        }
        if (File.Long.Exists(itemPath))
        {
            var file = new File(itemPath);
            file.Refresh();
            return file.IsHidden;
        }
        else if (Folder.Long.Exists(itemPath))
        {
            var folder = new Folder(itemPath);
            folder.Refresh();
            return folder.IsHidden;
        }
        return false;
    }

    public static bool Visible(string itemPath) => !Hidden(itemPath);

    public static bool ReadOnly(string itemPath)
    {
        foreach (var i in Drives)
        {
            if (i.Name == itemPath)
                return false;
        }
        if (File.Long.Exists(itemPath))
        {
            var file = new File(itemPath);
            file.Refresh();
            return file.IsReadOnly;
        }
        else if (Folder.Long.Exists(itemPath))
        {
            var folder = new Folder(itemPath);
            folder.Refresh();
            return folder.IsReadOnly;
        }
        return false;
    }

    ///

    /// <summary>
    /// Copies the given items to the given target path.
    /// </summary>
    /// <param name="items">The items to copy.</param>
    /// <param name="targetPath">The target path (does not include the name of the copied item).</param>
    public static void Copy(IEnumerable<Item> items, string targetPath)
    {
        foreach (var i in items)
        {
            var destination = i.Path.Replace(Path.GetDirectoryName(i.Path), targetPath);
            if (i.Path == destination)
            {
                destination = $@"{Path.GetDirectoryName(destination)}\{Path.GetFileNameWithoutExtension(destination)} (Copy){Path.GetExtension(destination)}";
            }

            try
            {
                switch (i.Type)
                {
                    case ItemType.Drive:
                        continue;

                    case ItemType.File:
                    case ItemType.Shortcut:
                        Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(i.Path, destination, UIOption.AllDialogs);
                        continue;

                    case ItemType.Folder:
                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(i.Path, destination, UIOption.AllDialogs);
                        continue;
                }
            }
            catch (Exception e)
            {
                Log.Write<Computer>(new Error(e));
            }
        }
    }

    public static void Copy(string i, string targetPath)
    {
        var destination = i.Replace(Path.GetDirectoryName(i), targetPath);
        if (i == destination)
        {
            if (File.Long.Exists(i))
                destination = File.Long.ClonePath(i);

            if (Folder.Long.Exists(i))
                destination = Folder.Long.ClonePath(i);
        }

        if (File.Long.Exists(i))
            Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(i, destination, UIOption.AllDialogs);

        if (Folder.Long.Exists(i))
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(i, destination, UIOption.AllDialogs);
    }

    ///

    /// <summary>
    /// Gets the final path of the given item if moved to the given folder with the (optional) new name.
    /// </summary>
    /// <param name="i">The item to move.</param>
    /// <param name="targetFolderPath">The folder where the item is moved to.</param>
    /// <param name="targetName">What to rename the item to (optional).</param>
    /// <returns></returns>
    static string MovedTargetPath(Item i, string targetFolderPath, string targetName)
    {
        if (i is Drive)
            throw new NotSupportedException();

        string result = null;

        //Various scenarios are addressed differently for files and folders. Further analysis may be necessary to identify potential others.
            
        if (i is File)
        {
            //Both paths must exist
            if (!File.Long.Exists(i.Path))
                throw new FileNotFoundException(i.Path);

            if (!Folder.Long.Exists(targetFolderPath))
                throw new DirectoryNotFoundException(targetFolderPath);

            //The folder path must end with \ when comparing
            var a = i.Path;
            var b = targetFolderPath.EndsWith(@"\") ? targetFolderPath : $@"{targetFolderPath}\";

            //Both paths must be lower when comparing
            a = a.ToLower();
            b = b.ToLower();

            /*
            1.  Not okay!
                C:\Folder a\Folder b\File x.png
                C:\Folder a\Folder b\
                
                Result:
                C:\Folder a\Folder b\File x.png
            */

            var c = Path.GetDirectoryName(a).ToLower();
            c = c.EndsWith(@"\") ? c : $@"{c}\";

            //If the parent path of <a> (c) equals path <b>
            if (b == c)
            {
                //If the file isn't getting renamed
                if (targetName.NullOrEmpty() || targetName.ToLower() == Path.GetFileName(a).ToLower())
                    throw new InvalidOperationException();
            }

            /*
            1.  Okay!
                C:\Folder a\Folder b\File x.png
                D:\
                
                Result:
                D:\File x.png

            2.  Okay!
                C:\Folder a\Folder b\File x.png
                C:\Folder a\
                
                Result:
                C:\Folder a\File x.png
                
            3.  Okay!
                C:\Folder a\Folder b\File x.png
                C:\Folder a\Folder b\Folder c\
                
                Result:
                C:\Folder a\Folder b\Folder c\File x.png
            */

            //Preserve original casing by using input variables
            result = $@"{targetFolderPath.TrimEnd('\\')}\{(targetName.NullOrEmpty() ? Path.GetFileName(i.Path) : targetName)}";

            //If a file with that path already exists (to do: Consider overwriting it)
            if (File.Long.Exists(result))
                throw new InvalidOperationException();
        }

        else if (i is Folder)
        {
            //Both paths must exist
            if (!Folder.Long.Exists(i.Path))
                throw new DirectoryNotFoundException(i.Path);

            if (!Folder.Long.Exists(targetFolderPath))
                throw new DirectoryNotFoundException(targetFolderPath);

            //Both paths must end with \ when comparing
            var a = i.Path.EndsWith(@"\") ? i.Path : $@"{i.Path}\";
            var b = targetFolderPath.EndsWith(@"\") ? targetFolderPath : $@"{targetFolderPath}\";

            //Both paths must be lower when comparing
            a = a.ToLower();
            b = b.ToLower();

            /*
            Path <b> cannot start with path <a>

            1.  Not okay!
                C:\Folder a\Folder b\Folder x\
                C:\Folder a\Folder b\Folder x\
                                
                Result:
                C:\Folder a\Folder b\Folder x\Folder x\

            2.  Not okay!
                C:\Folder a\Folder b\Folder x\
                C:\Folder a\Folder b\Folder x\Folder c\
                
                Result:
                C:\Folder a\Folder b\Folder x\Folder c\Folder x\
            */

            if (b.StartsWith(a))
                throw new InvalidOperationException();

            /*

            Path <b> cannot equal parent path of <a> (unless renaming!)

            3.  Not okay!
                C:\Folder a\Folder b\Folder x
                C:\Folder a\Folder b

                Result:
                C:\Folder a\Folder b\Folder x
            */

            var c = Path.GetDirectoryName(a).ToLower();
            c = c.EndsWith(@"\") ? c : $@"{c}\";

            //If the parent path of <a> equals path <b>
            if (b == c)
            {
                //If the folder isn't getting renamed
                if (targetName.NullOrEmpty() || targetName.ToLower() == Path.GetFileName(a).ToLower())
                    throw new InvalidOperationException();
            }

            /*
            1.  Okay!
                C:\Folder a\Folder b\Folder x\
                C:\
                    
                Result:
                C:\Folder x\

            2.  Okay!
                C:\Folder a\Folder b\Folder x\
                C:\Folder a\
                    
                Result:
                C:\Folder a\Folder x\

            3.  Okay!
                C:\Folder a\Folder b\Folder x\
                D:\Folder z\

                Result:
                D:\Folder z\Folder x\

            If none of the "Not okay" scenarios apply, an "Okay" scenario is assumed to...
            */

            //Preserve original casing by using input variables
            result = $@"{targetFolderPath.TrimEnd('\\')}\{(targetName.NullOrEmpty() ? Path.GetFileName(i.Path) : targetName)}";

            //If a folder with that path already exists (to do: Consider merging contents)
            if (Folder.Long.Exists(result))
                throw new InvalidOperationException();
        }

        return result;
    }

    ///

    /// <summary>
    /// Moves the given item to the given folder with the (optional) new name.
    /// </summary>
    /// <param name="i">The item to move.</param>
    /// <param name="targetFolderPath">TThe folder where the item is moved to.</param>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public static Result Move(Item i, string targetFolderPath, string targetName)
    {
        try
        {
            var destination = MovedTargetPath(i, targetFolderPath, targetName);

            switch (i.Type)
            {
                case ItemType.Drive:
                    return new Error(new InvalidOperationException());

                case ItemType.File:
                case ItemType.Shortcut:
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveFile(i.Path, destination, UIOption.AllDialogs);
                    break;

                case ItemType.Folder:
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(i.Path, destination, UIOption.AllDialogs);
                    break;
            }
            return new Success();
        }
        catch (Exception e)
        {
            Log.Write<Computer>(new Error(e));
            return new Error(e);
        }
    }

    /// <summary>
    /// Moves the given items to the given folder with the (optional) new name.
    /// </summary>
    /// <param name="items">The items to move.</param>
    /// <param name="targetFolderPath">The folder where the item is moved to.</param>
    public static void Move(IEnumerable<Item> items, string targetFolderPath)
    {
        foreach (var i in items)
            Move(i, targetFolderPath, null);
    }

    ///

    public static void Recycle(IEnumerable<string> paths, RecycleOption recycleOption = RecycleOption.SendToRecycleBin)
    {
        foreach (var i in paths)
        {
            Recycle(i, recycleOption);
        }
    }

    public static void Recycle(string path, RecycleOption recycleOption = RecycleOption.SendToRecycleBin)
    {
        try
        {
            if (System.IO.Directory.Exists(path))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(path, UIOption.AllDialogs, recycleOption);
            }

            else if (System.IO.File.Exists(path))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, UIOption.AllDialogs, recycleOption);
            }
        }
        catch { }
    }

    ///

    public static Result OpenInWindowsExplorer(string path)
        => File.Long.Open("explorer.exe", path);

    public static Result ShowInWindowsExplorer(string path)
        => File.Long.Open("explorer.exe", @"/select, {0}".F(path));

    #endregion
}