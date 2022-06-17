using Imagin.Core.Converters;
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
                if (i.Values?.Length >= 3)
                {
                    if (i.Values[0] is MemberModel member)
                    {
                        if (i.Values[1] is bool isIndeterminate)
                        {
                            if (i.Values[2] is string indeterminateText)
                            {
                                string result = null;

                                if (i.Values.Length >= 4)
                                {
                                    if (i.Values[3] is IValueConverter converter)
                                        return converter.Convert(member, typeof(string), null, CultureInfo.CurrentCulture)?.ToString();
                                }

                                if (isIndeterminate)
                                    return indeterminateText;

                                result = $"{(member.Placeholder ?? member.DisplayName ?? member.Name)}";
                                return member.Localize ? result.Translate() : result;
                            }
                        }
                    }
                }
                return null;
            });
            Bindings.Add(new Binding() 
                { Path = new(".") });
            Bindings.Add(new Binding() 
                { Path = new("IsIndeterminate") });
            Bindings.Add(new Ancestor(nameof(PropertyGrid.IndeterminateText), 
                typeof(PropertyGrid)));
            Bindings.Add(new Ancestor(nameof(PropertyGrid.PlaceholderConverter), 
                typeof(PropertyGrid)));
            Bindings.Add(new RemoteBinding(nameof(MainViewOptions.Language), 
                RemoteBindingSource.Options));
        }
    }
}