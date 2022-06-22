using Imagin.Core.Collections.Serialization;

namespace Imagin.Core.Controls;

public interface IColorControlOptions
{
    bool AutoSaveLayout { get; }

    IGroupWriter Colors { get; }

    IWriter Profiles { get; }
        
    //...

    Layouts Layouts { get; }

    //...

    void OnLoaded(ColorControl colorPicker);
}