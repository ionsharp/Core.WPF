using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows;

namespace Imagin.Core.Markup;

public class Cursor : Uri
{
    public System.Drawing.Point Point { get; set; } = new System.Drawing.Point(0, 0);

    public Cursor(string relativePath) : base(relativePath, AssemblyType.Current) { }

    public Cursor(string relativePath, AssemblyType assembly) : base(relativePath, assembly) { }

    public Cursor(string relativePath, int x, int y) : this(relativePath, x, y, AssemblyType.Current) { }

    public Cursor(string relativePath, int x, int y, AssemblyType assembly) : this(relativePath, assembly) => Point = new(x, y);

    public override object ProvideValue(IServiceProvider serviceProvider)
        => base.ProvideValue(serviceProvider).To<System.Uri>().GetImage().Bitmap(ImageExtensions.Png).Cursor(Point.X, Point.Y).Convert();
}