using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using System.Collections.ObjectModel;

namespace Imagin.Core.Controls
{
    public interface IColorControlOptions
    {
        bool AutoSaveLayout { get; }

        IGroupWriter Colors { get; }

        ObservableCollection<Namable<WorkingProfile>> Profiles { get; }
        
        //...

        Layouts Layouts { get; }

        //...

        void OnLoaded(ColorControl colorPicker);
    }
}