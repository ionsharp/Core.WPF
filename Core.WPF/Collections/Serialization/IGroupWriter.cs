using System;
using System.Collections;
using System.Windows.Input;

namespace Imagin.Core.Collections.Serialization
{
    public interface IGroupWriter : IList
    {
        ICommand ClearCommand { get; }

        ICommand ExportCommand { get; }

        ICommand ExportAllCommand { get; }

        ICommand ImportCommand { get; }

        Type GetItemType();
    }
}