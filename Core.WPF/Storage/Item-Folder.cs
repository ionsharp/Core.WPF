using Imagin.Core.Analytics;
using Imagin.Core.Conversion;
using Imagin.Core.Imports;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imagin.Core.Storage;

[Name("Folder"), Serializable, View(MemberView.Tab, typeof(Tab))]
public sealed class Folder : Container
{
    enum Tab { Contents }

    #region Properties

    [Tab(Tab.Contents), Range(0, int.MaxValue, 1), ReadOnly, StringFormat(NumberFormat.Default)]
    public int Characters { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents), Range(0, int.MaxValue, 1), ReadOnly, StringFormat(NumberFormat.Default)]
    public int Lines { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents), Range(0, int.MaxValue, 1), ReadOnly, StringFormat(NumberFormat.Default)]
    public int Words { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents)]
    [ReadOnly, StringFormat(NumberFormat.Default)]
    public int Files { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents)]
    [ReadOnly, StringFormat(NumberFormat.Default)]
    public int Folders { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents)]
    [ReadOnly, StringFormat(NumberFormat.Default)]
    public int HiddenFiles { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents)]
    [ReadOnly, StringFormat(NumberFormat.Default)]
    public int HiddenFolders { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(ShortTimeConverter)), ReadOnly]
    public DateTime? OldestFile { get => Get<DateTime?>(); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(ShortTimeConverter)), ReadOnly]
    public DateTime? OldestFolder { get => Get<DateTime?>(); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(ShortTimeConverter)), ReadOnly]
    public DateTime? NewestFile { get => Get<DateTime?>(); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(ShortTimeConverter)), ReadOnly]
    public DateTime? NewestFolder { get => Get<DateTime?>(); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long LargestFile { get => Get(0L); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long SmallestFile { get => Get(0L); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long LargestFolder { get => Get(0L); private set => Set(value); }

    [Tab(Tab.Contents), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly]
    public long SmallestFolder { get => Get(0L); private set => Set(value); }

    [Tab(Tab.Contents), ReadOnly, StringFormat(NumberFormat.Default)]
    public int ReadOnlyFiles { get => Get(0); private set => Set(value); }

    [Tab(Tab.Contents), ReadOnly, StringFormat(NumberFormat.Default)]
    public int ReadOnlyFolders { get => Get(0); private set => Set(value); }

    #endregion

    public Folder(string path) : base(ItemType.Folder, Origin.Local, path) => contents = new(RefreshContents, true);

    public class Long : BaseLong
    {
        #region Private

        const uint SHGFI_ICON = 0x100;

        const uint SHGFI_LARGEICON = 0x0;

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        static string Combine(string path1, string path2)
        {
            return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.');
        }

        static List<string> GetAllPathsFromPath(string path)
        {
            bool unc = false;
            var prefix = @"\\?\";
            if (path.StartsWith(prefix + @"UNC\"))
            {
                prefix += @"UNC\";
                unc = true;
            }
            var split = path.Split('\\');
            int i = unc ? 6 : 4;
            var list = new List<string>();
            var txt = "";

            for (int a = 0; a < i; a++)
            {
                if (a > 0) txt += "\\";
                txt += split[a];
            }
            for (; i < split.Length; i++)
            {
                txt = Combine(txt, split[i]);
                list.Add(txt);
            }

            return list;
        }

        static string GetCleanPath(string path)
        {
            if (path.StartsWith(@"\\?\UNC\")) return @"\\" + path.Substring(8);
            if (path.StartsWith(@"\\?\")) return path.Substring(4);
            return path;
        }

        static string GetWin32LongPath(string path)
        {

            if (path.StartsWith(@"\\?\")) return path;

            var newpath = path;
            if (newpath.StartsWith("\\"))
            {
                newpath = @"\\?\UNC\" + newpath.Substring(2);
            }
            else if (newpath.Contains(":"))
            {
                newpath = @"\\?\" + newpath;
            }
            else
            {
                var currdir = Environment.CurrentDirectory;
                newpath = Combine(currdir, newpath);
                while (newpath.Contains("\\.\\")) newpath = newpath.Replace("\\.\\", "\\");
                newpath = @"\\?\" + newpath;
            }
            return newpath.TrimEnd('.');
        }

        static void InternalGetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption, ref List<string> dirs)
        {
            IntPtr findHandle = default;

            try
            {
                findHandle = FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(path), searchPattern), out WIN32_FIND_DATA findData);
                if (findHandle != new IntPtr(-1))
                {
                    do
                    {
                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                string subdirectory = System.IO.Path.Combine(path, findData.cFileName);
                                dirs.Add(GetCleanPath(subdirectory));
                                if (searchOption == SearchOption.AllDirectories)
                                {
                                    InternalGetDirectories(subdirectory, searchPattern, searchOption, ref dirs);
                                }
                            }
                        }
                    } while (FindNextFile(findHandle, out findData));
                    FindClose(findHandle);
                }
                else
                {
                    ThrowWin32Exception();
                }
            }
            catch (Exception)
            {
                if (findHandle != null)
                    FindClose(findHandle);

                throw;
            }
        }

        static bool LongExists(string path)
        {
            var attr = GetFileAttributesW(path);
            return (attr != INVALID_FILE_ATTRIBUTES && ((attr & FILE_ATTRIBUTE_DIRECTORY) == FILE_ATTRIBUTE_DIRECTORY));
        }

        [DebuggerStepThrough]
        static void ThrowWin32Exception()
        {
            int code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new System.ComponentModel.Win32Exception(code);
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Gets the path with actual casing by querying each parent folder in the path. Performance is poor due to multiple queries!
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ActualPath(string path)
        {
            IEnumerable<string> folders;

            var last = path;
            var current = System.IO.Path.GetDirectoryName(path);

            var result = string.Empty;
            while (true)
            {
                folders = null;
                try
                {
                    folders = Directory.EnumerateDirectories(current);
                }
                catch { }

                //This will always happen once we reach top-most folder (the associated drive)!
                if (folders == null || folders.Empty())
                    break;

                foreach (var i in folders)
                {
                    if (i.ToLower().Equals(last.ToLower()))
                    {
                        var name = System.IO.Path.GetFileName(i);
                        result = result.Empty() ? name : $@"{name}\{result}";
                        last = current;
                        break;
                    }
                }
                current = System.IO.Path.GetDirectoryName(current);
            }

            var driveName = string.Empty;
            foreach (var i in Computer.Drives)
            {
                if (path.ToLower().StartsWith(i.Name.ToLower()))
                    driveName = i.Name;
            }
            return $@"{driveName}{result}";
        }

        ///

        /// <summary>
        /// Gets a new path based on the given path.
        /// </summary>
        /// <param name="path">The path to evaluate.</param>
        /// <param name="nameFormat">How to format the name (not including extension).</param>
        /// <returns>A new path based on the old path.</returns>
        public static string ClonePath(string folderPath, string nameFormat = StoragePath.DefaultCloneFormat) => StoragePath.Clone(folderPath, nameFormat, i => Exists(i));

        ///

        public static void Create(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return;

            if (directoryPath.Length < MAX_PATH)
            {
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }
            }
            else
            {
                var paths = GetAllPathsFromPath(GetWin32LongPath(directoryPath));
                foreach (var item in paths)
                {
                    if (!LongExists(item))
                    {
                        var ok = CreateDirectory(item, IntPtr.Zero);
                        if (!ok)
                        {
                            ThrowWin32Exception();
                        }
                    }
                }
            }
        }

        public static async Task<Result> TryCreate(string directoryPath)
        {
            Result result = null;
            await Task.Run(() =>
            {
                try
                {
                    Create(directoryPath);
                    result = new Success();
                }
                catch (Exception e)
                {
                    result = new Error(e);
                }
            });
            return result;
        }

        ///

        static void Delete(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                var files = GetFiles(directory, SearchOption.TopDirectoryOnly);
                foreach (string i in files)
                    File.Long.Delete(i);

                directories = GetFolders(directory, SearchOption.TopDirectoryOnly);
                Delete(directories);

                if (!RemoveDirectory(GetWin32LongPath(directory)))
                    ThrowWin32Exception();
            }
        }

        public static void Delete(string path, bool recursive = false)
        {
            if (path.Length < MAX_PATH)
            {
                Directory.Delete(path, recursive);
            }
            else
            {
                if (!recursive)
                {
                    bool ok = RemoveDirectory(GetWin32LongPath(path));
                    if (!ok) ThrowWin32Exception();
                }
                else
                {
                    var longPath = GetWin32LongPath(path);

                    var files = GetFiles(longPath, SearchOption.TopDirectoryOnly);
                    foreach (string i in files)
                        File.Long.Delete(i);

                    Delete(new string[] { longPath });
                }
            }
        }

        ///

        /// <summary>
        /// Multiple slashes and periods return <see langword="false"/>!
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            if (path == null)
                return false;

            if (path.OnlyContains('/') || path.OnlyContains('\\') || path.OnlyContains('.'))
                return false;

            if (path.Length < MAX_PATH)
                return Directory.Exists(path);

            return LongExists(GetWin32LongPath(path));
        }

        ///

        async public static Task<long> TryGetSize(string folderPath, System.Threading.CancellationToken token)
        {
            long result = 0;
            await Task.Run(async () =>
            {
                var files = Enumerable.Empty<string>();
                Try.Invoke(() => files = GetFiles(folderPath));
                foreach (var i in files)
                {
                    if (token.IsCancellationRequested)
                        return;

                    Try.Invoke(() =>
                    {
                        var fileInfo = new FileInfo(i);
                        result += fileInfo.Length;
                    });
                }

                var folders = Enumerable.Empty<string>();
                Try.Invoke(() => folders = GetFolders(folderPath));
                foreach (var i in folders)
                {
                    if (token.IsCancellationRequested)
                        return;

                    result += await TryGetSize(i, token);
                }
            });
            return result;
        }

        ///

        public static IEnumerable<string> GetFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (path == StoragePath.Root)
                yield break;

            var searchPattern = "*";

            var files = new List<string>();
            var directories = new List<string> { path };

            if (searchOption == SearchOption.AllDirectories)
            {
                //Add all the subpaths
                directories.AddRange(GetFolders(path, SearchOption.AllDirectories));
            }

            foreach (var i in directories)
            {
                IntPtr findHandle = default;
                try
                {
                    findHandle = FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(i), searchPattern), out WIN32_FIND_DATA findData);
                    if (findHandle != new IntPtr(-1))
                    {
                        do
                        {
                            if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
                            {
                                string filename = System.IO.Path.Combine(i, findData.cFileName);
                                files.Add(GetCleanPath(filename));
                            }
                        } while (FindNextFile(findHandle, out findData));
                        FindClose(findHandle);
                    }
                }
                catch (Exception)
                {
                    if (findHandle != null)
                        FindClose(findHandle);

                    throw;
                }
            }
            foreach (var i in files)
            {
                if (System.IO.Path.GetFileName(i).ToLower() == "desktop.ini")
                {
                    //This file is used by Windows to determine how a folder is displayed in Windows Explorer. Ignore it!
                    continue;
                }
                yield return i;
            }
        }

        public static IEnumerable<string> GetFolders(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (path == StoragePath.Root)
                return Enumerable.Empty<string>();

            var result = new List<string>();
            InternalGetDirectories(path, "*", searchOption, ref result);
            return result.ToArray();
        }

        public static IEnumerable<string> GetItems(string path)
        {
            foreach (var i in GetFolders(path))
                yield return i;

            foreach (var i in GetFiles(path))
                yield return i;
        }

        ///

        public static void Move(string source, string destination)
        {
            if (source.Length < MAX_PATH || destination.Length < MAX_PATH)
            {
                Directory.Move(source, destination);
            }
            else if (!MoveFileW(GetWin32LongPath(source), GetWin32LongPath(destination)))
                ThrowWin32Exception();
        }

        ///

        public static string Parent(string folderPath)
        {
            var result = new DirectoryInfo(folderPath);
            return result.Parent == null ? StoragePath.Root : result.Parent.FullName;
        }

        #endregion
    }

    readonly Method contents;

    void RefreshContents(string path, ref int index, CancellationToken token)
    {
        if (index > 0)
        {
            Folders++;
            Try.Invoke(() =>
            {
                var folderInfo = new DirectoryInfo(path);
                Dispatch.Invoke(() =>
                {
                    if (NewestFolder == null || folderInfo.CreationTime > NewestFolder.Value)
                        NewestFolder = folderInfo.CreationTime;

                    if (OldestFolder == null || folderInfo.CreationTime < OldestFolder.Value)
                        OldestFolder = folderInfo.CreationTime;

                    if (Computer.Hidden(path))
                        HiddenFolders++;

                    if (Computer.ReadOnly(path))
                        ReadOnlyFolders++;
                });
            },
            e => Log.Write<Folder>(e));
        }

        IEnumerable<string> files = default;
        Try.Invoke(() => files = Long.GetFiles(path), e => Log.Write<Folder>(e));

        double count = files?.Count() ?? 0;
        if (count > 0)
        {
            foreach (var i in files)
            {
                if (token.IsCancellationRequested) return;

                FileInfo fileInfo = null;
                var fileText = string.Empty;

                bool hidden = false; bool readOnly = false;
                Result result = Try.Invoke(() =>
                {
                    hidden = Computer.Hidden(i); readOnly = Computer.ReadOnly(i);

                    fileInfo = new FileInfo(i);
                    fileText = File.Long.ReadAllText(i, System.Text.Encoding.Unicode);
                },
                e => Log.Write<Folder>(e));

                Dispatch.Invoke(() =>
                {
                    Files++;

                    if (hidden)
                        HiddenFiles++;

                    if (readOnly)
                        ReadOnlyFiles++;

                    if (NewestFile == null || fileInfo.CreationTime > NewestFile)
                        NewestFile = fileInfo.CreationTime;

                    if (OldestFile == null || fileInfo.CreationTime < OldestFile)
                        OldestFile = fileInfo.CreationTime;

                    if (fileInfo.Length > LargestFile)
                        LargestFile = fileInfo.Length;

                    if (SmallestFile == 0 || fileInfo.Length < SmallestFile)
                        SmallestFile = fileInfo.Length;

                    Characters += fileText.Length; Size += fileInfo.Length;
                    Try.Invoke(() =>
                    {
                        Lines
                            += fileText.Lines();
                        Words
                            += fileText.Words();
                    });
                });
            }
        }

        IEnumerable<string> folders = default;
        Try.Invoke(() => folders = Long.GetFolders(path), e => Log.Write<Folder>(e));
        if (folders?.Count() > 0)
        {
            foreach (var i in folders)
            {
                if (token.IsCancellationRequested) return;

                index++;
                RefreshContents(i, ref index, token);
            }
        }
    }

    async Task RefreshContents(CancellationToken token)
    {
        RefreshingContents = true;

        Files = 0; Folders = 0;
        Characters = 0; Lines = 0; Size = 0; Words = 0;
        NewestFile = NewestFolder = OldestFile = OldestFolder = null;
        HiddenFiles = HiddenFolders = ReadOnlyFiles = ReadOnlyFolders = 0;
        LargestFile = SmallestFile = 0;

        int index = 0;
        await Task.Run(() => RefreshContents(Path, ref index, token), token);

        RefreshingContents = false;
    }

    public void RefreshContents() => _ = contents.Start();

    [Hide]
    public bool RefreshingContents { get => Get(false); set => Set(value); }

    ICommand refreshContentsCommand;
    [Pin(Pin.BelowOrRight), Content("Refresh"), Name("Refresh"), Image(SmallImages.Refresh), Style(ButtonStyle.Default), VisibilityTrigger(nameof(RefreshingContents), false)]
    public ICommand RefreshContentsCommand => refreshContentsCommand ??= new RelayCommand(RefreshContents, () => !contents.Started);

    ICommand cancelRefreshCommand;
    [Pin(Pin.BelowOrRight), Content("Cancel refresh"), Name("Cancel refresh"), Image(SmallImages.Block), Style(ButtonStyle.Cancel), VisibilityTrigger(nameof(RefreshingContents), true)]
    public ICommand CancelRefreshCommand => cancelRefreshCommand ??= new RelayCommand(() => contents.Cancel(), () => contents.Started);
}