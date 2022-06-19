using Imagin.Core.Conversion;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Controls
{
    public class MemberPlaceholder : MultiBinding
    {
        public MemberPlaceholder() : base()
        {
            Converter = new MultiConverter<string>(i =>
            {
                if (i.Values?.Length >= 1)
                {
                    if (i.Values[0] is MemberModel member)
                    {
                        string result = null;

                        if (i.Values.Length >= 2)
                        {
                            if (i.Values[1] is IValueConverter converter)
                                return converter.Convert(member, typeof(string), null, CultureInfo.CurrentCulture)?.ToString();
                        }

                        result = $"{(member.Placeholder ?? member.DisplayName ?? member.Name)}";
                        return member.Localize ? result.Translate() : result;
                    }
                }
                return null;
            });
            Bindings.Add(new Binding() 
                { Path = new(".") });
            Bindings.Add(new Ancestor(nameof(MemberGrid.PlaceholderConverter), 
                typeof(MemberGrid)));
            Bindings.Add(new RemoteBinding(nameof(MainViewOptions.Language), 
                RemoteBindingSource.Options));
        }
    }
}