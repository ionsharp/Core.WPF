using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;

namespace Imagin.Core.Controls
{
    [MemberVisibility(Property: MemberVisibility.Explicit)]
    public class ColorPanel : Panel
    {
        public override Uri Icon => Resources.InternalImage(Images.Color);

        public override string Title => "Color";
        
        public ColorPanel() : base() { }
    }
}