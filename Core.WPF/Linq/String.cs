using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace Imagin.Core.Linq;

public static partial class XString
{
    public static string Translate(this string input, string prefix = "", string suffix = "", string format = null)
    {
        var result = (string)LocExtension.GetLocalizedValue(typeof(string), input, LocalizeDictionary.Instance.SpecificCulture, null);
        return result.NullOrEmpty() ? Markup.LocExtension.MissingKeyFormat.F(prefix, format?.F(input) ?? input, suffix) : $"{prefix}{format?.F(result) ?? result}{suffix}";
    }
}