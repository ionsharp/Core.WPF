﻿using Imagin.Core.Conversion;
using System.Windows.Data;

namespace Imagin.Core.Data;

public sealed class TwoBinding : MultiBinding
{
    public TwoBinding() : base()
    {
        Converter = new MultiConverter<object[]>(i => i.Values);
    }
    
    public TwoBinding(string path1, object source1) : this()
    {
        Bindings.Add(new Binding()
        {
            Mode = BindingMode.OneWay,
            Path = new(path1),
            Source = source1
        });
        Bindings.Add(new Binding()
        {
            Mode = BindingMode.OneWay,
            Path = new(".")
        });
    }

    public TwoBinding(string path1, object source1, string path2, object source2) : this()
    {
        Bindings.Add(new Binding()
        {
            Mode = BindingMode.OneWay,
            Path = new(path1),
            Source = source1
        });
        Bindings.Add(new Binding()
        {
            Mode = BindingMode.OneWay,
            Path = new(path2),
            Source = source2,
        });
    }
}