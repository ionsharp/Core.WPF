using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Effects;

public class ColorModelEffect : BaseEffect
{
    public override string FilePath => "ColorModelEffect.ps";

    #region (enum) Modes

    public enum Modes
    {
        /// <summary>
        /// Given <see cref="Component4.X"/>, <see cref="Component4.Y"/>, and <see cref="Component4.Z"/>, displays original color with corresponding components adjusted.
        /// </summary>
        XYZ,
        /// <summary>
        /// Given <see cref="Component4.Z"/>, displays <see cref="Component4.X"/> and <see cref="Component4.Y"/> (in any order).
        /// </summary>
        XY,
        /// <summary>
        /// Given <see cref="Component4.X"/> and <see cref="Component4.Y"/>, displays <see cref="Component4.Z"/> (in any order).
        /// </summary>
        Z
    }

    #endregion

    #region Properties

    #region Profile

    public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(nameof(Profile), typeof(WorkingProfile), typeof(ColorModelEffect), new FrameworkPropertyMetadata(WorkingProfile.Default, OnProfileChanged));
    public WorkingProfile Profile
    {
        get => (WorkingProfile)GetValue(ProfileProperty);
        set => SetValue(ProfileProperty, value);
    }
    static void OnProfileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<ColorModelEffect>().OnProfileChanged(e);

    #endregion

    #region (C0) Model

    internal static readonly DependencyProperty ActualModelProperty = DependencyProperty.Register(nameof(ActualModel), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
    internal double ActualModel
    {
        get => (double)GetValue(ActualModelProperty);
        set => SetValue(ActualModelProperty, value);
    }

    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof(Model), typeof(Type), typeof(ColorModelEffect), new FrameworkPropertyMetadata(typeof(HSB)));
    public Type Model
    {
        get => (Type)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    #endregion

    #region (C1) XComponent

    public static readonly DependencyProperty XComponentProperty = DependencyProperty.Register(nameof(XComponent), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(1)));
    public double XComponent
    {
        get => (double)GetValue(XComponentProperty);
        set => SetValue(XComponentProperty, value);
    }

    #endregion

    #region (C2) YComponent

    public static readonly DependencyProperty YComponentProperty = DependencyProperty.Register(nameof(YComponent), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(1.0, PixelShaderConstantCallback(2)));
    public double YComponent
    {
        get => (double)GetValue(YComponentProperty);
        set => SetValue(YComponentProperty, value);
    }

    #endregion

    #region (C3) Mode

