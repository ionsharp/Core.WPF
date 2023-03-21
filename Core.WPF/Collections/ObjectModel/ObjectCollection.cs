using System;
using System.Collections.ObjectModel;

namespace Imagin.Core.Collections.ObjectModel;

[Serializable]
public class ObjectCollection : ObservableCollection<object>
{
    public ObjectCollection() : base() { }
}