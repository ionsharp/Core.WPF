using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows;

namespace Imagin.Core.Controls;

public abstract class ComponentSelector : ColorSelector
{
    public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register(nameof(EllipseDiameter), typeof(double), typeof(ComponentSelector), new FrameworkPropertyMetadata(12.0));
    public double EllipseDiameter
    {
        get => (double)GetValue(EllipseDiameterProperty);
        set => SetValue(EllipseDiameterProperty, value);
    }

    public static readonly DependencyProperty EllipsePositionProperty = DependencyProperty.Register(nameof(EllipsePosition), typeof(Point2), typeof(ComponentSelector), new FrameworkPropertyMetadata(null));
    public Point2 EllipsePosition
    {
        get => (Point2)GetValue(EllipsePositionProperty);
        private set => SetValue(EllipsePositionProperty, value);
    }

    public static readonly DependencyPropertyKey Is4Key = DependencyProperty.RegisterReadOnly(nameof(Is4), typeof(bool), typeof(ComponentSelector), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty Is4Property = Is4Key.DependencyProperty;
    public bool Is4
    {
        get => (bool)GetValue(Is4Property);
        protected set => SetValue(Is4Key, value);
    }

    public static readonly DependencyProperty RotateXProperty = DependencyProperty.Register(nameof(RotateX), typeof(double), typeof(ComponentSelector), new FrameworkPropertyMetadata(45.0));
    public double RotateX
    {
        get => (double)GetValue(RotateXProperty);
        set => SetValue(RotateXProperty, value);
    }

    public static readonly DependencyProperty RotateYProperty = DependencyProperty.Register(nameof(RotateY), typeof(double), typeof(ComponentSelector), new FrameworkPropertyMetadata(45.0));
    public double RotateY
    {
        get => (double)GetValue(RotateYProperty);
        set => SetValue(RotateYProperty, value);
    }

    public static readonly DependencyProperty RotateZProperty = DependencyProperty.Register(nameof(RotateZ), typeof(double), typeof(ComponentSelector), new FrameworkPropertyMetadata(0.0));
    public double RotateZ
    {
        get => (double)GetValue(RotateZProperty);
        set => SetValue(RotateZProperty, value);
    }

    public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(One), typeof(ComponentSelector), new FrameworkPropertyMetadata(One.Zero));
    public One X
    {
        get => (One)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(One), typeof(ComponentSelector), new FrameworkPropertyMetadata(One.Zero));
    public One Y
    {
        get => (One)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(One), typeof(ComponentSelector), new FrameworkPropertyMetadata(One.Zero));
    public One Z
    {
        get => (One)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    public static readonly DependencyProperty WProperty = DependencyProperty.Register(nameof(W), typeof(One), typeof(ComponentSelector), new FrameworkPropertyMetadata(One.Zero));
    public One W
    {
        get => (One)GetValue(WProperty);
        set => SetValue(WProperty, value);
    }

    public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(ComponentSelector), new FrameworkPropertyMetadata(2.0));
    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    //...

    public ComponentSelector() : base()
    {
        SetCurrentValue(EllipsePositionProperty, new Point2(0, 0));
    }

    //...

    protected One GetValue(Component4 component)
    {
        switch (component)
        {
            case Component4.X: return X;
            case Component4.Y: return Y;
            case Component4.Z: return Z;
            case Component4.W: return W;
        }
        throw new NotSupportedException();
    }

    protected void SetValue(Component4 component, One value)
    {
        switch (component)
        {
            case Component4.X:
                SetCurrentValue(XProperty, value);
                break;
            case Component4.Y:
                SetCurrentValue(YProperty, value);
                break;
            case Component4.Z:
                SetCurrentValue(ZProperty, value);
                break;
            case Component4.W:
                SetCurrentValue(WProperty, value);
                break;
        }
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ModelProperty)
            Is4 = Model.Inherits<ColorModel4>();
    }
}