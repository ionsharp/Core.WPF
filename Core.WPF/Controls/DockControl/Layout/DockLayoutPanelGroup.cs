using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Imagin.Core.Controls
{
    [Serializable]
    public class DockLayoutPanelGroup : DockLayoutElement
    {
        [XmlAttribute]
        public bool Collapse { get => Get(false); set => Set(value); }

        [XmlArray]
        public List<DockLayoutPanel> Panels { get => Get(new List<DockLayoutPanel>()); set => Set(value); }

        public DockLayoutPanelGroup() : base() { }

        public DockLayoutPanelGroup(IEnumerable<DockLayoutPanel> input) : base() => input?.ForEach(Panels.Add);

        public DockLayoutPanelGroup(params DockLayoutPanel[] input) : this(input as IEnumerable<DockLayoutPanel>) { }

        public DockLayoutPanelGroup(params Panel[] input) : this(input?.Select(i => new DockLayoutPanel((i as Panel).Name))) { }

        public DockLayoutPanelGroup(IEnumerable<Panel> input) : this(input?.ToArray()) { }

        public DockLayoutPanelGroup(params string[] input) : this(input?.Select(i => new DockLayoutPanel(i.ToString()))) { }
    }
}