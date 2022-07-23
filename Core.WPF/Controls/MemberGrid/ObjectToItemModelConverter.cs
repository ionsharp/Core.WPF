using Imagin.Core.Conversion;
using Imagin.Core.Reflection;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Controls;

[ValueConversion(typeof(object[]), typeof(ItemModel))]
public class ObjectToItemModelConverter : MultiConverter<ItemModel>
{
    public static ObjectToItemModelConverter Default { get; private set; } = new();
    public ObjectToItemModelConverter() : base() { }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2)
        {
            if (values[0] is MemberModel parent)
            {
                if (values[1] is object item)
                    return new ItemModel(parent, item);
            }
        }
        return Binding.DoNothing;
    }
}