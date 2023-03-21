using System.Globalization;
using System.Linq;
using Imagin.Core.Linq;
using System.IO;
using System.Windows.Controls;

namespace Imagin.Core.Validation;

public class FileExtensionRule : ValidationRule
{
    public bool IsOptional
    {
        get; set;
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string result = value?.ToString();

        if (!IsOptional && result.NullOrEmpty())
            return new ValidationResult(false, $"File extension wasn't specified.");

        var clean = Path.GetInvalidFileNameChars().Aggregate(result, (i, j) => i.Replace(j.ToString(), ""));
        if (result != clean)
            return new ValidationResult(false, $"File extension is invalid.");

        return new ValidationResult(true, null);
    }
}