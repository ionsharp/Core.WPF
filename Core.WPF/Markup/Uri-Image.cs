using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace Imagin.Core.Markup;

public class Image : Uri
{
    public Image() : base() { }

    public Image(string fileName) : base($"Images/{fileName}", AssemblyType.Current) { }

    public Image(string relativePath, AssemblyType assembly) : base(relativePath, assembly) { }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var result = (System.Uri)base.ProvideValue(serviceProvider);
        return result.GetImage();
    }
}

public class SmallImage : Image
{
    public SmallImages Image { set => RelativePath = $"Images/{value}.png"; }

    public SmallImage() : base(null, AssemblyType.Core) { }

    public SmallImage(string fileName) : base($"Images/{fileName}", AssemblyType.Core) { }
}

public class LargeImage : Image
{
    public LargeImages Image { set => RelativePath = GetRelativePath(value); }

    public LargeImage() : base(null, AssemblyType.Core) { }

    public static string GetRelativePath(LargeImages image) => $"Images/256-{image}.png";

    public static System.Uri GetUri(LargeImages image) => Resource.GetUri(GetRelativePath(image), AssemblyType.Core);
}