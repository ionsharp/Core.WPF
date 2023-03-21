using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Windows.Data;

namespace Imagin.Core.Data;

#region AttributeBinding

public abstract class AttributeBinding : MultiBind
{
    protected AttributeBinding(string path) : base()
    {
        Bindings.Add(new Binding(path));
    }

    protected static string Parse(object input)
    {
        if (input != null)
        {
            var result = $"{input}";
            if (!result.Empty())
                return result;
        }
        return null;
    }
}

#endregion

#region LocalizableAttributeBinding

public abstract class LocalizableAttributeBinding : AttributeBinding
{
    public LocalizableAttributeBinding() : this(".") { }

    public LocalizableAttributeBinding(string path) : base(path)
        => Bindings.Add(new Options(nameof(MainViewOptions.Language)));
}

#endregion

///

#region ImageAttributeBinding

public class ImageAttributeBinding : AttributeBinding
{
    public enum Sizes { Small, Large }

    public Sizes Size { set => ConverterParameter = value == Sizes.Small ? 0 : 1; }

    public enum Types { Image, String }

    public Types Type { get; set; } = Types.String;

    public ImageAttributeBinding() : this(".") { }

    public ImageAttributeBinding(string path) : base(path)
    {
        Converter = new MultiConverter<object>(i =>
        {
            if (i.Values?.Length >= 1)
            {
                if (i.Values[0] is object j)
                {
                    var result = Conversion.Converter.Get<ImageAttributeConverter>().Convert(j, ConverterParameter ?? 0);

                    if (Type == Types.Image)
                        return Try.Return(() => new Uri(result, UriKind.Absolute).GetImage());

                    return result;
                }
            }
            return null;
        });
    }
}

#endregion

///

#region DescriptionAttributeBinding

public class DescriptionAttributeBinding : LocalizableAttributeBinding
{
    public DescriptionAttributeBinding() : this(".") { }

    public DescriptionAttributeBinding(string path) : base(path) 
        => Convert = typeof(AttributeMultiConverter<DescriptionAttributeConverter>);
}

#endregion

#region NameAttributeBinding

public class NameAttributeBinding : LocalizableAttributeBinding
{
    public NameAttributeBinding() : this(".") { }

    public NameAttributeBinding(string path) : base(path) 
        => Convert = typeof(AttributeMultiConverter<NameAttributeConverter>);
}

#endregion

///

#region MemberImageBinding

public class MemberImageBinding : AttributeBinding
{
    public enum Types { Image, String }

    public Types Type { get; set; } = Types.String;

    public MemberImageBinding() : this(".") { }

    public MemberImageBinding(string path) : base(path)
    {
        Converter = new MultiConverter<object>(i =>
        {
            if (i.Values?.Length >= 1)
            {
                if (i.Values[0] is MemberModel j)
                {
                    var result = Conversion.Converter.Get<ImageAttributeConverter>().Convert(j, ConverterParameter);

                    if (Type == Types.Image)
                        return Try.Return(() => new Uri(result, UriKind.Absolute).GetImage());

                    return result;
                }
            }
            return null;
        });
        Bindings.Add(new Binding(nameof(MemberModel.Icon)));
        Bindings.Add(new Binding(nameof(MemberModel.Value)));
    }
}

#endregion

///

#region MemberDescriptionBinding

public class MemberDescriptionBinding : LocalizableAttributeBinding
{
    public MemberDescriptionBinding() : this(".") { }

    public MemberDescriptionBinding(string path) : base(path)
    {
        Convert = typeof(AttributeMultiConverter<DescriptionAttributeConverter>);
        Bindings.Add(new Binding(nameof(MemberModel.Description)));
        Bindings.Add(new Binding(nameof(MemberModel.Value)));
    }
}

#endregion

#region MemberNameBinding

public class MemberNameBinding : LocalizableAttributeBinding
{
    public MemberNameBinding() : this(".") { }

    public MemberNameBinding(string path) : base(path)
    {
        Convert = typeof(AttributeMultiConverter<NameAttributeConverter>);
        Bindings.Add(new Binding(nameof(MemberModel.DisplayName)));
        Bindings.Add(new Binding(nameof(MemberModel.Value)));
    }
}

#endregion