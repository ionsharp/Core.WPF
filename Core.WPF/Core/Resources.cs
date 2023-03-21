using Imagin.Core.Analytics;
using Imagin.Core.Collections.Generic;
using Imagin.Core.Reflection;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

namespace Imagin.Core;

public class Resources : ResourceDictionary, IPropertyChanged
{
    #region IPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    [NonSerialized]
    readonly ObjectDictionary NonSerializedProperties = new();
    ObjectDictionary IPropertyChanged.NonSerializedProperties => NonSerializedProperties;

    readonly ObjectDictionary SerializedProperties = new();
    ObjectDictionary IPropertyChanged.SerializedProperties => SerializedProperties;

    #endregion

    public static ResourceDictionary Load(string filePath)
    {
        var result = default(ResourceDictionary);
        using (var fileStream = File.OpenRead(filePath))
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            result = (ResourceDictionary)XamlReader.Load(fileStream);
        }
        return result;
    }

    public static ResourceDictionary Load(Uri fileUri)
    {
        using var stream = Application.GetResourceStream(fileUri).Stream;
        return (ResourceDictionary)XamlReader.Load(stream);
    }

    public static Result TryLoad(string filePath, out ResourceDictionary result)
    {
        try
        {
            result = Load(filePath);
            return new Success();
        }
        catch (Exception e)
        {
            result = default;
            return new Error(e);
        }
    }

    ///

    public static ResourceDictionary New(string assemblyName, string relativePath) => new() { Source = Resource.GetUri(assemblyName, relativePath) };

    public static void Save(string assemblyName, string resourcePath, string destinationPath)
    {
        using var fileStream = File.Create(destinationPath);
        using var stream = Application.GetResourceStream(Resource.GetUri(assemblyName, resourcePath)).Stream;
        stream.Seek(0, SeekOrigin.Begin);
        stream.CopyTo(fileStream);
    }

    ///

    public static Stream GetStream(Uri uri) => Application.GetResourceStream(uri).Stream;

    public static string GetText(string relativePath, AssemblyType assembly = AssemblyType.Current)
    {
        var uri = Resource.GetUri(relativePath, assembly);

        string result = default;
        using (var stream = Application.GetResourceStream(uri).Stream)
        {
            using var reader = new StreamReader(stream);
            string line;
            while ((line = reader.ReadLine()) != null)
                result += $"{line}\n";
        }
        return result;
    }

    ///

    public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

    public virtual void OnPropertyChanged(PropertyEventArgs e) { }

    public virtual void OnPropertyChanging(PropertyChangingEventArgs e) { }

    public virtual void OnPropertyGet(GetPropertyEventArgs e) { }
}