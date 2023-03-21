using Imagin.Core.Models;
using System;

namespace Imagin.Core.Config;

public class ApplicationLink
{
    public virtual Type View { get; }

    public virtual Type ViewModel { get; }

    public virtual Type ViewOptions { get; }
}

public class ApplicationLink<X, Y, Z> : ApplicationLink where X : IMainView where Y : IMainViewModel where Z : MainViewOptions
{
    public override Type View => typeof(X);

    public override Type ViewModel => typeof(Y);

    public override Type ViewOptions => typeof(Z);

    ///

    public ApplicationLink() : base() { }
}