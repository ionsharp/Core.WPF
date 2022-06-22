using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;

namespace Imagin.Core.Controls
{
    [MemberVisibility(Property: MemberVisibility.Explicit)]
    public class ColorProfilePanel : Panel
    {
        public override Uri Icon => Resources.InternalImage(Images.Checkers);

        public override string Title => "Profile";

        public ColorProfilePanel() : base() { }
    }
}