    internal static readonly DependencyProperty ActualModeProperty = DependencyProperty.Register(nameof(ActualMode), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata((double)(int)Modes.XY, PixelShaderConstantCallback(3)));
    internal double ActualMode
    {
        get => (double)GetValue(ActualModeProperty);
        set => SetValue(ActualModeProperty, value);
    }

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(Modes), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Modes.XY));
    public Modes Mode
    {
        get => (Modes)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    #endregion

    #region (C4) Shape

    internal static readonly DependencyProperty ActualShapeProperty = DependencyProperty.Register(nameof(ActualShape), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata((double)(int)Shapes2.Square, PixelShaderConstantCallback(4)));
    internal double ActualShape
    {
        get => (double)GetValue(ActualShapeProperty);
        set => SetValue(ActualShapeProperty, value);
    }

    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(Shape), typeof(Shapes2), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Shapes2.Square));
    public Shapes2 Shape
    {
        get => (Shapes2)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    #endregion

    #region (C5) Depth

    public static readonly DependencyProperty DepthProperty = DependencyProperty.Register(nameof(Depth), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(double), PixelShaderConstantCallback(5)));
    public double Depth
    {
        get => (double)GetValue(DepthProperty);
        set => SetValue(DepthProperty, value);
    }

    #endregion

    #region (C6-9) X|Y|Z|W

    //(C6)

    public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(6)));
    public double X
    {
        get => (double)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    //(C7)

    public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(7)));
    public double Y
    {
        get => (double)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    //(C8)

    public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(8)));
    public double Z
    {
        get => (double)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    //(C9)

    public static readonly DependencyProperty WProperty = DependencyProperty.Register(nameof(W), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(9)));
    public double W
    {
        get => (double)GetValue(WProperty);
        set => SetValue(WProperty, value);
    }

    #endregion

    #region (C10) Companding

    public static readonly DependencyProperty CompandingProperty = DependencyProperty.Register(nameof(Companding), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(4.0, PixelShaderConstantCallback(10)));
    /// <summary>The default is <see cref="WorkingProfile.Default"/>.</summary>
    public double Companding
    {
        get => (double)GetValue(CompandingProperty);
        set => SetValue(CompandingProperty, value);
    }

    #endregion

    #region (C11) Gamma

    public static readonly DependencyProperty GammaProperty = DependencyProperty.Register(nameof(Gamma), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(double), PixelShaderConstantCallback(11)));
    public double Gamma
    {
        get => (double)GetValue(GammaProperty);
        set => SetValue(GammaProperty, value);
    }

    #endregion

    #region (C12-13) White(X|Y)

    //(C12)

    public static readonly DependencyProperty WhiteXProperty = DependencyProperty.Register(nameof(WhiteX), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Illuminant2.D65.X, PixelShaderConstantCallback(12)));
    /// <summary>The default is <see cref="Illuminant2.D65"/>.</summary>
    public double WhiteX
    {
        get => (double)GetValue(WhiteXProperty);
        set => SetValue(WhiteXProperty, value);
    }

    //(C13)

    public static readonly DependencyProperty WhiteYProperty = DependencyProperty.Register(nameof(WhiteY), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Illuminant2.D65.Y, PixelShaderConstantCallback(13)));
    /// <summary>The default is <see cref="Illuminant2.D65"/>.</summary>
    public double WhiteY
    {
        get => (double)GetValue(WhiteYProperty);
        set => SetValue(WhiteYProperty, value);
    }

    #endregion

    //...

    #region (C14-16) LMS_XYZ_(x|y|z)

    //(C14)

    public static readonly DependencyProperty LMS_XYZ_xProperty = DependencyProperty.Register(nameof(LMS_XYZ_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(14)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_xProperty);
        set => SetValue(LMS_XYZ_xProperty, value);
    }

    //(C15)

    public static readonly DependencyProperty LMS_XYZ_yProperty = DependencyProperty.Register(nameof(LMS_XYZ_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(15)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_yProperty);
        set => SetValue(LMS_XYZ_yProperty, value);
    }

    //(C16)

    public static readonly DependencyProperty LMS_XYZ_zProperty = DependencyProperty.Register(nameof(LMS_XYZ_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(16)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_zProperty);
        set => SetValue(LMS_XYZ_zProperty, value);
    }

    #endregion

    #region (C17-19) RGB_XYZ_(x|y|z)

    //(C17)

    public static readonly DependencyProperty RGB_XYZ_xProperty = DependencyProperty.Register(nameof(RGB_XYZ_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(17)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_xProperty);
        set => SetValue(RGB_XYZ_xProperty, value);
    }

    //(C18)

    public static readonly DependencyProperty RGB_XYZ_yProperty = DependencyProperty.Register(nameof(RGB_XYZ_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(18)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_yProperty);
        set => SetValue(RGB_XYZ_yProperty, value);
    }

    //(C19)

    public static readonly DependencyProperty RGB_XYZ_zProperty = DependencyProperty.Register(nameof(RGB_XYZ_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(19)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_zProperty);
        set => SetValue(RGB_XYZ_zProperty, value);
    }

    #endregion

    #region (C20-22) XYZ_LMS_(x|y|z)

    //(C20)

    public static readonly DependencyProperty XYZ_LMS_xProperty = DependencyProperty.Register(nameof(XYZ_LMS_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(20)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_xProperty);
        set => SetValue(XYZ_LMS_xProperty, value);
    }

    //(C21)

    public static readonly DependencyProperty XYZ_LMS_yProperty = DependencyProperty.Register(nameof(XYZ_LMS_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(21)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_yProperty);
        set => SetValue(XYZ_LMS_yProperty, value);
    }

    //(C22)

    public static readonly DependencyProperty XYZ_LMS_zProperty = DependencyProperty.Register(nameof(XYZ_LMS_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(22)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_zProperty);
        set => SetValue(XYZ_LMS_zProperty, value);
    }

    #endregion

    #region (C23-25) XYZ_RGB_(x|y|z)

    //(C23)

    public static readonly DependencyProperty XYZ_RGB_xProperty = DependencyProperty.Register(nameof(XYZ_RGB_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(23)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_xProperty);
        set => SetValue(XYZ_RGB_xProperty, value);
    }

    //(C24)

    public static readonly DependencyProperty XYZ_RGB_yProperty = DependencyProperty.Register(nameof(XYZ_RGB_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(24)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_yProperty);
        set => SetValue(XYZ_RGB_yProperty, value);
    }

    //(C25)

    public static readonly DependencyProperty XYZ_RGB_zProperty = DependencyProperty.Register(nameof(XYZ_RGB_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(25)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_zProperty);
        set => SetValue(XYZ_RGB_zProperty, value);
    }

    #endregion

    //...

    #region (C26-28) LABk_LMSk_(x|y|z)

    //(26)

    public static readonly DependencyProperty LABk_LMSk_xProperty = DependencyProperty.Register(nameof(LABk_LMSk_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(26)));
    public System.Windows.Media.Media3D.Point3D LABk_LMSk_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LABk_LMSk_xProperty);
        set => SetValue(LABk_LMSk_xProperty, value);
    }

    //(27)

    public static readonly DependencyProperty LABk_LMSk_yProperty = DependencyProperty.Register(nameof(LABk_LMSk_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(27)));
    public System.Windows.Media.Media3D.Point3D LABk_LMSk_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LABk_LMSk_yProperty);
        set => SetValue(LABk_LMSk_yProperty, value);
    }

    //(28)

    public static readonly DependencyProperty LABk_LMSk_zProperty = DependencyProperty.Register(nameof(LABk_LMSk_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(28)));
    public System.Windows.Media.Media3D.Point3D LABk_LMSk_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LABk_LMSk_zProperty);
        set => SetValue(LABk_LMSk_zProperty, value);
    }

    #endregion

    #region (C29-31) LMSk_LABk_(x|y|z)

    //(29)

    public static readonly DependencyProperty LMSk_LABk_xProperty = DependencyProperty.Register(nameof(LMSk_LABk_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(29)));
    public System.Windows.Media.Media3D.Point3D LMSk_LABk_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_LABk_xProperty);
        set => SetValue(LMSk_LABk_xProperty, value);
    }

    //(30)

    public static readonly DependencyProperty LMSk_LABk_yProperty = DependencyProperty.Register(nameof(LMSk_LABk_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(30)));
    public System.Windows.Media.Media3D.Point3D LMSk_LABk_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_LABk_yProperty);
        set => SetValue(LMSk_LABk_yProperty, value);
    }

    //(31)

    public static readonly DependencyProperty LMSk_LABk_zProperty = DependencyProperty.Register(nameof(LMSk_LABk_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(31)));
    public System.Windows.Media.Media3D.Point3D LMSk_LABk_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_LABk_zProperty);
        set => SetValue(LMSk_LABk_zProperty, value);
    }

    #endregion

    #region (C32-34) LMSk_XYZk_(x|y|z)

    //(32)

    public static readonly DependencyProperty LMSk_XYZk_xProperty = DependencyProperty.Register(nameof(LMSk_XYZk_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(32)));
    public System.Windows.Media.Media3D.Point3D LMSk_XYZk_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_XYZk_xProperty);
        set => SetValue(LMSk_XYZk_xProperty, value);
    }

    //(33)

    public static readonly DependencyProperty LMSk_XYZk_yProperty = DependencyProperty.Register(nameof(LMSk_XYZk_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(33)));
    public System.Windows.Media.Media3D.Point3D LMSk_XYZk_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_XYZk_yProperty);
        set => SetValue(LMSk_XYZk_yProperty, value);
    }

    //(34)

    public static readonly DependencyProperty LMSk_XYZk_zProperty = DependencyProperty.Register(nameof(LMSk_XYZk_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(34)));
    public System.Windows.Media.Media3D.Point3D LMSk_XYZk_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMSk_XYZk_zProperty);
        set => SetValue(LMSk_XYZk_zProperty, value);
    }

    #endregion

    #region (C35-37) XYZk_LMSk_(x|y|z)

    //(35)

    public static readonly DependencyProperty XYZk_LMSk_xProperty = DependencyProperty.Register(nameof(XYZk_LMSk_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(35)));
    public System.Windows.Media.Media3D.Point3D XYZk_LMSk_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZk_LMSk_xProperty);
        set => SetValue(XYZk_LMSk_xProperty, value);
    }

    //(36)

    public static readonly DependencyProperty XYZk_LMSk_yProperty = DependencyProperty.Register(nameof(XYZk_LMSk_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(36)));
    public System.Windows.Media.Media3D.Point3D XYZk_LMSk_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZk_LMSk_yProperty);
        set => SetValue(XYZk_LMSk_yProperty, value);
    }

    //(37)

    public static readonly DependencyProperty XYZk_LMSk_zProperty = DependencyProperty.Register(nameof(XYZk_LMSk_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(37)));
    public System.Windows.Media.Media3D.Point3D XYZk_LMSk_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZk_LMSk_zProperty);
        set => SetValue(XYZk_LMSk_zProperty, value);
    }

    #endregion

    //...

    #region (C38) xyYC_exy

    public static readonly DependencyProperty xyYC_exyProperty = DependencyProperty.Register(nameof(xyYC_exy), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(38)));
    public System.Windows.Media.Media3D.Point3D xyYC_exy
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(xyYC_exyProperty);
        set => SetValue(xyYC_exyProperty, value);
    }

    #endregion

    #endregion

    #region ColorModelEffect

    public ColorModelEffect() : base()
    {
        this.Bind(ActualModelProperty,
            $"{nameof(Model)}", this, BindingMode.OneWay, new SimpleConverter<Type, double>(i => Colour.GetIndex(i)));
        this.Bind(ActualModeProperty,
            $"{nameof(Mode)}", this, BindingMode.OneWay, new SimpleConverter<Modes, double>(i => (int)i));
        this.Bind(ActualShapeProperty,
            $"{nameof(Shape)}", this, BindingMode.OneWay, new SimpleConverter<Shapes2, double>(i => (int)i));

        //...

        UpdateShaderValue(ActualModelProperty);

        UpdateShaderValue(XComponentProperty);
        UpdateShaderValue(YComponentProperty);

        UpdateShaderValue(ActualModeProperty);
        UpdateShaderValue(ActualShapeProperty);
        UpdateShaderValue(DepthProperty);

        UpdateShaderValue(XProperty);
        UpdateShaderValue(YProperty);
        UpdateShaderValue(ZProperty);
        UpdateShaderValue(WProperty);

        UpdateShaderValue(CompandingProperty);
        UpdateShaderValue(GammaProperty);

        UpdateShaderValue(WhiteXProperty);
        UpdateShaderValue(WhiteYProperty);

        //...

        UpdateShaderValue(LMS_XYZ_xProperty);
        UpdateShaderValue(LMS_XYZ_yProperty);
        UpdateShaderValue(LMS_XYZ_zProperty);

        UpdateShaderValue(RGB_XYZ_xProperty);
        UpdateShaderValue(RGB_XYZ_yProperty);
        UpdateShaderValue(RGB_XYZ_zProperty);

        UpdateShaderValue(XYZ_LMS_xProperty);
        UpdateShaderValue(XYZ_LMS_yProperty);
        UpdateShaderValue(XYZ_LMS_zProperty);

        UpdateShaderValue(XYZ_RGB_xProperty);
        UpdateShaderValue(XYZ_RGB_yProperty);
        UpdateShaderValue(XYZ_RGB_zProperty);

        //...

        UpdateShaderValue(LABk_LMSk_xProperty);
        UpdateShaderValue(LABk_LMSk_yProperty);
        UpdateShaderValue(LABk_LMSk_zProperty);

        UpdateShaderValue(LMSk_LABk_xProperty);
        UpdateShaderValue(LMSk_LABk_yProperty);
        UpdateShaderValue(LMSk_LABk_zProperty);

        UpdateShaderValue(LMSk_XYZk_xProperty);
        UpdateShaderValue(LMSk_XYZk_yProperty);
        UpdateShaderValue(LMSk_XYZk_zProperty);

        UpdateShaderValue(XYZk_LMSk_xProperty);
        UpdateShaderValue(XYZk_LMSk_yProperty);
        UpdateShaderValue(XYZk_LMSk_zProperty);

        //...

        SetCurrentValue(LABk_LMSk_xProperty, GetPoint(Labk.LAB_LMS, 0));
        SetCurrentValue(LABk_LMSk_yProperty, GetPoint(Labk.LAB_LMS, 1));
        SetCurrentValue(LABk_LMSk_zProperty, GetPoint(Labk.LAB_LMS, 2));

        SetCurrentValue(LMSk_LABk_xProperty, GetPoint(Labk.LMS_LAB, 0));
        SetCurrentValue(LMSk_LABk_yProperty, GetPoint(Labk.LMS_LAB, 1));
        SetCurrentValue(LMSk_LABk_zProperty, GetPoint(Labk.LMS_LAB, 2));

        SetCurrentValue(LMSk_XYZk_xProperty, GetPoint(Labk.LMS_XYZ, 0));
        SetCurrentValue(LMSk_XYZk_yProperty, GetPoint(Labk.LMS_XYZ, 1));
        SetCurrentValue(LMSk_XYZk_zProperty, GetPoint(Labk.LMS_XYZ, 2));

        SetCurrentValue(XYZk_LMSk_xProperty, GetPoint(Labk.XYZ_LMS, 0));
        SetCurrentValue(XYZk_LMSk_yProperty, GetPoint(Labk.XYZ_LMS, 1));
        SetCurrentValue(XYZk_LMSk_zProperty, GetPoint(Labk.XYZ_LMS, 2));

        //...

        UpdateShaderValue(xyYC_exyProperty);
        SetCurrentValue(xyYC_exyProperty, new System.Windows.Media.Media3D.Point3D(xyYC.Colors[xyYC.MinHue][1], xyYC.Colors[xyYC.MinHue][2], xyYC.Colors[xyYC.MinHue][3]));

        OnProfileChanged(new(Profile));
    }

    #endregion

    #region Methods

    void Update_xyYC(One input)
    {
        int j = new DoubleRange(0, 1).Convert(10, 76, input).Round().Int32();

        Numerics.Vector row = default;
        for (var i = xyYC.MinHue; i < xyYC.MaxHue + 1; i++)
        {
            if (xyYC.Colors.ContainsKey(i))
            {
                if (j <= xyYC.Colors[i][0])
                {
                    row = xyYC.Colors[i];
                    break;
                }
            }
        }
        SetCurrentValue(xyYC_exyProperty, new System.Windows.Media.Media3D.Point3D(row[1], row[2], row[3]));
    }

    System.Windows.Media.Media3D.Point3D GetPoint(Matrix m, int y) => new System.Windows.Media.Media3D.Point3D(m[y][0], m[y][1], m[y][2]);

    protected virtual void OnProfileChanged(Value<WorkingProfile> input)
    {
        SetCurrentValue(CompandingProperty,
            input.New.Compress.GetAttribute<IndexAttribute>().Index.Double());
        SetCurrentValue(GammaProperty, 
            input.New.Compress is GammaCompression transfer ? transfer.Gamma : 0.0);

        SetCurrentValue(WhiteXProperty, input.New.Chromacity.X);
        SetCurrentValue(WhiteYProperty, input.New.Chromacity.Y);

        //...

        var m_RGB_XYZ 
            = XYZ.GetMatrix(input.New.Primary, (XYZ)(xyY)(xy)input.New.Chromacity);
        var m_XYZ_LMS
            = input.New.Adapt;

        var m_LMS_XYZ 
            = m_XYZ_LMS.Invert3By3();
        var m_XYZ_RGB 
            = m_RGB_XYZ.Invert3By3();

        SetCurrentValue(LMS_XYZ_xProperty, GetPoint(m_LMS_XYZ, 0));
        SetCurrentValue(LMS_XYZ_yProperty, GetPoint(m_LMS_XYZ, 1));
        SetCurrentValue(LMS_XYZ_zProperty, GetPoint(m_LMS_XYZ, 2));

        SetCurrentValue(RGB_XYZ_xProperty, GetPoint(m_RGB_XYZ, 0));
        SetCurrentValue(RGB_XYZ_yProperty, GetPoint(m_RGB_XYZ, 1));
        SetCurrentValue(RGB_XYZ_zProperty, GetPoint(m_RGB_XYZ, 2));

        SetCurrentValue(XYZ_LMS_xProperty, GetPoint(m_XYZ_LMS, 0));
        SetCurrentValue(XYZ_LMS_yProperty, GetPoint(m_XYZ_LMS, 1));
        SetCurrentValue(XYZ_LMS_zProperty, GetPoint(m_XYZ_LMS, 2));

        SetCurrentValue(XYZ_RGB_xProperty, GetPoint(m_XYZ_RGB, 0));
        SetCurrentValue(XYZ_RGB_yProperty, GetPoint(m_XYZ_RGB, 1));
        SetCurrentValue(XYZ_RGB_zProperty, GetPoint(m_XYZ_RGB, 2));
    }

    #endregion
}