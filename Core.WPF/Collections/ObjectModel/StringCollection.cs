using Imagin.Core.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Imagin.Core.Collections.ObjectModel;

[Serializable]
public class StringCollection : ObservableCollection<string>
{
    public StringCollection() : base() { }

    public StringCollection(IEnumerable<string> input) : base(input) { }
}