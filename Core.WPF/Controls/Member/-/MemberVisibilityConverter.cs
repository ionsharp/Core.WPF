using Imagin.Core.Conversion;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Controls;

public class MemberVisibilityBinding : MultiBind
{
    public MemberVisibilityBinding() : base()
    {
        Convert = typeof(MemberVisibilityConverter);

        Bindings.Add(new Binding("."));

        Bindings.Add(new Ancestor(nameof(MemberGrid.SearchName), typeof(MemberGrid)));
        Bindings.Add(new Ancestor(nameof(MemberGrid.Search), typeof(MemberGrid)));

        Bindings.Add(new Binding(nameof(MemberModel.IsIndeterminate)));

        Bindings.Add(new Binding(nameof(MemberModel.IsVisible)));
        Bindings.Add(new Binding(nameof(MemberModel.Members)));
        Bindings.Add(new Binding(nameof(MemberModel.Value)));

        Bindings.Add(new Binding($"{nameof(MemberModel.Members)}.{nameof(IList.Count)}"));
        Bindings.Add(new Binding($"{nameof(MemberModel.Value)}.{nameof(IList.Count)}"));
    }
}

public class MemberVisibilityConverter : MultiConverter<Visibility>
{
    public MemberVisibilityConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length >= 3)
        {
            if (values[0] is MemberModel model)
            {
                if (!model.IsVisible)
                    return Visibility.Collapsed;

                if (model is IAssignableMemberModel assignable)
                {
                    if (!model.IsIndeterminate && assignable.HideIfNull && ((model.Value is string value && value.NullOrWhiteSpace()) || model.Value == null))
                        return Visibility.Collapsed;
                }

                var modelType = model.Value?.GetType() ?? model.Type;

                if (model.IsObjectType(modelType) && (model.Members == null || model.Members.Count == 0))
                    return Visibility.Collapsed;

                if (model.IsCollectionType(modelType) && model.Value == null && (model.Members == null || model.Members.All.Count == 0))
                    return Visibility.Collapsed;

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
                            if (!a.StartsWith(b))
                                return Visibility.Collapsed;
                        }
                    }
                }

                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }
}