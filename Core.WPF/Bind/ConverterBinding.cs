using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Data;

#region ConverterBinding

public abstract class ConverterBinding : Bind
{
    public virtual bool Invert { get => ConverterParameter is int i && i == 1; set => ConverterParameter = value ? 1 : 0; }

    protected ConverterBinding() : this(".") { }

    protected ConverterBinding(string path) : base(path) { }
}

#endregion

///

#region CompareBinding

public abstract class CompareBinding : ConverterBinding
{
    public enum Types { Equal, Greater, Lesser }

    public enum Results { Boolean, Visibility }

    ///

    public override bool Invert { get; set; } = false;

    public Results Result { get; set; } = Results.Boolean;

    public virtual Types Type { get; set; } = Types.Equal;

    ///

    public CompareBinding() : this(".") { }

    public CompareBinding(string path) : base(path) { }
}

#endregion

#region CompareBinding<T>

public abstract class CompareBinding<T> : CompareBinding
{
    static readonly Dictionary<Type, IValueConverter> Converters = new();

    public T Value { get; set; } = default;

    public CompareBinding() : this(".") { }

    public CompareBinding(string path) : this(path, default) { }

    public CompareBinding(string path, T value) : base(path)
    {
        Value = value;
    }

    protected abstract object Compare(T a);
}

#endregion

///

#region CompareInt32Binding

public class CompareInt32Binding : CompareBinding<int>
{
    public CompareInt32Binding() : this(".") { }

    public CompareInt32Binding(string path) : this(path, 0) { }

    public CompareInt32Binding(string path, int value) : this(path, Types.Equal, value)
    {
        Converter = new ValueConverter<int, object>(true, i => Compare(i.Value));
    }

    public CompareInt32Binding(string path, Types type, int value) : base(path, value)
    {
        Type = type;
        Converter = new ValueConverter<int, object>(true, i => Compare(i.Value));
    }

    protected override object Compare(int a)
    {
        bool result = false; var b = Value;
        switch (Type)
        {
            case Types.Equal:
                result = Invert ? a != b : a == b;
                break;

            case Types.Greater:
                result = Invert ? a <= b : a > b;
                break;

            case Types.Lesser:
                result = Invert ? a >= b : a < b;
                break;
        }
        return Result == Results.Boolean ? result : result.Visibility();
    }
}

#endregion

///

#region IsBinding

public class IsBinding : ConverterBinding
{
    public Type Type { set => ConverterParameter = value; }

    public IsBinding() : this(".", null) { }

    public IsBinding(Type type) : this(".", type) { }

    public IsBinding(string path, Type type) : base(path)
    {
        Convert = typeof(IsConverter);
        Type = type;
    }
}

#endregion

#region VisibilityBinding

public class VisibilityBinding : ConverterBinding
{
    public VisibilityBinding() : this(".") { }

    public VisibilityBinding(string path) : this(path, 0) { }

    public VisibilityBinding(string path, int converterParameter) : base(path)
    {
        Converter = new ValueConverter<object, object>(true, i =>
        {
            if (i.ActualValue is bool || i.ActualValue is Visibility)
                return Conversion.Converter.Get<BooleanToVisibilityConverter>().Convert(i.ActualValue, null, i.ActualParameter, null);

            if (i.ActualValue is int)
                return Conversion.Converter.Get<Int32ToVisibilityConverter>().Convert(i.ActualValue, null, i.ActualParameter, null);

            if (i.ActualValue is string)
                return Conversion.Converter.Get<StringToVisibilityConverter>().Convert(i.ActualValue, null, i.ActualParameter, null);

            return Conversion.Converter.Get<ObjectToVisibilityConverter>().Convert(i.ActualValue, null, i.ActualParameter, null);
        }, i =>
        {
            if (i.ActualValue is bool || i.ActualValue is Visibility)
                return Conversion.Converter.Get<BooleanToVisibilityConverter>().ConvertBack(i.ActualValue, null, i.ActualParameter, null);

            if (i.ActualValue is int)
                return Conversion.Converter.Get<Int32ToVisibilityConverter>().ConvertBack(i.ActualValue, null, i.ActualParameter, null);

            if (i.ActualValue is string)
                return Conversion.Converter.Get<StringToVisibilityConverter>().ConvertBack(i.ActualValue, null, i.ActualParameter, null);

            return Conversion.Converter.Get<ObjectToVisibilityConverter>().ConvertBack(i.ActualValue, null, i.ActualParameter, null);
        });
        ConverterParameter = converterParameter;
    }
}

#endregion

///

#region ResultConverterBinding : ConverterBinding

public abstract class ResultConverterBinding : ConverterBinding
{
    public enum Results { Boolean, Visibility }

    public Results Result { get; set; } = Results.Boolean;

    protected ResultConverterBinding() : this(".") { }

    protected ResultConverterBinding(string path) : base(path) { }
}

#endregion

#region EqualBinding : ResultConverterBinding

public class EqualBinding : ResultConverterBinding
{
    public readonly object Value;

    public EqualBinding() : this(".") { }

    public EqualBinding(string path) : this(path, null) { }

    public EqualBinding(string path, object value) : base(path)
    {
        Value = value;
        Converter = new ValueConverter<object, object>(true, i =>
        {
            var result = (i.Value == Value || Equals(i.Value, Value) || ReferenceEquals(i.Value, Value)) is bool j ? Invert ? !j : j : false;
            return Result == Results.Boolean ? result : result.Visibility();
        });
    }
}

#endregion