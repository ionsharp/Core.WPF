using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System.Windows;
using System.Windows.Media;
using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Controls
{
    public class AlphaSlider : BaseComponentSlider
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(AlphaSlider), new FrameworkPropertyMetadata(default(Color)));
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(byte), typeof(AlphaSlider), new FrameworkPropertyMetadata(default(byte), OnValueChanged));
        public byte Value
        {
            get => (byte)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => sender.As<AlphaSlider>().OnValueChanged(e);

        public AlphaSlider() : base() { }

        protected override void Mark() => ArrowPosition.Y = ((1 - (Value.Double() / 255)) * ActualHeight) - 8;

        protected override void OnMouseChanged(Vector2<One> input)
        {
            base.OnMouseChanged(input);
            SetCurrentValue(ValueProperty, Clamp(input.Y.Value * 255, 255).Byte());
        }

        protected virtual void OnValueChanged(Value<byte> input) => Mark();
    }
}