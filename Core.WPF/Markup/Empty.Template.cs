using System;
using System.Windows;
using System.Windows.Markup;

namespace Imagin.Core.Markup
{
    public class EmptyTemplate : MarkupExtension
    {
        public EmptyTemplate() : base() { }

        public override object ProvideValue(IServiceProvider serviceProvider) => new DataTemplate();
    }
}