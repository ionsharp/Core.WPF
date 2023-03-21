using System;
using System.Windows;

namespace Imagin.Core.Config;

public interface IApplication
{
    event EventHandler<EventArgs> Loaded;

    ApplicationLink Link { get; }

    ResourceDictionary Resources { get; }
}