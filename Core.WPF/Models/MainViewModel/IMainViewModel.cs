using System.Collections.Generic;
using System.Windows;

namespace Imagin.Core.Models;

public interface IMainViewModel
{
    Window View { get; set; }

    void OnLoaded(IList<string> arguments);

    void OnReloaded(IList<string> arguments);
}