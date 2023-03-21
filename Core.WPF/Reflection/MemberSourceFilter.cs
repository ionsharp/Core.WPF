using System;

namespace Imagin.Core.Controls;

internal class MemberSourceFilter
{
    public readonly Type Attribute;

    public readonly bool Ignore;

    public readonly object Source;

    public readonly string View;

    public MemberSourceFilter(object source, Type attribute, bool ignore, string view = default) : base()
    {
        Source = source; Attribute = attribute; Ignore = ignore; View = view;
    }
}