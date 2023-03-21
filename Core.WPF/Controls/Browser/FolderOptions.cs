using Imagin.Core.Data;
using Imagin.Core.Storage;
using System;

namespace Imagin.Core.Controls;

[Description("Folder options"), Name("Folder options"), Serializable]
public class FolderOptions : Base
{
    enum Category { Group, Sort, View }

    [Category(Category.Group), Description("The direction to group items.")]
    public SortDirection GroupDirection { get => Get(SortDirection.Ascending); set => Set(value); }

    [Category(Category.Group), Description("The property to group items by.")]
    public ItemProperty GroupName { get => Get(ItemProperty.None); set => Set(value); }

    [Pin(Pin.BelowOrRight), Description("The size of items."), Name("Size"), Range(8.0, 512.0, 4.0, Style = RangeStyle.Both)]
    public double Size { get => Get(64.0); set => Set(value); }

    [Category(Category.Sort), Description("The direction to sort items.")]
    public SortDirection SortDirection { get => Get(SortDirection.Ascending); set => Set(value); }

    [Category(Category.Sort), Description("The property to sort items by.")]
    public ItemProperty SortName { get => Get(ItemProperty.Name); set => Set(value); }

    [Pin(Pin.AboveOrLeft), Category(Category.View), Description("How to view items.")]
    public BrowserView View { get => Get(BrowserView.Thumbnails); set => Set(value); }

    [Category(Category.View), Description("Show check boxes."), Name("CheckBoxes")]
    public bool ViewCheckBoxes { get => Get(false); set => Set(value); }

    [Category(Category.View), Description("Show file extensions."), Name("FileExtensions")]
    public bool ViewFileExtensions { get => Get(false); set => Set(value); }

    [Category(Category.View), Description("Show files."), Name("Files")]
    public bool ViewFiles { get => Get(true); set => Set(value); }
}