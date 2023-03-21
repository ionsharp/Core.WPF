using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Imagin.Core.Linq;

namespace Imagin.Core.Storage;

public static class StorageDialog
{
    static object New(string title, StorageDialogMode mode, Controls.SelectionMode selectionMode, IEnumerable<string> fileExtensions, string defaultPath)
    {
        CommonFileDialog result = null;
        switch (mode)
        {
            case StorageDialogMode.Open:
            case StorageDialogMode.OpenFile:
                result = new CommonOpenFileDialog();
                result.EnsureFileExists = true;
                result.EnsurePathExists = true;
                break;

            case StorageDialogMode.OpenFolder:
                var folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select a folder...";
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderDialog.ShowNewFolderButton = true;
                return folderDialog;

            case StorageDialogMode.SaveFile:
                result = new CommonSaveFileDialog();
                result.EnsureFileExists = false;
                result.EnsurePathExists = false;
                break;
        }

        if (result is CommonFileDialog j)
        {
            if (j is CommonOpenFileDialog k)
                k.Multiselect = selectionMode == Controls.SelectionMode.Multiple;

            j.Title = title;
            j.InitialDirectory = defaultPath;
            j.EnsureValidNames = true;

            if (mode != StorageDialogMode.OpenFolder)
            {
                var fileExtensionGroups = fileExtensions.GroupBy
                (
                    x => x.Substring(0, 1).ToUpper(), 
                    (letter, extensions) => new { Letter = letter, Extensions = extensions.OrderBy(x => x).ToList() }
                )
                .OrderBy(x => x.Letter);

                foreach (var i in fileExtensions)
                    result.Filters.Add(new CommonFileDialogFilter($"{i.ToUpper()} files", i));
                
                /*
                    result.Filters.Add(new CommonFileDialogFilter("(*) All files", "*"));
                    foreach (var i in fileExtensionGroups)
                        result.Filters.Add(new CommonFileDialogFilter($"{i.Letter} (*) files", i.Extensions.Select(j => j).ToString(";")));
                */
            }
        }
        return result;
    }

    public static bool Show(out string[] paths, string title = "", StorageDialogMode mode = StorageDialogMode.OpenFile, IEnumerable<string> fileExtensions = null, string defaultPath = "")
    {
        if (mode == StorageDialogMode.SaveFile)
            throw new NotSupportedException();

        paths = new string[0];

        object dialog = New(title, mode, Controls.SelectionMode.Multiple, fileExtensions, defaultPath);
        if (dialog is CommonOpenFileDialog fileDialog)
        {
            var result = fileDialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok && fileDialog.FileNames?.Count() > 0)
            {
                paths = fileDialog.FileNames.ToArray();
                return true;
            }
        }
        else if (dialog is FolderBrowserDialog folderDialog)
        {
            var result = folderDialog.ShowDialog();
            if (result == DialogResult.OK && folderDialog.SelectedPath?.Count() > 0)
            {
                paths = new string[] { folderDialog.SelectedPath };
                return true;
            }
        }
        return false;
    }

    public static bool Show(out string path, string title = "", StorageDialogMode mode = StorageDialogMode.OpenFile, IEnumerable<string> fileExtensions = null, string defaultPath = "")
    {
        path = string.Empty;

        object dialog = New(title, mode, Controls.SelectionMode.Single, fileExtensions, defaultPath);

        if (dialog is CommonFileDialog fileDialog)
        {
            var result = fileDialog.ShowDialog();
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
        }
        else if (dialog is FolderBrowserDialog folderDialog)
        {
            var result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (folderDialog.SelectedPath?.Length > 0)
                {
                    path = folderDialog.SelectedPath;
                    return true;
                }
            }
        }
        return false;
    }
}