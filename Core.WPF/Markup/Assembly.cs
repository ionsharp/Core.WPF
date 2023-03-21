using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows.Markup;

namespace Imagin.Core.Markup;

public abstract class AssemblyExtension : MarkupExtension
{
    public string AssemblyName { get; set; }

    AssemblyType assembly = AssemblyType.Unspecified;
    public AssemblyType Assembly
    {
        get => assembly;
        set
        {
            assembly = value;
            AssemblyName = XAssembly.GetAssemblyName(value);
        }
    }

    public AssemblyExtension() : this(AssemblyType.Current) { }

    public AssemblyExtension(AssemblyType assembly) : base() => Assembly = assembly;
}

///

public sealed class AssemblyCopyright : AssemblyExtension
{
    public AssemblyCopyright() : base() { }

    public AssemblyCopyright(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Copyright;
}

public sealed class AssemblyDescription : AssemblyExtension
{
    public AssemblyDescription() : base() { }

    public AssemblyDescription(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Description;
}

public sealed class AssemblyFileVersion : AssemblyExtension
{
    public AssemblyFileVersion() : base() { }

    public AssemblyFileVersion(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).FileVersion.ShortVersion();
}

///

public class AssemblyName : AssemblyExtension
{
    public AssemblyName() : base() { }

    public AssemblyName(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Name;
}

public sealed class DefaultAssemblyName : AssemblyName
{
    public DefaultAssemblyName() : base(AssemblyType.Core) { }
}

///

public sealed class AssemblyProduct : AssemblyExtension
{
    public AssemblyProduct() : base() { }

    public AssemblyProduct(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Product;
}

public sealed class AssemblyTitle : AssemblyExtension
{
    public AssemblyTitle() : base() { }

    public AssemblyTitle(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Title;
}

[Obsolete("Not discoverable. Use 'AssemblyFileVersion' instead.")]
public class AssemblyVersion : AssemblyExtension
{
    public AssemblyVersion() : base() { }

    public AssemblyVersion(AssemblyType assembly) : base(assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => XAssembly.GetProperties(AssemblyName).Version.ShortVersion();
}

///

public sealed class AssemblyIcon : Image
{
    public AssemblyIcon() : base("Logo.ico", AssemblyType.Current) { }

    public AssemblyIcon(AssemblyType assembly) : base("Logo.ico", assembly) { }
}