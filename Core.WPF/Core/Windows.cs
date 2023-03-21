using Imagin.Core.Imports;
using Imagin.Core.Linq;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

namespace Imagin.Core;

public static class Windows
{
    #region (class) Desktop

    public static class Desktop
    {
        public enum StretchMode
        {
            Centered,
            Stretched,
            Tiled
        }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll")]
        static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        /// <summary>
        /// Path to current desktop wallpaper.
        /// </summary>
        public static string Background
        {
            get
            {
                var result = string.Empty;
                var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
                if (key != null)
                {
                    result = key.GetValue("WallPaper").ToString();
                    key.Close();
                }
                return result;
            }
        }

        public static StretchMode BackgroundStretchMode
        {
            get
            {
                var Key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                var a = Key.GetValue(@"WallpaperStyle").ToString();
                var b = Key.GetValue(@"TileWallpaper").ToString();

                if (a == "1" && b == "0")
                    return StretchMode.Centered;

                if (a == "2" && b == "0")
                    return StretchMode.Stretched;

                if (a == "1" && b == "1")
                    return StretchMode.Tiled;

                return StretchMode.Centered;
            }
        }

        static Desktop() { }

        public static bool IsActive()
        {
            try
            {
                var handle = GetForegroundWindow();

                const int maxChars = 256;
                var className = new StringBuilder(maxChars);
                if (GetClassName(handle.ToInt32(), className, maxChars) > 0)
                {
                    var cName = className.ToString();
                    if (cName == "Progman" || cName == "WorkerW")
                        return true;
                }
            }
            catch { }
            return false;
        }

        public static bool SetBackground(string path, StretchMode stretchMode = StretchMode.Centered)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                switch (stretchMode)
                {
                    case StretchMode.Centered:
                        key.SetValue(@"WallpaperStyle", "1");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                    case StretchMode.Stretched:
                        key.SetValue(@"WallpaperStyle", "2");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                    case StretchMode.Tiled:
                        key.SetValue(@"WallpaperStyle", "1");
                        key.SetValue(@"TileWallpaper", "1");
                        break;
                }
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    #endregion

    #region (class) Icon

    public static class Icon
    {
        #region Constants

        const int WM_CLOSE = 0x0010;

        const int SHGFI_ICON = 0x100;

        const int SHGFI_SMALLICON = 0x1;

        const int SHGFI_LARGEICON = 0x0;

        const int SHIL_JUMBO = 0x4;

        const int SHIL_EXTRALARGE = 0x2;

        #endregion

        #region Structs

        struct Pair
        {
            public System.Drawing.Icon Icon
            {
                get; set;
            }

            public IntPtr HandleToDestroy
            {
                set; get;
            }
        }

        #endregion

        #region Methods

        #region Imports

        [DllImport("user32")]
        static extern IntPtr SendMessage(IntPtr handle, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary> 
        [DllImport("shell32.dll", EntryPoint = "#727")]
        extern static int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

        /// <summary>
        /// The signature of SHGetFileInfo (located in Shell32.dll)
        /// </summary>
        [DllImport("Shell32.dll")]
        static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport("Shell32.dll")]
        static extern int SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, ref IntPtr ppidl);

        [DllImport("user32")]
        static extern int DestroyIcon(IntPtr hIcon);

        #endregion

        #region Internal

        /*
        static ImageSource SystemIcon(bool Small, ShellApi.CSIDL csidl)
        {
            IntPtr pidlTrash = IntPtr.Zero;
            int hr = SHGetSpecialFolderLocation(IntPtr.Zero, (int)csidl, ref pidlTrash);
            System.Diagnostics.Debug.Assert(hr == 0);

            SHFILEINFO shinfo = new SHFILEINFO();

            uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

            // Get a handle to the large icon
            uint flags;
            uint SHGFI_PIDL = 0x000000008;
            if (!Small)
            {
                flags = SHGFI_PIDL | SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES;
            }
            else
            {
                flags = SHGFI_PIDL | SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES;
            }

            var res = SHGetFileInfo(pidlTrash, 0, ref shinfo, Marshal.SizeOf(shinfo), flags);
            System.Diagnostics.Debug.Assert(res != 0);

            var myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
            Marshal.FreeCoTaskMem(pidlTrash);
            var bs = myIcon.ToImageSource();
            myIcon.Dispose();
            bs.Freeze(); // importantissimo se no fa memory leak
            DestroyIcon(shinfo.hIcon);
            SendMessage(shinfo.hIcon, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return bs;

        }
        */

        static ImageSource Extract(string FileName, bool Small, bool checkDisk, bool addOverlay)
        {
            SHFILEINFO shinfo = new();

            uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            uint SHGFI_LINKOVERLAY = 0x000008000;

            uint flags;
            if (Small)
            {
                flags = SHGFI_ICON | SHGFI_SMALLICON;
            }
            else
            {
                flags = SHGFI_ICON | SHGFI_LARGEICON;
            }
            if (!checkDisk)
                flags |= SHGFI_USEFILEATTRIBUTES;
            if (addOverlay)
                flags |= SHGFI_LINKOVERLAY;

            var res = SHGetFileInfo(FileName, 0, ref shinfo, Marshal.SizeOf(shinfo), flags);
            if (res == 0)
            {
                throw (new System.IO.FileNotFoundException());
            }

            var myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);

            var bs = myIcon.ImageSource();
            myIcon.Dispose();
            bs.Freeze(); // importantissimo se no fa memory leak
            DestroyIcon(shinfo.hIcon);
            SendMessage(shinfo.hIcon, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return bs;
        }

        static ImageSource ExtractLarge(string FileName, bool jumbo, bool checkDisk)
        {
            try
            {
                SHFILEINFO shinfo = new();

                uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
                uint SHGFI_SYSICONINDEX = 0x4000;

                int FILE_ATTRIBUTE_NORMAL = 0x80;

                uint flags;
                flags = SHGFI_SYSICONINDEX;

                if (!checkDisk)  // This does not seem to work. If I try it, a folder icon is always returned.
                    flags |= SHGFI_USEFILEATTRIBUTES;

                var res = SHGetFileInfo(FileName, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), flags);
                if (res == 0)
                {
                    throw (new System.IO.FileNotFoundException());
                }
                var iconIndex = shinfo.iIcon;

                // Get the System IImageList object from the Shell:
                Guid iidImageList = new("46EB5926-582E-4017-9FDF-E8998DAA0950");

                int size = jumbo ? SHIL_JUMBO : SHIL_EXTRALARGE;
                var hres = SHGetImageList(size, ref iidImageList, out IImageList iml);
                // writes iml
                //if (hres == 0)
                //{
                //    throw (new System.Exception("Error SHGetImageList"));
                //}

                IntPtr hIcon = IntPtr.Zero;
                int ILD_TRANSPARENT = 1;
                hres = iml.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
                //if (hres == 0)
                //{
                //    throw (new System.Exception("Error iml.GetIcon"));
                //}

                var myIcon = System.Drawing.Icon.FromHandle(hIcon);
                var bs = myIcon.ImageSource();
                myIcon.Dispose();
                bs.Freeze(); // very important to avoid memory leak
                DestroyIcon(hIcon);
                SendMessage(hIcon, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                return bs;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Static

        public static ImageSource GetLarge(string Path)
        {
            return ExtractLarge(Path, true, true);
        }

        /*
        public static ImageSource GetSystem(bool Small, ShellApi.CSIDL Kind)
        {
            return SystemIcon(Small, Kind);
        }
        */

        #endregion

        #endregion
    }

    #endregion
}