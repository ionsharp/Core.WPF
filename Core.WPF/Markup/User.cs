using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace Imagin.Core.Markup;

public class UserName : MarkupExtension
{
    public UserName() : base() { }

    public override object ProvideValue(IServiceProvider serviceProvider) => User.Name;
}

public class UserImage : MarkupExtension
{
    public UserImage() : base() { }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        try
        {
            return (ImageSource)new ImageSourceConverter().ConvertFromString($@"C:\Users\{User.Name}\AppData\Local\Temp\{User.Name}.bmp"/*User.Image*/);
        }
        catch { return null; }
    }
}