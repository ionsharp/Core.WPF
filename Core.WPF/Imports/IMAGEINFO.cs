using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Imagin.Core.Imports
{
    [StructLayout(LayoutKind.Sequential)]
    struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public Rect rcImage;
    }
}
