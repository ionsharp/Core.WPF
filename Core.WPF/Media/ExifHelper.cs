using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Imagin.Core.Media;

/// <summary>
/// Utility class for working with EXIF (Exchangeable image file format) data in images. Provides abstraction for most common data and generic utilities for work with all other. 
/// </summary>
/// <remarks>
/// Copyright (c) Michal A. Valášek - Altair Communications, 2003-2005
/// Copmany: http://software.altaircom.net, E-mail: support@altaircom.net
/// Private: http://www.rider.cz, E-mail: rider@rider.cz
/// This is free software licensed under GNU Lesser General Public License
/// </remarks>
/// <history>
/// [altair] 10.09.2003 Created
/// [altair] 12.06.2004 Added capability to write EXIF data
/// [altair] 11.07.2004 Added option to change encoding
/// [altair] 04.09.2005 Changed source of Width and Height properties from EXIF to image
/// [altair] 05.09.2005 Code clean-up and minor changes
/// </history>
public class ExifHelper : IDisposable
{
    System.Drawing.Bitmap _Image;

    System.Text.Encoding _Encoding = System.Text.Encoding.UTF8;

    #region DefaultTags

    public static readonly Dictionary<object, ExifTag> DefaultTags = new()
    {
        { 0x0, new(0x0, "GPSVersionID", "GPS tag version") },
        { 0x5, new(0x5, "GPSAltitudeRef", "Altitude reference") },
        { 0x2, new(0x2, "GPSLatitude", "Latitude") },
        { 0x4, new(0x4, "GPSLongitude", "Longitude") },
        { 0x6, new(0x6, "GPSAltitude", "Altitude") },
        { 0x7, new(0x7, "GPSTimeStamp", "GPS time (atomic clock)") },
        { 0xB, new(0xB, "GPSDOP", "Measurement precision") },
        { 0xD, new(0xD, "GPSSpeed", "Speed of GPS receiver") },
        { 0xF, new(0xF, "GPSTrack", "Direction of movement") },
        { 0x1, new(0x1, "GPSLatitudeRef", "North or South Latitude") },
        { 0x3, new(0x3, "GPSLongitudeRef", "East or West Longitude") },
        { 0x8, new(0x8, "GPSSatellites", "GPS satellites used for measurement") },
        { 0x9, new(0x9, "GPSStatus", "GPS receiver status") },
        { 0xA, new(0xA, "GPSMeasureMode", "GPS measurement mode") },
        { 0xC, new(0xC, "GPSSpeedRef", "Speed unit") },
        { 0xE, new(0xE, "GPSTrackRef", "Reference for direction of movement") },

        { 0x1E, new(0x1E, "GPSDifferential", "GPS differential correction") },
        { 0x11, new(0x11, "GPSImgDirection", "Direction of image") },
        { 0x14, new(0x14, "GPSDestLatitude", "Latitude of destination") },
        { 0x16, new(0x16, "GPSDestLongitude", "Longitude of destination") },
        { 0x18, new(0x18, "GPSDestBearing", "Bearing of destination") },
        { 0x1A, new(0x1A, "GPSDestDistance", "Distance to destination") },
        { 0x10, new(0x10, "GPSImgDirectionRef", "Reference for direction of image") },
        { 0x12, new(0x12, "GPSMapDatum", "Geodetic survey data used") },
        { 0x13, new(0x13, "GPSDestLatitudeRef", "Reference for latitude of destination") },
        { 0x15, new(0x15, "GPSDestLongitudeRef", "Reference for longitude of destination") },
        { 0x17, new(0x17, "GPSDestBearingRef", "Reference for bearing of destination") },
        { 0x19, new(0x19, "GPSDestDistanceRef", "Reference for distance to destination") },
        { 0x1D, new(0x1D, "GPSDateStamp", "GPS date") },
        { 0x1B, new(0x1B, "GPSProcessingMethod", "Name of GPS processing method") },
        { 0x1C, new(0x1C, "GPSAreaInformation", "Name of GPS area") },

        { 0x100, new(0x100, "ImageWidth", "Image width") },
        { 0x101, new(0x101, "ImageHeight", "Image height") },
        { 0x111, new(0x111, "StripOffsets", "Image data location") },
        { 0x116, new(0x116, "RowsPerStrip", "Number of rows per strip") },
        { 0x117, new(0x117, "StripByteCounts", "Bytes per compressed strip") },
        { 0x102, new(0x102, "BitsPerSample", "Number of bits per component") },
        { 0x103, new(0x103, "Compression", "Compression scheme") },
        { 0x106, new(0x106, "PhotometricInterpretation", "Pixel composition") },
        { 0x112, new(0x112, "Orientation", "Orientation of image") },
        { 0x115, new(0x115, "SamplesPerPixel", "Number of components") },
        { 0x11C, new(0x11C, "PlanarConfiguration", "Image data arrangement") },
        { 0x212, new(0x212, "YCbCrSubSampling", "Subsampling ratio of Y to C") },
        { 0x213, new(0x213, "YCbCrPositioning", "Y and C positioning") },
        { 0x128, new(0x128, "ResolutionUnit", "Unit of X and Y resolution") },
        { 0x12D, new(0x12D, "TransferFunction", "Transfer function") },
        { 0x201, new(0x201, "JPEGInterchangeFormat", "Offset to JPEG SOI") },
        { 0x202, new(0x202, "JPEGInterchangeFormatLength", "Bytes of JPEG data") },
        { 0x11A, new(0x11A, "XResolution", "Image resolution in width direction") },
        { 0x11B, new(0x11B, "YResolution", "Image resolution in height direction") },
        { 0x13E, new(0x13E, "WhitePoint", "White point chromaticity") },
        { 0x13F, new(0x13F, "PrimaryChromaticities", "Chromaticities of primaries") },
        { 0x211, new(0x211, "YCbCrCoefficients", "Color space transformation matrix coefficients") },
        { 0x214, new(0x214, "ReferenceBlackWhite", "Pair of black and white reference values") },
        { 0x132, new(0x132, "DateTime", "File change date and time") },
        { 0x10E, new(0x10E, "ImageDescription", "Image title") },
        { 0x10F, new(0x10F, "Make", "Image input equipment manufacturer") },
        { 0x110, new(0x110, "Model", "Image input equipment model") },
        { 0x131, new(0x131, "Software", "Software used") },
        { 0x13B, new(0x13B, "Artist", "Person who created the image") },

        { 0xA002, new(0xA002, "PixelXDimension", "Valid image width") },
        { 0xA003, new(0xA003, "PixelYDimension", "Valid image height") },
        { 0xA001, new(0xA001, "ColorSpace", "Color space information") },
        { 0x8822, new(0x8822, "ExposureProgram", "Exposure program") },
        { 0x8827, new(0x8827, "ISOSpeedRatings", "ISO speed rating") },
        { 0x9207, new(0x9207, "MeteringMode", "Metering mode") },
        { 0x9208, new(0x9208, "LightSource", "Light source") },
        { 0x9209, new(0x9209, "Flash", "Flash") },
        { 0x9214, new(0x9214, "SubjectArea", "Subject area") },
        { 0xA210, new(0xA210, "FocalPlaneResolutionUnit", "Focal plane resolution unit") },
        { 0xA214, new(0xA214, "SubjectLocation", "Subject location") },
        { 0xA217, new(0xA217, "SensingMethod", "Sensing method") },
        { 0xA401, new(0xA401, "CustomRendered", "Custom image processing") },
        { 0xA402, new(0xA402, "ExposureMode", "Exposure mode") },
        { 0xA403, new(0xA403, "WhiteBalance", "White balance") },
        { 0xA405, new(0xA405, "FocalLengthIn35mmFilm", "Focal length in 35 mm film") },
        { 0xA406, new(0xA406, "SceneCaptureType", "Scene capture type") },
        { 0xA408, new(0xA408, "Contrast", "Contrast") },
        { 0xA409, new(0xA409, "Saturation", "Saturation") },
        { 0xA40A, new(0xA40A, "Sharpness", "Sharpness") },
        { 0xA40C, new(0xA40C, "SubjectDistanceRange", "Subject distance range") },
        { 0x9201, new(0x9201, "ShutterSpeedValue", "Shutter speed") },
        { 0x9203, new(0x9203, "BrightnessValue", "Brightness") },
        { 0x9204, new(0x9204, "ExposureBiasValue", "Exposure bias") },
        { 0x9102, new(0x9102, "CompressedBitsPerPixel", "Image compression mode") },
        { 0x829A, new(0x829A, "ExposureTime", "Exposure time") },
        { 0x829D, new(0x829D, "FNumber", "F number") },
        { 0x9202, new(0x9202, "ApertureValue", "Aperture") },
        { 0x9205, new(0x9205, "MaxApertureValue", "Maximum lens aperture") },
        { 0x9206, new(0x9206, "SubjectDistance", "Subject distance") },
        { 0x920A, new(0x920A, "FocalLength", "Lens focal length") },
        { 0xA20B, new(0xA20B, "FlashEnergy", "Flash energy") },
        { 0xA20E, new(0xA20E, "FocalPlaneXResolution", "Focal plane X resolution") },
        { 0xA20F, new(0xA20F, "FocalPlaneYResolution", "Focal plane Y resolution") },
        { 0xA215, new(0xA215, "ExposureIndex", "Exposure index") },
        { 0xA404, new(0xA404, "DigitalZoomRatio", "Digital zoom ratio") },
        { 0xA407, new(0xA407, "GainControl", "Gain control") },
        { 0x8298, new(0x8298, "Copyright", "Copyright holder") },
        { 0xA004, new(0xA004, "RelatedSoundFile", "Related audio file") },
        { 0x9003, new(0x9003, "DateTimeOriginal", "Date and time of original data generation") },
        { 0x9004, new(0x9004, "DateTimeDigitized", "Date and time of digital data generation") },
        { 0x9290, new(0x9290, "SubSecTime", "DateTime subseconds") },
        { 0x9291, new(0x9291, "SubSecTimeOriginal", "DateTimeOriginal subseconds") },
        { 0x9292, new(0x9292, "SubSecTimeDigitized", "DateTimeDigitized subseconds") },
        { 0xA420, new(0xA420, "ImageUniqueID", "Unique image ID") },
        { 0x8824, new(0x8824, "SpectralSensitivity", "Spectral sensitivity") },
        { 0x8828, new(0x8828, "OECF", "Optoelectric conversion factor") },
        { 0xA20C, new(0xA20C, "SpatialFrequencyResponse", "Spatial frequency response") },
        { 0xA300, new(0xA300, "FileSource", "File source") },
        { 0xA301, new(0xA301, "SceneType", "Scene type") },
        { 0xA302, new(0xA302, "CFAPattern", "CFA pattern") },
        { 0xA40B, new(0xA40B, "DeviceSettingDescription", "Device settings description") },
        { 0x9000, new(0x9000, "ExifVersion", "Exif version") },
        { 0xA000, new(0xA000, "FlashpixVersion", "Supported Flashpix version") },
        { 0x9101, new(0x9101, "ComponentsConfiguration", "Meaning of each component") },
        { 0x927C, new(0x927C, "MakerNote", "Manufacturer notes") },
        { 0x9286, new(0x9286, "UserComment", "User comments") },
    };

