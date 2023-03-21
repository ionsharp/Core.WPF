using Imagin.Core;
using Imagin.Core.Linq;
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCopyright("No rights reserved.")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyDescription("Common elements for WPF projects.")]
[assembly: AssemblyProduct("Imagin.Core")]
[assembly: AssemblyTrademark("Imagin")]

[assembly: AssemblyCompany("Imagin")]
[assembly: AssemblyTitle("Imagin.Core.Wpf")]

//[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("7.1.0.0")]
[assembly: AssemblyVersion("7.1.0.0")]

//Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: Guid("6bd41976-f6f8-461f-a071-fabc002761c9")]

//In order to begin building localizable applications, set <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file inside a <PropertyGroup>.  For example, if you are using US english in your source files, set the <UICulture> to en-US.  Then uncomment the NeutralResourceLanguage attribute below.  Update the "en-US" in the line below to match the UICulture setting in the project file.
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.None)]

[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Analytics)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Behavior)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Collections)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.CollectionsConcurrent)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.CollectionsObjectModel)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.CollectionsSerialization)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Colors)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Config)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Controls)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Conversion)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Core)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Data)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Effects)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Local)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Input)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Linq)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Markup)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Models)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Numerics)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Media)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Reflection)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Storage)]
[assembly: XmlnsDefinition(Constants.Xmlns, AssemblyPath.Validation)]

//[assembly: XmlnsPrefix(Constants.Xmlns.Root, "i")]

namespace Imagin.Core;

public class Constants
{
    public const string Xmlns = "http://imagin.tech/imagin/wpf";
}