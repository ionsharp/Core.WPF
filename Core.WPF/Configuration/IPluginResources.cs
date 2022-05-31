using System.Collections.Generic;

namespace Imagin.Core.Configuration
{
    public interface IPluginResources
    {
        object GetValue(string key);

        IEnumerable<object> GetValues();
    }
}