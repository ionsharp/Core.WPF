using System;
using Imagin.Core.Reflection;

namespace Imagin.Core.Markup;

public class Read : Uri
{
    public Read(string relativePath, AssemblyType assembly = AssemblyType.Current) : base(relativePath, assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider) => Resources.GetText(RelativePath, Assembly);
}