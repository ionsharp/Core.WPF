﻿using Imagin.Core.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public class BaseComponentSlider : ColorSelector
{
    public static readonly DependencyProperty ArrowForegroundProperty = DependencyProperty.Register(nameof(ArrowForeground), typeof(Brush), typeof(BaseComponentSlider), new FrameworkPropertyMetadata(Brushes.Black));
    public Brush ArrowForeground
    {
        get => (Brush)GetValue(ArrowForegroundProperty);
        set => SetValue(ArrowForegroundProperty, value);
    }

    static readonly DependencyPropertyKey ArrowPositionKey = DependencyProperty.RegisterReadOnly(nameof(ArrowPosition), typeof(Point2), typeof(BaseComponentSlider), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ArrowPositionProperty = ArrowPositionKey.DependencyProperty;
    public Point2 ArrowPosition
    {
        get => (Point2)GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionKey, value);
    }

    public static readonly DependencyProperty ArrowTemplateProperty = DependencyProperty.Register(nameof(ArrowTemplate), typeof(DataTemplate), typeof(BaseComponentSlider), new FrameworkPropertyMetadata(null));
    public DataTemplate ArrowTemplate
    {
        get => (DataTemplate)GetValue(ArrowTemplateProperty);
        set => SetValue(ArrowTemplateProperty, value);
    }

    public BaseComponentSlider() : base()
    {
        ArrowPosition = new Point2(0, -8);
    }
}