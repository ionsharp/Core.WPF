using Imagin.Core.Conversion;
using Imagin.Core.Reflection;
using System.Windows.Data;

namespace Imagin.Core.Controls;

public class MemberGroupConverterSelector : ConverterSelector
{
    public static readonly MemberGroupConverterSelector Default = new();

    public override IValueConverter Select(object input)
    {
        return $"{input}" switch
        {
            nameof(MemberGroupName.Category) => new ValueConverter<MemberModel, string>(i => i.Category),
            nameof(MemberGroupName.DisplayName) => new ValueConverter<MemberModel, string>(i => Converter.Get<FirstLetterConverter>().Convert(i.DisplayName, null, null, null)?.ToString()),
            //nameof(MemberGroupName.DeclaringType) => new ValueConverter<MemberModel, string>(i => i.DeclaringType.Name),
            nameof(MemberGroupName.Type) => new ValueConverter<MemberModel, string>(i => i.Type.Name),
            _ => default,
        };
    }
}