using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Storage;
using System;
using System.Windows;

namespace Imagin.Core.Controls
{
    [DisplayName(nameof(ExplorerOptions))]
    [Serializable]
    public class ExplorerOptions : ControlOptions<Explorer>
    {
        enum Category
        {
            Browser,
            Favorites,
            General
        }

        #region Browser

        BrowserOptions browserOptions = new();
        [Category(Category.Browser)]
        public BrowserOptions BrowserOptions
        {
            get => browserOptions;
            set => this.Change(ref browserOptions, value);
        }

        #endregion

        #region Favorites

        [field: NonSerialized]
        Favorites favorites = null;
        [Hidden]
        public Favorites Favorites
        {
            get => favorites;
            set => this.Change(ref favorites, value);
        }

        bool showFavoriteBar = true;
        [Category(Category.Favorites)]
        [DisplayName("ShowBar")]
        public bool ShowFavoriteBar
        {
            get => showFavoriteBar;
            set => this.Change(ref showFavoriteBar, value);
        }

        SortDirection favoriteSortDirection = SortDirection.Ascending;
        [Category(Category.Favorites)]
        [DisplayName("SortDirection")]
        public SortDirection FavoriteSortDirection
        {
            get => favoriteSortDirection;
            set => this.Change(ref favoriteSortDirection, value);
        }

        ItemProperty favoriteSortName = ItemProperty.Name;
        [Category(Category.Favorites)]
        [DisplayName("SortName")]
        public ItemProperty FavoriteSortName
        {
            get => favoriteSortName;
            set => this.Change(ref favoriteSortName, value);
        }

        #endregion

        #region General

        string defaultPath = StoragePath.Root;
        [Category(Category.General)]
        [MemberStyle(StringStyle.FolderPath)]
        public string DefaultPath
        {
            get => defaultPath;
            set => this.Change(ref defaultPath, value);
        }

        History history = new(Explorer.DefaultLimit);
        [Hidden]
        public History History
        {
            get => history;
            set => this.Change(ref history, value);
        }

        string path = StoragePath.Root;
        [Hidden]
        public string Path
        {
            get => path;
            set
            {
                if (value.NullOrEmpty())
                    return;

                this.Change(ref path, value);
            }
        }

        #endregion

        public override string ToString() => nameof(Explorer);
    }
}