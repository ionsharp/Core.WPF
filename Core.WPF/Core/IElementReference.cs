using System.Windows;

namespace Imagin.Core;

public interface IElementReference
{
    void SetReference(IElementKey key, FrameworkElement element);
}