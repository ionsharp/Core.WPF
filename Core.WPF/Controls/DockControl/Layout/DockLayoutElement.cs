using System;
using System.Xml.Serialization;

namespace Imagin.Core.Controls;

[Serializable]
public abstract class DockLayoutElement : Base
{
    [XmlAttribute]
    public double MinHeight { get => Get(double.NaN); set => Set(value); }

    [XmlAttribute]
    public string Height { get => Get("1*"); set => Set(value); }

    [XmlAttribute]
    public double MaxHeight { get => Get(double.NaN); set => Set(value); }

    [XmlAttribute]
    public double MinWidth { get => Get(double.NaN); set => Set(value); }

    [XmlAttribute]
    public string Width { get => Get("1*"); set => Set(value); }

    [XmlAttribute]
    public double MaxWidth { get => Get(double.NaN); set => Set(value); }

    public DockLayoutElement() : base() { }
}