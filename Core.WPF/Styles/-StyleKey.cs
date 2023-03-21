using Imagin.Core.Linq;
using Imagin.Core.Markup;
using Imagin.Core.Reflection;

namespace Imagin.Core.Controls;

/// <summary>See <see cref="StyleKeys"/>.</summary>
public sealed class StyleKey : Uri
{
    public const string KeyFormat = "Styles/{0}.xaml";

    public StyleKeys Key { set => RelativePath = KeyFormat.F(value); }

    public StyleKey() : base() => Assembly = AssemblyType.Core;
}