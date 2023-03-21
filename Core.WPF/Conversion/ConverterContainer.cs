using System;
using System.Windows;

namespace Imagin.Core.Conversion;

public class ConverterContainer : DependencyObject
{
    public static readonly DependencyProperty DataKeyProperty = DependencyProperty.Register(nameof(DataKey), typeof(object), typeof(ConverterContainer), new FrameworkPropertyMetadata(null));
    public object DataKey
    {
        get => GetValue(DataKeyProperty);
        set => SetValue(DataKeyProperty, value);
    }

    public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register(nameof(Converter), typeof(Type), typeof(ConverterContainer), new FrameworkPropertyMetadata(null));
    public Type Converter
    {
        get => (Type)GetValue(ConverterProperty);
        set => SetValue(ConverterProperty, value);
    }

    public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register(nameof(Parameter), typeof(object), typeof(ConverterContainer), new FrameworkPropertyMetadata(null));
    public object Parameter
    {
        get => (object)GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public ConverterContainer() : base() { }
}