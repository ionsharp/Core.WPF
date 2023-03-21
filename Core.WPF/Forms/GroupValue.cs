using Imagin.Core.Collections.Serialization;
using Imagin.Core.Reflection;
using System;
using System.Windows;

namespace Imagin.Core.Models;

[Categorize(false), ViewSource(ShowHeader = false)]
public class GroupValueForm<T> : Base<object>, IGeneric
{
    [Name("Group"), Pin(Pin.AboveOrLeft), Require]
    [Int32Style(Int32Style.Index, nameof(Groups), "Name")]
    public int GroupIndex { get => Get(-1); set => Set(value); }

    [Hide]
    public IGroupWriter Groups { get; private set; }

    [Editable, HideName, NameFromValue]
    public override object Value { get => base.Value; set => base.Value = value; }

    public GroupValueForm(IGroupWriter groups, object value, int groupIndex) : base(value)
    {
        Groups = groups; GroupIndex = groupIndex;
    }

    Type IGeneric.GetGenericType() => typeof(T);
}