using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Controls;
using Imagin.Core.Reflection;
using Imagin.Core.Text;
using System.Collections.Generic;

namespace Imagin.Core.Models;

public interface IDockViewOptions
{
    bool AutoSaveDocuments { get; }

    Layouts Layouts { get; }

    Dictionary<string, MemberDictionary> PanelOptions { get; }

    List<Document> RememberedDocuments { get; }
}

public interface IFileDockViewOptions : IDockViewOptions
{
    Encoding Encoding { get; }

    ObservableCollection<string> RecentFiles { get; }
}