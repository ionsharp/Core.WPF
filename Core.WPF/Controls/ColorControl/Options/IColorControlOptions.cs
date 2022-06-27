using Imagin.Core.Collections.Serialization;

namespace Imagin.Core.Controls;

public interface IColorControlOptions
{
    bool AutoSaveLayout { get; }

    IGroupWriter Colors { get; }

    IGroupWriter Illuminants { get; }

    Layouts Layouts { get; }

    IGroupWriter Profiles { get; }
    
    //...

    void OnLoaded(ColorControl colorPicker);
}