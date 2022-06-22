using System;

namespace Imagin.Core.Controls;

internal class SourceFilter
{
    public readonly bool Ignore;

    public readonly object Source;

    public readonly Type Type;

    public readonly string View;

    public SourceFilter(object source, Type type, bool ignore, string view = default) : base()
    {
        Source = source;
        Type = type;
        Ignore = ignore;
        View = view;
    }
}