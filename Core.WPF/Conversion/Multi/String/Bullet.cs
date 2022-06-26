using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Text;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(string))]
public class BulletMultiConverter : MultiConverter<string>
{
    public static BulletMultiConverter Default { get; private set; } = new BulletMultiConverter();
    BulletMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 0)
        {
            if (values[0] is Bullets bullet)
            {
                switch (values.Length)
                {
                    case 1:
                        return bullet.ToString(1);

                    case 2:
                        double.TryParse($"{values[1]}", out double index);
                        return bullet.ToString(index);
                }
            }
        }
        return Binding.DoNothing;
    }
}