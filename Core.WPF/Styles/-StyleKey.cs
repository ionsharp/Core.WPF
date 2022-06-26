using Imagin.Core.Linq;
using Imagin.Core.Markup;

namespace Imagin.Core.Controls
{
    /// <summary>See <see cref="StyleKeys"/>.</summary>
    public class StyleKey : Uri
    {
        public const string KeyFormat = "Styles/{0}.xaml";

        public StyleKeys Key
        {
            set => RelativePath = KeyFormat.F(value);
        }

        public StyleKey() : base() => Assembly = AssemblyProperties.Name;
    }
}