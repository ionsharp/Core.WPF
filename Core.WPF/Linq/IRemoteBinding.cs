using Imagin.Core.Data;
using Imagin.Core.Models;

namespace Imagin.Core.Linq
{
    public static class XIRemoteBinding
    {
        public static object GetSource(this IRemoteBinding input, RemoteBindingSource source)
        {
            return source switch
            {
                RemoteBindingSource.Application 
                    => Get.Where<Config.BaseApplication>(),
                RemoteBindingSource.MainViewModel 
                    => Get.Where<IMainViewModel>(),
                RemoteBindingSource.Options 
                    => Get.Where<MainViewOptions>(),
                RemoteBindingSource.Resources 
                    => Get.Current<Config.ApplicationResources>(),
                _ => null,
            };
        }
    }
}