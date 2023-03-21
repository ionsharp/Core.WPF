using System.Globalization;
using System.Windows.Controls;

namespace Imagin.Core.Validation;

public class NegativeIndexRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string result = value?.ToString();

        if (value is int index)
        {
            if (index >= 0)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, $"A selection is required.");
            }
        }
        else return new ValidationResult(false, $"A valid selection is required.");
    }
}