    #endregion

    #region Enums

    /// <summary>
    /// Contains possible values of EXIF tag names (ID)
    /// </summary>
    /// <remarks>See GdiPlusImaging.h</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum TagNames : int
    {
        ExifIFD = 0x8769,
        GpsIFD = 0x8825,
        NewSubfileType = 0xfe,
        SubfileType = 0xff,
        ImageWidth = 0x100,
        ImageHeight = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterp = 0x106,
        ThreshHolding = 0x107,
        CellWidth = 0x108,
        CellHeight = 0x109,
        FillOrder = 0x10a,
        DocumentName = 0x10d,
        ImageDescription = 0x10e,
        EquipMake = 0x10f,
        EquipModel = 0x110,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripBytesCount = 0x117,
        MinSampleValue = 0x118,
        MaxSampleValue = 0x119,
        XResolution = 0x11a,
        YResolution = 0x11b,
        PlanarConfig = 0x11c,
        PageName = 0x11d,
        XPosition = 0x11e,
        YPosition = 0x11f,
        FreeOffset = 0x120,
        FreeByteCounts = 0x121,
        GrayResponseUnit = 0x122,
        GrayResponseCurve = 0x123,
        T4Option = 0x124,
        T6Option = 0x125,
        ResolutionUnit = 0x128,
        PageNumber = 0x129,
        TransferFuncition = 0x12d,
        SoftwareUsed = 0x131,
        DateTime = 0x132,
        Artist = 0x13b,
        HostComputer = 0x13c,
        Predictor = 0x13d,
        WhitePoint = 0x13e,
        PrimaryChromaticities = 0x13f,
        ColorMap = 0x140,
        HalftoneHints = 0x141,
        TileWidth = 0x142,
        TileLength = 0x143,
        TileOffset = 0x144,
        TileByteCounts = 0x145,
        InkSet = 0x14c,
        InkNames = 0x14d,
        NumberOfInks = 0x14e,
        DotRange = 0x150,
        TargetPrinter = 0x151,
        ExtraSamples = 0x152,
        SampleFormat = 0x153,
        SMinSampleValue = 0x154,
        SMaxSampleValue = 0x155,
        TransferRange = 0x156,
        JPEGProc = 0x200,
        JPEGInterFormat = 0x201,
        JPEGInterLength = 0x202,
        JPEGRestartInterval = 0x203,
        JPEGLosslessPredictors = 0x205,
        JPEGPointTransforms = 0x206,
        JPEGQTables = 0x207,
        JPEGDCTables = 0x208,
        JPEGACTables = 0x209,
        YCbCrCoefficients = 0x211,
        YCbCrSubsampling = 0x212,
        YCbCrPositioning = 0x213,
        REFBlackWhite = 0x214,
        ICCProfile = 0x8773,
        Gamma = 0x301,
        ICCProfileDescriptor = 0x302,
        SRGBRenderingIntent = 0x303,
        ImageTitle = 0x320,
        Copyright = 0x8298,
        ResolutionXUnit = 0x5001,
        ResolutionYUnit = 0x5002,
        ResolutionXLengthUnit = 0x5003,
        ResolutionYLengthUnit = 0x5004,
        PrintFlags = 0x5005,
        PrintFlagsVersion = 0x5006,
        PrintFlagsCrop = 0x5007,
        PrintFlagsBleedWidth = 0x5008,
        PrintFlagsBleedWidthScale = 0x5009,
        HalftoneLPI = 0x500a,
        HalftoneLPIUnit = 0x500b,
        HalftoneDegree = 0x500c,
        HalftoneShape = 0x500d,
        HalftoneMisc = 0x500e,
        HalftoneScreen = 0x500f,
        JPEGQuality = 0x5010,
        GridSize = 0x5011,
        ThumbnailFormat = 0x5012,
        ThumbnailWidth = 0x5013,
        ThumbnailHeight = 0x5014,
        ThumbnailColorDepth = 0x5015,
        ThumbnailPlanes = 0x5016,
        ThumbnailRawBytes = 0x5017,
        ThumbnailSize = 0x5018,
        ThumbnailCompressedSize = 0x5019,
        ColorTransferFunction = 0x501a,
        ThumbnailData = 0x501b,
        ThumbnailImageWidth = 0x5020,
        ThumbnailImageHeight = 0x502,
        ThumbnailBitsPerSample = 0x5022,
        ThumbnailCompression = 0x5023,
        ThumbnailPhotometricInterp = 0x5024,
        ThumbnailImageDescription = 0x5025,
        ThumbnailEquipMake = 0x5026,
        ThumbnailEquipModel = 0x5027,
        ThumbnailStripOffsets = 0x5028,
        ThumbnailOrientation = 0x5029,
        ThumbnailSamplesPerPixel = 0x502a,
        ThumbnailRowsPerStrip = 0x502b,
        ThumbnailStripBytesCount = 0x502c,
        ThumbnailResolutionX = 0x502d,
        ThumbnailResolutionY = 0x502e,
        ThumbnailPlanarConfig = 0x502f,
        ThumbnailResolutionUnit = 0x5030,
        ThumbnailTransferFunction = 0x5031,
        ThumbnailSoftwareUsed = 0x5032,
        ThumbnailDateTime = 0x5033,
        ThumbnailArtist = 0x5034,
        ThumbnailWhitePoint = 0x5035,
        ThumbnailPrimaryChromaticities = 0x5036,
        ThumbnailYCbCrCoefficients = 0x5037,
        ThumbnailYCbCrSubsampling = 0x5038,
        ThumbnailYCbCrPositioning = 0x5039,
        ThumbnailRefBlackWhite = 0x503a,
        ThumbnailCopyRight = 0x503b,
        LuminanceTable = 0x5090,
        ChrominanceTable = 0x5091,
        FrameDelay = 0x5100,
        LoopCount = 0x5101,
        PixelUnit = 0x5110,
        PixelPerUnitX = 0x5111,
        PixelPerUnitY = 0x5112,
        PaletteHistogram = 0x5113,
        ExifExposureTime = 0x829a,
        ExifFNumber = 0x829d,
        ExifExposureProg = 0x8822,
        ExifSpectralSense = 0x8824,
        ExifISOSpeed = 0x8827,
        ExifOECF = 0x8828,
        ExifVer = 0x9000,
        ExifDTOrig = 0x9003,
        ExifDTDigitized = 0x9004,
        ExifCompConfig = 0x9101,
        ExifCompBPP = 0x9102,
        ExifShutterSpeed = 0x9201,
        ExifAperture = 0x9202,
        ExifBrightness = 0x9203,
        ExifExposureBias = 0x9204,
        ExifMaxAperture = 0x9205,
        ExifSubjectDist = 0x9206,
        ExifMeteringMode = 0x9207,
        ExifLightSource = 0x9208,
        ExifFlash = 0x9209,
        ExifFocalLength = 0x920a,
        ExifMakerNote = 0x927c,
        ExifUserComment = 0x9286,
        ExifDTSubsec = 0x9290,
        ExifDTOrigSS = 0x9291,
        ExifDTDigSS = 0x9292,
        ExifFPXVer = 0xa000,
        ExifColorSpace = 0xa001,
        ExifPixXDim = 0xa002,
        ExifPixYDim = 0xa003,
        ExifRelatedWav = 0xa004,
        ExifInterop = 0xa005,
        ExifFlashEnergy = 0xa20b,
        ExifSpatialFR = 0xa20c,
        ExifFocalXRes = 0xa20e,
        ExifFocalYRes = 0xa20f,
        ExifFocalResUnit = 0xa210,
        ExifSubjectLoc = 0xa214,
        ExifExposureIndex = 0xa215,
        ExifSensingMethod = 0xa217,
        ExifFileSource = 0xa300,
        ExifSceneType = 0xa301,
        ExifCfaPattern = 0xa302,
        GpsVer = 0x0,
        GpsLatitudeRef = 0x1,
        GpsLatitude = 0x2,
        GpsLongitudeRef = 0x3,
        GpsLongitude = 0x4,
        GpsAltitudeRef = 0x5,
        GpsAltitude = 0x6,
        GpsGpsTime = 0x7,
        GpsGpsSatellites = 0x8,
        GpsGpsStatus = 0x9,
        GpsGpsMeasureMode = 0xa,
        GpsGpsDop = 0xb,
        GpsSpeedRef = 0xc,
        GpsSpeed = 0xd,
        GpsTrackRef = 0xe,
        GpsTrack = 0xf,
        GpsImgDirRef = 0x10,
        GpsImgDir = 0x11,
        GpsMapDatum = 0x12,
        GpsDestLatRef = 0x13,
        GpsDestLat = 0x14,
        GpsDestLongRef = 0x15,
        GpsDestLong = 0x16,
        GpsDestBearRef = 0x17,
        GpsDestBear = 0x18,
        GpsDestDistRef = 0x19,
        GpsDestDist = 0x1a
    }

