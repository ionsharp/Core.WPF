using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Controls
{
    [DisplayName(nameof(ConsoleOptions))]
    [Serializable]
    public class ConsoleOptions : ControlOptions<Console>
    {
        ByteVector4 background = new(0, 0, 0, 255);
        public SolidColorBrush Background
        {
            get => XSolidColorBrush.Convert(background);
            set
            {
                value.Convert(out ByteVector4 result);
                this.Change(ref background, result);
            }
        }

        string backgroundImage = string.Empty;
        [FilePath]
        public string BackgroundImage
        {
            get => backgroundImage;
            set => this.Change(ref backgroundImage, value);
        }

        string backgroundStretch = $"{Stretch.Fill}";
        public Stretch BackgroundStretch
        {
            get => (Stretch)Enum.Parse(typeof(Stretch), backgroundStretch);
            set => this.Change(ref backgroundStretch, $"{value}");
        }

        string fontFamily = "Consolas";
        public FontFamily FontFamily
        {
            get
            {
                if (fontFamily == null)
                    return default;

                FontFamily result = null;
                Try.Invoke(() => result = new FontFamily(fontFamily));
                return result;
            }
            set => this.Change(ref fontFamily, value.Source);
        }

        double fontSize = 16.0;
        [Range(12.0, 48.0, 1.0)]
        [SliderUpDown]
        public double FontSize
        {
            get => fontSize;
            set => this.Change(ref fontSize, value);
        }

        ByteVector4 foreground = new(255);
        public SolidColorBrush Foreground
        {
            get => XSolidColorBrush.Convert(foreground);
            set
            {
                value.Convert(out ByteVector4 result);
                this.Change(ref foreground, result);
            }
        }

        string output = string.Empty;
        [Hidden]
        public string Output
        {
            get => output;
            set => this.Change(ref output, value);
        }

        string textWrap = $"{TextWrapping.NoWrap}";
        public TextWrapping TextWrap
        {
            get => (TextWrapping)Enum.Parse(typeof(TextWrapping), textWrap);
            set => this.Change(ref textWrap, value.ToString());
        }

        public ConsoleOptions() : base() { }
    }
}