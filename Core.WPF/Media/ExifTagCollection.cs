using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Imagin.Core.Media;

public sealed class ExifTagCollection : IEnumerable<ExifTag>
{
    Dictionary<int, ExifTag> tags;

    #region ExifTagCollection

    public ExifTag this[int id] => tags[id];

    public ExifTagCollection(string fileName) : this(fileName, true, false) { }

    public ExifTagCollection(string fileName, bool useEmbeddedColorManagement, bool validateImageData)
    {
        try
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement, validateImageData);
                ReadTags(image.PropertyItems);
            }
        } catch { }
    }

    public ExifTagCollection(System.Drawing.Image image) => ReadTags(image.PropertyItems);

    #endregion

    #region Methods

    static string GetComponentsConfig(byte[] bytes)
    {
        string s = "";
        string[] vals = new string[] { "", "Y", "Cb", "Cr", "R", "G", "B" };

        foreach (byte b in bytes)
            s += vals[b];

        return s;
    }

    void ReadTags(PropertyItem[] pitems)
    {
        Encoding ascii = Encoding.ASCII;
        tags = new Dictionary<int, ExifTag>();

        foreach (DictionaryEntry Entry in ExifHelper.Tags)
        {
            ExifTag TagToAdd = (ExifTag)Entry.Value;
            string value = "";
            foreach (PropertyItem pitem in pitems)
            {
                ExifTag TagToCheck = (ExifTag)ExifHelper.Tags[pitem.Id];
                if (TagToCheck == null) continue;
                if (TagToCheck.Id != TagToAdd.Id) continue;
                if (pitem.Type == 0x1)
                {
                    #region BYTE (8-bit unsigned int)
                    if (pitem.Value.Length == 4)
                        value = "Version " + pitem.Value[0].ToString() + "." + pitem.Value[1].ToString();
                    else if (pitem.Id == 0x5 && pitem.Value[0] == 0)
                        value = "Sea level";
                    else
                        value = pitem.Value[0].ToString();
                    #endregion
                }
                else if (pitem.Type == 0x2)
                {
                    #region ASCII (8 bit ASCII code)

                    value = ascii.GetString(pitem.Value).Trim('\0');

                    if (pitem.Id == 0x1 || pitem.Id == 0x13)
                        if (value == "N") value = "North latitude";
                        else if (value == "S") value = "South latitude";
                        else value = "reserved";

                    if (pitem.Id == 0x3 || pitem.Id == 0x15)
                        if (value == "E") value = "East longitude";
                        else if (value == "W") value = "West longitude";
                        else value = "reserved";

                    if (pitem.Id == 0x9)
                        if (value == "A") value = "Measurement in progress";
                        else if (value == "V") value = "Measurement Interoperability";
                        else value = "reserved";

                    if (pitem.Id == 0xA)
                        if (value == "2") value = "2-dimensional measurement";
                        else if (value == "3") value = "3-dimensional measurement";
                        else value = "reserved";

                    if (pitem.Id == 0xC || pitem.Id == 0x19)
                        if (value == "K") value = "Kilometers per hour";
                        else if (value == "M") value = "Miles per hour";
                        else if (value == "N") value = "Knots";
                        else value = "reserved";

                    if (pitem.Id == 0xE || pitem.Id == 0x10 || pitem.Id == 0x17)
                        if (value == "T") value = "True direction";
                        else if (value == "M") value = "Magnetic direction";
                        else value = "reserved";
                    #endregion
                }
                else if (pitem.Type == 0x3)
                {
                    #region 3 = SHORT (16-bit unsigned int)

                    UInt16 uintval = BitConverter.ToUInt16(pitem.Value, 0);

                    // orientation // lookup table					
                    switch (pitem.Id)
                    {
                        case 0x8827: // ISO speed rating
                            value = "ISO-" + uintval.ToString();
                            break;
                        case 0xA217: // sensing method
                            {
                                switch (uintval)
                                {
                                    case 1: value = "Not defined"; break;
                                    case 2: value = "One-chip color area sensor"; break;
                                    case 3: value = "Two-chip color area sensor"; break;
                                    case 4: value = "Three-chip color area sensor"; break;
                                    case 5: value = "Color sequential area sensor"; break;
                                    case 7: value = "Trilinear sensor"; break;
                                    case 8: value = "Color sequential linear sensor"; break;
                                    default: value = " reserved"; break;
                                }
                            }
                            break;
                        case 0x8822: // Exposure program
                            switch (uintval)
                            {
                                case 0: value = "Not defined"; break;
                                case 1: value = "Manual"; break;
                                case 2: value = "Normal program"; break;
                                case 3: value = "Aperture priority"; break;
                                case 4: value = "Shutter priority"; break;
                                case 5: value = "Creative program (biased toward depth of field)"; break;
                                case 6: value = "Action program (biased toward fast shutter speed)"; break;
                                case 7: value = "Portrait mode (for closeup photos with the background out of focus)"; break;
                                case 8: value = "Landscape mode (for landscape photos with the background in focus)"; break;
                                default: value = "reserved"; break;
                            }
                            break;
                        case 0x9207: // metering mode
                            switch (uintval)
                            {
                                case 0: value = "unknown"; break;
                                case 1: value = "Average"; break;
                                case 2: value = "Center Weighted Average"; break;
                                case 3: value = "Spot"; break;
                                case 4: value = "MultiSpot"; break;
                                case 5: value = "Pattern"; break;
                                case 6: value = "Partial"; break;
                                case 255: value = "Other"; break;
                                default: value = "reserved"; break;
                            }
                            break;
                        case 0x9208: // Light source
                            {
                                switch (uintval)
                                {
                                    case 0: value = "unknown"; break;
                                    case 1: value = "Daylight"; break;
                                    case 2: value = "Fluorescent"; break;
                                    case 3: value = "Tungsten (incandescent light)"; break;
                                    case 4: value = "Flash"; break;
                                    case 9: value = "Fine weather"; break;
                                    case 10: value = "Cloudy weather"; break;
                                    case 11: value = "Shade"; break;
                                    case 12: value = "Daylight fluorescent (D 5700 – 7100K)"; break;
                                    case 13: value = "Day white fluorescent (N 4600 – 5400K)"; break;
                                    case 14: value = "Cool white fluorescent (W 3900 – 4500K)"; break;
                                    case 15: value = "White fluorescent (WW 3200 – 3700K)"; break;
                                    case 17: value = "Standard light A"; break;
                                    case 18: value = "Standard light B"; break;
                                    case 19: value = "Standard light C"; break;
                                    case 20: value = "D55"; break;
                                    case 21: value = "D65"; break;
                                    case 22: value = "D75"; break;
                                    case 23: value = "D50"; break;
                                    case 24: value = "ISO studio tungsten"; break;
                                    case 255: value = "ISO studio tungsten"; break;
                                    default: value = "other light source"; break;
                                }
                            }
                            break;
                        case 0x9209: // Flash
                            {
                                switch (uintval)
                                {
                                    case 0x0: value = "Flash did not fire"; break;
                                    case 0x1: value = "Flash fired"; break;
                                    case 0x5: value = "Strobe return light not detected"; break;
                                    case 0x7: value = "Strobe return light detected"; break;
                                    case 0x9: value = "Flash fired, compulsory flash mode"; break;
                                    case 0xD: value = "Flash fired, compulsory flash mode, return light not detected"; break;
                                    case 0xF: value = "Flash fired, compulsory flash mode, return light detected"; break;
                                    case 0x10: value = "Flash did not fire, compulsory flash mode"; break;
                                    case 0x18: value = "Flash did not fire, auto mode"; break;
                                    case 0x19: value = "Flash fired, auto mode"; break;
                                    case 0x1D: value = "Flash fired, auto mode, return light not detected"; break;
                                    case 0x1F: value = "Flash fired, auto mode, return light detected"; break;
                                    case 0x20: value = "No flash function"; break;
                                    case 0x41: value = "Flash fired, red-eye reduction mode"; break;
                                    case 0x45: value = "Flash fired, red-eye reduction mode, return light not detected"; break;
                                    case 0x47: value = "Flash fired, red-eye reduction mode, return light detected"; break;
                                    case 0x49: value = "Flash fired, compulsory flash mode, red-eye reduction mode"; break;
                                    case 0x4D: value = "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected"; break;
                                    case 0x4F: value = "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected"; break;
                                    case 0x59: value = "Flash fired, auto mode, red-eye reduction mode"; break;
                                    case 0x5D: value = "Flash fired, auto mode, return light not detected, red-eye reduction mode"; break;
                                    case 0x5F: value = "Flash fired, auto mode, return light detected, red-eye reduction mode"; break;
                                    default: value = "reserved"; break;
                                }
                            }
                            break;
                        case 0x0128: //ResolutionUnit
                            {
                                switch (uintval)
                                {
                                    case 2: value = "Inch"; break;
                                    case 3: value = "Centimeter"; break;
                                    default: value = "No Unit"; break;
                                }
                            }
                            break;
                        case 0xA409: // Saturation
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Normal"; break;
                                    case 1: value = "Low saturation"; break;
                                    case 2: value = "High saturation"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;

                        case 0xA40A: // Sharpness
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Normal"; break;
                                    case 1: value = "Soft"; break;
                                    case 2: value = "Hard"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA408: // Contrast
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Normal"; break;
                                    case 1: value = "Soft"; break;
                                    case 2: value = "Hard"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0x103: // Compression
                            {
                                switch (uintval)
                                {
                                    case 1: value = "Uncompressed"; break;
                                    case 6: value = "JPEG compression (thumbnails only)"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0x106: // PhotometricInterpretation
                            {
                                switch (uintval)
                                {
                                    case 2: value = "RGB"; break;
                                    case 6: value = "YCbCr"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0x112: // Orientation
                            {
                                switch (uintval)
                                {
                                    case 1: value = "The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side."; break;
                                    case 2: value = "The 0th row is at the visual top of the image, and the 0th column is the visual right-hand side."; break;
                                    case 3: value = "The 0th row is at the visual bottom of the image, and the 0th column is the visual right-hand side."; break;
                                    case 4: value = "The 0th row is at the visual bottom of the image, and the 0th column is the visual left-hand side."; break;
                                    case 5: value = "The 0th row is the visual left-hand side of the image, and the 0th column is the visual top."; break;
                                    case 6: value = "The 0th row is the visual right-hand side of the image, and the 0th column is the visual top."; break;
                                    case 7: value = "The 0th row is the visual right-hand side of the image, and the 0th column is the visual bottom."; break;
                                    case 8: value = "The 0th row is the visual left-hand side of the image, and the 0th column is the visual bottom."; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0x213: // YCbCrPositioning
                            {
                                switch (uintval)
                                {
                                    case 1: value = "centered"; break;
                                    case 6: value = "co-sited"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA001: // ColorSpace
                            {
                                switch (uintval)
                                {
                                    case 1: value = "sRGB"; break;
                                    case 0xFFFF: value = "Uncalibrated"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA401: // CustomRendered
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Normal process"; break;
                                    case 1: value = "Custom process"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA402: // ExposureMode
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Auto exposure"; break;
                                    case 1: value = "Manual exposure"; break;
                                    case 2: value = "Auto bracket"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA403: // WhiteBalance
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Auto white balance"; break;
                                    case 1: value = "Manual white balance"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA406: // SceneCaptureType
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Standard"; break;
                                    case 1: value = "Landscape"; break;
                                    case 2: value = "Portrait"; break;
                                    case 3: value = "Night scene"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;

                        case 0xA40C: // SubjectDistanceRange
                            {
                                switch (uintval)
                                {
                                    case 0: value = "unknown"; break;
                                    case 1: value = "Macro"; break;
                                    case 2: value = "Close view"; break;
                                    case 3: value = "Distant view"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0x1E: // GPSDifferential
                            {
                                switch (uintval)
                                {
                                    case 0: value = "Measurement without differential correction"; break;
                                    case 1: value = "Differential correction applied"; break;
                                    default: value = "Reserved"; break;
                                }
                            }
                            break;
                        case 0xA405: // FocalLengthIn35mmFilm
                            value = uintval.ToString() + " mm";
                            break;
                        default://
                            value = uintval.ToString();
                            break;
                    }
                    #endregion
                }
                else if (pitem.Type == 0x4)
                {
                    #region 4 = LONG (32-bit unsigned int)
                    value = BitConverter.ToUInt32(pitem.Value, 0).ToString();
                    #endregion
                }
                else if (pitem.Type == 0x5)
                {
                    #region 5 = RATIONAL (Two LONGs, unsigned)

                    ExifHelper.GPSRational rat = new ExifHelper.GPSRational(pitem.Value);

                    switch (pitem.Id)
                    {
                        case 0x9202: // ApertureValue
                            value = "F/" + Math.Round(Math.Pow(Math.Sqrt(2), rat.Double()), 2).ToString();
                            break;
                        case 0x9205: // MaxApertureValue
                            value = "F/" + Math.Round(Math.Pow(Math.Sqrt(2), rat.Double()), 2).ToString();
                            break;
                        case 0x920A: // FocalLength
                            value = rat.Double().ToString() + " mm";
                            break;
                        case 0x829D: // F-number
                            value = "F/" + rat.Double().ToString();
                            break;
                        case 0x11A: // Xresolution
                            value = rat.Double().ToString();
                            break;
                        case 0x11B: // Yresolution
                            value = rat.Double().ToString();
                            break;
                        case 0x829A: // ExposureTime
                            value = rat.ToString() + " sec";
                            break;
                        case 0x2: // GPSLatitude                                
                            value = new ExifHelper.GPSRational(pitem.Value).Decimal().ToString();
                            break;
                        case 0x4: // GPSLongitude
                            value = new ExifHelper.GPSRational(pitem.Value).Decimal().ToString();
                            break;
                        case 0x6: // GPSAltitude
                            value = rat.Double() + " meters";
                            break;
                        case 0xA404: // Digital Zoom Ratio
                            value = rat.Double().ToString();
                            if (value == "0") value = "none";
                            break;
                        case 0xB: // GPSDOP
                            value = rat.Double().ToString();
                            break;
                        case 0xD: // GPSSpeed
                            value = rat.Double().ToString();
                            break;
                        case 0xF: // GPSTrack
                            value = rat.Double().ToString();
                            break;
                        case 0x11: // GPSImgDir
                            value = rat.Double().ToString();
                            break;
                        case 0x14: // GPSDestLatitude
                            value = new ExifHelper.GPSRational(pitem.Value).ToString();
                            break;
                        case 0x16: // GPSDestLongitude
                            value = new ExifHelper.GPSRational(pitem.Value).ToString();
                            break;
                        case 0x18: // GPSDestBearing
                            value = rat.Double().ToString();
                            break;
                        case 0x1A: // GPSDestDistance
                            value = rat.Double().ToString();
                            break;
                        case 0x7: // GPSTimeStamp                                
                            value = new ExifHelper.GPSRational(pitem.Value).ToString(":");
                            break;

                        default:
                            value = rat.ToString();
                            break;
                    }

                    #endregion
                }
                else if (pitem.Type == 0x7)
                {
                    #region UNDEFINED (8-bit)
                    switch (pitem.Id)
                    {
                        case 0xA300: //FileSource
                            {
                                if (pitem.Value[0] == 3)
                                    value = "DSC";
                                else
                                    value = "reserved";
                                break;
                            }
                        case 0xA301: //SceneType
                            if (pitem.Value[0] == 1)
                                value = "A directly photographed image";
                            else
                                value = "reserved";
                            break;
                        case 0x9000:// Exif Version
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            break;
                        case 0xA000: // Flashpix Version
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            if (value == "0100")
                                value = "Flashpix Format Version 1.0";
                            else value = "reserved";
                            break;
                        case 0x9101: //ComponentsConfiguration
                            value = GetComponentsConfig(pitem.Value);
                            break;
                        case 0x927C: //MakerNote
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            break;
                        case 0x9286: //UserComment
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            break;
                        case 0x1B: //GPS Processing Method
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            break;
                        case 0x1C: //GPS Area Info
                            value = ascii.GetString(pitem.Value).Trim('\0');
                            break;
                        default:
                            value = "-";
                            break;
                    }
                    #endregion
                }
                else if (pitem.Type == 0x9)
                {
                    #region 9 = SLONG (32-bit int)
                    value = BitConverter.ToInt32(pitem.Value, 0).ToString();
                    #endregion
                }
                else if (pitem.Type == 0xA)
                {
                    #region 10 = SRATIONAL (Two SLONGs, signed)

                    ExifHelper.GPSRational rat = new ExifHelper.GPSRational(pitem.Value);

                    switch (pitem.Id)
                    {
                        case 0x9201: // ShutterSpeedValue
                            value = "1/" + Math.Round(Math.Pow(2, rat.Double()), 2).ToString();
                            break;
                        case 0x9203: // BrightnessValue
                            value = Math.Round(rat.Double(), 4).ToString();
                            break;
                        case 0x9204: // ExposureBiasValue
                            value = Math.Round(rat.Double(), 2).ToString() + " eV";
                            break;
                        default:
                            value = rat.ToString();
                            break;
                    }
                    #endregion
                }
                if (value.Length > 0)
                {
                    break;
                }
            }
            TagToAdd.Value = value;
            tags.Add(TagToAdd.Id, TagToAdd);
        }
    }

    ///

    IEnumerator IEnumerable.GetEnumerator() => tags.Values.GetEnumerator();
    public IEnumerator<ExifTag> GetEnumerator() => tags.Values.GetEnumerator();

    #endregion
}