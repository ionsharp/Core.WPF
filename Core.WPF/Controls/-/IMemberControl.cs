using System.Windows;

namespace Imagin.Core.Controls;

public interface IMemberControl
{
    object GetValue(DependencyProperty property);

    void SetValue(DependencyProperty property, object value);

    void SetValue(DependencyPropertyKey property, object value);
}