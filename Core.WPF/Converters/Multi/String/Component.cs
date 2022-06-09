using Imagin.Core.Colors;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Converters
{
    [ValueConversion(typeof(object[]), typeof(string))]
    public class ComponentMultiConverter : MultiConverter<string>
    {
        public static ComponentMultiConverter Default { get; private set; } = new ComponentMultiConverter();
        public ComponentMultiConverter() : base() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length > 0)
            {
                if (values.Length > 1)
                {
                    if (values[0] is Components component)
                    {
                        if (values[1] is Type model)
                        {
                            var result = ColorModel.GetComponent(model, (int)component);
                            return $"({result.Symbol}) {result.Name}";
                        }
                    }
                }
                return $"{values[0]}";
            }
            return Binding.DoNothing;
        }
    }
}