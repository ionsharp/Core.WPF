using System.Windows;

namespace Imagin.Core;

/// <summary>Specifies an old and new value.</summary>
public class ReadOnlyValue<T>
{
    public readonly T Old = default;

    public readonly T New = default;

    public ReadOnlyValue(T @new) : this(default, @new) { }

    public ReadOnlyValue(T old, T @new)
    {
        Old = old;
        New = @new;
    }

    public static implicit operator ReadOnlyValue<T>(DependencyPropertyChangedEventArgs e)
        => new(e.OldValue is T i ? i : default, e.NewValue is T j ? j : default);
}

/// <inheritdoc />
public class ReadOnlyValue : ReadOnlyValue<object>
{
    public ReadOnlyValue(object old, object @new) : base(old, @new) { }
}