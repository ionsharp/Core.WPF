using Imagin.Core.Numerics;
using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Imagin.Core.Media
{
    [Serializable]
    public class GradientStep : Base
    {
        ByteVector4 color = default;
        [XmlElement]
        public ByteVector4 Color
        {
            get => color;
            set => this.Change(ref color, value);
        }

        double offset = 0;
        [XmlAttribute]
        public double Offset
        {
            get => offset;
            set => this.Change(ref offset, value);
        }

        public GradientStep() : base() { }

        public GradientStep(double offset, ByteVector4 color) : this()
        {
            Offset = offset;
            Color = color;
        }

        public GradientStep(double offset, Color color) : this(offset, new ByteVector4(color.R, color.G, color.B, color.A)) { }
    }
}