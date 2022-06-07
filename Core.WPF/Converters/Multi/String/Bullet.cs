using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Text;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Converters;

[ValueConversion(typeof(object[]), typeof(string))]
public class BulletMultiConverter : MultiConverter<string>
{
    public static BulletMultiConverter Default { get; private set; } = new BulletMultiConverter();
    BulletMultiConverter() { }

    //...

    object TryConvert(Bullets bullet, double index)
    {
        object result = Binding.DoNothing;
        Try.Invoke(out result, () =>
        {
            var i = index.Int32();
            return bullet switch
            {
                Bullets.LetterUpperPeriod => $"{Number.Convert(i, NumberStyle.Letter)}.".ToUpper(),
                Bullets.LetterUpperParenthesis => $"{Number.Convert(i, NumberStyle.Letter)})".ToUpper(),
                Bullets.LetterLowerPeriod => $"{Number.Convert(i, NumberStyle.Letter)}.".ToLower(),
                Bullets.LetterLowerParenthesis => $"{Number.Convert(i, NumberStyle.Letter)})".ToLower(),
                Bullets.NumberPeriod => $"{index}.",
                Bullets.NumberParenthesis => $"{index})",
                Bullets.RomanNumberUpperPeriod => $"{Number.Convert(i, NumberStyle.Roman)}.".ToUpper(),
                Bullets.RomanNumberUpperParenthesis => $"{Number.Convert(i, NumberStyle.Roman)})".ToUpper(),
                Bullets.RomanNumberLowerPeriod => $"{Number.Convert(i, NumberStyle.Roman)}.".ToLower(),
                Bullets.RomanNumberLowerParenthesis => $"{Number.Convert(i, NumberStyle.Roman)})".ToLower(),
                _ => throw new NotSupportedException(),
            };
        });
        return result;
    }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 0)
        {
            if (values[0] is Bullets bullet)
            {
                switch (values.Length)
                {
                    case 1:
                        return TryConvert(bullet, 1);

                    case 2:
                        double.TryParse($"{values[1]}", out double index);
                        return TryConvert(bullet, index);
                }
            }
        }
        return Binding.DoNothing;
    }
}