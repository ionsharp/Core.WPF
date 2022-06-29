using Imagin.Core.Colors;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using System;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Linq;

public static partial class XColor
{
    public static readonly ResourceKey ComponentToolTipTemplateKey = new();

    public static readonly ResourceKey ModelToolTipTemplateKey = new();

    public static readonly ResourceKey ToolTipTemplateKey = new();

    #region System.Drawing

    public static void Convert(this System.Drawing.Color color, out Color result) => result = Color.FromArgb(color.A, color.R, color.G, color.B);

    #endregion

    #region System.Windows.Media

    #region A

    public static Color A(this Color color, byte a) 
        => Color.FromArgb(a, color.R, color.G, color.B);

    public static Color A(this Color color, Func<byte, byte> a) 
        => Color.FromArgb(a(color.A), color.R, color.G, color.B);

    #endregion

    #region R

    public static Color R(this Color color, byte r)
        => Color.FromArgb(color.A, r, color.G, color.B);

    public static Color R(this Color color, Func<byte, byte> r)
        => Color.FromArgb(color.A, r(color.R), color.G, color.B);

    #endregion

    #region G

    public static Color G(this Color color, byte g) 
        => Color.FromArgb(color.A, color.R, g, color.B);

    public static Color G(this Color color, Func<byte, byte> g)
        => Color.FromArgb(color.A, color.R, g(color.G), color.B);

    #endregion

    #region B

    public static Color B(this Color color, byte b) 
        => Color.FromArgb(color.A, color.R, color.G, b);

    public static Color B(this Color color, Func<byte, byte> b)
        => Color.FromArgb(color.A, color.R, color.G, b(color.B));

    #endregion

    #region Blend

    static double BlendColorBurnf(double a, double b)
    => ((b == 0.0) ? b : Math.Max((1.0 - ((1.0 - a) / b)), 0.0));

    static double BlendColorDodgef(double a, double b)
        => ((b == 1.0) ? b : Math.Min(a / (1.0 - b), 1.0));

    static double BlendHardMixf(double a, double b)
        => BlendVividLightf(a, b) < 0.5 ? 0 : 1;

    static double BlendReflectf(double a, double b)
        => (b == 1.0) ? b : Math.Min(a * a / (1.0 - b), 1.0);

    static double BlendSoftLightf(double a, double b)
        => ((b < 0.5) ? (2.0 * a * b + a * a * (1.0 - 2.0 * b)) : (Math.Sqrt(a) * (2.0 * b - 1.0) + 2.0 * a * (1.0 - b)));

    static double BlendVividLightf(double a, double b)
        => b < 0.5 ? BlendColorBurnf(a, 2.0 * b) : BlendColorDodgef(a, 2.0 * (b - 0.5));

    //...

