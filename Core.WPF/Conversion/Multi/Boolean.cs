using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

public class ConditionMultiConverter : MultiConverter<bool>
{
    public static bool Convert(string condition, object[] values)
    {
        bool result = true, o = true;

        Try.Invoke(() =>
        {
            var temp = false;
            for (var i = 0; i < condition.Length; i++)
            {
                if (char.IsNumber(condition[i]))
                {
                    temp = (bool)values[(int)condition[i]];
                    result = o ? result && temp : result || temp;
                }
                else if (condition[i] == '|')
                {
                    o = false;
                }
                else if (condition[i] == '&')
                {
                    o = true;
                }
                else if (condition[i] == '(')
                {
                    var endIndex = i + 1;
                    var endCount = 0;

                    for (var j = i + 1; j < condition.Length; j++)
                    {
                        if (condition[j] == '(')
                        {
                            endCount++;
                        }
                        else if (condition[j] == ')')
                        {
                            if (endCount == 0)
                            {
                                endIndex = j;
                                break;
                            }
                            else endCount--;
                        }
                    }

                    var newCondition = condition.Substring(i + 1, endIndex);

                    temp = Convert(newCondition, values);
                    result = o ? result && temp : result || temp;
                }
            }
        });
        return result;
    }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 0)
        {
            if (parameter is object x && x.ToString() is string y)
                return Convert(y, values);
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(bool))]
public class ValueEqualsParameterMultiConverter : MultiConverter<bool>
{
    public ValueEqualsParameterMultiConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
            return values[0]?.Equals(values[1]);

        return default(bool);
    }
}