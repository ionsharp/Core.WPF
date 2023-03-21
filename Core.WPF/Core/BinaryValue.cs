using Imagin.Core.Linq;
using System;
using System.Runtime.Serialization;

namespace Imagin.Core;

[Serializable]
public class BinaryValue<NonSerializable, Serializable, Converter> : Base where Converter : Conversion.ValueConverter<NonSerializable, Serializable>
{
    [NonSerialized]
    Converter converter = default;

    Serializable serializedValue = default;

    public NonSerializable Value { get => Get<NonSerializable>(default, false); set => Set(value, false); }

    public BinaryValue() : base() { }

    public BinaryValue(NonSerializable value) : this() => Value = value;

    protected NonSerializable ConvertBack(Serializable input)
    {
        converter ??= typeof(Converter).Create<Converter>();
        return converter.ConvertBack(input) is NonSerializable result ? result : default;
    }

    protected Serializable ConvertTo(NonSerializable input)
    {
        converter ??= typeof(Converter).Create<Converter>();
        return converter.Convert(input) is Serializable result ? result : default;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext context)
    {
        var result = ConvertTo(Value);
        serializedValue = result is Serializable i ? i : default;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        var result = ConvertBack(serializedValue);
        Value = result is NonSerializable i ? i : default;
    }
}