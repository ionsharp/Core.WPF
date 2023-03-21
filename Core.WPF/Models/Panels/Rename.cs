using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imagin.Core.Models;

public enum RenameExtensionFormat { Original, Capitalized, Lower, Upper }

[Description("Rename multiple files algorithmically."), Name(DefaultTitle), Serializable]
public class RenamePanel : TaskPanel
{
    public static readonly ResourceKey TemplateKey = new();

    enum Category { Extension, Name, Other }

    public const string DefaultTitle = "Rename";

    public const string DefaultExtension = "file";

    protected override TaskManagement Execution => TaskManagement.Managed;

    #region Properties

    [Category(Category.Extension), Name("Format"), Lock]
    public RenameExtensionFormat ExtensionFormat { get => Get(RenameExtensionFormat.Lower); set => Set(value); }

    [Category(Category.Extension), Lock, Name("Replace with"), Placeholder("Replace extension with")]
    public string ExtensionReplaceWith { get => Get(""); set => Set(value); }

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Rename);

    [Category(Category.Name), Lock, Range(1.0, 10.0, 1.0)]
    public double Increment { get => Get(1.0); set => Set(value); }

    [Category(Category.Name), Lock]
    public int Origin { get => Get(0); set => Set(value); }

    [Lock, Pin(Pin.AboveOrLeft), Placeholder("Folder"), StringStyle(StringStyle.FolderPath)]
    public string Path { get => Get(""); set => Set(value); }

    [Category(Category.Other), Lock]
    public bool SubFolders { get => Get(false); set => Set(value); }

    [Hide]
    public override string Title => DefaultTitle;

    #endregion

    public RenamePanel() : base() { }

    void Rename(string folder, bool subFolders, CancellationToken token)
    {
        string[] files = null;
        if (Try.Invoke(() => files = System.IO.Directory.GetFiles(Path)))
        {
            if (files?.Length > 0)
            {
                Dictionary<string, int> track = new();

                double j = 0;
                foreach (var i in files)
                {
                    if (token.IsCancellationRequested)
                        return;

                    Dispatch.Invoke(() => Progress = (j + 1).Double() / files.Length);
                    j++;

                    string oldExtension
                        = System.IO.Path.GetExtension(i);
                    oldExtension
                        = oldExtension?.Length > 0
                        ? oldExtension.StartsWith(".") ? oldExtension.Substring(1) : oldExtension
                        : DefaultExtension;

                    string newExtension = "";

                    var lowerExtension = oldExtension?.ToLower();

                    var oldName = System.IO.Path.GetFileNameWithoutExtension(i);
                    var newName = "";

                    if (lowerExtension == null || lowerExtension.Length == 0)
                        lowerExtension = DefaultExtension;

                    if (!track.ContainsKey(lowerExtension))
                        track.Add(lowerExtension, 0);

                    var skip = false;

                    track[lowerExtension] = Origin;
                    while (System.IO.File.Exists(GetPath(folder, track[lowerExtension], lowerExtension)))
                    {
                        var tryPath = GetPath(folder, track[lowerExtension], lowerExtension);
                        if (tryPath == i)
                        {
                            skip = true;
                            break;
                        }

                        track[lowerExtension] += Increment.Int32();
                    }

                    if (skip) continue;

                    newExtension
                        = GetExtension(oldExtension);
                    newName
                        = $"{track[lowerExtension]}";

                    var m = GetPath(folder, oldName, lowerExtension);
                    var n = GetPath(folder, newName, newExtension);

                    if (m.ToLower() != n.ToLower())
                        Try.Invoke(() => System.IO.File.Move(m, n), e => Core.Analytics.Log.Write<RenamePanel>(new Error(e)));
                }
            }
        }

        if (subFolders)
        {
            string[] folders = null;
            if (Try.Invoke(() => folders = System.IO.Directory.GetDirectories(folder)))
            {
                if (folders?.Length > 0)
                {
                    foreach (var i in folders)
                        Rename(i, subFolders, token);
                }
            }
        }
    }

    protected override Task ExecuteAsync(object parameter, CancellationToken token) => throw new NotImplementedException();

    protected override void ExecuteSync(object parameter, CancellationToken token) => Rename(Path, SubFolders, token);

    protected override void OnExecuted() { }

    //...

    public string GetExtension(string oldExtension)
    {
        switch (ExtensionFormat)
        {
            case RenameExtensionFormat.Capitalized:
                return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(oldExtension);

            case RenameExtensionFormat.Lower:
                return oldExtension.ToLower();

            case RenameExtensionFormat.Upper:
                return oldExtension.ToUpper();

            default: return oldExtension;
        }
    }

    public string GetPath(string folder, object name, string extension) => @$"{folder}\{name}.{extension}";

    [Pin(Pin.BelowOrRight), Name(DefaultTitle), Image(SmallImages.Rename), Index(0), Style(ButtonStyle.Default)]
    [Trigger(nameof(MemberModel.IsVisible), nameof(IsNotBusy))]
    public override ICommand StartCommand => base.StartCommand;
}