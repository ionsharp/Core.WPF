using System;

namespace Imagin.Core.Reflection;

public enum ArrayStyle
{
    Comma, Bullet,
}

public enum BooleanStyle
{
    Image, Switch
}

public enum CollectionStyle
{
    Bullet, Button, Comma, Default, ImageToggleButton
}

public enum ColorStyle
{
    Alpha
}

public enum DoubleStyle
{
    Angle, Progress, ProgressRound, Unit
}

public enum EnumStyle
{
    FlagCheck, FlagSelect, FlagSwitch, FlagToggle
}

public enum EnumerableStyle
{
    Comma, Bullet,
}

public enum Int32Style
{
    Index,
}

public enum ListStyle
{
    Comma, Bullet,
}

public enum ObjectStyle
{
    Button,
    Shallow,
    Deep
}

public enum StringStyle
{
    FilePath, FolderPath,
    MultiFiles, MultiFolders, MultiLines, MultiPaths,
    Password, Search, Thumbnail, Tokens
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MemberStyleAttribute : Attribute
{
    public readonly Enum Style;

    public MemberStyleAttribute(ArrayStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(BooleanStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(CollectionStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(ColorStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(DoubleStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(EnumStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(EnumerableStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(Int32Style style)
        : base() => Style = style;

    public MemberStyleAttribute(ListStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(ObjectStyle style)
        : base() => Style = style;

    public MemberStyleAttribute(StringStyle style)
        : base() => Style = style;
}