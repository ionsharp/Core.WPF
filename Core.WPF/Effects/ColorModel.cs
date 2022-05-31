using Imagin.Core.Colors;
using Imagin.Core.Converters;
using Imagin.Core.Linq;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Effects;

[ValueConversion(typeof(int), typeof(string))]
public class ColorModelConverter : Converters.Converter<int, string>
{
    public static ColorModelConverter Default { get; private set; } = new();
    public ColorModelConverter() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<int> input) => ColorVector.Type[input.Value].Name;

    protected override ConverterValue<int> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

public class ColorModelEffect : BaseEffect
{

    #region (enum) Modes

    public enum Modes 
    {
        /// <summary>
        /// Given <see cref="Components.X"/>, <see cref="Components.Y"/>, and <see cref="Components.Z"/>, displays original color with corresponding components adjusted.
        /// </summary>
        XYZ,
        /// <summary>
        /// Given <see cref="Components.Z"/>, displays <see cref="Components.X"/> and <see cref="Components.Y"/> (in any order).
        /// </summary>
        XY,
        /// <summary>
        /// Given <see cref="Components.X"/> and <see cref="Components.Y"/>, displays <see cref="Components.Z"/> (in any order).
        /// </summary>
        Z
    }

    #endregion

    #region Properties

    public override string FilePath => "ColorModel.ps";

    public static readonly DependencyProperty ProfileIndexProperty = DependencyProperty.Register(nameof(ProfileIndex), typeof(WorkingProfiles), typeof(ColorModelEffect), new FrameworkPropertyMetadata(WorkingProfiles.sRGB));
    public WorkingProfiles ProfileIndex
    {
        get => (WorkingProfiles)GetValue(ProfileIndexProperty);
        set => SetValue(ProfileIndexProperty, value);
    }

    public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(nameof(Profile), typeof(WorkingProfile), typeof(ColorModelEffect), new FrameworkPropertyMetadata(WorkingProfile.Default.sRGB, OnProfileChanged));
    public WorkingProfile Profile
    {
        get => (WorkingProfile)GetValue(ProfileProperty);
        set => SetValue(ProfileProperty, value);
    }
    static void OnProfileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<ColorModelEffect>().OnProfileChanged(e);

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

    #region (C1) Component

