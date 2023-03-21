using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Imagin.Core.Storage;

public abstract class Item : Namable
{
    enum Tab { Attributes, Properties }

    #region Properties

    static readonly Dictionary<string, PropertyChangedEventArgs> EventArgCache;

    [Hide]
    public override string Name { get => base.Name; set => base.Name = value; }

    [Tab(Tab.Properties), Name("Created"), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public virtual DateTime Created { get => Get<DateTime>(); set => Set(value); }

    [Hide]
    public bool IsChanged { get => Get(false); private set => Set(value); }

    bool isHidden = false;
    [Tab(Tab.Attributes), Name("Hidden")]
    public virtual bool IsHidden
    {
        get => isHidden;
        set
        {
            Try.Invoke(() =>
            {
                if (value)
                    File.Long.AddAttribute(Path, FileAttributes.Hidden);

                else File.Long.RemoveAttribute(Path, FileAttributes.Hidden);
                isHidden = value;
            },
            e => Analytics.Log.Write<Item>(e));
            Update(() => IsHidden);
        }
    }

    bool isReadOnly = false;
    [Tab(Tab.Attributes), Name("ReadOnly")]
    public virtual bool IsReadOnly
    {
        get => isReadOnly;
        set
        {
            Try.Invoke(() =>
            {
                if (value)
                    File.Long.AddAttribute(Path, FileAttributes.ReadOnly);

                else File.Long.RemoveAttribute(Path, FileAttributes.ReadOnly);
                isReadOnly = value;
            },
            e => Analytics.Log.Write<Item>(e));
            Update(() => IsReadOnly);
        }
    }

    [Hide]
    public virtual bool IsSelected { get => Get(false); set => Set(value); }

    [Tab(Tab.Properties), Name("Accessed"), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public virtual DateTime LastAccessed { get => Get<DateTime>(); set => Set(value); }

    [Tab(Tab.Properties), Name("Modified"), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly]
    public virtual DateTime LastModified { get => Get<DateTime>(); set => Set(value); }

    [Pin(Pin.AboveOrLeft), Name("Path"), ReadOnly]
    public virtual string Path { get => Get(""); set => Set(value); }

    [Hide]
    public int Permissions { get => Get(0); set => Set(value); }

    [Tab(Tab.Properties), Name("Size"), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long Size { get => Get(0L); set => Set(value); }

    [Hide]
    public ItemType Type { get => Get<ItemType>(); protected set => Set(value); }

    #endregion

    #region Item

    Item() : base() { }

    protected Item(ItemType type, Origin origin, string path) : this()
    {
        Type = type;
        Path = path;
    }

    static Item()
    {
        EventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
    }

    #endregion

    #region Methods

    void From(FileSystemInfo i)
    {
        if (i == null)
            return;

        LastAccessed
            = i.LastAccessTime;
        Created
            = i.CreationTime;

        ///

        isHidden
            = this is Drive || Path == StoragePath.Root ? false : i.Attributes.HasFlag(FileAttributes.Hidden);
        Update(() => IsHidden);

        isReadOnly
            = i.Attributes.HasFlag(FileAttributes.ReadOnly);
        Update(() => IsReadOnly);

        ///

        LastModified
            = i.LastWriteTime;
        Name
            = System.IO.Path.GetFileName(i.FullName);
            
        if (i is FileInfo)
        {
            Size = i.To<FileInfo>().Length;
        }
        else if (i is DirectoryInfo) { }
    }

    ///

    public abstract FileSystemInfo Read();

    ///

    public async Task RefreshAsync() => await RefreshAsync(Path);

    public async Task RefreshAsync(string Path) => await Task.Run(() => Refresh(Path));

    ///

    public void Refresh() => From(Read());

    public void Refresh(string path)
    {
        Path = path;
        Refresh();
    }

    #endregion

    #region BaseLong

    public class BaseLong
    {
        protected const int FILE_ATTRIBUTE_ARCHIVE = 0x20;
        protected const int INVALID_FILE_ATTRIBUTES = -1;

        protected const int FILE_READ_DATA = 0x0001;
        protected const int FILE_WRITE_DATA = 0x0002;
        protected const int FILE_APPEND_DATA = 0x0004;
        protected const int FILE_READ_EA = 0x0008;
        protected const int FILE_WRITE_EA = 0x0010;

        protected const int FILE_READ_ATTRIBUTES = 0x0080;
        protected const int FILE_WRITE_ATTRIBUTES = 0x0100;

        protected const int FILE_SHARE_NONE = 0x00000000;
        protected const int FILE_SHARE_READ = 0x00000001;

        protected const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

        protected const long FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE;

        protected const long FILE_GENERIC_READ = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE;

        protected const long READ_CONTROL = 0x00020000L;
        protected const long STANDARD_RIGHTS_READ = READ_CONTROL;
        protected const long STANDARD_RIGHTS_WRITE = READ_CONTROL;

        protected const long SYNCHRONIZE = 0x00100000L;

        protected const int CREATE_NEW = 1;
        protected const int CREATE_ALWAYS = 2;
        protected const int OPEN_EXISTING = 3;

        protected const int MAX_PATH = 260;
        protected const int MAX_ALTERNATE = 14;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        protected struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
            public uint nFileSizeLow;  //|
            public uint dwReserved0;   //|
            public uint dwReserved1;   //v
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool CopyFileW(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern int GetFileAttributesW(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool DeleteFileW(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool MoveFileW(string lpExistingFileName, string lpNewFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool GetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool RemoveDirectory(string path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern int SetFileAttributesW(string lpFileName, int fileAttributes);
    }

    #endregion
}