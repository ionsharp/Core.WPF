using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using Imagin.Core.Text;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Imagin.Core.Models;

#region FileDocument

[Description("Interface for managing a file."), Name("File document"), Serializable, View(Reflection.MemberView.Tab, typeof(Tab))]
public abstract class FileDocument : Document
{
    enum Category { Attributes, Extension, Format, Properties, Size, Save }

    enum Tab { [Description("File related properties."), Image(SmallImages.File)]File,  }

    #region Properties

    [Category(Category.Properties), Description("When the file was created."), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly, Tab(Tab.File), XmlIgnore]
    public DateTime Created { get => Get(DateTime.Now, false); private set => Set(value, false); }

    [Hide, XmlIgnore]
    public virtual string Extension => System.IO.Path.GetExtension(Path);

    [Hide, XmlIgnore]
    public virtual string[] Extensions => XArray.New<string>(Extension);

    [Category(Category.Extension), Name("Extension"), ReadOnly, Tab(Tab.File), XmlIgnore]
    public string ExtensionDescription => File.Long.Description($".{Extension}");

    [Hide]
    public override object Icon => Path;

    [field: NonSerialized]
    bool isHidden = false;
    [Category(Category.Attributes), Description("If the file is hidden or not."), Name("Hidden"), XmlIgnore, ReadOnly, Tab(Tab.File)]
    public virtual bool IsHidden
    {
        get => isHidden;
        set
        {
            Try.Invoke(() =>
            {
                if (value)
                    File.Long.AddAttribute(Path, System.IO.FileAttributes.Hidden);

                else File.Long.RemoveAttribute(Path, System.IO.FileAttributes.Hidden);
                isHidden = value;
            },
            e => Log.Write<FileDocument>(e));
            Update(() => IsHidden);
        }
    }

    [field: NonSerialized]
    bool isReadOnly = false;
    [Category(Category.Attributes), Description("If the file is read only or not."), Name("ReadOnly"), XmlIgnore, ReadOnly, Tab(Tab.File)]
    public virtual bool IsReadOnly
    {
        get => isReadOnly;
        set
        {
            Try.Invoke(() =>
            {
                if (value)
                    File.Long.AddAttribute(Path, System.IO.FileAttributes.ReadOnly);

                else File.Long.RemoveAttribute(Path, System.IO.FileAttributes.ReadOnly);
                isReadOnly = value;
            },
            e => Log.Write<FileDocument>(e));
            Update(() => IsReadOnly);
        }
    }

    [Category(Category.Properties), Description("When the file was last accessed."), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly, Tab(Tab.File), XmlIgnore]
    [Name("Accessed")]
    public DateTime LastAccessed { get => Get(DateTime.Now, false); private set => Set(value, false); }

    [Footer, Category(Category.Properties), Description("When the file was last modified."), StringStyle(Converter = typeof(RelativeTimeConverter)), ReadOnly, Tab(Tab.File), XmlIgnore]
    [Name("Modified")]
    public DateTime LastModified { get => Get(DateTime.Now, false); private set => Set(value, false); }

    [Pin(Pin.AboveOrLeft), Horizontal, UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Tab(Tab.File), XmlIgnore]
    public string Name
    {
        get => System.IO.Path.GetFileNameWithoutExtension(Path);
        set
        {
            value = StoragePath.CleanName(value).TrimWhitespace();
            if (value.NullOrEmpty())
                value = $"Untitled";

            value += $".{Extension}";

            var folderPath = System.IO.Path.GetDirectoryName(Path);
            if (Folder.Long.Exists(folderPath))
            {
                var oldPath = Path;
                var newPath = $@"{folderPath}\{value}.{Extension}";

                if (!File.Long.Exists(newPath))
                {
                    if (Try.Invoke(() => System.IO.File.Move(oldPath, newPath), e => Log.Write<FileDocument>(e)))
                        value = newPath;

                    else goto skip;
                }
                else
                {
                    Log.Write<FileDocument>(new Error("A file with that name already exists!"));
                    goto skip;
                }

                goto handle;
                skip: 
                { 
                    Update(() => Name); 
                    return; 
                }
            }
            handle: Path = value;
        }
    }

    [Hide, Index(-1), HideName, Footer, StringStyle(StringStyle.Thumbnail), XmlIgnore]
    public string Path { get => Get<string>(null, false); set => Set(value, false); }

