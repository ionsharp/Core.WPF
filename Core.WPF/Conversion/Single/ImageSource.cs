using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(string), typeof(ImageSource))]
public class StringToImageSourceConverter : ValueConverter<string, ImageSource>
{
    public StringToImageSourceConverter() : base() { }

    protected override ConverterValue<ImageSource> ConvertTo(ConverterData<string> input)
    {
        var i = new BitmapImage();

        i.BeginInit();
        i.UriSource = new Uri(input.Value, UriKind.Absolute);
        i.EndInit();

        return i;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<ImageSource> input) => Nothing.Do;
}

[ValueConversion(typeof(Uri), typeof(ImageSource))]
public class UriToImageSourceConverter : ValueConverter<Uri, ImageSource>
{
    public UriToImageSourceConverter() : base() { }

    protected override ConverterValue<ImageSource> ConvertTo(ConverterData<Uri> input)
    {
        var i = new BitmapImage();

        i.BeginInit();
        i.UriSource = input.Value;
        i.EndInit();

        return i;
    }

    protected override ConverterValue<Uri> ConvertBack(ConverterData<ImageSource> input) => Nothing.Do;
}