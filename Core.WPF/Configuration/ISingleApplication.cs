using System.Collections.Generic;

namespace Imagin.Core.Configuration
{
    public interface ISingleApplication
    {
        void OnReopened(IList<string> Arguments);
    }
}