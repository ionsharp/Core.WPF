using Imagin.Core.Input;

namespace Imagin.Core.Models;

public interface IDockViewModel
{
    event ChangedEventHandler<Content> ActiveContentChanged;

    event ChangedEventHandler<Document> ActiveDocumentChanged;

    event ChangedEventHandler<Panel> ActivePanelChanged;

    ///

    event DefaultEventHandler<Document> DocumentAdded;

    event DefaultEventHandler<Document> DocumentRemoved;

    ///

    Content ActiveContent { get; set; }

    Document ActiveDocument { get; set; }

    Panel ActivePanel { get; set; }

    ///

    DocumentCollection Documents { get; }

    PanelCollection Panels { get; }
}