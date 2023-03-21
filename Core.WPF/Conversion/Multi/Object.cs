using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(object))]
public class GradientMultiConverter : MultiConverter<object>
{
    public GradientMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 4)
        {
            if (values[0] is Color[] colors)
            {
                if (values[1] is double value)
                {
                    if (values[2] is double minimum)
                    {
                        if (values[3] is double maximum)
                        {
                            var totalProgress = new DoubleRange(minimum, maximum).Convert(0, 1, value);

                            double offset = 0;

                            Color? result = null;

                            List<Color> gradient = new();

                            var j = 1.0 / (colors.Length.Double() - 1);
                            for (int i = 0; i < colors.Length; i++, offset += j)
                            {
                                var a = colors[i];

                                gradient.Add(a);
                                if (i < colors.Length - 1 && totalProgress > offset && totalProgress < offset + j)
                                {
                                    var b = colors[i + 1];

                                    var localProgress = (totalProgress - offset) / j;
                                    result = a.Blend(b, Media.BlendModes.Normal, localProgress);
                                    gradient.Add(result.Value);
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                if ($"{parameter}" == "0")
                                {
                                    var g = new LinearGradientBrush();
                                    for (var i = 0; i < gradient.Count; i++)
                                        g.GradientStops.Add(new GradientStop(gradient[i], i.Double() / (gradient.Count.Double() - 1)));

                                    return g;
                                }
                                else return new SolidColorBrush(result.Value);
                            }
                        }
                    }
                }
            }
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(object))]
public class PropertyValueMultiConverter : MultiConverter<object>
{
    public PropertyValueMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
        {
            if (values[0] is object source)
            {
                if (values[1] is string propertyName)
                    return Try.Return(() => source.GetPropertyValue(propertyName));
            }
        }
        return Binding.DoNothing;
    }
}