    /// <summary>
    /// Real position of 0th row and column of picture
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum Orientations
    {
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        BottomLeft = 4,
        LeftTop = 5,
        RightTop = 6,
        RightBottom = 7,
        LftBottom = 8
    }

    /// <summary>
    /// Exposure programs
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum ExposurePrograms
    {
        Manual = 1,
        Normal = 2,
        AperturePriority = 3,
        ShutterPriority = 4,
        Creative = 5,
        Action = 6,
        Portrait = 7,
        Landscape = 8
    }

    /// <summary>
    /// Exposure metering modes
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum ExposureMeteringModes
    {
        Unknown = 0,
        Average = 1,
        CenterWeightedAverage = 2,
        Spot = 3,
        MultiSpot = 4,
        MultiSegment = 5,
        Partial = 6,
        Other = 255
    }

    /// <summary>
    /// Flash activity modes
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum FlashModes
    {
        NotFired = 0,
        Fired = 1,
        FiredButNoStrobeReturned = 5,
        FiredAndStrobeReturned = 7
    }

    /// <summary>
    /// Possible light sources (white balance)
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public enum LightSources
    {
        Unknown = 0,
        Daylight = 1,
        Fluorescent = 2,
        Tungsten = 3,
        Flash = 10,
        StandardLightA = 17,
        StandardLightB = 18,
        StandardLightC = 19,
        D55 = 20,
        D65 = 21,
        D75 = 22,
        Other = 255
    }

