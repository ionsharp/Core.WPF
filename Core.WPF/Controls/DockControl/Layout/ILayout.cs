using System;
using System.Collections.Generic;

namespace Imagin.Core.Controls
{
    public interface ILayout
    {
        IEnumerable<Uri> GetDefaultLayouts();

        Layouts Layouts { get; }
    }
}