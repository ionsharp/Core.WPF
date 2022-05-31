using Imagin.Core;
using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Configuration;
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

        [Hidden]
        public Favorites Favorites => explorerOptions.Favorites;

        #region Controls

        ExplorerOptions explorerOptions = new();
        [Category(Category.Explorer)]
        [DisplayName("Options")]
        public ExplorerOptions ExplorerOptions
        {
            get => explorerOptions;
            set => this.Change(ref explorerOptions, value);
        }

        #endregion

        #endregion

        #region Methods

        protected override IEnumerable<IWriter> GetData()
        {
            yield return ExplorerOptions.Favorites;
        }

        public override IEnumerable<Uri> GetDefaultLayouts()
        {
            yield return Resources.Uri(nameof(Demo), Layouts.DefaultPath);
        }

        protected override void OnLoaded()
        {
            ExplorerOptions.Favorites = new Favorites($@"{ApplicationProperties.GetFolderPath(DataFolders.Documents)}\{nameof(Explorer)}", new Limit(250, Limit.Actions.RemoveFirst));
            base.OnLoaded();
        }

        #endregion
    }
}