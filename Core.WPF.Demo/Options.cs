using Imagin.Core;
using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Config;
using Imagin.Core.Controls;
using Imagin.Core.Models;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;

namespace Demo
{
    [Serializable]
    public class Options : DockViewOptions<MainViewModel>
    {
        #region (enum) Category

        enum Category
        {
            Explorer
        }

        #endregion

        #region Properties

        ExplorerOptions explorerOptions = new();
        [Category(Category.Explorer)]
        [DisplayName("Options")]
        public ExplorerOptions ExplorerOptions
        {
            get => explorerOptions;
            set => this.Change(ref explorerOptions, value);
        }

        #endregion

        #region Methods

        public override IEnumerable<Uri> GetDefaultLayouts()
        {
            yield return Resources.Uri(nameof(Demo), Layouts.DefaultPath);
        }

        #endregion
    }
}