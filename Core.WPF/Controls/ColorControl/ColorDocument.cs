using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Imagin.Core.Controls;

[MemberVisibility(Property: MemberVisibility.Explicit)]
[Serializable]
public class ColorDocument : Document
{
    #region Events

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
    ColorModel color = null;
    public ColorModel Color
    {
        get => color;
        set => this.Change(ref color, value);
    }

    Dimensions dimension = Dimensions.Three;
    [Featured(AboveBelow.Above), Index(0), Label(false), Localize(false), Visible]
    public Dimensions Dimension
    {
        get => dimension;
        set => this.Change(ref dimension, value);
    }

    public override object Icon => Color.ActualColor;

    StringColor oldColor = DefaultOldColor;
    public System.Windows.Media.Color OldColor
    {
        get => oldColor.Value;
        set => this.Change(ref oldColor, new StringColor(value));
    }

    Shapes2 shape = Shapes2.Square;
    [Featured(AboveBelow.Above), Index(1), Label(false), Localize(false), Visible]
    public Shapes2 Shape
    {
        get => shape;
        set => this.Change(ref shape, value);
    }

    //...

    double rotateX = 45;
    [DisplayName("X°"), Format(RangeFormat.Slider), Index(0), Localize(false), Range(0.0, 360.0, 1.0), Visible, Width(86)]
    public double RotateX
    {
        get => rotateX;
        set => this.Change(ref rotateX, value);
    }

    double rotateY = 45;
    [DisplayName("Y°"), Format(RangeFormat.Slider), Index(1), Localize(false), Range(0.0, 360.0, 1.0), Visible, Width(86)]
    public double RotateY
    {
        get => rotateY;
        set => this.Change(ref rotateY, value);
    }

    double rotateZ = 0;
    [DisplayName("Z°"), Format(RangeFormat.Slider), Index(2), Localize(false), Range(0.0, 360.0, 1.0), Visible, Width(86)]
    public double RotateZ
    {
        get => rotateZ;
        set => this.Change(ref rotateZ, value);
    }

    //...

    double zoom = 2.0;
    [Featured(AboveBelow.Below), Format(RangeFormat.Slider), Index(0), Range(0.0, 5.0, 0.01), Visible, Width(86)]
    public double Zoom
    {
        get => zoom;
        set => this.Change(ref zoom, value);
    }

    public override string Title => $"#{Color.ActualColor.Hexadecimal()}";

    public override object ToolTip => Color.ActualColor;

    #endregion

    #region ColorDocument

    public ColorDocument() : this(DefaultNewColor, DefaultModel) { }

    public ColorDocument(System.Windows.Media.Color color, Type model) : base()
    {
        Color = new(color);
        Color.ModelType = model;
        Color.ActualColor = color;
    }

    #endregion

    #region Methods

    public override void Subscribe()
    {
        base.Subscribe();
        Color.PropertyChanged += OnColorChanged;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        Color.PropertyChanged -= OnColorChanged;
    }

    public override void Save() { }

    //...

    void OnColorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.Changed(() => Color);
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
        }
    }

    //...

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext input) { }

    #endregion

    #region Commands

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
    new public ICommand SelectCommand => selectCommand ??= new RelayCommand(() => OldColor = Color.ActualColor, () => true);

    [field: NonSerialized]
    ICommand saveColorCommand;
    public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(() => ColorSaved?.Invoke(this, new(Color.ActualColor)), () => true);

    #endregion
}