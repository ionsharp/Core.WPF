using Imagin.Core.Collections.Generic;
using Imagin.Core.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Imagin.Core.Models
{
    public class PanelCollection : ObservableCollection<Panel>
    {
        public Panel this[string name] => this.FirstOrDefault(i => i.Name == name);

        public PanelCollection() : base() { }

        public PanelCollection(params Panel[] input) : base(input) { }

        public PanelCollection(IEnumerable<Panel> input) : base(input) { }
    }
}