    [Category(Category.Size), Description("The size of the file (in bytes)."), StringStyle(Converter = typeof(FileSizeConverter), ConverterParameter = FileSizeFormat.BinaryUsingSI), ReadOnly, Footer, Tab(Tab.File), XmlIgnore]
    public long Size { get => Get(0L); private set => Set(value); }

    [Hide, XmlIgnore]
    public override string Title => $"{System.IO.Path.GetFileNameWithoutExtension(Path)}{(IsModified ? "*" : string.Empty)}";

    [Hide, XmlIgnore]
    public override object ToolTip => Path;

    [Hide]
    public abstract string[] WritableExtensions { get; }

    #endregion

    #region FileDocument

    public FileDocument() : base() { }

    #endregion

    #region Methods

    bool CheckBusy()
    {
        if (IsBusy)
        {
            Dialog.ShowWarning("Save", new Warning("The document is already saving."), Buttons.Ok);
            return true;
        }
        return false;
    }

    async void Save(string filePath)
    {
        Log.Write<Document>($"async Document.Save(string filePath)");

        if (CheckBusy()) return;
        IsModified = false;

        IsBusy = true;

        var result = await SaveAsync(filePath);

        if (result && Extension == System.IO.Path.GetExtension(filePath).Substring(1))
            Path = filePath;

        IsBusy = false;
    }

    void SaveAs()
    {
        if (StorageDialog.Show(out string path, "SaveAs".Translate() + "...", StorageDialogMode.SaveFile, WritableExtensions, Path)) 
            Save(path);
    }

    protected abstract Task<bool> SaveAsync(string filePath);

    public override void OnModified(ModifiedEventArgs e)
    {
        base.OnModified(e);
        LastModified = DateTime.Now;
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Path):

                Try.Invoke(() =>
                {
                    var fileInfo = new System.IO.FileInfo(Path);
                    LastAccessed
                        = fileInfo.LastAccessTime;
                    Created
                        = fileInfo.CreationTime;
                    LastModified
                        = fileInfo.LastWriteTime;
                    Size
                        = fileInfo.Length;
                });

                Update(() => Name);
                Update(() => Title);
                Update(() => ToolTip);
                break;
        }
    }

    public sealed override void Save()
    {
        if (CheckBusy()) return;
        if (!File.Long.Exists(Path))
            SaveAs();

        else Save(Path);
    }

    #endregion

    #region Commands

    [Float(Float.Above), Category(Category.Save), Name("Save"), Image(SmallImages.Save), Tab(Tab.File), XmlIgnore]
    public override ICommand SaveCommand => base.SaveCommand;

    [field: NonSerialized]
    ICommand saveAsCommand;
    [Float(Float.Above), Category(Category.Save), Name("SaveAs"), Image(SmallImages.SaveAs), Tab(Tab.File), XmlIgnore]
    public ICommand SaveAsCommand => saveAsCommand ??= new RelayCommand(SaveAs);

    #endregion
}

#endregion

#region ImageFileDocument

[Name("Image file document"), Description("Interface for managing an image file."), Serializable]
public abstract class ImageFileDocument : FileDocument
{
    internal Bitmap Source;

    public ImageFileDocument() : base() { }
}

#endregion

#region TextFileDocument

[Name("Text file document"), Description("Interface for managing a text file."), Serializable]
public class TextFileDocument : FileDocument
{
    enum Category { Count, Format }

    [Category(Category.Count)]
    [Footer, Name("Characters")]
    [ReadOnly]
    [StringFormat(NumberFormat.Default)]
    [XmlIgnore]
    public virtual int CountCharacters => Text.Length;

    [Category(Category.Count)]
    [Footer, Name("Lines")]
    [ReadOnly]
    [StringFormat(NumberFormat.Default)]
    [XmlIgnore]
    public virtual int CountLines => Text.Lines();

    [Category(Category.Count)]
    [Footer, Name("Words")]
    [ReadOnly]
    [StringFormat(NumberFormat.Default)]
    [XmlIgnore]
    public virtual int CountWords => Text.Words();

    [Category(Category.Format), Footer, ReadOnly, XmlIgnore]
    public Encoding Encoding { get => Get<Encoding>(default, false); private set => Set(value, false); }

    [Hide, Modify]
    public string Text { get => Get(""); set => Set(value); }

    public override string[] WritableExtensions => throw new NotImplementedException();

    public TextFileDocument() : base() { }

    protected virtual void OnTextChanged()
    {
        Update(() => CountCharacters);
        Update(() => CountLines);
        Update(() => CountWords);
    }

    protected override Task<bool> SaveAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Text):
                OnTextChanged();
                break;
        }
    }
}

#endregion