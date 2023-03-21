using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls;

public sealed class SelectionPresenter : Presenter<ItemsControl>
{
    MouseHookListener mouseListener;

    ScrollViewer scrollViewer = null;

    ///

    readonly DoubleRegion Selection = new();

    readonly List<Rect> selections = new();

    ///

    bool isDragging = false;

    Rect previousSelection = new();

    Point startPoint;

    ///

    double scrollOffset
        => XItemsControl.GetDragScrollOffset(Control);

    double scrollOffsetMaximum
        => XItemsControl.GetDragScrollOffsetMaximum(Control);

    double scrollTolerance
        => XItemsControl.GetDragScrollTolerance(Control);

    SelectionMode selectionMode
    {
        get
        {
            bool result()
            {
                if (Control is ListBox a)
                    return a.SelectionMode == System.Windows.Controls.SelectionMode.Single;

                if (Control is DataGrid b)
                    return b.SelectionMode == DataGridSelectionMode.Single;

                if (Control is TreeView c)
                    return XTreeView.GetSelectionMode(c) == Controls.SelectionMode.Single;

                return false;
            }
            return result() ? Controls.SelectionMode.Single : Controls.SelectionMode.Multiple;
        }
    }

    ///

    public SelectionPresenter() : base() => Content = Selection;

    ///

    protected override object OnBackgroundCoerced(object input) => input is Brush i ? i : Brushes.Transparent;

    ///

    protected override void OnLoaded(Presenter<ItemsControl> i)
    {
        base.OnLoaded(i);
        //If not present, assume abscence is intended!
        scrollViewer = i.FindParent<ScrollViewer>(); //throw new ParentNotFoundException<SelectionPresenter, ScrollViewer>();

        if (XItemsControl.GetCanDragSelectGlobally(Control))
        {
            mouseListener = new(new Input.WinApi.GlobalHooker());

            mouseListener.MouseDown
                += OnGlobalMouseDown;
            mouseListener.MouseMove
                += OnGlobalMouseMove;
            mouseListener.MouseUp
                += OnGlobalMouseUp;

            mouseListener.Start();
        }
    }

    protected override void OnUnloaded(Presenter<ItemsControl> i)
    {
        base.OnUnloaded(i);
        if (mouseListener != null)
        {
            mouseListener.Stop();

            mouseListener.MouseDown
                -= OnGlobalMouseDown;
            mouseListener.MouseMove
                -= OnGlobalMouseMove;
            mouseListener.MouseUp
                -= OnGlobalMouseUp;

            mouseListener.Dispose();
            mouseListener = null;
        }
    }

    ///

    bool IsItemHit(System.Windows.Forms.MouseEventArgs e)
    {
        foreach (var i in Control.Items)
        {
            var container = Control.GetContainer(i);
            if (container != null)
            {
                var j = new Rect(Canvas.GetLeft(container), Canvas.GetTop(container), container.ActualWidth, container.ActualHeight);
                if (j.Contains(new Point(e.Location.X, e.Location.Y)))
                    return true;
            }
        }
        return false;
    }

    ///

    void OnGlobalMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (Control == null)
            return;

        if (!XItemsControl.GetCanDragSelectGlobally(Control))
            return;

        if (XItemsControl.GetSelectionGlobalPredicate(Control)?.Invoke() == false)
            return;

        if (!XItemsControl.GetCanDragSelect(Control))
            return;

        if (IsItemHit(e))
            return;

