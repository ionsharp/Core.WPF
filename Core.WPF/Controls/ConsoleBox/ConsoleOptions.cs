using Imagin.Core.Conversion;
using Imagin.Core.Media;
using System;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Core.Controls;

[Name(nameof(ConsoleOptions)), Serializable]
public class ConsoleOptions : ControlOptions<ConsoleBox>
{
    public SolidColorBrush Background { get => GetFrom(Brushes.Black, Converter.Get<SolidColorBrushToStringConverter>()); set => SetFrom(value, Converter.Get<SolidColorBrushToStringConverter>()); }

    [StringStyle(StringStyle.FilePath)]
    public string BackgroundImage { get => Get(""); set => Set(value); }

    public Stretch BackgroundStretch { get => GetFromString(Stretch.Fill); set => SetFromString(value); }

    public FontFamily FontFamily { get => GetFrom(new FontFamily("Consolas"), Converter.Get<FontFamilyToStringConverter>()); set => SetFrom(value, Converter.Get<FontFamilyToStringConverter>()); }

    [Range(12.0, 48.0, 1.0, Style = RangeStyle.Both)]
    public double FontSize { get => Get(16.0); set => Set(value); }

    public SolidColorBrush Foreground { get => GetFrom(Brushes.White, Converter.Get<SolidColorBrushToStringConverter>()); set => SetFrom(value, Converter.Get<SolidColorBrushToStringConverter>()); }

    [Hide]
    public string Output { get => Get(""); set => Set(value); }

    public TextWrapping TextWrap { get => GetFromString(TextWrapping.NoWrap); set => SetFromString(value); }

    public ConsoleOptions() : base() { }
}