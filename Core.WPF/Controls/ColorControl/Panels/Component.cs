using Imagin.Core.Models;
using System;

namespace Imagin.Core.Controls
{
    [MemberVisibility(Property: MemberVisibility.Explicit)]
    public class ComponentPanel : Panel
    {
        public override Uri Icon => null;

        public override string Title => "Component";

        public override bool TitleVisibility => false;

        public ComponentPanel() : base() { }
    }
}