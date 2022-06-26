using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Imagin.Core.Controls;

[Explicit]
[Serializable]
public class ColorDocument : Document
{
    #region Events

    [field: NonSerialized]
    public event DefaultEventHandler<System.Windows.Media.Color> ColorChanged;

    [field: NonSerialized]
    public event DefaultEventHandler<System.Windows.Media.Color> ColorSaved;
    
    #endregion

    #region Fields

    public static readonly System.Windows.Media.Color DefaultOldColor = System.Windows.Media.Colors.Black;

    public static readonly System.Windows.Media.Color DefaultNewColor = System.Windows.Media.Colors.White;

    public static readonly Type DefaultModel = typeof(HSB);

    #endregion

    #region Properties

    byte alpha = 255;
    public byte Alpha
    {
        get => alpha;
        set => this.Change(ref alpha, value);
    }

    [field: NonSerialized]
    ColorViewModel color = null;
    public ColorViewModel Color
    {
        get => color;
        set => this.Change(ref color, value);
    }

    double depth = 0;
    [Feature(AboveBelow.Below), SliderUpDown, Localize(false), Range(0.0, 128.0, 1.0), StringFormat("N0"), Visible, Width(128)]
    public double Depth
    {
        get => depth;
        set => this.Change(ref depth, value);
    }

    Dimensions dimension = Dimensions.One;
    [Feature(AboveBelow.Above), Index(0), Label(false), Localize(false), Visible]
    public Dimensions Dimension
    {
        get => dimension;
        set => this.Change(ref dimension, value);
    }

    public override object Icon => Color.ActualColor;

    [Hidden]
    public bool Dimension12 => !Dimension3;

    [Hidden]
    public bool Dimension3 => Dimension == Dimensions.Three;

    StringColor oldColor = DefaultOldColor;
    public System.Windows.Media.Color OldColor
    {
        get => oldColor.Value;
        set => this.Change(ref oldColor, new StringColor(value));
    }

    [Hidden]
    public readonly ColorControlOptions Options;

    double rotateX = 45;
    [DisplayName("X°"), SliderUpDown, Index(0), Localize(false), Range(0.0, 360.0, 1.0), StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateX
    {
        get => rotateX;
        set => this.Change(ref rotateX, value);
    }

    double rotateY = 45;
    [DisplayName("Y°"), SliderUpDown, Index(1), Localize(false), Range(0.0, 360.0, 1.0), StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateY
    {
        get => rotateY;
        set => this.Change(ref rotateY, value);
    }

    double rotateZ = 0;
    [DisplayName("Z°"), SliderUpDown, Index(2), Localize(false), Range(0.0, 360.0, 1.0), StringFormat("N0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double RotateZ
    {
        get => rotateZ;
        set => this.Change(ref rotateZ, value);
    }

    Shapes2 shape = Shapes2.Square;
    [Feature(AboveBelow.Above), Index(1), Label(false), Localize(false), Visible]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension12))]
    public Shapes2 Shape
    {
        get => shape;
        set => this.Change(ref shape, value);
    }

    public override string Title
    {
        get
        {
            Color.ActualColor.Convert(out Hexadecimal color);
            return $"#{color}";
        }
    }

    public override object ToolTip => Color.ActualColor;

    double zoom = 1.8;
    [SliderUpDown, Index(3), Range(0.0, 5.0, 0.01), StringFormat("P0"), Visible, Width(86)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(Dimension3))]
    public double Zoom
    {
        get => zoom;
        set => this.Change(ref zoom, value);
    }

    #endregion

    #region ColorDocument

    public ColorDocument() : this(DefaultNewColor, DefaultModel, null) { }

    public ColorDocument(ColorControlOptions options) : this(DefaultNewColor, DefaultModel, options) { }

    public ColorDocument(System.Windows.Media.Color color, Type model, ColorControlOptions options) : base()
    {
        Color = new(color, options);
        Color.Model = Color.Models.SourceCollection.To<IList>().FirstOrDefault<NamableCategory<Type>>(i => i.Value == model);
        Color.ActualColor = color;
    }

    #endregion

    #region Methods

    public override void Subscribe()
    {
        base.Subscribe();
        Color.ColorChanged += OnColorChanged;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        Color.ColorChanged -= OnColorChanged;
    }

    public override void Save() { }

    //...

    void OnColorChanged(object sender, EventArgs<System.Windows.Media.Color> e)
    {
        this.Changed(() => Color);
        ColorChanged?.Invoke(this, new(e.Value));
    }

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Alpha):
                Color.If(i => i.ActualColor = Color.ActualColor.A(Alpha));
                break;

            case nameof(Color):
                this.Changed(() => Title);
                this.Changed(() => ToolTip);
                break;

            case nameof(Dimension):
                this.Changed(() => Dimension3);
                this.Changed(() => Dimension12);
                break;
        }
    }

    //...

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext input) { }

    #endregion

    #region Commands

    [field: NonSerialized]
    ICommand pickCommand;
    public ICommand PickCommand => pickCommand ??= new RelayCommand<System.Windows.Media.Color>(i => Color.ActualColor = i);

    [field: NonSerialized]
    ICommand revertCommand;
    public ICommand RevertCommand => revertCommand ??= new RelayCommand(() =>
    {
        var oldColor = OldColor;
        OldColor = Color.ActualColor;
        Color.ActualColor = oldColor;
    },
    () => true);

    [field: NonSerialized]
    ICommand selectCommand;
    public ICommand SelectCommand => selectCommand ??= new RelayCommand(() => OldColor = Color.ActualColor, () => true);

    [field: NonSerialized]
    ICommand saveColorCommand;
    public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(() => ColorSaved?.Invoke(this, new(Color.ActualColor)), () => true);

    #endregion
}