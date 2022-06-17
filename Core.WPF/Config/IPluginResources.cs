using System.Collections.Generic;

namespace Imagin.Core.Config
{
    public interface IPluginResources
    {
        object GetValue(string key);

        IEnumerable<object> GetValues();
    }
}