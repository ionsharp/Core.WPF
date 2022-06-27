using Imagin.Core.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Storage;

public static class StorageWindow
{
    static CommonFileDialog New(string title, StorageWindowModes mode, SelectionMode selectionMode, IEnumerable<string> fileExtensions, string defaultPath)
    {
        CommonFileDialog result = null;
        switch (mode)
        {
            case StorageWindowModes.Open:
            case StorageWindowModes.OpenFile:
            case StorageWindowModes.OpenFolder:
                result = new CommonOpenFileDialog();
                result.EnsureFileExists = true;
                result.EnsurePathExists = true;
                ((CommonOpenFileDialog)result).Multiselect = selectionMode == SelectionMode.Multiple;
                break;

            case StorageWindowModes.SaveFile:
                result = new CommonSaveFileDialog();
                break;
        }

        result.Title = title;
        result.InitialDirectory = defaultPath;
        result.EnsureValidNames = true;

        foreach (var i in fileExtensions)
            result.Filters.Add(new CommonFileDialogFilter($"{i.ToUpper()} files", i));

        return result;
    }

    public static bool Show(out string[] paths, string title = "", StorageWindowModes mode = StorageWindowModes.OpenFile, IEnumerable<string> fileExtensions = null, string defaultPath = "")
    {
        if (mode == StorageWindowModes.SaveFile)
            throw new NotSupportedException();

        paths = new string[0];

        CommonOpenFileDialog dialog = (CommonOpenFileDialog)New(title, mode, SelectionMode.Multiple, fileExtensions, defaultPath);

        var result = dialog.ShowDialog();
        if (result == CommonFileDialogResult.Ok && dialog.FileNames?.Count() > 0)
        {
            paths = dialog.FileNames.ToArray();
            return true;
        }
        return false;
    }

    public static bool Show(out string path, string title = "", StorageWindowModes mode = StorageWindowModes.OpenFile, IEnumerable<string> fileExtensions = null, string defaultPath = "")
    {
        path = string.Empty;

        CommonFileDialog dialog = New(title, mode, SelectionMode.Single, fileExtensions, defaultPath);

        var result = dialog.ShowDialog();
        if (result == CommonFileDialogResult.Ok)
        {
            if (dialog is CommonOpenFileDialog a && a.FileNames?.Count() > 0)
            {
                path = a.FileName;
                return true;
            }
            if (dialog is CommonSaveFileDialog b && b.FileName?.Length > 0)
            {
                path = b.FileName;
                return true;
            }
        }
        return false;
    }
}