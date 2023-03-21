using Imagin.Core.Conversion;
using Imagin.Core.Data;
using System;

namespace Imagin.Core.Controls;

public class SourceBinding : Bind
{
    public Type Attribute { get; set; }

    public bool Ignore { get; set; }

    string view;
    public string View
    {
        get => view;
        set
        {
            view = value;
            Attribute = typeof(ViewAttribute);
        }
    }

    public SourceBinding() : this(".") { }

    public SourceBinding(Type attribute) : this(".", attribute) { }

    public SourceBinding(string path) : this(path, null) { }

    public SourceBinding(string path, Type attribute) : base(path)
    {
        Attribute = attribute;
        Converter = new ValueConverter<object, MemberSourceFilter>(i => new(i, Attribute, Ignore, View));
    }
}