using System;
using System.Collections.ObjectModel;

namespace Imagin.Core.Collections.ObjectModel
{
    [Serializable]
    public class StringCollection : ObservableCollection<string>
    {
        public StringCollection() : base() { }
    }
}