using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using Imagin.Core.Serialization;
using Imagin.Core.Storage;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Imagin.Core.Config;

public class Extensions : ItemCollection
{
    /// <summary>Extensions defined in an external assembly (compiled/not serialized).</summary>
    public const string ExternalExtension = "dll";

    /// <summary>Extensions defined internally (uncompiled/serialized).</summary>
    public const string InternalExtension = "ext";

    #region Properties

    public event EventHandler<EventArgs<IExtension>> Added;

    public event EventHandler<EventArgs<IExtension>> Removed;

    ///

    public event EventHandler<EventArgs<IExtension>> Disabled;

    public event EventHandler<EventArgs<IExtension>> Enabled;

    ///

    public ObservableCollection<IExtension> Source { get => this.Get<ObservableCollection<IExtension>>(new()); set => this.Set(value); }

    public override string ItemName => "Extension";

    #endregion

    #region Extensions

    public Extensions(string path) : base(path, new Filter(ItemType.File, ExternalExtension, InternalExtension)) { }

    #endregion

    #region Methods

    static IExtension Create(AssemblyContext assemblyContext)
    {
        var result = Find(assemblyContext.Assembly).Create<IExtension>();
        result.AssemblyContext = assemblyContext;
        return result;
    }

    /// <summary>Search for and return extension type defined in specified assembly.</summary>
    static Type Find(Assembly Assembly)
    {
        foreach (var i in Assembly.GetTypes())
        {
            if (i.Implements<IExtension>())
                return i;
        }
        throw new TypeLoadException();
    }

    ///

    protected override void OnAdded(Item item)
    {
        base.OnAdded(item);

        IExtension extension = null;
        Result result = null;

        var fileExtension = System.IO.Path.GetExtension(item.Path).Substring(1).ToLower();
        result = fileExtension switch
        {
            ExternalExtension => Try.Invoke(() =>
            {
                var guid = Guid.NewGuid();

                var assemblyContext = new AssemblyContext(guid, Assembly.Load(System.IO.File.ReadAllBytes(item.Path)), AppDomain.CreateDomain(guid.ToString()));
                extension = Create(assemblyContext);
            }),
            InternalExtension => BinarySerializer.Deserialize(item.Path, out extension),
            _ => new Error(new NotSupportedException())
        };

        if (!result)
        {
            Remove(item);
            Log.Write<Extensions>(result);
        }
        else
        {
            extension.FilePath = item.Path;
            extension.Disabled += OnDisabled;
            extension.Enabled += OnEnabled;

            Source.Add(extension);
            OnAdded(extension);
        }
    }

    protected override void OnRemoved(Item item)
    {
        base.OnAdded(item);
        if (XList.FirstOrDefault<IExtension>(Source, i => i.FilePath == item.Path) is IExtension extension)
        {
            if (extension.IsEnabled)
                extension.IsEnabled = false;

            extension.Enabled -= OnEnabled;

            if (extension.AssemblyContext?.AppDomain != null)
                AppDomain.Unload(extension.AssemblyContext.AppDomain);

            Source.Remove(extension);
            OnRemoved(extension);
        }
    }

    ///

    protected virtual void OnDisabled(object sender, EventArgs e)
    {
        var extension = sender as IExtension;
        Log.Write<Extensions>(new Message($"Disabled extension '{extension.Name}'"));
        Disabled?.Invoke(this, new EventArgs<IExtension>(extension));
    }

    protected virtual void OnEnabled(object sender, EventArgs e)
    {
        var extension = sender as IExtension;
        Log.Write<Extensions>(new Success($"Enabled extension '{extension.Name}'"));
        Enabled?.Invoke(this, new EventArgs<IExtension>(extension));
    }

    ///

    protected virtual void OnAdded(IExtension e) => Added?.Invoke(this, new EventArgs<IExtension>(e));

    protected virtual void OnRemoved(IExtension e) => Removed?.Invoke(this, new EventArgs<IExtension>(e));

    ///

    ICommand restoreCommand;
    public virtual ICommand ResetCommand => restoreCommand ??= new RelayCommand(() =>
    {
        var extensions = new ObservableCollection<IExtension>(XAssembly.GetDerivedTypes<IExtension>(AssemblyType.Core).Select(i => i.Create<IExtension>()));

        var result = new ReadOnlySelectionForm(extensions);
        Dialog.ShowObject("Reset extension", result, Resource.GetImageUri(SmallImages.Reset), i =>
        {
            if (i == 0)
            {
                if (result.SelectedIndex > -1)
                {
                    var extension = extensions[result.SelectedIndex].GetType().Create<IExtension>();
                    var extensionPath = $@"{Path}\{extension.Name}.ext";

                    if (System.IO.File.Exists(extensionPath))
                        Try.Invoke(() => System.IO.File.Delete(extensionPath));

                    BinarySerializer.Serialize(extensionPath, extension);
                }
            }
        },
        Controls.Buttons.SaveCancel);
    });

    #endregion
}