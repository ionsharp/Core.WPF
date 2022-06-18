using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(object[]), typeof(Visibility))]
    public class BooleanToVisibilityMultiConverter : MultiConverter<Visibility>
    {
        public static BooleanToVisibilityMultiConverter Default { get; private set; } = new BooleanToVisibilityMultiConverter();
        BooleanToVisibilityMultiConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length > 0)
            {
                foreach (var i in values)
                {
                    if (i is bool a)
                    {
                        if (!a)
                            return Visibility.Collapsed;
                    }
                    if (i is Visibility b)
                    {
                        if (b != Visibility.Visible)
                            return Visibility.Collapsed;
                    }
                }
                return Visibility.Visible;
            }
            return Binding.DoNothing;
        }
    }
}