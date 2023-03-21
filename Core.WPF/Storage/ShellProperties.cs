using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Imagin.Core.Storage;

public static class ShellProperties
{
    [DllImport("shell32.dll", SetLastError = true)]
    static extern int SHMultiFileProperties(IDataObject pdtobj, int flags);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr ILCreateFromPath(string path);

    [DllImport("shell32.dll", CharSet = CharSet.None)]
    static extern void ILFree(IntPtr pidl);

    [DllImport("shell32.dll", CharSet = CharSet.None)]
    static extern int ILGetSize(IntPtr pidl);

    static MemoryStream GetShellIds(StringCollection paths)
    {
        //Convert list of paths into a list of PIDLs.
        var pos = 0;
        var pidls = new byte[paths.Count][];
        foreach (var filename in paths)
        {
            //Get PIDL based on name
            var pidl = ILCreateFromPath(filename);
            var pidlSize = ILGetSize(pidl);
            //Copy over to our managed array
            pidls[pos] = new byte[pidlSize];
            Marshal.Copy(pidl, pidls[pos++], 0, pidlSize);
            ILFree(pidl);
        }

        //Determine where in CIDL we will start pumping PIDLs
        var pidlOffset = 4 * (paths.Count + 2);
        //Start the CIDL stream
        var result = new MemoryStream();

        var writer = new BinaryWriter(result);
        writer.Write(paths.Count); //Initialize CIDL witha count of files
        writer.Write(pidlOffset); //Calcualte and write relative offsets of every pidl starting with root

        pidlOffset += 4; //Root is 4 bytes
        foreach (var pidl in pidls)
        {
            writer.Write(pidlOffset);
            pidlOffset += pidl.Length;
        }

        //Write the root PIDL (0) followed by all PIDLs
        writer.Write(0);
        foreach (var pidl in pidls)
            writer.Write(pidl);

        //Stream now contains the CIDL
        return result;
    }

    public static int Show(string Path)
    {
        return Show(new string[] { Path });
    }

    public static int Show(IEnumerable<string> paths)
    {
        var files = new StringCollection();
        foreach (var i in paths)
            files.Add(i);

        var Data = new DataObject();
        Data.SetFileDropList(files);
        Data.SetData("Preferred DropEffect", new MemoryStream(XArray.New<byte>(5, 0, 0, 0)), true);
        Data.SetData("Shell IDList Array", GetShellIds(files), true);

        return SHMultiFileProperties(Data, 0);
    }
}