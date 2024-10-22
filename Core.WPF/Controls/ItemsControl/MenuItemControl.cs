﻿using System.Windows;
using System.Windows.Controls;

namespace Imagin.Core.Controls
{
    public class MenuItemControl : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride() => new MenuItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is MenuItem;
    }
}