using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls;

/// <inheritdoc/>
public abstract class TransformAdorner : Adorner, ISubscribe, IUnsubscribe
{
    public const double DefaultThumbHeight = 10;

    public const double DefaultThumbWidth = 10;

    ///

    protected readonly VisualCollection Children;

    protected override int VisualChildrenCount => Children.Count;

    public FrameworkElement Element => AdornedElement as FrameworkElement;

    ///

    public TransformAdorner(FrameworkElement element) : base(element) => Children = new VisualCollection(this);

    ///

    protected virtual void BuildThumb(ref Thumb thumb, Cursor cursor, double height = DefaultThumbHeight, double width = DefaultThumbWidth, DependencyProperty property = null)
    {
        if (thumb == null)
        {
            thumb = new Thumb()
            {
                Background = Brushes.Black,
                BorderThickness = new Thickness(0),
                Cursor = cursor,
                Height = height, Width = width,
            };
            thumb.Bind(StyleProperty, new PropertyPath("(0)", property), AdornedElement);
            Children.Add(thumb);
        }
    }

    protected bool CanHandle(Thumb Thumb)
    {
        var result = true;

        if (AdornedElement == null || Thumb == null)
            result = false;

        if (result)
            EnforceSize(Element);

        return result;
    }

    /// <summary>
    /// This method ensures that the Widths and Heights are initialized. Sizing to content produces Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height need to be set first.  It also sets the maximum size of the adorned element.
    /// </summary>
    protected void EnforceSize(FrameworkElement input)
    {
        if (input.Width.Equals(double.NaN))
            input.Width = input.DesiredSize.Width;

        if (input.Height.Equals(double.NaN))
            input.Height = input.DesiredSize.Height;

        if (input.Parent is FrameworkElement parent)
        {
            input.MaxHeight
                = parent.ActualHeight;
            input.MaxWidth
                = parent.ActualWidth;
        }
    }

    protected override Visual GetVisualChild(int index) => Children[index];

    ///

    public virtual void Subscribe() { }

    public virtual void Unsubscribe() { }
}

/// <inheritdoc/>
public class RotateAdorner : TransformAdorner
{
    #region Properties

    Canvas canvas;

    Point centerPoint;

    double initialAngle;

    System.Windows.Vector startVector;

    ///

    readonly Thumb RotateButton;

    readonly LineElement RotateLine;

    public double Snap => XElement.GetRotateSnap(Element);

    RotateTransform transform = null;
    public RotateTransform Transform
    {
        get
        {
            if (transform is RotateTransform)
            {
                if (ReferenceEquals(transform, Element.RenderTransform))
                    return transform;

                transform.Unbind(RotateTransform.AngleProperty);
            }

            if (Element.RenderTransform is RotateTransform e)
            {
                transform = e;
                return transform;
            }

            transform = new RotateTransform();

            Element.RenderTransform = transform;
            return transform;
        }
    }

    #endregion

    #region RotateAdorner

    public RotateAdorner(FrameworkElement element) : base(element)
    {
        RotateLine = new LineElement() { Height = 10, Orientation = Orientation.Vertical };
        RotateLine.Bind(LineElement.StrokeProperty, new PropertyPath("(0)", XElement.RotateLineColorProperty), AdornedElement);
        Children.Add(RotateLine);

        BuildThumb(ref RotateButton, Cursors.Hand, 16, 16);
    }

    #endregion

    #region Methods

    void OnRotating(object sender, DragDeltaEventArgs e)
    {
        if (!CanHandle(sender as Thumb) || canvas == null)
            return;

        var currentPoint
            = Mouse.GetPosition(canvas);
        var deltaVector
            = Point.Subtract(currentPoint, centerPoint);

        double angle = System.Windows.Vector.AngleBetween(startVector, deltaVector);
        Transform.Angle = M.NearestFactor(initialAngle + Math.Round(angle, 0), Snap);

        Element.RenderTransformOrigin = new Point(0.5, 0.5);
        Element.InvalidateMeasure();
    }

    void OnRotationStarted(object sender, DragStartedEventArgs e)
    {
        if (!CanHandle(sender as Thumb))
            return;

        canvas = VisualTreeHelper.GetParent(Element) as Canvas;
        if (canvas == null)
            return;

        centerPoint = Element.TranslatePoint(new Point(Element.Width * Element.RenderTransformOrigin.X, Element.Height * Element.RenderTransformOrigin.Y), canvas);

        var startPoint = Mouse.GetPosition(canvas);
        startVector = Point.Subtract(startPoint, centerPoint);

        Transform.Bind(RotateTransform.AngleProperty, $"{nameof(DataContext)}.{nameof(IRotate.Rotation)}", Element, System.Windows.Data.BindingMode.TwoWay);
        initialAngle = Transform.Angle;
    }

    ///

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        //Desired width/height
        var dW
            = AdornedElement.DesiredSize.Width;
        var dH
            = AdornedElement.DesiredSize.Height;

        //Adorner width/height
        var aW
            = DesiredSize.Width;
        var aH
            = DesiredSize.Height;

        RotateButton.Arrange(new Rect((dW / 2) - (aW / 2), (-aH / 2) - 24, aW, aH));
        RotateLine.Arrange(new Rect((dW / 2) - (aW / 2), (-aH / 2) - 10, aW, aH));

        return finalSize;
    }

    /// <inheritdoc/>
    protected override void BuildThumb(ref Thumb thumb, Cursor cursor, double height = DefaultThumbHeight, double width = DefaultThumbWidth, DependencyProperty property = null)
        => base.BuildThumb(ref thumb, cursor, height, width, XElement.RotateThumbStyleProperty);

    ///

    /// <inheritdoc/>
    public override void Subscribe()
    {
        base.Subscribe();
        RotateButton.DragDelta
            += OnRotating;
        RotateButton.DragStarted
            += OnRotationStarted;
    }

    /// <inheritdoc/>
    public override void Unsubscribe()
    {
        base.Unsubscribe();
        RotateButton.DragDelta
            -= OnRotating;
        RotateButton.DragStarted
            -= OnRotationStarted;
    }

    #endregion
}