using Imagin.Core.Linq;
using System;

namespace Imagin.Core.Markup
{
    public class Image : Uri
    {
        public Image() : base() { }

        public Image(string relativePath) : base(relativePath) { }

        public Image(string assembly, string relativePath) : base(assembly, relativePath) { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = (System.Uri)base.ProvideValue(serviceProvider);
            return result.GetImage();
        }
    }

    public class InternalImage : Image
    {
        public Images Image { set => RelativePath = $"Images/{value}.png"; }

        public override string Assembly => AssemblyProperties.Name;

        public InternalImage() : base() { }

        public InternalImage(string fileName) : base($"Images/{fileName}") { }
    }

    public class ProjectImage : Image
    {
        public override string Assembly => XAssembly.ShortName();

        public ProjectImage(string fileName) : base($"Images/{fileName}") { }
    }
}