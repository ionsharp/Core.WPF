using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(object[]), typeof(string))]
    public class ColorMultiConverter : MultiConverter<string>
    {
        public static ColorMultiConverter Default { get; private set; } = new();
        public ColorMultiConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 3)
            {
                if (values[0] is Color color)
                {
                    if (values[1] is string model)
                    {
                        if (values[2] is int precision)
                        {
                            color.Convert(out RGB rgb);

                            Type type = Colour.Types.FirstOrDefault(i => i.Name == model);
                            var result = rgb.To(type, WorkingProfile.Default);

                            return $"{(Vector)result.Values.Transform(i => i.Round(precision))}";
                        }
                    }
                }
            }
            return null;
        }
    }
}