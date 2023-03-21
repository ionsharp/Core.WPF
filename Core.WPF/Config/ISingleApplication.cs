using System.Collections.Generic;

namespace Imagin.Core.Config;

public interface ISingleApplication
{
    void OnReopened(IList<string> Arguments);
}