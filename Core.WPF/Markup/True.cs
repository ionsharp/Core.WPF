using System;
using System.Windows.Markup;

namespace Imagin.Core.Markup
{
    public class True : MarkupExtension
    {
        public True() : base() { }

        public override object ProvideValue(IServiceProvider serviceProvider) => true;
    }
}