using Imagin.Core.Colors;
using Imagin.Core.Numerics;
using System;
using System.Windows;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

public class ComponentSlider : BaseComponentSlider
{
    readonly Handle handle = false;

    public static readonly DependencyProperty ComponentProperty = DependencyProperty.Register(nameof(Component), typeof(Component4), typeof(ComponentSlider), new FrameworkPropertyMetadata(Component4.X));
    public Component4 Component
    {
        get => (Component4)GetValue(ComponentProperty);
        set => SetValue(ComponentProperty, value);
    }

    public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register(nameof(Shape), typeof(Shapes2), typeof(ComponentSlider), new FrameworkPropertyMetadata(Shapes2.Square));
    public Shapes2 Shape
    {
        get => (Shapes2)GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(One), typeof(ComponentSlider), new FrameworkPropertyMetadata(One.Zero));
    public One X
    {
        get => (One)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(One), typeof(ComponentSlider), new FrameworkPropertyMetadata(One.Zero));
    public One Y
    {
        get => (One)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(One), typeof(ComponentSlider), new FrameworkPropertyMetadata(One.Zero));
    public One Z
    {
        get => (One)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    public static readonly DependencyProperty WProperty = DependencyProperty.Register(nameof(W), typeof(One), typeof(ComponentSlider), new FrameworkPropertyMetadata(One.Zero));
    public One W
    {
        get => (One)GetValue(WProperty);
        set => SetValue(WProperty, value);
    }

    public ComponentSlider() : base() { }

    DependencyProperty GetProperty(Component4 input)
    {
        switch (input)
        {
            case Component4.X: return XProperty;
            case Component4.Y: return YProperty;
            case Component4.Z: return ZProperty;
            case Component4.W: return WProperty;
        }
        throw new NotSupportedException();
    }

    protected override void Mark()
    {
        if (Shape == Shapes2.Square)
        {
            var y = (One)GetValue(GetProperty(Component));

            ArrowPosition.X = 0;
            ArrowPosition.Y = ((1 - y) * ActualHeight) - 8;
        }
    }

    protected override void OnMouseChanged(Vector2<One> input)
    {
        base.OnMouseChanged(input);

        One y = Shape == Shapes2.Circle ? 1 - GetDistance(0.5, 0.5, input.X, input.Y) / GetDistance(0.5, 0.5, 1, 1) : input.Y;
        handle.Invoke(() => SetCurrentValue(GetProperty(Component), y));

        if (Shape == Shapes2.Circle)
        {
            ArrowPosition.X = (input.X * ActualWidth) - 6;
            ArrowPosition.Y = ((1 - input.Y) * ActualHeight) - 6;
        }
        else
        {
            ArrowPosition.X = 0;
            ArrowPosition.Y = ((1 - input.Y) * ActualHeight) - 8;
        }
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        handle.SafeInvoke(() =>
        {
            if (e.Property == GetProperty(Component))
                Mark();
        });
    }
}