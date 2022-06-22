using Imagin.Core.Conversion;
using Imagin.Core.Data;
using System;

namespace Imagin.Core.Controls;

/// <summary>Filters an object by <see cref="Attribute"/> before <see cref="MemberGrid"/> reads it.</summary>
public class SourceBinding : LocalBinding
{
    public Type Filter { get; private set; }

    public bool Ignore { get; set; }

    string view;
    public string View
    {
        get => view;
        set
        {
            view = value;
            Filter = typeof(ViewAttribute);
        }
    }

    public SourceBinding() : this(null) { }

    public SourceBinding(Type type) : this(".", type) { }

    public SourceBinding(string path, Type type) : base(path)
    {
        Filter = type;
        Converter = new SimpleConverter<object, SourceFilter>(i => new(i, Filter, Ignore, View));
    }
}