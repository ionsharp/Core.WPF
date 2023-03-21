using Imagin.Core.Imports;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Imagin.Core.Storage;

public static class RecycleBin
{
    static Shell Shell;

    public enum RecycleFlags : int
    {
        /// <summary>Don't ask for confirmation</summary>
        SHRB_NOCONFIRMATION = 0x00000001,
        /// <summary>Don't show progress</summary>
        SHRB_NOPROGRESSUI = 0x00000001,
        /// <summary>Don't make sound when the action is executed</summary>
        SHRB_NOSOUND = 0x00000004
    }

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

    static bool DoVerb(FolderItem Item, string Verb)
    {
        foreach (FolderItemVerb FIVerb in Item.Verbs())
        {
            if (FIVerb.Name.ToUpper().Contains(Verb.ToUpper()))
            {
                FIVerb.DoIt();
                return true;
            }
        }
        return false;
    }

    static bool Recycle(string path, FileOperationFlags flags)
    {
        try
        {
            var fs = new SHFILEOPSTRUCT
            {
                wFunc = FileOperationType.FO_DELETE,
                pFrom = path + '\0' + '\0',
                fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
            };
            SHFileOperation(ref fs);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void Empty() => SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHRB_NOCONFIRMATION);

    public static IEnumerable<string> GetContents()
    {
        Shell = new Shell();
        Shell32.Folder RecycleBin = Shell.NameSpace(10);
        foreach (FolderItem2 Entry in RecycleBin.Items())
            yield return Entry.Path;
        Marshal.FinalReleaseComObject(Shell);
    }

    public static long GetSize()
    {
        long size = 0;
        foreach (var i in GetContents())
            size += Computer.GetItem(i, true).Size;

        return size;
    }

    /// <summary>
    /// Restore item at given path.
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public static bool Restore(string path)
    {
        Shell = new Shell();
        var Recycler = Shell.NameSpace(10);
        for (int i = 0; i < Recycler.Items().Count; i++)
        {
            var FolderItem = Recycler.Items().Item(i);
            string FileName = Recycler.GetDetailsOf(FolderItem, 0);

            if (Path.GetExtension(FileName) == "")
                FileName += Path.GetExtension(FolderItem.Path);

            //Necessary for systems with hidden file extensions.
            string FilePath = Recycler.GetDetailsOf(FolderItem, 1);
            if (path == Path.Combine(FilePath, FileName))
            {
                DoVerb(FolderItem, "ESTORE");
                return true;
            }
        }
        return false;
    }
}