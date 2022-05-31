using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Imagin.Core.Native
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
