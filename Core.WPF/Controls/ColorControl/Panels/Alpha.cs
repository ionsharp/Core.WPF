using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;

namespace Imagin.Core.Controls
{
    [MemberVisibility(Property: MemberVisibility.Explicit)]
    public class AlphaPanel : Panel
    {
        public override Uri Icon => null;

        public override string Title => "Alpha";

        public override bool TitleVisibility => false;

        public AlphaPanel() : base() { }
    }
}