using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Imagin.Core.Reflection;

namespace Imagin.Core.Linq;

public static partial class XType
{
    /// <summary>https://stackoverflow.com/questions/299515/reflection-to-identify-extension-methods</summary>
    public static IEnumerable<MethodInfo> GetExtensionMethods(this Type input, AssemblyType assemblyType = AssemblyType.Core)
    {
        var assembly = XAssembly.GetAssembly(assemblyType);
        var query = from type in assembly.GetTypes()
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where method.GetParameters()[0].ParameterType == input
            select method;
        return query;
    }
}