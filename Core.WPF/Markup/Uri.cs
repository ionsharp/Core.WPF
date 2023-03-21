using System;
using Imagin.Core.Reflection;

namespace Imagin.Core.Markup;

public class Uri : AssemblyExtension
{
    public string RelativePath { get; set; }

    public Uri() : this(null) { }

    public Uri(string relativePath) : this(relativePath, AssemblyType.Current) { }

    public Uri(string relativePath, AssemblyType assembly) : base(assembly)
        => RelativePath = relativePath;

    public Uri(string assemblyName, string relativePath) : this(relativePath)
        => AssemblyName = assemblyName;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (AssemblyName == null)
            return new System.Uri(RelativePath, UriKind.Relative);

        return Resource.GetUri(AssemblyName, RelativePath);
    }
}