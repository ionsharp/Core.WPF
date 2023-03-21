using System.Windows;

namespace Imagin.Core.Linq;

public static class XDependencyPropertyChangedEventArgs
{
    public static ReadOnlyValue<T> Convert<T>(this DependencyPropertyChangedEventArgs e)
        => new ReadOnlyValue<T>(e.OldValue is T i ? i : default, e.NewValue is T j ? j : default);

    public static ReadOnlyValue Convert(this DependencyPropertyChangedEventArgs e)
        => new ReadOnlyValue(e.OldValue, e.NewValue);
}