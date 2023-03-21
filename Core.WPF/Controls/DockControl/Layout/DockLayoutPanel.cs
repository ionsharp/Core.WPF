using Imagin.Core.Numerics;
using System;
using System.Xml.Serialization;

namespace Imagin.Core.Controls
{
    [Serializable]
    public class DockLayoutPanel : Base
    {
        [XmlAttribute]
        public bool IsSelected { get => Get(false); set => Set(value); }

        [XmlAttribute]
        public string Name { get => Get(""); set => Set(value); }

        DockLayoutPanel() : base() { }

        public DockLayoutPanel(string name) : base() => Name = name;

        public DockLayoutPanel(Models.Panel panel) : this(panel.Name) => IsSelected = panel.IsSelected;
    }
}