    internal static readonly DependencyProperty ActualComponentProperty = DependencyProperty.Register(nameof(ActualComponent), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(1)));
    internal double ActualComponent
    {
        get => (double)GetValue(ActualComponentProperty);
        set => SetValue(ActualComponentProperty, value);
    }

    public static readonly DependencyProperty ComponentProperty = DependencyProperty.Register(nameof(Component), typeof(Components), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Components.X));
    public Components Component
    {
        get => (Components)GetValue(ComponentProperty);
        set => SetValue(ComponentProperty, value);
    }

    #endregion

    #region (C2) Mode

    internal static readonly DependencyProperty ActualModeProperty = DependencyProperty.Register(nameof(ActualMode), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata((double)(int)Modes.XY, PixelShaderConstantCallback(2)));
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

    #region (C3-5) X|Y|Z

    //(C3)

    public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(3)));
    public double X
    {
        get => (double)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    //(C4)

    public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(4)));
    public double Y
    {
        get => (double)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    //(C5)

    public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(5)));
    public double Z
    {
        get => (double)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    #endregion

    //(C6)

    public static readonly DependencyProperty CompandingProperty = DependencyProperty.Register(nameof(Companding), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(4.0, PixelShaderConstantCallback(6)));
    /// <summary>The default is <see cref="WorkingProfile.Default.sRGB"/>.</summary>
    public double Companding
    {
        get => (double)GetValue(CompandingProperty);
        set => SetValue(CompandingProperty, value);
    }

    //(C7)

    public static readonly DependencyProperty GammaProperty = DependencyProperty.Register(nameof(Gamma), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(double), PixelShaderConstantCallback(7)));
    public double Gamma
    {
        get => (double)GetValue(GammaProperty);
        set => SetValue(GammaProperty, value);
    }

    #region (C8-9) White(X|Y)

    //(C8)

    public static readonly DependencyProperty WhiteXProperty = DependencyProperty.Register(nameof(WhiteX), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Illuminant2.D65.X, PixelShaderConstantCallback(8)));
    /// <summary>The default is <see cref="Illuminant2.D65"/>.</summary>
    public double WhiteX
    {
        get => (double)GetValue(WhiteXProperty);
        set => SetValue(WhiteXProperty, value);
    }

    //(C9)

    public static readonly DependencyProperty WhiteYProperty = DependencyProperty.Register(nameof(WhiteY), typeof(double), typeof(ColorModelEffect), new FrameworkPropertyMetadata(Illuminant2.D65.Y, PixelShaderConstantCallback(9)));
    /// <summary>The default is <see cref="Illuminant2.D65"/>.</summary>
    public double WhiteY
    {
        get => (double)GetValue(WhiteYProperty);
        set => SetValue(WhiteYProperty, value);
    }

    #endregion

    #region (C10-12) LMS_XYZ_(x|y|z)

    //(C10)

    public static readonly DependencyProperty LMS_XYZ_xProperty = DependencyProperty.Register(nameof(LMS_XYZ_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(10)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_xProperty);
        set => SetValue(LMS_XYZ_xProperty, value);
    }

    //(C11)

    public static readonly DependencyProperty LMS_XYZ_yProperty = DependencyProperty.Register(nameof(LMS_XYZ_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(11)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_yProperty);
        set => SetValue(LMS_XYZ_yProperty, value);
    }

    //(C12)

    public static readonly DependencyProperty LMS_XYZ_zProperty = DependencyProperty.Register(nameof(LMS_XYZ_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(12)));
    public System.Windows.Media.Media3D.Point3D LMS_XYZ_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(LMS_XYZ_zProperty);
        set => SetValue(LMS_XYZ_zProperty, value);
    }

    #endregion

    #region (C13-15) RGB_XYZ_(x|y|z)

    //(C13)

    public static readonly DependencyProperty RGB_XYZ_xProperty = DependencyProperty.Register(nameof(RGB_XYZ_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(13)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_xProperty);
        set => SetValue(RGB_XYZ_xProperty, value);
    }

    //(C14)

    public static readonly DependencyProperty RGB_XYZ_yProperty = DependencyProperty.Register(nameof(RGB_XYZ_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(14)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_yProperty);
        set => SetValue(RGB_XYZ_yProperty, value);
    }

    //(C15)

    public static readonly DependencyProperty RGB_XYZ_zProperty = DependencyProperty.Register(nameof(RGB_XYZ_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(15)));
    public System.Windows.Media.Media3D.Point3D RGB_XYZ_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(RGB_XYZ_zProperty);
        set => SetValue(RGB_XYZ_zProperty, value);
    }

    #endregion

    #region (C16-18) XYZ_LMS_(x|y|z)

    //(C16)

    public static readonly DependencyProperty XYZ_LMS_xProperty = DependencyProperty.Register(nameof(XYZ_LMS_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(16)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_xProperty);
        set => SetValue(XYZ_LMS_xProperty, value);
    }

    //(C17)

    public static readonly DependencyProperty XYZ_LMS_yProperty = DependencyProperty.Register(nameof(XYZ_LMS_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(17)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_yProperty);
        set => SetValue(XYZ_LMS_yProperty, value);
    }

    //(C18)

    public static readonly DependencyProperty XYZ_LMS_zProperty = DependencyProperty.Register(nameof(XYZ_LMS_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(18)));
    public System.Windows.Media.Media3D.Point3D XYZ_LMS_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_LMS_zProperty);
        set => SetValue(XYZ_LMS_zProperty, value);
    }

    #endregion

    #region (C19-21) XYZ_RGB_(x|y|z)

    //(C19)

    public static readonly DependencyProperty XYZ_RGB_xProperty = DependencyProperty.Register(nameof(XYZ_RGB_x), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(19)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_x
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_xProperty);
        set => SetValue(XYZ_RGB_xProperty, value);
    }

    //(C20)

    public static readonly DependencyProperty XYZ_RGB_yProperty = DependencyProperty.Register(nameof(XYZ_RGB_y), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(20)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_y
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_yProperty);
        set => SetValue(XYZ_RGB_yProperty, value);
    }

    //(C21)

    public static readonly DependencyProperty XYZ_RGB_zProperty = DependencyProperty.Register(nameof(XYZ_RGB_z), typeof(System.Windows.Media.Media3D.Point3D), typeof(ColorModelEffect), new FrameworkPropertyMetadata(default(System.Windows.Media.Media3D.Point3D), PixelShaderConstantCallback(21)));
    public System.Windows.Media.Media3D.Point3D XYZ_RGB_z
    {
        get => (System.Windows.Media.Media3D.Point3D)GetValue(XYZ_RGB_zProperty);
        set => SetValue(XYZ_RGB_zProperty, value);
    }

    #endregion

    #endregion

    #region ColorModelEffect

    public ColorModelEffect() : base()
    {
        this.Bind(ActualComponentProperty,
            $"{nameof(Component)}", this, BindingMode.OneWay, new SimpleConverter<Components, double>(i => (int)i));
        this.Bind(ActualModelProperty,
            $"{nameof(Model)}", this, BindingMode.OneWay, new SimpleConverter<Type, double>(i => ColorVector.Index[i]));
        this.Bind(ActualModeProperty,
            $"{nameof(Mode)}", this, BindingMode.OneWay, new SimpleConverter<Modes, double>(i => (int)i));
        
        
        this.Bind(ProfileProperty, $"{nameof(ProfileIndex)}", this, BindingMode.OneWay, new SimpleConverter<WorkingProfiles, WorkingProfile>(i => (WorkingProfile)typeof(WorkingProfile.Default).GetProperty($"{i}", BindingFlags.Public | BindingFlags.Static).GetValue(null)));

        //...

        UpdateShaderValue(ActualModelProperty);
        UpdateShaderValue(ActualComponentProperty);

        UpdateShaderValue(ActualModeProperty);

        UpdateShaderValue(XProperty);
        UpdateShaderValue(YProperty);
        UpdateShaderValue(ZProperty);

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

        OnProfileChanged(new(Profile));
    }

    #endregion

    #region Methods

    int GetCompandingIndex(ICompanding input)
    {   
        if (input is GammaCompanding)
            return 0;

        if (input is LCompanding)
            return 1;

        if (input is Rec709Companding)
            return 2;

        if (input is Rec2020Companding)
            return 3;

        if (input is sRGBCompanding)
            return 4;

        return 0;
    }

    protected virtual void OnProfileChanged(Value<WorkingProfile> input)
    {
        SetCurrentValue(CompandingProperty, 
            GetCompandingIndex(input.New.Companding).Double());
        SetCurrentValue(GammaProperty, 
            input.New.Companding is GammaCompanding gammaCompanding ? gammaCompanding.Gamma : 0d);

        var white = Illuminant.GetChromacity(input.New.White);
        SetCurrentValue(WhiteXProperty, white.X);
        SetCurrentValue(WhiteYProperty, white.Y);

        //...

        var m_RGB_XYZ 
            = WorkingProfile.GetRxGyBz(input.New.Chromacity, input.New.White);
        var m_XYZ_LMS 
            = LMSTransformationMatrix.VonKriesHPEAdjusted;

        var m_LMS_XYZ 
            = m_XYZ_LMS.Invert3By3();
        var m_XYZ_RGB 
            = m_RGB_XYZ.Invert3By3();

        System.Windows.Media.Media3D.Point3D f(Matrix m, int y) => new System.Windows.Media.Media3D.Point3D(m[y][0], m[y][1], m[y][2]);

        SetCurrentValue(LMS_XYZ_xProperty, f(m_LMS_XYZ, 0));
        SetCurrentValue(LMS_XYZ_yProperty, f(m_LMS_XYZ, 1));
        SetCurrentValue(LMS_XYZ_zProperty, f(m_LMS_XYZ, 2));

        SetCurrentValue(RGB_XYZ_xProperty, f(m_RGB_XYZ, 0));
        SetCurrentValue(RGB_XYZ_yProperty, f(m_RGB_XYZ, 1));
        SetCurrentValue(RGB_XYZ_zProperty, f(m_RGB_XYZ, 2));

        SetCurrentValue(XYZ_LMS_xProperty, f(m_XYZ_LMS, 0));
        SetCurrentValue(XYZ_LMS_yProperty, f(m_XYZ_LMS, 1));
        SetCurrentValue(XYZ_LMS_zProperty, f(m_XYZ_LMS, 2));

        SetCurrentValue(XYZ_RGB_xProperty, f(m_XYZ_RGB, 0));
        SetCurrentValue(XYZ_RGB_yProperty, f(m_XYZ_RGB, 1));
        SetCurrentValue(XYZ_RGB_zProperty, f(m_XYZ_RGB, 2));
    }

    #endregion
}