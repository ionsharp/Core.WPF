using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Imagin.Core.Validation;

public class FolderExistsRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string result = value?.ToString();

        if (string.IsNullOrEmpty(result))
            return new ValidationResult(false, $"Folder not specified.");

        if (!Directory.Exists(result))
            return new ValidationResult(false, $"Folder '{result}' doesn't exist.");

        return new ValidationResult(true, null);
    }
}