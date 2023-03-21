using Imagin.Core.Conversion;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Controls;

[ValueConversion(typeof(object[]), typeof(double))]
public class BarGraphConverter : MultiConverter<object>
{
    public static BarGraphConverter Default { get; private set; } = new();
    public BarGraphConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 3)
        {
            if (values[0] is double x)
            {
                if (values[1] is double y)
                {
                    if (values[2] is double z)
                        return y > 0 ? x / y * z : Binding.DoNothing;
                }
            }
        }
        return Binding.DoNothing;
    }
}

public class BarGraphItem : ContentControl
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(BarGraphItem), new FrameworkPropertyMetadata(0.0));
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public BarGraphItem() : base() { }
}

public class BarGraph : ItemsControl
{
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(BarGraph), new FrameworkPropertyMetadata(0.0));
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public BarGraph() : base() { }

    protected override DependencyObject GetContainerForItemOverride() => new BarGraphItem();

    protected override bool IsItemItsOwnContainerOverride(object item) => item is BarGraphItem;
}