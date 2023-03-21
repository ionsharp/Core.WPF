using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System.IO;
using System.Windows.Data;
using System;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(double), typeof(string))]
public class DoubleFileSizeConverter : ValueConverter<double, string>
{
    public DoubleFileSizeConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<double> input)
        => new FileSize(input.Value.Int64()).ToString(input.ActualParameter is FileSizeFormat i ? i : FileSizeFormat.BinaryUsingSI);

    protected override ConverterValue<double> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class FileExtensionConverter : ValueConverter<string, string>
{
    public FileExtensionConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
    {
        string result = null;
        return !Try.Invoke(() => result = Path.GetExtension(input.Value))
            ? (ConverterValue<string>)Nothing.Do
            : input.Parameter == 0 ? result.Replace(".", string.Empty) : input.Parameter == 1 ? result : throw input.InvalidParameter;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(object), typeof(string))]
public class FileNameConverter : ValueConverter<object, string>
{
    public FileNameConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<object> input)
    {
        var path = input.Value.ToString();

        if (path == StoragePath.Root)
            return StoragePath.RootName;

        if (path.EndsWith(@":\"))
        {
            foreach (var i in Computer.Drives)
            {
                if (path.Equals(i.Name))
                    return $"{i.VolumeLabel} ({i.Name.Replace(@"\", string.Empty)})";
            }
            return path;
        }

        return Folder.Long.Exists(path) || input.Parameter == 1
            ? Path.GetFileName(path)
            : input.Parameter == 0
                ? Path.GetFileNameWithoutExtension(path)
                : throw input.InvalidParameter;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(long), typeof(string))]
public class FileSizeConverter : ValueConverter<long, string>
{
    public FileSizeConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<long> input)
        => new FileSize(input.Value).ToString(input.ActualParameter is FileSizeFormat i ? i : FileSizeFormat.BinaryUsingSI);

    protected override ConverterValue<long> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(double), typeof(string))]
public class FileSpeedConverter : ValueConverter<double, string>
{
    public FileSpeedConverter() : base() { }

    protected override bool Is(object input) => input is double || input is string;

    protected override ConverterValue<string> ConvertTo(ConverterData<double> input)
    {
        long result = 0;

        if (input.ActualValue is double a)
            result = a.Int64();

        if (input.ActualValue is string b)
            result = b.Int64();

        return $"{new FileSize(result).ToString(FileSizeFormat.BinaryUsingSI)}/s";
    }

    protected override ConverterValue<double> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(Uri), typeof(string))]
public class UriFileNameConverter : ValueConverter<Uri, string>
{
    public UriFileNameConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<Uri> input)
    {
        string x = "", y = "";
        foreach (var i in input.Value.OriginalString)
        {
            if (i == '/')
            {
                x = "";
            }
            else x += i;
        }
        foreach (var i in x)
        {
            if (i == '.')
                return y;

            y += i;
        }
        return x;
    }

    protected override ConverterValue<Uri> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(string), typeof(string))]
public class ItemAccessedConverter : ValueConverter<string, string>
{
    public ItemAccessedConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
        => new FileInfo(input.Value).LastAccessTime.ToString();

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class ItemCreatedConverter : ValueConverter<string, string>
{
    public ItemCreatedConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
        => new FileInfo(input.Value).CreationTime.ToString();

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class ItemDescriptionConverter : ValueConverter<string, string>
{
    public ItemDescriptionConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input) => Computer.FriendlyDescription(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class ItemModifiedConverter : ValueConverter<string, string>
{
    public ItemModifiedConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
        => new FileInfo(input.Value).LastWriteTime.ToString();

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(string))]
public class ItemSizeConverter : ValueConverter<string, string>
{
    public ItemSizeConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input)
        => new FileSize(new FileInfo(input.Value).Length).ToString(input.ActualParameter is FileSizeFormat i ? i : FileSizeFormat.BinaryUsingSI);

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(ItemType))]
public class ItemTypeConverter : ValueConverter<string, ItemType>
{
    public ItemTypeConverter() : base() { }

    protected override ConverterValue<ItemType> ConvertTo(ConverterData<string> input) => Computer.GetType(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<ItemType> input) => Nothing.Do;
}

///

[ValueConversion(typeof(string), typeof(string))]
public class ShortcutLocationConverter : ValueConverter<string, string>
{
    public ShortcutLocationConverter() : base() { }

    protected override ConverterValue<string> ConvertTo(ConverterData<string> input) => Shortcut.TargetPath(input.Value);

    protected override ConverterValue<string> ConvertBack(ConverterData<string> input) => Nothing.Do;
}

///

[ValueConversion(typeof(string), typeof(double))]
public class DriveAvailableSizeConverter : ValueConverter<string, double>
{
    public DriveAvailableSizeConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<string> input)
    {
        Try.Invoke(out double result, () =>
        {
            foreach (var i in Computer.Drives)
            {
                if (i.Name.ToLower() == input.Value.ToLower())
                    return i.AvailableFreeSpace.Double();
            }
            return 0;
        });
        return result;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(bool))]
public class DriveSizeLowConverter : ValueConverter<string, bool>
{
    public DriveSizeLowConverter() : base() { }

    protected override ConverterValue<bool> ConvertTo(ConverterData<string> input)
    {
        Try.Invoke(out bool result, () =>
        {
            foreach (var i in Computer.Drives)
            {
                if (i.Name.ToLower() == input.Value.ToLower())
                    return i.AvailableFreeSpace < 10000000000L;
            }
            return false;
        });
        return result;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<bool> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(double))]
public class DriveTotalSizeConverter : ValueConverter<string, double>
{
    public DriveTotalSizeConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<string> input)
    {
        Try.Invoke(out double result, () =>
        {
            foreach (var i in Computer.Drives)
            {
                if (i.Name.ToLower() == input.Value.ToLower())
                    return i.TotalSize.Double();
            }
            return 0;
        });
        return result;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(double))]
public class DriveUsedPercentConverter : ValueConverter<string, double>
{
    public DriveUsedPercentConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<string> input)
    {
        Try.Invoke(out double result, () =>
        {
            foreach (var i in Computer.Drives)
            {
                if (i.Name.ToLower() == input.Value.ToLower())
                    return i.TotalSize.Double() == 0 ? 0 : (i.TotalSize.Double() - i.AvailableFreeSpace.Double()) / i.TotalSize.Double();
            }
            return 0;
        });
        return result;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<double> input) => Nothing.Do;
}

[ValueConversion(typeof(string), typeof(double))]
public class DriveUsedSizeConverter : ValueConverter<string, double>
{
    public DriveUsedSizeConverter() : base() { }

    protected override ConverterValue<double> ConvertTo(ConverterData<string> input)
    {
        Try.Invoke(out double result, () =>
        {
            foreach (var i in Computer.Drives)
            {
                if (i.Name.ToLower() == input.Value.ToLower())
                    return i.TotalSize.Double() - i.AvailableFreeSpace.Double();
            }
            return 0;
        });
        return result;
    }

    protected override ConverterValue<string> ConvertBack(ConverterData<double> input) => Nothing.Do;
}