    /// <summary>
    /// EXIF data types
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 12.6.2004 Created
    /// </history>
    public enum ExifDataTypes : short
    {
        UnsignedByte = 1,
        AsciiString = 2,
        UnsignedShort = 3,
        UnsignedLong = 4,
        UnsignedRational = 5,
        SignedByte = 6,
        Undefined = 7,
        SignedShort = 8,
        SignedLong = 9,
        SignedRational = 10,
        SingleFloat = 11,
        DoubleFloat = 12
    }

    public struct GPSRational
    {
        public GPSRational(byte[] value) { }

        public string ToString(string input) => "";
    }

    /// <summary>
    /// Represents rational which is type of some Exif properties
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public struct Rational
    {
        public Int32 Numerator;

        public Int32 Denominator;

        /// <summary>
        /// Converts rational to string representation
        /// </summary>
        /// <param name="Delimiter">Optional, default "/". String to be used as delimiter of components.</param>
        /// <returns>String representation of the rational.</returns>
        /// <remarks></remarks>
        /// <history>
        /// [altair] 10.09.2003 Created
        /// </history>

        public string ToString(string Delimiter = "/")
        {
            return Numerator + Delimiter + Denominator;
        }


        /// <summary>
        /// Converts rational to double precision real number
        /// </summary>
        /// <returns>The rational as double precision real number.</returns>
        /// <remarks></remarks>
        /// <history>
        /// [altair] 10.09.2003 Created
        /// </history>

        public double ToDouble()
        {
            return Numerator / Denominator;
        }
    }

