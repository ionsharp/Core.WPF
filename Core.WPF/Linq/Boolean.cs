using System.Windows;

namespace Imagin.Core.Linq;

public static class XBoolean
{
    public static Visibility Visibility(this bool input, Visibility falseVisibility = System.Windows.Visibility.Collapsed) => input ? System.Windows.Visibility.Visible : falseVisibility;
}