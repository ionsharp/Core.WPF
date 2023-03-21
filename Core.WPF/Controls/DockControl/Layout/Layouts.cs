using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Imagin.Core.Controls;

public class LayoutEventArgs : EventArgs<object>
{
    public LayoutEventArgs(object filePath) : base(filePath) { }
}

public class LayoutSavedEventArgs : EventArgs<string>
{
    public LayoutSavedEventArgs(string filePath) : base(filePath) { }
}

public delegate void LayoutEventHandler(Layouts sender, LayoutEventArgs e);

public delegate void LayoutSavedEventHandler(Layouts sender, LayoutSavedEventArgs e);

[Categorize(false), ViewSource(ShowHeader = false)]
public class Layouts : PathCollection
{
    public const string DefaultPath = DefaultFolderPath + "Default.xml";

    public const string DefaultFolderPath = "Layouts/";

    ///

    public event LayoutEventHandler Applied;

    public event LayoutSavedEventHandler Saved;

    ///

    static readonly XmlSerializer Serializer = new(typeof(DockLayout), new XmlAttributeOverrides(), XArray.New<Type>(typeof(DoubleSize), typeof(ControlLength), typeof(ControlLengthUnit), typeof(DockLayout), typeof(DockLayoutDocumentGroup), typeof(DockLayoutElement), typeof(DockLayoutGroup), typeof(DockLayoutPanel), typeof(DockLayoutPanelGroup), typeof(DockLayoutWindow), typeof(Point2)), new XmlRootAttribute(nameof(Layout)), $"{nameof(Imagin)}.{nameof(Core)}.{nameof(Controls)}");

    ///

    [Hide]
    public readonly int DefaultLayout;

    [Hide]
    public ObservableCollection<Uri> DefaultLayouts { get; private set; } = new();

    [Hide]
    public override string ItemName => "Layout";

    [Hide]
    public object Layout { get => this.Get<object>(); set => this.Set(value); }

    ///

    public Layouts(string folderPath, IEnumerable<Uri> defaultLayouts, int defaultLayout) : base(folderPath, new Filter(ItemType.File, "xml")) 
    {
        defaultLayouts.ForEach(i => DefaultLayouts.Add(i));
        DefaultLayout = defaultLayout;

        this.Set(() => Layout, DefaultLayouts[DefaultLayout], true, true);
    }

    ///

    public string FilePath(string nameWithoutExtension) => $@"{Path}\{nameWithoutExtension}.xml";

    ///

    Result Apply(string filePath)
    {
        try
        {
            var layout = (DockLayout)Serializer.Deserialize(new StringReader(Storage.File.Long.ReadAllText(filePath, System.Text.Encoding.UTF8)));
            return new Success<DockLayout>(layout);
        }
        catch (Exception e)
        {
            Log.Write<Layouts>(e);
            return e;
        }
    }

    Result Apply(Stream stream)
    {
        Result result = null;

        try
        {
            var layout = (DockLayout)Serializer.Deserialize(stream);
            result = new Success<DockLayout>(layout);
        }
        catch (Exception e)
        {
            Log.Write<Layouts>(e);
            result = e;
        }
        finally
        {
            stream?.Close();
            stream?.Dispose();
        }

        return result;
    }

    public async Task<DockLayout> Apply()
    {
        Result result = null;
        Success<DockLayout> success = null;
        await Task.Run(() =>
        {
            //> string
            if (Layout is string i && !i.Empty())
                result = Apply(i);

            //> Uri
            else if (Layout is Uri j)
                result = Apply(Resources.GetStream(j));

            success = result ? result as Success<DockLayout> : success;
            //> Uri (default!)
            if (success == null)
            {
                var defaultLayout = DefaultLayout >= 0 && DefaultLayout < DefaultLayouts.Count ? DefaultLayout : 0;
                result = Apply(Resources.GetStream(DefaultLayouts[defaultLayout]));
            }

            success = result ? result as Success<DockLayout> : success;
        });

        return success?.Data;
    }

    ///

    protected override void OnExported(string path)
    {
        base.OnExported(path);
        Save(path);
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Layout))
            OnApplied(Layout);
    }

    ///

    protected virtual void OnApplied(object filePath) => Applied?.Invoke(this, new LayoutEventArgs(filePath));

    protected virtual void OnSaved(string filePath) => Saved?.Invoke(this, new LayoutSavedEventArgs(filePath));

    ///

    internal Result Save(string filePath, DockLayout layout)
    {
        Result result = default;

        var directoryName = System.IO.Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryName))
        {
            try
            {
                Directory.CreateDirectory(directoryName);
                result = new Success();
            }
            catch (Exception e)
            {
                result = e;
                Log.Write<Layouts>(result);
                return result;
            }
        }

        try
        {
            Serializer.Serialize(new StreamWriter(filePath), layout);
            result = new Success();
            Log.Write<Layouts>(new Success($"Saved layout '{filePath}'"));
        }
        catch (Exception e)
        {
            result = e;
            Log.Write<Layouts>(result);
        }
        return result;
    }

    public void Save(string nameWithoutExtension) => OnSaved(FilePath(nameWithoutExtension));
}