        if (e.Button == System.Windows.Forms.MouseButtons.Left && selectionMode == SelectionMode.Multiple)
        {
            isDragging = true;

            Panel.SetZIndex(this, int.MaxValue);

            startPoint = new(e.Location.X, e.Location.Y);
            if (!ModifierKeys.Control.Pressed() && !ModifierKeys.Shift.Pressed())
            {
                selections.Clear();
                Control.TryClearSelection();
            }

            previousSelection = new Rect();
        }
    }

    void OnGlobalMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (isDragging)
        {
            var selection = GetSelection(startPoint, new(e.Location.X, e.Location.Y));
            if (isDragging)
            {
                Selection.X = selection.X; Selection.Y = selection.Y;
                Selection.Height = selection.Height; Selection.Width = selection.Width;

                var tLeft
                    = new Point(Selection.TopLeft.X, Selection.TopLeft.Y);
                var bRight
                    = new Point(Selection.BottomRight.X, Selection.BottomRight.Y);

                Select(Control, new Rect(tLeft, bRight));
            }
        }
    }

    void OnGlobalMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (Control == null || !XItemsControl.GetCanDragSelectGlobally(Control))
            return;

        if (isDragging)
        {
            var endPoint = new Point(e.Location.X, e.Location.Y);
            isDragging = false;

            Panel.SetZIndex(this, 0);

            if (!Try.Invoke(() => selections.Add(previousSelection)))
                Try.Invoke(() => selections.Clear());

            Selection.X = 0; Selection.Y = 0; Selection.Height = 0; Selection.Width = 0;
            startPoint = default;
        }
    }

    ///

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (!XItemsControl.GetCanDragSelect(Control))
            return;

        if (e.ChangedButton == MouseButton.Left && selectionMode == SelectionMode.Multiple)
        {
            isDragging = true;

            Panel.SetZIndex(this, int.MaxValue);

            CaptureMouse();
            startPoint = e.GetPosition(this);
            if (!ModifierKeys.Control.Pressed() && !ModifierKeys.Shift.Pressed())
                selections.Clear();

            previousSelection = new Rect();
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (isDragging)
        {
            var selection = GetSelection(startPoint, e.GetPosition(this));
            //If still dragging after determining selection
            if (isDragging)
            {
                //Update the visual selection
                Selection.X = selection.X; Selection.Y = selection.Y;
                Selection.Height = selection.Height; Selection.Width = selection.Width;

                var tLeft
                    = new Point(Selection.TopLeft.X, Selection.TopLeft.Y);
                var bRight
                    = new Point(Selection.BottomRight.X, Selection.BottomRight.Y);

                //Select the items that lie below it
                Select(Control, new Rect(tLeft, bRight));
                //Scroll as mouse moves
                Scroll(e.GetPosition(Control));
            }
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.LeftButton == MouseButtonState.Released && isDragging)
        {
            var endPoint = e.GetPosition(this);
            isDragging = false;

            Panel.SetZIndex(this, 0);

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            if (!Try.Invoke(() => selections.Add(previousSelection)))
                Try.Invoke(() => selections.Clear());

            Selection.X = 0; Selection.Y = 0; Selection.Height = 0; Selection.Width = 0;
            startPoint = default;
        }
    }

    ///

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ContentProperty)
        {
            if (Content?.Equals(Selection) == false)
                throw new ExternalChangeException<SelectionPresenter>(nameof(Content));
        }
    }

    ///

    Rect GetItemBounds(FrameworkElement i)
    {
        var topLeft = i.TranslatePoint(new Point(0, 0), this);
        return new Rect(topLeft.X, topLeft.Y, i.ActualWidth, i.ActualHeight);
    }

    Rect GetSelection(Point a, Point b)
    {
        b = new Point(Clamp(b.X, ActualWidth), Clamp(b.Y, ActualHeight));

        double
            x = a.X < b.X ? a.X : b.X,
            y = a.Y < b.Y ? a.Y : b.Y;

        //Now, the point is precisely what it should be
        var width
            = (a.X > b.X ? a.X - b.X : b.X - a.X).Abs();
        var height
            = (a.Y > b.Y ? a.Y - b.Y : b.Y - a.Y).Abs();

        return new Rect(new Point(x, y), new Size(width, height));
    }

    ///

    /// <summary>
    /// Scroll based on current position.
    /// </summary>
    /// <param name="point"></param>
    void Scroll(Point point)
    {
        if (scrollViewer == null)
            return;

        double x = point.X, y = point.Y;

        //Up
        var y1 = scrollTolerance;
        var y1i = y1 - y;
        y1i = y1i < 0 ? y1i : 0;
        y1i = scrollOffset + y1i > scrollOffsetMaximum ? scrollOffsetMaximum : scrollOffset + y1i;

        //Bottom
        var y0 = Control.ActualHeight - scrollTolerance;
        var y0i = y - y0;
        y0i = y0i < 0 ? 0 : y0i;
        y0i = scrollOffset + y0i > scrollOffsetMaximum ? scrollOffsetMaximum : scrollOffset + y0i;

        //Right
        var x1 = Control.ActualWidth - scrollTolerance;
        var x1i = x - x1;
        x1i = x1i < 0 ? 0 : x1i;
        x1i = scrollOffset + x1i > scrollOffsetMaximum ? scrollOffsetMaximum : scrollOffset + x1i;

        //Left
        var x0 = scrollTolerance;
        var x0i = x0 - x;
        x0i = x0i < 0 ? 0 : x0i;
        x0i = scrollOffset + x0i > scrollOffsetMaximum ? scrollOffsetMaximum : scrollOffset + x0i;

        //Up
        if (y < y1)
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - y1i);

        //Bottom 
        else if (y > y0)
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + y0i);

        //Left  
        if (x < x0)
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - x0i);

        //Right
        else if (x > x1)
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + x1i);
    }

    ///

    /// <summary>
    /// Gets whether or not the given <see cref="Rect"/> intersects with any previous selection.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    bool? IntersectsWith(Rect input)
    {
        var j = 0;

        var result = false;
        Try.Invoke(() =>
        {
            foreach (var i in selections)
            {
                if (input.IntersectsWith(i))
                {
                    result = j % 2 == 0;
                    j++;
                }
            }
        }, e => j = 0);
        return j == 0 ? null : (bool?)result;
    }

    ///

    /// <summary>
    /// Select items in control based on given area.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="area"></param>
    void Select(ItemsControl control, Rect area)
    {
        foreach (var i in control.Items)
        {
            var item = i is FrameworkElement j ? j : control.ItemContainerGenerator.ContainerFromItem(i) as FrameworkElement;
            if (item == null || item.Visibility != Visibility.Visible)
                continue;

            var itemBounds = GetItemBounds(item);

            //Check if current (or previous) selection intersects with item bounds
            bool? intersectsWith = null;
            if (itemBounds.IntersectsWith(area))
                intersectsWith = true;

            else if (itemBounds.IntersectsWith(previousSelection))
                intersectsWith = false;

            bool? result = null;
            if ((ModifierKeys.Control.Pressed() || ModifierKeys.Shift.Pressed()))
            {
                //Check whether or not the current item intersects with any previous selection
                var intersectedWith = IntersectsWith(itemBounds);

                //If current item has never insected with a previous selection...
                if (intersectedWith == null)
                {
                    result = intersectsWith;
                }
                else
                {
                    result = intersectedWith.Value;
                    //If current item also intersects with current (or previous) selection, flip it once more
                    if (intersectsWith != null && intersectsWith.Value)
                        result = !result;
                }
            }
            else result = intersectsWith;

            //If we are allowed to make a selection, make it
            if (result != null)
                item.TrySelect(result.Value);

            //If TreeViewItem, repeat the above for each child
            if (item is TreeViewItem)
                Select(item as ItemsControl, area);
        }
        previousSelection = area;
    }
}