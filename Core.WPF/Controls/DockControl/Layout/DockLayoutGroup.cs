using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Imagin.Core.Controls
{
    [Serializable]
    public class DockLayoutGroup : DockLayoutElement
    {
        [XmlArray]
        [XmlArrayItem("Element")]
        public ObservableCollection<DockLayoutElement> Elements { get => Get(new ObservableCollection<DockLayoutElement>()); set => Set(value); }

        [XmlAttribute]
        public Orientation Orientation { get => Get(Orientation.Horizontal); set => Set(value); }

        public DockLayoutGroup() : base() { }

        public DockLayoutGroup(Orientation orientation) : base()
        {
            Orientation = orientation;
        }
    }
}