    public static Color Blend(this Color a, Color b, BlendModes blendMode = BlendModes.Normal, double amount = 1)
    {
        b = b.A((b.A.Double() / 255d * amount * 255d).Byte());

        double a1 = Normalize(a.A), r1 = Normalize(a.R), g1 = Normalize(a.G), b1 = Normalize(a.B),
            a2 = Normalize(b.A), r2 = Normalize(b.R), g2 = Normalize(b.G), b2 = Normalize(b.B);

        double a3 = 0, r3 = 0, g3 = 0, b3 = 0;

        HSB hsb1 = null, hsb2 = null;
        RGB rgb = null;

        a3 = a1;
        switch (blendMode)
        {
            #region Average
            case BlendModes.Average:
                a3 = (a1 + a2) / 2;
                r3 = (r1 + r2) / 2;
                g3 = (g1 + g2) / 2;
                b3 = (b1 + b2) / 2;
                break;
            #endregion
            #region Color
            case BlendModes.Color:
                hsb1 = new HSB();
                hsb1.From(Colour.New<RGB>(r1 * 255, g1 * 255, b1 * 255), WorkingProfile.Default);

                hsb2 = new HSB();
                hsb2.From(Colour.New<RGB>(r2 * 255, g2 * 255, b2 * 255), WorkingProfile.Default);

                Colour.New<HSB>(hsb2.X, hsb1.Y, hsb1.Z).To(out rgb, WorkingProfile.Default);
                r3 = rgb.X; g3 = rgb.Y; b3 = rgb.Z;
                break;
            #endregion
            #region ColorBurn
            case BlendModes.ColorBurn:
                //R = 1 - (1 - a) / b
                r3 = Clamp(1 - (1 - r1) / r2, 1);
                g3 = Clamp(1 - (1 - g1) / g2, 1);
                b3 = Clamp(1 - (1 - b1) / b2, 1);
                break;
            #endregion
            #region ColorDodge
            case BlendModes.ColorDodge:
                //R = a / (1 - b)
                r3 = BlendColorDodgef(r1, r2);
                g3 = BlendColorDodgef(g1, g2);
                b3 = BlendColorDodgef(b1, b2);
                break;
            #endregion
            #region Darken
            case BlendModes.Darken:
                r3 = r1 < r2 ? r1 : r2;
                g3 = g1 < g2 ? g1 : g2;
                b3 = b1 < b2 ? b1 : b2;
                break;
            #endregion
            #region Difference
            case BlendModes.Difference:
                r3 = (r1 - r2).Abs();
                g3 = (g1 - g2).Abs();
                b3 = (b1 - b2).Abs();
                break;
            #endregion
            #region Exclusion
            case BlendModes.Exclusion:
                r3 = r1 + r2 - (2 * r1 * r2);
                g3 = g1 + g2 - (2 * g1 * g2);
                b3 = b1 + b2 - (2 * b1 * b2);
                break;
            #endregion
            #region Glow
            case BlendModes.Glow:
                r3 = BlendReflectf(r2, r1);
                g3 = BlendReflectf(g2, g1);
                b3 = BlendReflectf(b2, b1);
                break;
            #endregion
            #region HardLight
            case BlendModes.HardLight:
                r3 = r1 < 0.5 ? r1 * r2 : (r1 <= 1 || r2 <= 1 ? r1 + r2 - (r1 * r2) : Math.Max(r1, r2));
                g3 = g1 < 0.5 ? g1 * g2 : (g1 <= 1 || g2 <= 1 ? g1 + g2 - (g1 * g2) : Math.Max(g1, g2));
                b3 = b1 < 0.5 ? b1 * b2 : (b1 <= 1 || b2 <= 1 ? b1 + b2 - (b1 * b2) : Math.Max(b1, b2));
                break;
            #endregion
            #region HardMix
            case BlendModes.HardMix:
                r3 = BlendHardMixf(r1, r2);
                g3 = BlendHardMixf(g1, g2);
                b3 = BlendHardMixf(b1, b2);
                break;
            #endregion
            #region Hue
            case BlendModes.Hue:
                hsb1 = new HSB();
                hsb1.From(Colour.New<RGB>(r1 * 255, g1 * 255, b1 * 255), WorkingProfile.Default);

                hsb2 = new HSB();
                hsb2.From(Colour.New<RGB>(r2 * 255, g2 * 255, b2 * 255), WorkingProfile.Default);

                Colour.New<HSB>(hsb2.X, hsb1.Y, hsb1.Z).To(out rgb, WorkingProfile.Default);
                r3 = rgb.X; g3 = rgb.Y; b3 = rgb.Z;
                break;
            #endregion
            #region Lighten
            case BlendModes.Lighten:
                r3 = r1 > r2 ? r1 : r2;
                g3 = g1 > g2 ? g1 : g2;
                b3 = b1 > b2 ? b1 : b2;
                break;
            #endregion
            #region LinearBurn
            case BlendModes.LinearBurn:
                //R = a + b - 1
                r3 = Clamp(r1 + r2 - 1, 1);
                g3 = Clamp(g1 + g2 - 1, 1);
                b3 = Clamp(b1 + b2 - 1, 1);
                break;
            #endregion
            #region LinearDodge
            case BlendModes.LinearDodge:
                r3 = r1 + r2;
                g3 = g1 + g2;
                b3 = b1 + b2;
                break;
            #endregion
            #region LinearLight
            case BlendModes.LinearLight:
                //if (Blend > ½) R = Base + 2×(Blend-½); if (Blend <= ½) R = Base + 2×Blend - 1
                r3 = r2 > 0.5 ? r1 + 2 * (r2 - 0.5) : r1 + 2 * r2 - 1;
                g3 = g2 > 0.5 ? g1 + 2 * (g2 - 0.5) : g1 + 2 * g2 - 1;
                b3 = b2 > 0.5 ? b1 + 2 * (b2 - 0.5) : b1 + 2 * b2 - 1;
                break;
            #endregion
            #region Luminosity
            case BlendModes.Luminosity:
                hsb1 = new HSB();
                hsb1.From(Colour.New<RGB>(r1 * 255, g1 * 255, b1 * 255), WorkingProfile.Default);

                hsb2 = new HSB();
                hsb2.From(Colour.New<RGB>(r2 * 255, g2 * 255, b2 * 255), WorkingProfile.Default);

                Colour.New<HSB>(hsb2.X, hsb1.Y, hsb1.Z).To(out rgb, WorkingProfile.Default);
                r3 = rgb.X; g3 = rgb.Y; b3 = rgb.Z;
                break;
            #endregion
            #region Multiply
            case BlendModes.Multiply:
                r3 = r1 * r2;
                g3 = g1 * g2;
                b3 = b1 * b2;
                break;
            #endregion
            #region Negation
            case BlendModes.Negation:
                r3 = 1 - Math.Abs(1 - r1 - r2);
                g3 = 1 - Math.Abs(1 - g1 - g2);
                b3 = 1 - Math.Abs(1 - b1 - b2);
                break;
            #endregion
            #region Normal
            case BlendModes.Normal:
                a3 = 1.0 - (1.0 - a2) * (1.0 - a1);
                r3 = r2 * a2 / a3 + r1 * a1 * (1.0 - a2) / a3;
                g3 = g2 * a2 / a3 + g1 * a1 * (1.0 - a2) / a3;
                b3 = b2 * a2 / a3 + b1 * a1 * (1.0 - a2) / a3;

                a3 = double.IsNaN(a3) ? 0 : a3;
                r3 = double.IsNaN(r3) ? 0 : r3;
                g3 = double.IsNaN(g3) ? 0 : g3;
                b3 = double.IsNaN(b3) ? 0 : b3;
                break;
            #endregion
            #region Overlay
            case BlendModes.Overlay:
                //if (Base > ½) R = 1 - (1-2×(Base-½)) × (1-Blend); if (Base <= ½) R = (2×Base) × Blend
                r3 = r1 > 0.5 ? 1 - (1 - 2 * (r1 - 0.5)) * (1 - r2) : (2 * r1) * r2;
                g3 = g1 > 0.5 ? 1 - (1 - 2 * (g1 - 0.5)) * (1 - g2) : (2 * g1) * g2;
                b3 = b1 > 0.5 ? 1 - (1 - 2 * (b1 - 0.5)) * (1 - b2) : (2 * b1) * b2;
                break;
            #endregion
            #region Phoenix
            case BlendModes.Phoenix:
                r3 = r1 - r2;
                g3 = g1 - g2;
                b3 = b1 - b2;
                break;
            #endregion
            #region PinLight
            case BlendModes.PinLight:
                //if (Blend > ½) R = max(Base,2×(Blend-½)); if (Blend <= ½) R = min(Base,2×Blend))
                r3 = r2 > 0.5 ? Math.Max(r1, 2 * (r2 - 0.5)) : Math.Min(r1, 2 * r2);
                g3 = g2 > 0.5 ? Math.Max(g1, 2 * (g2 - 0.5)) : Math.Min(g1, 2 * g2);
                b3 = b2 > 0.5 ? Math.Max(b1, 2 * (b2 - 0.5)) : Math.Min(b1, 2 * b2);
                break;
            #endregion
            #region Reflect
            case BlendModes.Reflect:
                r3 = r1 / (r2 == 0 ? 0.01 : r2);
                g3 = g1 / (g2 == 0 ? 0.01 : g2);
                b3 = b1 / (b2 == 0 ? 0.01 : b2);
                break;
            #endregion
            #region Saturation
            case BlendModes.Saturation:
                hsb1 = new HSB();
                hsb1.From(Colour.New<RGB>(r1 * 255, g1 * 255, b1 * 255), WorkingProfile.Default);

                hsb2 = new HSB();
                hsb2.From(Colour.New<RGB>(r2 * 255, g2 * 255, b2 * 255), WorkingProfile.Default);

                Colour.New<HSB>(hsb2.X, hsb1.Y, hsb1.Z).To(out rgb, WorkingProfile.Default);
                r3 = rgb.X; g3 = rgb.Y; b3 = rgb.Z;
                break;
            #endregion
            #region Screen
            case BlendModes.Screen:
                r3 = r1 <= 1 || r2 <= 1 ? r1 + r2 - (r1 * r2) : Math.Max(r1, r2);
                g3 = g1 <= 1 || g2 <= 1 ? g1 + g2 - (g1 * g2) : Math.Max(g1, g2);
                b3 = b1 <= 1 || b2 <= 1 ? b1 + b2 - (b1 * b2) : Math.Max(b1, b2);
                break;
            #endregion
            #region SoftLight
            case BlendModes.SoftLight:
                r3 = BlendSoftLightf(r1, r2);
                g3 = BlendSoftLightf(g1, g2);
                b3 = BlendSoftLightf(b1, b2);
                break;
            #endregion
            #region VividLight
            case BlendModes.VividLight:
                r3 = BlendVividLightf(r1, r2);
                g3 = BlendVividLightf(g1, g2);
                b3 = BlendVividLightf(b1, b2);
                break;
                #endregion
        }
        return Color.FromArgb(Denormalize(a3), Denormalize(r3), Denormalize(g3), Denormalize(b3));
    }

