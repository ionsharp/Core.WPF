using System.Windows;

namespace Imagin.Core
{
    public interface IFrameworkReference
    {
        void SetReference(IFrameworkKey key, FrameworkElement element);
    }
}