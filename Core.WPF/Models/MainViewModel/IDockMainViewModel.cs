namespace Imagin.Core.Models;

public interface IDockMainViewModel
{
    Document ActiveDocument { get; }

    DocumentCollection Documents { get; }

    PanelCollection Panels { get; }
}