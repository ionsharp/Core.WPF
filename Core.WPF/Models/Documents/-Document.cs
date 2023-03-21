using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

[Base(typeof(Document)), Serializable]
public abstract class Document : Content
{
    public const SecondaryDocks DefaultDockPreference = SecondaryDocks.Left;

    [Hide, XmlIgnore]
    public virtual bool CanClose { get => Get(true); set => Set(value); }

    [Hide, XmlIgnore]
    public virtual bool CanMinimize { get => Get(true); set => Set(value); }

    [Hide, XmlIgnore]
    public virtual SecondaryDocks DockPreference { get; } = DefaultDockPreference;

    [Hide]
    public virtual object Icon => default;

    [Hide, XmlIgnore]
    public virtual bool IsMinimized { get => Get(false); set => Set(value); }

    [Hide, XmlIgnore]
    public override object ToolTip => this;

    public Document() : base() { }

    public abstract void Save();

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(IsModified):
                Update(() => Title);
                break;
        }
    }

    [NonSerialized]
    ICommand saveCommand;
    [Hide, XmlIgnore]
    public virtual ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
}