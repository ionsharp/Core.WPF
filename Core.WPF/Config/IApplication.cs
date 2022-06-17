using System;
using System.Windows;

namespace Imagin.Core.Config
{
    public interface IApplication
    {
        event EventHandler<EventArgs> Loaded;

        ApplicationProperties Properties { get; }

        ResourceDictionary Resources { get; }
    }
}