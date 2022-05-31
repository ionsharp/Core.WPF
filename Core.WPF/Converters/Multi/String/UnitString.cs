using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Converters
{
    [ValueConversion(typeof(object[]), typeof(string))]
    public class UnitStringMultiConverter : MultiConverter<string>
    {
        public static UnitStringMultiConverter Default { get; private set; } = new UnitStringMultiConverter();
        UnitStringMultiConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length >= 2 && values[0]?.IsAny(typeof(double), typeof(double?), typeof(int), typeof(int?)) == true && values[1] is Unit)
            {
                var value = double.Parse(values[0].ToString());

                var funit = parameter is Unit ? (Unit)parameter : Unit.Pixel;
                var tunit = values[1].As<Unit>();

                var resolution = values.Length > 2 ? (float)values[2] : 72f;
                var places = values.Length > 3 ? (int)values[3] : 3;

                return $"{value.Convert(funit, tunit, resolution).Round(places)} {tunit.GetAttribute<AbbreviationAttribute>().Abbreviation}";
            }
            return string.Empty;
        }
    }
}