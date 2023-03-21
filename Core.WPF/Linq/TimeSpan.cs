using System;
using System.Windows;

namespace Imagin.Core.Linq;

public static class XTimeSpan
{
    public static Duration Duration(this TimeSpan i) => new(i);
}