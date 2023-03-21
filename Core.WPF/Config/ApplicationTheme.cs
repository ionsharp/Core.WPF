using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Imagin.Core.Config;

[Image(SmallImages.Palette), Name("Theme"), View(MemberView.Single)]
public class ThemeDictionary : ResourceDictionary { }

public sealed class ApplicationTheme : Resources
{
    public readonly string FolderPath;

    ///

    public ThemeDictionary CurrentTheme => MergedDictionaries.Count > 0 ? (ThemeDictionary)MergedDictionaries[0] : null;

    public PathCollection CustomThemes { get; private set; } = new PathCollection(new Filter(ItemType.File, "theme"));

    Collection<ThemeResource> DefaultResources { get; set; } = new Collection<ThemeResource>();

    class ThemeResource : Dictionary<DefaultThemes, Uri>
    {
        public ThemeResource(string assemblyName) => typeof(DefaultThemes).GetEnumValues().Cast<DefaultThemes>().ForEach(i => Add(i, Resource.GetUri(assemblyName, ThemeKey.KeyFormat.F(i))));
    }

    ///

    public ApplicationTheme(BaseApplication application) : base()
    {
        /// Themes
        DefaultResources.Add(new(XAssembly.Name));

        /// Styles
        application.Resources.MergedDictionaries.Add(this);
        typeof(StyleKeys).GetEnumValues().Cast<StyleKeys>().ForEach(i => application.Resources.MergedDictionaries.Add(New(XAssembly.Name, StyleKey.KeyFormat.F(i))));

        ///

        FolderPath = application.RootFolderPath + $@"\Themes";
        if (!Folder.Long.Exists(FolderPath))
            System.IO.Directory.CreateDirectory(FolderPath);

        CustomThemes.Subscribe();
        _ = CustomThemes.RefreshAsync(FolderPath);
    }

    ///

    public string ThemePath(string fileName) => $@"{FolderPath}\{fileName}.theme";

    ///

    public void LoadTheme(DefaultThemes theme) => LoadTheme($"{theme}");

    public void LoadTheme(string theme)
    {
        BeginInit();
        MergedDictionaries.Clear();

        if (!Enum.TryParse(theme, out DefaultThemes type))
        {
            XResourceDictionary.TryLoad(theme, out ResourceDictionary result);
            if (result != null)
            {
                MergedDictionaries.Add(result);
                goto End;
            }
        }
        foreach (var i in DefaultResources)
        {
            var result = new ThemeDictionary { Source = i[type] };
            MergedDictionaries.Add(result);
        }

        End:
        {
            EndInit();
            XPropertyChanged.Update(this, () => CurrentTheme);
        }
    }

    public Result SaveTheme(string nameWithoutExtension) => CurrentTheme.TrySave(ThemePath(nameWithoutExtension));
}