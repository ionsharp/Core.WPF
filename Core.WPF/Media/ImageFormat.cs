using Imagin.Core.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Media;

public struct ImageFormat
{
    readonly string extension;
    public string Extension => extension;

    readonly bool isReadable;
    public bool IsReadable => isReadable;

    readonly bool isWritable;
    public bool IsWritable => isWritable;

    public ImageFormat(string extension, bool isReadable, bool isWritable)
    {
        this.extension = extension; this.isReadable = isReadable; this.isWritable = isWritable;
    }

    static IEnumerable<ImageFormat> formats;
    public static IEnumerable<ImageFormat> Formats
    {
        get
        {
            formats = formats ?? XArray.New
            (
                new ImageFormat("image", true, true),
                //...
                new ImageFormat("aai", true, true),
                new ImageFormat("art", true, true),
                new ImageFormat("arw", true, false),
                new ImageFormat("avi", true, false),
                new ImageFormat("avs", true, true),
                new ImageFormat("bpg", true, true),
                new ImageFormat("bmp", true, true),
                new ImageFormat("bmp2", true, true),
                new ImageFormat("bmp3", true, true),
                new ImageFormat("cals", true, false),   //10
                new ImageFormat("cin", true, true),
                new ImageFormat("cmyk", true, true),
                new ImageFormat("cmyka", true, true),
                new ImageFormat("cr2", true, false),
                new ImageFormat("crw", true, false),
                new ImageFormat("cur", true, false),
                new ImageFormat("cut", true, false),
                new ImageFormat("dcm", true, false),
                new ImageFormat("dcr", true, false),
                new ImageFormat("dcx", true, true),   //20
                new ImageFormat("dds", true, true),
                new ImageFormat("dib", true, true),
                new ImageFormat("djvu", true, false),
                new ImageFormat("dng", true, false),
                new ImageFormat("dot", true, false),
                new ImageFormat("dpx", true, true),
                new ImageFormat("emf", true, false),
                new ImageFormat("epdf", true, false),
                new ImageFormat("epdf", true, true),
                new ImageFormat("exr", true, true),   //30
                new ImageFormat("fax", true, true),
                new ImageFormat("fits", true, true),
                new ImageFormat("fpx", true, true),
                new ImageFormat("fig", true, false),
                new ImageFormat("gif", true, true),
                new ImageFormat("gray", true, true),
                new ImageFormat("hdr", true, true),
                new ImageFormat("hrz", true, true),
                new ImageFormat("ico", true, true),
                new ImageFormat("info", false, true),   //40
                new ImageFormat("inline", true, true),
                new ImageFormat("jp2", true, true),
                new ImageFormat("jpt", true, true),
                new ImageFormat("j2c", true, true),
                new ImageFormat("j2k", true, true),
                new ImageFormat("jpeg", true, true),
                new ImageFormat("jpg", true, true),
                new ImageFormat("json", false, true),
                new ImageFormat("mat", true, false),
                new ImageFormat("miff", true, true),   //50
                new ImageFormat("mono", true, true),
                new ImageFormat("mng", true, true),
                new ImageFormat("m2v", true, true),
                new ImageFormat("mpeg", true, true),
                new ImageFormat("mpc", true, true),
                new ImageFormat("mpr", true, true),
                new ImageFormat("mrw", true, false),
                new ImageFormat("mtv", true, true),
                new ImageFormat("mvg", true, true),
                new ImageFormat("nef", true, false),   //60
                new ImageFormat("orf", true, false),
                new ImageFormat("otb", true, true),
                new ImageFormat("p7", true, true),
                new ImageFormat("palm", false, true),
                new ImageFormat("clipboard", true, false),
                new ImageFormat("pbm", true, true),
                new ImageFormat("pcd", true, true),
                new ImageFormat("pcds", true, true),
                new ImageFormat("pcx", true, true),
                new ImageFormat("pdb", true, true),   //70
                new ImageFormat("pef", true, false),
                new ImageFormat("pfa", true, false),
                new ImageFormat("pfb", true, false),
                new ImageFormat("pfm", true, true),
                new ImageFormat("pgm", true, true),
                new ImageFormat("picon", true, true),
                new ImageFormat("pict", true, true),
                new ImageFormat("pix", true, false),
                new ImageFormat("png", true, true),
                new ImageFormat("pnm", true, true),
                new ImageFormat("ppm", true, true),
                new ImageFormat("psb", true, true),
                new ImageFormat("psd", true, true),
                new ImageFormat("ptif", true, true),   //90
                new ImageFormat("pwp", true, false),
                new ImageFormat("rad", true, false),
                new ImageFormat("raf", true, false),
                new ImageFormat("rgb", true, true),
                new ImageFormat("rgba", true, true),
                new ImageFormat("rfg", true, true),
                new ImageFormat("rla", true, false),
                new ImageFormat("rle", true, false),
                new ImageFormat("sct", true, false),
                new ImageFormat("sfw", true, false),   //100
                new ImageFormat("sgi", true, true),
                new ImageFormat("sun", true, true),
                new ImageFormat("svg", true, true),
                new ImageFormat("tga", true, true),
                new ImageFormat("tiff", true, true),
                new ImageFormat("tim", true, false),
                new ImageFormat("txt", true, true),
                new ImageFormat("uil", false, true),
                new ImageFormat("uyvy", true, true),
                new ImageFormat("vicar", true, true),   //110
                new ImageFormat("viff", true, true),
                new ImageFormat("wbmp", true, true),
                new ImageFormat("wpg", false, true),
                new ImageFormat("x", true, true),
                new ImageFormat("xbm", true, true),
                new ImageFormat("xcf", true, true),
                new ImageFormat("xpm", true, true),
                new ImageFormat("xwd", true, true),
                new ImageFormat("x3f", true, false),
                new ImageFormat("ycbcr", true, true),   //120
                new ImageFormat("ycbcra", true, true),
                new ImageFormat("yuv", true, true)
            );
            return formats;
        }
    }

    public static IEnumerable<ImageFormat> GetReadable()
        => Formats.Where(i => i.IsReadable);

    public static IEnumerable<ImageFormat> GetWritable()
        => Formats.Where(i => i.IsWritable);

    public static bool CanRead(string extension)
        => Formats.Where(i => i.Extension.ToLower() == extension.ToLower()).FirstOrDefault().IsReadable;

    public static bool CanWrite(string extension)
        => Formats.Where(i => i.Extension.ToLower() == extension.ToLower()).FirstOrDefault().IsWritable;
}