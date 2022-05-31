using Imagin.Core;
using System;

namespace Demo;

public abstract class Test : Base
{
    public Type Type { get; private set; }

    public Test(Type type)
    {
        Type = type;
    }
}

public abstract class Test<T> : Test
{
    public Test() : base(typeof(T)) { }
}