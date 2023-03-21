using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Imagin.Core.Media;

public class ICCProfile
{
    #region ProfileFilename

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class ProfileFilename
    {
        public uint type;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string profileData;
        public uint dataSize;

        public ProfileFilename(string filename)
        {
            type = ProfileFilenameType;
            profileData = filename;
            dataSize = (uint)filename.Length * 2 + 2;
        }
    };

    #endregion

    #region Imports

    public const uint ProfileFilenameType = 1;
    public const uint ProfileMembufferType = 2;

    public const uint ProfileRead = 1;
    public const uint ProfileReadWrite = 2;

    #endregion

    #region (enum) FileShare

    public enum FileShare : uint
    {
        Read = 1,
        Write = 2,
        Delete = 4
    };

    #endregion

    #region (enum) CreateDisposition

    public enum CreateDisposition : uint
    {
        CreateNew = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5
    };

    #endregion

    #region (enum) LogicalColorSpace

    public enum LogicalColorSpace : uint
    {
        CalibratedRGB = 0x00000000,
        sRGB = 0x73524742,
        WindowsColorSpace = 0x57696E20
    };

    #endregion

    #region (enum) ColorTransformMode

    public enum ColorTransformMode : uint
    {
        ProofMode = 0x00000001,
        NormalMode = 0x00000002,
        BestMode = 0x00000003,
        EnableGamutChecking = 0x00010000,
        UseRelativeColorimetric = 0x00020000,
        FastTranslate = 0x00040000,
        PreserveBlack = 0x00100000,
        WCSAlways = 0x00200000
    };

    #endregion

    #region (enum) ColorType

    enum ColorType : int
    {
        Gray = 1,
        RGB = 2,
        XYZ = 3,
        Yxy = 4,
        Lab = 5,
        _3_Channel = 6,
        CMYK = 7,
        _5_Channel = 8,
        _6_Channel = 9,
        _7_Channel = 10,
        _8_Channel = 11,
        Named = 12
    };

    #endregion

    #region Constants

    public const uint IntentPerceptual = 0;
    public const uint IntentRelativeColorimetric = 1;
    public const uint IntentSaturation = 2;
    public const uint IntentAbsoluteColorimetric = 3;

    public const uint IndexDontCare = 0;

    #endregion

    #region (struct) CMYKColor

    [StructLayout(LayoutKind.Sequential)]
    public struct CMYKColor
    {
        public ushort cyan;
        public ushort magenta;
        public ushort yellow;
        public ushort black;
    };

    #endregion

    #region (struct) RGBColor

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBColor
    {
        public ushort red;
        public ushort green;
        public ushort blue;
        public ushort pad;
    };

    #endregion

    #region Imports

    [DllImport("mscms.dll", SetLastError = true, EntryPoint = "OpenColorProfileW", CallingConvention = CallingConvention.Winapi)]
    static extern IntPtr OpenColorProfile(
        [MarshalAs(UnmanagedType.LPStruct)] ProfileFilename profile,
        uint desiredAccess,
        FileShare shareMode,
        CreateDisposition creationMode);

    [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    static extern bool CloseColorProfile(IntPtr hProfile);

    [DllImport("mscms.dll", SetLastError = true, EntryPoint = "GetStandardColorSpaceProfileW", CallingConvention = CallingConvention.Winapi)]
    static extern bool GetStandardColorSpaceProfile(
        uint machineName,
        LogicalColorSpace profileID,
        [MarshalAs(UnmanagedType.LPTStr), In, Out] StringBuilder profileName,
        ref uint size);

    [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    static extern IntPtr CreateMultiProfileTransform(
        [In] IntPtr[] profiles,
        uint nProfiles,
        [In] uint[] intents,
        uint nIntents,
        ColorTransformMode flags,
        uint indexPreferredCMM);

    [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    static extern bool DeleteColorTransform(IntPtr hTransform);

    [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    static extern bool TranslateColors(
        IntPtr hColorTransform,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] RGBColor[] inputColors,
        uint nColors,
        ColorType ctInput,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), Out] CMYKColor[] outputColors,
        ColorType ctOutput);

    #endregion

    public static void Test()
    {
        bool success;

        StringBuilder profileName = new(256);
        uint size = (uint)profileName.Capacity * 2;
        success = GetStandardColorSpaceProfile(0, LogicalColorSpace.sRGB, profileName, ref size);

        ProfileFilename sRGBFilename = new(profileName.ToString());
        IntPtr hSRGBProfile = OpenColorProfile(sRGBFilename, ProfileRead, FileShare.Read, CreateDisposition.OpenExisting);

        ProfileFilename isoCoatedFilename = new(@"C:\Users\me\Documents\ISOcoated_v2_300_eci.icc");
        IntPtr hIsoCoatedProfile = OpenColorProfile(isoCoatedFilename, ProfileRead, FileShare.Read, CreateDisposition.OpenExisting);

        IntPtr[] profiles = new IntPtr[] { hSRGBProfile, hIsoCoatedProfile };
        uint[] intents = new uint[] { IntentPerceptual };
        IntPtr transform = CreateMultiProfileTransform(profiles, 2, intents, 1, ColorTransformMode.BestMode, IndexDontCare);

        RGBColor[] rgbColors = new RGBColor[1];
        rgbColors[0] = new RGBColor();
        CMYKColor[] cmykColors = new CMYKColor[1];
        cmykColors[0] = new CMYKColor();

        rgbColors[0].red = 30204;
        rgbColors[0].green = 4420;
        rgbColors[0].blue = 60300;

        success = TranslateColors(transform, rgbColors, 1, ColorType.RGB, cmykColors, ColorType.CMYK);

        success = DeleteColorTransform(transform);

        success = CloseColorProfile(hSRGBProfile);
        success = CloseColorProfile(hIsoCoatedProfile);
    }
}