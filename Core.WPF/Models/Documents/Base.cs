using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

[Serializable]
public abstract class Document : Content, ICloneable
{
    public const SecondaryDocks DefaultDockPreference = SecondaryDocks.Left;

    bool canClose = true;
    [Hidden]
    [XmlIgnore]
    public virtual bool CanClose
    {
        get => canClose;
        set => this.Change(ref canClose, value);
    }

    [Hidden]
    [XmlIgnore]
    public virtual bool CanMinimize { get; } = true;

    [Hidden]
    [XmlIgnore]
    public virtual SecondaryDocks DockPreference { get; } = DefaultDockPreference;

    [Hidden]
    public virtual object Icon => default;

    bool isMinimized = false;
    [Hidden]
    [XmlIgnore]
    public virtual bool IsMinimized
    {
        get => isMinimized;
        set => this.Change(ref isMinimized, value);
    }

    [field: NonSerialized]
    bool isModified = false;
    [Hidden, XmlIgnore]
    public virtual bool IsModified
    {
        get => isModified;
        set => this.Change(ref isModified, value);
    }

    void Check(Type type, List<Type> oldTypes = null)
    {
        oldTypes = oldTypes ?? new();
        if (oldTypes.Contains(type))
            return;

        try
        {
            if (!type.IsClass || type == typeof(string))
                return;

            oldTypes.Add(type);

            void f(Type t) => Log.Write<Document>(new Warning($"The class '{t.FullName}' is not marked as serializable."));

            if (!type.HasAttribute<SerializableAttribute>())
            {
                f(type);
            }

            var types = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (types?.Length > 0)
            {
                foreach (var i in types)
                {
                    if (type != i.FieldType)
                    {
                        if (!i.HasAttribute<NonSerializedAttribute>())
                        {
                            if (i.FieldType.IsClass)
                            {
                                if (!i.FieldType.HasAttribute<SerializableAttribute>())
                                {
                                    f(i.FieldType);
                                }
                                else Check(i.FieldType, oldTypes);
                            }
                        }
                    }
                }
            }
        }
        catch { }
    }

    public Document() : base() { }

    object ICloneable.Clone() => Clone();
    public abstract Document Clone();

    public abstract void Save();

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(IsModified):
                this.Changed(() => Title);
                break;
        }
    }

    [field: NonSerialized]
    ICommand saveCommand;
    [Hidden]
    [XmlIgnore]
    public virtual ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
}