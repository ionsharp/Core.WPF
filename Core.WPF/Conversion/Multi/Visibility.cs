using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class AandBorCMultiConverter : MultiConverter<Visibility>
{
    public static AandBorCMultiConverter Default { get; private set; } = new();
    AandBorCMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 3)
        {
            if (values[0] is bool a)
            {
                if (values[1] is bool b)
                {
                    if (values[2] is bool c)
                    {
                        return (a && (b || c)).Visibility();
                    }
                }
            }
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class BooleanToVisibilityMultiConverter : MultiConverter<Visibility>
{
    public static BooleanToVisibilityMultiConverter Default { get; private set; } = new BooleanToVisibilityMultiConverter();
    BooleanToVisibilityMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length > 0)
        {
            foreach (var i in values)
            {
                if (i is bool a)
                {
                    if (!a)
                        return Visibility.Collapsed;
                }
                if (i is Visibility b)
                {
                    if (b != Visibility.Visible)
                        return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }
        return Binding.DoNothing;
    }
}

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class EnumFlagsToVisibilityMultiConverter : MultiConverter<Visibility>
{
    public static EnumFlagsToVisibilityMultiConverter Default { get; private set; } = new EnumFlagsToVisibilityMultiConverter();
    EnumFlagsToVisibilityMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        object a = null, b = null;

        var Result = true;

        var i = 0;
        foreach (var j in values)
        {
            if (Numerics.M.Even(i))
            {
                a = j;
            }
            else
            {
                b = j;
                if (a != null && b != null)
                {
                    Result = Result && a.As<Enum>().HasFlag(b as Enum);
                    a = null;
                    b = null;
                }
            }
            i++;
        }

        return Result.Visibility();
    }
}

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class ItemVisibilityConverter : MultiConverter<Visibility>
{
    public static ItemVisibilityConverter Default { get; private set; } = new ItemVisibilityConverter();
    ItemVisibilityConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        //(0) Item
        //(1) File attributes
        //(2) File extensions
        //(3) Folder attributes
        //(4) View files
        if (values?.Length == 5)
        {
            if (values[0] is Item item)
            {
                if (values[1] is Attributes fileAttributes)
                {
                    if (values[3] is Attributes folderAttributes)
                    {
                        if (values[4] is bool viewFiles)
                        {
                            if (item is Storage.File file)
                            {
                                if (viewFiles)
                                {
                                    if (!fileAttributes.HasFlag(Attributes.Hidden))
                                    {
                                        if (file.IsHidden)
                                            return Visibility.Collapsed;
                                    }

                                    if (!fileAttributes.HasFlag(Attributes.ReadOnly))
                                    {
                                        if (file.IsReadOnly)
                                            return Visibility.Collapsed;
                                    }

                                    if (values[2] is string fileExtensions)
                                    {
                                        var e = fileExtensions.Split(XArray.New<char>(';'), StringSplitOptions.RemoveEmptyEntries).Select(i => i.TrimExtension());
                                        if (!e.Any() || e.Contains(Path.GetExtension(file.Path).TrimExtension()))
                                            return Visibility.Visible;

                                        return Visibility.Collapsed;
                                    }

                                    return Visibility.Visible;
                                }
                            }
                            else if (item is Folder folder)
                            {
                                if (!folderAttributes.HasFlag(Attributes.Hidden))
                                {
                                    if (folder.IsHidden)
                                        return Visibility.Collapsed;
                                }

                                if (!folderAttributes.HasFlag(Attributes.ReadOnly))
                                {
                                    if (folder.IsReadOnly)
                                        return Visibility.Collapsed;
                                }

                                return Visibility.Visible;
                            }
                            else if (item is Drive)
                            {
                                return Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }
        return Visibility.Collapsed;
    }
}

[ValueConversion(typeof(object[]), typeof(Visibility))]
public class StartsWithToVisibilityMultiConverter : MultiConverter<Visibility>
{
    public static StartsWithToVisibilityMultiConverter Default { get; private set; } = new StartsWithToVisibilityMultiConverter();
    StartsWithToVisibilityMultiConverter() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 2)
        {

            var input = values[0].ToString();
            var query = values[1].ToString();

            if (query.NullOrEmpty())
            {
                return Visibility.Visible;
            }

            if (input.StartsWith(query))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
        return null;
    }
}