using Imagin.Core.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Imagin.Core.Models
{
    [Serializable]
    public class DocumentCollection : ObservableCollection<Document>
    {
        public DocumentCollection() : base() { }

        public DocumentCollection(params Document[] input) : base(input) { }

        public DocumentCollection(IEnumerable<Document> input) : base(input) { }
    }
}