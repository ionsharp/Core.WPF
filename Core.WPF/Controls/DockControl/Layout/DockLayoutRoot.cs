using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Imagin.Core.Controls
{
    [Serializable]
    public abstract class DockLayoutRoot : Base
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Panel")]
        public List<DockLayoutPanel> Top { get => Get(new List<DockLayoutPanel>()); set => Set(value); }

        [XmlArray]
        [XmlArrayItem(ElementName = "Panel")]
        public List<DockLayoutPanel> Left { get => Get(new List<DockLayoutPanel>()); set => Set(value); }

        [XmlArray]
        [XmlArrayItem(ElementName = "Panel")]
        public List<DockLayoutPanel> Right { get => Get(new List<DockLayoutPanel>()); set => Set(value); }

        [XmlArray]
        [XmlArrayItem(ElementName = "Panel")]
        public List<DockLayoutPanel> Bottom { get => Get(new List<DockLayoutPanel>()); set => Set(value); }

        public DockLayoutElement Root { get => Get<DockLayoutElement>(); set => Set(value); }

        public DockLayoutRoot() : base() { }
    }
}