    #endregion

    #region Fields

    public static readonly Hashtable Tags = new();

    #endregion

    #region ExifHelper

    public ExifHelper() => DefaultTags.ForEach(i => Tags.Add(i.Key, i.Value));

    /// <summary>
    /// Initializes new instance of this class.
    /// </summary>
    /// <param name="Bitmap">Bitmap to read exif information from</param>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public ExifHelper(ref System.Drawing.Bitmap Bitmap)
    {
        if (Bitmap == null)
            throw new ArgumentNullException("Bitmap");
        this._Image = Bitmap;
    }

    /// <summary>
    /// Initializes new instance of this class.
    /// </summary>
    /// <param name="FileName">Name of file to be loaded</param>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 13.06.2004 Created
    /// </history>
    public ExifHelper(string FileName)
    {
        this._Image = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(FileName);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get or set encoding used for string metadata
    /// </summary>
    /// <value>Encoding used for string metadata</value>
    /// <remarks>Default encoding is UTF-8</remarks>
    /// <history>
    /// [altair] 11.07.2004 Created
    /// [altair] 05.09.2005 Changed from shared to instance member
    /// </history>
    public System.Text.Encoding Encoding
    {
        get { return this._Encoding; }
        set
        {
            if (value == null)
                throw new ArgumentNullException();
            this._Encoding = Encoding;
        }
    }

    /// <summary>
    /// Returns copy of bitmap this instance is working on
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 13.06.2004 Created
    /// </history>
    public System.Drawing.Bitmap GetBitmap()
    {
        return (System.Drawing.Bitmap)this._Image.Clone();
    }

    public static IEnumerable<string> GetProperties(ListSortDirection sort)
    {
        var properties = new List<string>();
        foreach (DictionaryEntry i in ExifHelper.Tags)
            properties.Add(((ExifTag)i.Value).Name);

        return sort == ListSortDirection.Ascending ? properties.OrderBy(i => i) : properties.OrderByDescending(i => i);
    }

    /// <summary>
    /// Returns all available data in formatted string form
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public override string ToString()
    {
        System.Text.StringBuilder SB = new System.Text.StringBuilder();

        SB.Append("Image:");
        SB.Append("\\n\\tDimensions:        " + this.Width + " x " + this.Height + " px");
        SB.Append("\\n\\tResolution:        " + this.ResolutionX + " x " + this.ResolutionY + " dpi");
        SB.Append("\\n\\tOrientation:       " + Enum.GetName(typeof(Orientations), this.Orientation));
        SB.Append("\\n\\tTitle:             " + this.Title);
        SB.Append("\\n\\tDescription:       " + this.Description);
        SB.Append("\\n\\tCopyright:         " + this.Copyright);
        SB.Append("\\nEquipment:");
        SB.Append("\\n\\tMaker:             " + this.EquipmentMaker);
        SB.Append("\\n\\tModel:             " + this.EquipmentModel);
        SB.Append("\\n\\tSoftware:          " + this.Software);
        SB.Append("\\nDate and time:");
        SB.Append("\\n\\tGeneral:           " + this.DateTimeLastModified.ToString());
        SB.Append("\\n\\tOriginal:          " + this.DateTimeOriginal.ToString());
        SB.Append("\\n\\tDigitized:         " + this.DateTimeDigitized.ToString());
        SB.Append("\\nShooting conditions:");
        SB.Append("\\n\\tExposure time:     " + this.ExposureTime.ToString("N4") + " s");
        SB.Append("\\n\\tExposure program:  " + Enum.GetName(typeof(ExposurePrograms), this.ExposureProgram));
        SB.Append("\\n\\tExposure mode:     " + Enum.GetName(typeof(ExposureMeteringModes), this.ExposureMeteringMode));
        SB.Append("\\n\\tAperture:          F" + this.Aperture.ToString("N2"));
        SB.Append("\\n\\tISO sensitivity:   " + this.ISO);
        SB.Append("\\n\\tSubject distance:  " + this.SubjectDistance.ToString("N2") + " m");
        SB.Append("\\n\\tFocal length:      " + this.FocalLength);
        SB.Append("\\n\\tFlash:             " + Enum.GetName(typeof(FlashModes), this.FlashMode));
        SB.Append("\\n\\tLight source (WB): " + Enum.GetName(typeof(LightSources), this.LightSource));

        SB.Replace("\\n", System.Environment.NewLine);
        //SB.Replace("\\t", System.Environment.Tab);
        return SB.ToString();
    }

    #region Nicely formatted well-known properties

    /// <summary>
    /// Brand of equipment (EXIF EquipMake)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string EquipmentMaker
    {
        get { return this.GetPropertyString((int)TagNames.EquipMake); }
    }

    /// <summary>
    /// Model of equipment (EXIF EquipModel)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string EquipmentModel
    {
        get { return this.GetPropertyString((int)TagNames.EquipModel); }
    }

    /// <summary>
    /// Software used for processing (EXIF Software)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string Software
    {
        get { return this.GetPropertyString((int)TagNames.SoftwareUsed); }
    }

    /// <summary>
    /// Orientation of image (position of row 0, column 0) (EXIF Orientation)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public Orientations Orientation
    {
        get
        {
            Int32 X = this.GetPropertyInt16((int)TagNames.Orientation);

            if (!Enum.IsDefined(typeof(Orientations), X))
            {
                return Orientations.TopLeft;
            }
            else
            {
                return (Orientations)Enum.Parse(typeof(Orientations), Enum.GetName(typeof(Orientations), X));
            }
        }
    }

    /// <summary>
    /// Time when image was last modified (EXIF DateTime).
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public DateTime DateTimeLastModified
    {
        get
        {
            try
            {
                return DateTime.ParseExact(this.GetPropertyString((int)TagNames.DateTime), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.DateTime, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
            }
            catch (Exception)
            {
            }
        }
    }

    /// <summary>
    /// Time when image was taken (EXIF DateTimeOriginal).
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public DateTime DateTimeOriginal
    {
        get
        {
            try
            {
                return DateTime.ParseExact(this.GetPropertyString((int)TagNames.ExifDTOrig), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.ExifDTOrig, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
            }
            catch { }
        }
    }

    /// <summary>
    /// Time when image was digitized (EXIF DateTimeDigitized).
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public DateTime DateTimeDigitized
    {
        get
        {
            try
            {
                return DateTime.ParseExact(this.GetPropertyString((int)TagNames.ExifDTDigitized), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.ExifDTDigitized, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
            }
            catch { }
        }
    }

    /// <summary>
    /// Image width
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 04.09.2005 Changed output to Int32, load from image instead of EXIF
    /// </history>
    public Int32 Width
    {
        get { return this._Image.Width; }
    }

    /// <summary>
    /// Image height
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 04.09.2005 Changed output to Int32, load from image instead of EXIF
    /// </history>
    public Int32 Height
    {
        get { return this._Image.Height; }
    }

    /// <summary>
    /// X resolution in dpi (EXIF XResolution/ResolutionUnit)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double ResolutionX
    {
        get
        {
            double R = this.GetPropertyRational((int)TagNames.XResolution).ToDouble();

            if (this.GetPropertyInt16((int)TagNames.ResolutionUnit) == 3)
            {
                //-- resolution is in points/cm
                return R * 2.54;
            }
            else
            {
                //-- resolution is in points/inch
                return R;
            }
        }
    }

    /// <summary>
    /// Y resolution in dpi (EXIF YResolution/ResolutionUnit)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double ResolutionY
    {
        get
        {
            double R = this.GetPropertyRational((int)TagNames.YResolution).ToDouble();

            if (this.GetPropertyInt16((int)TagNames.ResolutionUnit) == 3)
            {
                //-- resolution is in points/cm
                return R * 2.54;
            }
            else
            {
                //-- resolution is in points/inch
                return R;
            }
        }
    }

    /// <summary>
    /// Image title (EXIF ImageTitle)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string Title
    {
        get { return this.GetPropertyString((int)TagNames.ImageTitle); }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.ImageTitle, value);
            }
            catch { }
        }
    }

    /// <summary>
    /// User comment (EXIF UserComment)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 13.06.2004 Created
    /// </history>
    public string UserComment
    {
        get { return this.GetPropertyString((int)TagNames.ExifUserComment); }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.ExifUserComment, value);
            }
            catch { }
        }
    }

    /// <summary>
    /// Artist name (EXIF Artist)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 13.06.2004 Created
    /// </history>
    public string Artist
    {
        get { return this.GetPropertyString((int)TagNames.Artist); }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.Artist, value);
            }
            catch { }
        }
    }

    /// <summary>
    /// Image description (EXIF ImageDescription)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string Description
    {
        get { return this.GetPropertyString((int)TagNames.ImageDescription); }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.ImageDescription, value);
            }
            catch { }
        }
    }

    /// <summary>
    /// Image copyright (EXIF Copyright)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string Copyright
    {
        get { return this.GetPropertyString((int)TagNames.Copyright); }
        set
        {
            try
            {
                this.SetPropertyString((int)TagNames.Copyright, value.ToString());
            }
            catch { }
        }
    }

    /// <summary>
    /// Exposure time in seconds (EXIF ExifExposureTime/ExifShutterSpeed)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double ExposureTime
    {
        get
        {
            if (this.IsPropertyDefined((int)TagNames.ExifExposureTime))
            {
                //-- Exposure time is explicitly specified
                return this.GetPropertyRational((int)TagNames.ExifExposureTime).ToDouble();
            }
            else if (this.IsPropertyDefined((int)TagNames.ExifShutterSpeed))
            {
                //-- Compute exposure time from shutter speed
                return 1 / (Math.Pow(2, this.GetPropertyRational((int)TagNames.ExifShutterSpeed).ToDouble()));
            }
            else
            {
                //-- Can't figure out 
                return 0;
            }
        }
    }

    /// <summary>
    /// Aperture value as F number (EXIF ExifFNumber/ExifApertureValue)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double Aperture
    {
        get
        {
            if (this.IsPropertyDefined((int)TagNames.ExifFNumber))
            {
                return this.GetPropertyRational((int)TagNames.ExifFNumber).ToDouble();
            }
            else if (this.IsPropertyDefined((int)TagNames.ExifAperture))
            {
                return Math.Pow(System.Math.Sqrt(2), this.GetPropertyRational((int)TagNames.ExifAperture).ToDouble());
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Exposure program used (EXIF ExifExposureProg)
    /// </summary>
    /// <value></value>
    /// <remarks>If not specified, returns Normal (2)</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public ExposurePrograms ExposureProgram
    {
        get
        {
            Int32 X = this.GetPropertyInt16((int)TagNames.ExifExposureProg);

            if (Enum.IsDefined(typeof(ExposurePrograms), X))
            {
                return (ExposurePrograms)Enum.Parse(typeof(ExposurePrograms), Enum.GetName(typeof(ExposurePrograms), X));
            }
            else
            {
                return ExposurePrograms.Normal;
            }
        }
    }

    /// <summary>
    /// ISO sensitivity
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public Int16 ISO
    {
        get { return this.GetPropertyInt16((int)TagNames.ExifISOSpeed); }
    }

    /// <summary>
    /// Subject distance in meters (EXIF SubjectDistance)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double SubjectDistance
    {
        get { return this.GetPropertyRational((int)TagNames.ExifSubjectDist).ToDouble(); }
    }

    /// <summary>
    /// Exposure method metering mode used (EXIF MeteringMode)
    /// </summary>
    /// <value></value>
    /// <remarks>If not specified, returns Unknown (0)</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public ExposureMeteringModes ExposureMeteringMode
    {
        get
        {
            Int32 X = this.GetPropertyInt16((int)TagNames.ExifMeteringMode);

            if (Enum.IsDefined(typeof(ExposureMeteringModes), X))
            {
                return (ExposureMeteringModes)Enum.Parse(typeof(ExposureMeteringModes), Enum.GetName(typeof(ExposureMeteringModes), X));
            }
            else
            {
                return ExposureMeteringModes.Unknown;
            }
        }
    }

    /// <summary>
    /// Focal length of lenses in mm (EXIF FocalLength)
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public double FocalLength
    {
        get { return this.GetPropertyRational((int)TagNames.ExifFocalLength).ToDouble(); }
    }

    /// <summary>
    /// Flash mode (EXIF Flash)
    /// </summary>
    /// <value></value>
    /// <remarks>If not present, value NotFired (0) is returned</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public FlashModes FlashMode
    {
        get
        {
            Int32 X = this.GetPropertyInt16((int)TagNames.ExifFlash);

            if (Enum.IsDefined(typeof(FlashModes), X))
            {
                return (FlashModes)Enum.Parse(typeof(FlashModes), Enum.GetName(typeof(FlashModes), X));
            }
            else
            {
                return FlashModes.NotFired;
            }
        }
    }

    /// <summary>
    /// Light source / white balance (EXIF LightSource)
    /// </summary>
    /// <value></value>
    /// <remarks>If not specified, returns Unknown (0).</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public LightSources LightSource
    {
        get
        {
            Int32 X = this.GetPropertyInt16((int)TagNames.ExifLightSource);

            if (Enum.IsDefined(typeof(LightSources), X))
            {
                return (LightSources)Enum.Parse(typeof(LightSources), Enum.GetName(typeof(LightSources), X));
            }
            else
            {
                return LightSources.Unknown;
            }
        }
    }

    #endregion

    #region Support methods for working with EXIF properties

    /// <summary>
    /// Checks if current image has specified certain property
    /// </summary>
    /// <param name="PropertyID"></param>
    /// <returns>True if image has specified property, False otherwise.</returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public bool IsPropertyDefined(Int32 PID)
    {
        return System.Convert.ToBoolean(Array.IndexOf(this._Image.PropertyIdList, PID) > -1);
    }

    /// <summary>
    /// Gets specified Int32 property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="DefaultValue">Optional, default 0. Default value returned if property is not present.</param>
    /// <remarks>Value of property or DefaultValue if property is not present.</remarks>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public Int32 GetPropertyInt32(Int32 PID, Int32 DefaultValue = 0)
    {
        if (this.IsPropertyDefined(PID))
        {
            return GetInt32(this._Image.GetPropertyItem(PID).Value);
        }
        else
        {
            return DefaultValue;
        }
    }

    /// <summary>
    /// Gets specified Int16 property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="DefaultValue">Optional, default 0. Default value returned if property is not present.</param>
    /// <remarks>Value of property or DefaultValue if property is not present.</remarks>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public Int16 GetPropertyInt16(Int32 PID, Int16 DefaultValue = 0)
    {
        if (this.IsPropertyDefined(PID))
        {
            return GetInt16(this._Image.GetPropertyItem(PID).Value);
        }
        else
        {
            return DefaultValue;
        }
    }

    /// <summary>
    /// Gets specified string property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="DefaultValue">Optional, default String.Empty. Default value returned if property is not present.</param>
    /// <returns></returns>
    /// <remarks>Value of property or DefaultValue if property is not present.</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public string GetPropertyString(Int32 PID, string DefaultValue = "")
    {
        if (this.IsPropertyDefined(PID))
        {
            return GetString(this._Image.GetPropertyItem(PID).Value);
        }
        else
        {
            return DefaultValue;
        }
    }

    /// <summary>
    /// Gets specified property in raw form
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="DefaultValue">Optional, default Nothing. Default value returned if property is not present.</param>
    /// <returns></returns>
    /// <remarks>Is recommended to use typed methods (like <see cref="GetPropertyString" /> etc.) instead, when possible.</remarks>
    /// <history>
    /// [altair] 05.09.2005 Created
    /// </history>
    public byte[] GetProperty(Int32 PID, byte[] DefaultValue = null)
    {
        if (this.IsPropertyDefined(PID))
        {
            return this._Image.GetPropertyItem(PID).Value;
        }
        else
        {
            return DefaultValue;
        }
    }

    /// <summary>
    /// Gets specified rational property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <returns></returns>
    /// <remarks>Value of property or 0/1 if not present.</remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public Rational GetPropertyRational(Int32 PID)
    {
        if (this.IsPropertyDefined(PID))
        {
            return GetRational(this._Image.GetPropertyItem(PID).Value);
        }
        else
        {
            Rational R = default(Rational);
            R.Numerator = 0;
            R.Denominator = 1;
            return R;
        }
    }

    /// <summary>
    /// Sets specified string property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="Value">Value to be set</param>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 12.6.2004 Created
    /// </history>
    public void SetPropertyString(Int32 PID, string Value)
    {
        byte[] Data = this._Encoding.GetBytes(Value + "\0");
        SetProperty(PID, Data, ExifDataTypes.AsciiString);
    }

    /// <summary>
    /// Sets specified Int16 property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="Value">Value to be set</param>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 12.6.2004 Created
    /// </history>
    public void SetPropertyInt16(Int32 PID, Int16 Value)
    {
        byte[] Data = new byte[2];
        Data[0] = System.Convert.ToByte(Value & 0xff);
        Data[1] = System.Convert.ToByte((Value & 0xff00) >> 8);
        SetProperty(PID, Data, ExifDataTypes.SignedShort);
    }

    /// <summary>
    /// Sets specified Int32 property
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="Value">Value to be set</param>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 13.06.2004 Created
    /// </history>
    public void SetPropertyInt32(Int32 PID, Int32 Value)
    {
        byte[] Data = new byte[4];
        for (Int32 I = 0; I <= 3; I++)
        {
            Data[I] = System.Convert.ToByte(Value & 0xff);
            Value >>= 8;
        }
        SetProperty(PID, Data, ExifDataTypes.SignedLong);
    }

    /// <summary>
    /// Sets specified propery in raw form
    /// </summary>
    /// <param name="PID">Property ID</param>
    /// <param name="Data">Raw data</param>
    /// <param name="Type">EXIF data type</param>
    /// <remarks>Is recommended to use typed methods (like <see cref="SetPropertyString" /> etc.) instead, when possible.</remarks>
    /// <history>
    /// [altair] 12.6.2004 Created
    /// </history>
    public void SetProperty(Int32 PID, byte[] Data, ExifDataTypes Type)
    {
        System.Drawing.Imaging.PropertyItem P = this._Image.PropertyItems[0];
        P.Id = PID;
        P.Value = Data;
        P.Type = (short)Type;
        P.Len = Data.Length;
        this._Image.SetPropertyItem(P);
    }

    /// <summary>
    /// Reads Int32 from EXIF bytearray.
    /// </summary>
    /// <param name="B">EXIF bytearray to process</param>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 05.09.2005 Changed from public shared to private instance method
    /// </history>
    private Int32 GetInt32(byte[] B)
    {
        if (B.Length < 4)
            throw new ArgumentException("Data too short (4 bytes expected)", "B");
        return B[3] << 24 | B[2] << 16 | B[1] << 8 | B[0];
    }

    /// <summary>
    /// Reads Int16 from EXIF bytearray.
    /// </summary>
    /// <param name="B">EXIF bytearray to process</param>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 05.09.2005 Changed from public shared to private instance method
    /// </history>
    private Int16 GetInt16(byte[] B)
    {
        if (B.Length < 2)
            throw new ArgumentException("Data too short (2 bytes expected)", "B");
        return System.Convert.ToInt16(B[1] << 8 | B[0]);
    }

    /// <summary>
    /// Reads string from EXIF bytearray.
    /// </summary>
    /// <param name="B">EXIF bytearray to process</param>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 05.09.2005 Changed from public shared to private instance method
    /// </history>
    private string GetString(byte[] B)
    {
        string R = this._Encoding.GetString(B);
        if (R.EndsWith("\0"))
            R = R.Substring(0, R.Length - 1);
        return R;
    }

    /// <summary>
    /// Reads rational from EXIF bytearray.
    /// </summary>
    /// <param name="B">EXIF bytearray to process</param>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// [altair] 05.09.2005 Changed from public shared to private instance method
    /// </history>
    private Rational GetRational(byte[] B)
    {
        Rational R = new Rational();
        byte[] N = new byte[4];
        byte[] D = new byte[4];
        Array.Copy(B, 0, N, 0, 4);
        Array.Copy(B, 4, D, 0, 4);
        R.Denominator = this.GetInt32(D);
        R.Numerator = this.GetInt32(N);
        return R;
    }

    #endregion

    #region IDisposable implementation

    /// <summary>
    /// Disposes unmanaged resources of this class
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// [altair] 10.09.2003 Created
    /// </history>
    public void Dispose()
    {
        this._Image.Dispose();
    }

    #endregion

    #endregion
}