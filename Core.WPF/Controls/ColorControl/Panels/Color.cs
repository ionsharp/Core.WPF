using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls
{
    [Explicit]
    public class ColorPanel : Panel
    {
        public override Uri Icon => Resources.InternalImage(Images.Color);

        public override string Title => "Color";
        
        public ColorPanel() : base() { }
    }
}