using Imagin.Core.Linq;
using XAMLMarkupExtensions.Base;

namespace Imagin.Core.Markup;

public class LocExtension : WPFLocalizeExtension.Extensions.LocExtension
{
    public static string MissingKeyFormat = "{0}[Key: {1}]{2}";

    ///

    public string Format { get; set; }

    ///

    public bool Lower { get; set; }

    public bool Upper { get; set; }

    ///

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    ///

    ///<inheritdoc/>
    public LocExtension() : base() { }

    ///<inheritdoc/>
    public LocExtension(object key) : base(key) { }

    ///

    public override object FormatOutput(TargetInfo endPoint, TargetInfo info)
    {
        var result = base.FormatOutput(endPoint, info) as string;
        if (result != null)
        {
            result = $"{Prefix}{Format?.F(result) ?? result}{Suffix}";
            result = Lower ? result.ToLower() : Upper ? result.ToUpper() : result;
        }
        return result;
    }
}