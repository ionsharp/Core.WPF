using Imagin.Core.Analytics;
using Imagin.Core.Collections.Generic;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Controls
{
    public class LayoutEventArgs : EventArgs<string>
    {
        public LayoutEventArgs(string filePath) : base(filePath) { }
    }

    public class LayoutSavedEventArgs : LayoutEventArgs
    {
        public LayoutSavedEventArgs(string filePath) : base(filePath) { }
    }

    public delegate void LayoutEventHandler(Layouts sender, LayoutEventArgs e);

    public delegate void LayoutSavedEventHandler(Layouts sender, LayoutSavedEventArgs e);

    public class Layouts : PathCollection
    {
        public const string DefaultPath = DefaultFolderPath + "Default.xml";

        public const string DefaultFolderPath = "Layouts/";

        public event LayoutEventHandler Applied;

        public event LayoutSavedEventHandler Saved;

        static readonly XmlSerializer Serializer = new(typeof(DockLayout), new XmlAttributeOverrides(), XArray.New<Type>(typeof(DoubleSize), typeof(ControlLength), typeof(ControlLengthUnit), typeof(DockLayout), typeof(DockLayoutDocumentGroup), typeof(DockLayoutElement), typeof(DockLayoutGroup), typeof(DockLayoutPanel), typeof(DockLayoutPanelGroup), typeof(DockLayoutWindow), typeof(Point2)), new XmlRootAttribute(nameof(Layout)), $"{nameof(Imagin)}.{nameof(Core)}.{nameof(Controls)}");

        //...

        string layout = string.Empty;
        [Hidden]
        public virtual string Layout
        {
            get => layout;
            set
            {
                this.Change(ref layout, value);
                OnApplied(value);
            }
        }

        //...

        [Hidden]
        public ObservableCollection<Uri> DefaultLayouts { get; private set; } = new();

        //...

        public Layouts(string folderPath, IEnumerable<Uri> defaultLayouts) : base(folderPath, new Filter(ItemType.File, "xml")) 
        {
            defaultLayouts.ForEach(i => DefaultLayouts.Add(i));
        }

        //...

        public string FilePath(string nameWithoutExtension) => $@"{Path}\{nameWithoutExtension}.xml";

        //...

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
                System.Console.WriteLine($"<DockControl> {e.Message}...");
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
                if (!layout.NullOrEmpty())
                    result = Apply(layout);

                success = result ? result as Success<DockLayout> : success;
                if (success == null)
                    result = Apply(Resources.Stream(DefaultLayouts[0]));

                success = result ? result as Success<DockLayout> : success;
            });

            return success?.Data;
        }

        //...

        protected virtual void OnApplied(string filePath) => Applied?.Invoke(this, new LayoutEventArgs(filePath));

        protected virtual void OnSaved(string filePath) => Saved?.Invoke(this, new LayoutSavedEventArgs(filePath));

        //...

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
            }
            catch (Exception e)
            {
                result = e;
                Log.Write<Layouts>(result);
            }
            return result;
        }

        //...

        public void Save() => OnSaved(layout);

        public void Save(string nameWithoutExtension) => OnSaved(FilePath(nameWithoutExtension));

        public void Update(string filePath)
        {
            layout = filePath;
            XPropertyChanged.Changed(this, () => Layout);
        }

        //...

        [field: NonSerialized]
        ICommand deleteLayoutCommand;
        [Hidden]
        public virtual ICommand DeleteLayoutCommand => deleteLayoutCommand ??= new RelayCommand(() => Computer.Recycle(Layout), () =>
        {
            var result = false;
            Try.Invoke(() => result = Storage.File.Long.Exists(Layout));
            return result;
        });

        [field: NonSerialized]
        ICommand saveLayoutCommand;
        [Hidden]
        public virtual ICommand SaveLayoutCommand => saveLayoutCommand ??= new RelayCommand(() =>
        {
            var x = new BaseNamable("File name without extension");
            MemberWindow.ShowDialog("Save layout", x, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);

            if (result == 0)
            {
                Save(x.Name);
                Update(FilePath(x.Name));

                Dialog.Show("Save layout", "Layout saved!", DialogImage.Information, Buttons.Ok);
                return;
            }
        }, () => true);
    }
}