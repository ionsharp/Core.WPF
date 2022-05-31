using Imagin.Core.Collections;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System.Collections.Generic;

namespace Imagin.Core.Controls
{
    public partial class ExplorerWindow : StorageWindow
    {
        public ExplorerWindow(string title, StorageWindowModes mode, SelectionMode selectionMode, StorageWindowFilterModes filterMode, IEnumerable<string> fileExtensions, string defaultPath) : base(title, mode, selectionMode, filterMode, fileExtensions, defaultPath)
        {
            InitializeComponent();
            Explorer.SelectionChanged += OnSelectionChanged;
            this.RegisterHandler(null, i => Explorer.SelectionChanged -= OnSelectionChanged);
        }

        void OnSelectionChanged(object sender, EventArgs<ICollectionChanged> e) => Selection = e.Value;
    }
}