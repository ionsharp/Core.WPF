using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
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
            if (values?.Length == 4)
            {
                if (values[0] is MemberModel model)
                {
                    if (values[1] is bool isVisible)
                    {
                        if (isVisible)
                        {
                            if (values[2] is MemberSearchName name)
                            {
                                if (values[3] is string search)
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
                                    return Visibility.Visible;
                                }
                            }
                        }

                    }
                }
            }
            return Visibility.Collapsed;
        }
    }
}