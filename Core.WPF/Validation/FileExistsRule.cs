using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Imagin.Core.Validation;

public class FileExistsRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string result = value?.ToString();

        if (string.IsNullOrEmpty(result))
            return new ValidationResult(false, $"File not specified.");

        if (!File.Exists(result))
            return new ValidationResult(false, $"File '{result}' doesn't exist.");

        return new ValidationResult(true, null);
    }
}