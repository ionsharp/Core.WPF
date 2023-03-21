using Imagin.Core.Reflection;
using System;

namespace Imagin.Core.Config;

/// <summary>An independent component that extends functionality of an application.</summary>
public interface IExtension
{
    event EventHandler<EventArgs> Enabled;

    event EventHandler<EventArgs> Disabled;

    AssemblyContext AssemblyContext { get; set; }

    string Author { get; }

    string Description { get; }

    string FilePath { get; set; }

    string Icon { get; }

    bool IsEnabled { get; set; }

    string Name { get; }

    string Uri { get; }

    Version Version { get; }

    void OnEnabled();

    void OnDisabled();
}