    #endregion

    #region Convert

    public static void Convert(this Color input, out ByteVector4 result)
        => result = new(input.R, input.G, input.B, input.A);

    public static void Convert(this Color input, out System.Drawing.Color result)
        => result = System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);

    public static void Convert(this Color input, out RGB result)
        => result = Colour.New<RGB>(input.R.Double(), input.G.Double(), input.B.Double());

    public static void Convert<T>(this Color input, out T result, WorkingProfile profile) where T : ColorModel, new()
    {
        input.Convert(out RGB color);
        result = color.To<T>(profile);
    }

    public static void Convert(this Color input, out Vector3 result)
        => result = new(Normalize(input.R), Normalize(input.G), Normalize(input.B));

    public static void Convert(this Color input, out Vector4 result)
        => result = new(Normalize(input.A), Normalize(input.R), Normalize(input.G), Normalize(input.B));

    //...

    public static Color Convert(ByteVector4 input) => Color.FromArgb(input.A, input.R, input.G, input.B);

    public static Color Convert(Vector3 input)
        => Color.FromArgb(255, Denormalize(input.X), Denormalize(input.Y), Denormalize(input.Z));

    public static Color Convert(Vector4 input)
        => Color.FromArgb(Denormalize(input.W), Denormalize(input.X), Denormalize(input.Y), Denormalize(input.Z));

    public static Color Convert(RGB input)
    {
        var result = new Vector(input.Value).Transform(i => Clamp(i, 255).Byte());
        return Color.FromArgb(255, result[0], result[1], result[2]);
    }

    #endregion

    #region Decode/Encode

    public static Color Decode(this int color)
    {
        var a = (byte)(color >> 24);
        //Prevent division by zero
        int ai = a;
        if (ai == 0)
            ai = 1;

        //Scale inverse alpha to use cheap integer mul bit shift
        ai = ((255 << 8) / ai);

        return Color.FromArgb(a,
            (byte)((((color >> 16) & 0xFF) * ai) >> 8),
            (byte)((((color >> 8) & 0xFF) * ai) >> 8),
            (byte)((((color & 0xFF) * ai) >> 8)));
    }

    public static int Encode(this Color color)
    {
        var col = 0;
        if (color.A != 0)
        {
            var a = color.A + 1;
            col = (color.A << 24)
                | ((byte)((color.R * a) >> 8) << 16)
                | ((byte)((color.G * a) >> 8) << 8)
                | ((byte)((color.B * a) >> 8));
        }
        return col;
    }

    public static int Encode(this Color input, System.Windows.Media.PixelFormat format)
    {
        var result = 0;
        if (format == PixelFormats.BlackWhite)
        {

        }
        else if (format == PixelFormats.Cmyk32)
        {

        }
        else if (format == PixelFormats.Rgba128Float)
        {
            result = input.A << 96;
            result |= input.B << 64;
            result |= input.G << 32;
            result |= input.R << 0;
        }
        else if (format == PixelFormats.Rgba64)
        {
            result = input.A << 48;
            result |= input.B << 32;
            result |= input.G << 16;
            result |= input.R << 0;
        }
        else if (format == PixelFormats.Bgra32)
        {
            result = input.A << 24;
            result |= input.R << 16;
            result |= input.G << 8;
            result |= input.B << 0;
        }
        else if (format == PixelFormats.Gray32Float)
        {
        }
        else if (format == PixelFormats.Gray16)
        {
        }
        else if (format == PixelFormats.Gray8)
        {
        }
        return result;
    }

    #endregion

    #region Get(Hue/Saturation/Brightness)

    /// <summary>
    /// Gets distance to a color from given color.
    /// </summary>
    /// <param name="First"></param>
    /// <param name="Second"></param>
    /// <returns></returns>
    public static double GetDistance(this Color First, Color Second)
    {
        return System.Math.Sqrt(System.Math.Pow(First.R - Second.R, 2) + System.Math.Pow(First.G - Second.G, 2) + System.Math.Pow(First.B - Second.B, 2));
    }

    /// <summary>
    /// Gets the hue-saturation-lightness (HSL) hue value, in degrees, for this <see cref="Color"/> structure.
    /// </summary>
    /// <returns>The hue, in degrees, of this <see cref="Color"/>. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSL color space.</returns>
    public static double GetHue(this Color color)
    {
        color.Convert(out System.Drawing.Color result);
        return result.GetHue();
    }

    /// <summary>
    /// Gets the hue-saturation-lightness (HSL) saturation value for this <see cref="Color"/> structure.
    /// </summary>
    /// <returns>The saturation of this <see cref="Color"/>. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.</returns>
    public static double GetSaturation(this Color color)
    {
        color.Convert(out System.Drawing.Color result);
        return result.GetSaturation();
    }

    /// <summary>
    /// Gets the hue-saturation-lightness (HSL) lightness value for this <see cref="Color"/> structure.
    /// </summary>
    /// <returns>The lightness of this <see cref="Color"/>. The lightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.</returns>
    public static double GetBrightness(this Color color)
    {
        color.Convert(out System.Drawing.Color result);
        return result.GetBrightness();
    }

    #endregion

    #endregion
}