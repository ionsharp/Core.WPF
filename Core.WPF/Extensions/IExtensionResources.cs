using System.Collections.Generic;

namespace Imagin.Core.Config;

public interface IExtensionResources
{
    object GetValue(string key);

    IEnumerable<object> GetValues();
}