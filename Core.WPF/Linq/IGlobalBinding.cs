using Imagin.Core.Data;
using Imagin.Core.Models;
using System.Windows;

namespace Imagin.Core.Linq;

public static class XGlobalSource
{
    public static object GetGlobalSource(this IGlobalSource input, GlobalSource source)
    {
        return source switch
        {
            GlobalSource.Application
                => Current.Get<Application>(),
            GlobalSource.MainView
                => Current.Get<IMainViewModel>().View,
            GlobalSource.MainViewModel
                => Current.Get<IMainViewModel>(),
            GlobalSource.Options 
                => Current.Get<MainViewOptions>(),
            GlobalSource.Resources 
                => Current.Get<Config.ApplicationTheme>(),
            _ => null,
        };
    }
}