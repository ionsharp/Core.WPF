using Imagin.Core.Data;
using Imagin.Core.Linq;
using System;

namespace Imagin.Core.Storage;

[Name("Extensions")]
[Serializable]
public class FilterExtensions : Base
{
    public Exclusivity Filter { get => Get(Exclusivity.Include); set => Set(value); }

    [Name("Extensions"), StringStyle(StringStyle.Tokens)]
    public string Value { get => Get(""); set => Set(value); }

    public FilterExtensions() : base() { }

    public override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);
        if (e.PropertyName == nameof(Value))
            e.NewValue = e.NewValue?.ToString().Replace(".", string.Empty);
    }

    public override string ToString() => Value.NullOrEmpty() ? "All" : $"{Filter} .{Value.Replace(";", ", .").TrimEnd('.').TrimEnd(' ').TrimEnd(',')}";
}