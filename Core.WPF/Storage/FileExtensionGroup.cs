using Imagin.Core.Collections.ObjectModel;

namespace Imagin.Core.Storage;

public class FileExtensionGroup : Base
{
    public const string Wild = "*";

    public bool IsWild => FileExtensions.Contains("*");

    public StringCollection FileExtensions { get => Get(new StringCollection()); private set => Set(value); }

    public FileExtensionGroup(params string[] fileExtensions)
    {
        if (fileExtensions?.Length > 0)
        {
            foreach (var i in fileExtensions)
                FileExtensions.Add(i);
        }
    }
}

public class FileExtensionGroups : ObservableCollection<FileExtensionGroup>
{
    public void Add(params string[] fileExtensions) => Add(new FileExtensionGroup(fileExtensions));
}