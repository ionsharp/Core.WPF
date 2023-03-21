using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Config;

/// <summary>A (partially) independent component that extends functionality of an application.</summary>
[Categorize(false), Serializable]
public abstract class Extension : Base, IExtension
{
    [field: NonSerialized]
    public event EventHandler<EventArgs> Disabled;

    [field: NonSerialized]
    public event EventHandler<EventArgs> Enabled;

    ///

    [Hide]
    public AssemblyContext AssemblyContext { get; set; }

    ///

    [Hide]
    public bool IsEnabled
    {
        get => Get(false);
        set
        {
            Set(value);
            value.If(true, OnEnabled, OnDisabled);
        }
    }

    ///

    public abstract string Author { get; }

    public abstract string Description { get; }

    [Hide]
    public string FilePath { get; set; }

    public abstract string Icon { get; }

    public abstract string Name { get; }

    public abstract string Uri { get; }

    public abstract Version Version { get;  }

    ///

    public Extension() : base() { }

    ///

    [Hide]
    public virtual void OnDisabled() => Disabled?.Invoke(this, new());

    [Hide]
    public virtual void OnEnabled() => Enabled?.Invoke(this, new());
}

[Serializable]
public abstract class PanelExtension : Extension
{
    [Hide]
    public abstract Models.Panel Panel { get; }

    [Hide]
    public abstract IExtensionResources Resources { get; }

    [NonSerialized]
    DataTemplate template = null;
    [Hide]
    public DataTemplate Template
    {
        get
        {
            if (template == null)
            {
                if (Resources?.GetValues() is IEnumerable<object> values)
                {
                    foreach (var i in values)
                    {
                        if (i is DataTemplate j)
                        {
                            if ((Type)j.DataType == Panel.GetType())
                            {
                                template = j;
                                break;
                            }
                        }
                    }
                }
            }
            return template;
        }
    }

    [Hide]
    public override void OnDisabled()
    {
        base.OnEnabled();
        Current.Get<IDockMainViewModel>().If(i => i.Panels.Remove(Panel));
    }

    [Hide]
    public override void OnEnabled()
    {
        base.OnEnabled();
        Current.Get<IDockMainViewModel>().If(i => i.Panels.Add(Panel));
    }
}