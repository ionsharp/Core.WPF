using Imagin.Core.Collections.Serialization;
using System.Collections;

namespace Imagin.Core.Controls;

public interface IColorControlOptions
{
    bool AutoSaveDocuments { get; }

    bool RememberDocuments { get; }

    IList Documents { get; }

    IList Panels { get; }

    //...

    bool AutoSaveLayout { get; }

    Layouts Layouts { get; }

    //...

    IGroupWriter Colors { get; }

    IGroupWriter Illuminants { get; }

    IGroupWriter Matrices { get; }

    IGroupWriter Profiles { get; }
    
    //...

    void OnLoaded(ColorControl colorPicker);
}