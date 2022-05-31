using System;
using System.Windows.Markup;

namespace Imagin.Core.Markup
{
    public class EmptyString : MarkupExtension
    {
        public EmptyString() : base() { }

        public override object ProvideValue(IServiceProvider serviceProvider) => string.Empty;
    }
}