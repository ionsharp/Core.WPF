using Imagin.Core.Analytics;
using Imagin.Core.Converters;
using Imagin.Core.Linq;
using System;
using System.Globalization;
using System.Windows;

namespace Imagin.Core.Controls
{
    public class MemberVisibilityConverter : MultiConverter<Visibility>
    {
        public static MemberVisibilityConverter Default { get; private set; } = new MemberVisibilityConverter();
        MemberVisibilityConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 3)
            {
                if (values[0] is MemberModel model)
                {
                    if (values[1] is MemberSearchName name)
                    {
                        if (values[2] is string search)
                        {
                            var a = string.Empty;
                            var b = search.ToLower();

                            if (!b.Empty())
                            {
                                switch (name)
                                {
                                    case MemberSearchName.Category:
                                        a = model.Category?.ToLower() ?? string.Empty;
                                        break;
                                    case MemberSearchName.Name:
                                        a = model.DisplayName?.ToLower() ?? string.Empty;
                                        break;
                                }
                                return a.StartsWith(b).Visibility();
                            }
                        }
                    }
                }
            }
            return Visibility.Visible;
        }
    }
}