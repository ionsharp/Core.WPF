using Imagin.Core.Reflection;
using System;
using System.Windows.Media;

namespace Imagin.Core.Markup;

public class Font : Uri
{
    public Font(string relativePath, AssemblyType assembly = AssemblyType.Current) : base(relativePath, assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var uri = (System.Uri)base.ProvideValue(serviceProvider);

        FontFamily result = default;
        Try.Invoke(() => result = (FontFamily)new FontFamilyConverter().ConvertFromString(uri.OriginalString));
        return result;
    }
}