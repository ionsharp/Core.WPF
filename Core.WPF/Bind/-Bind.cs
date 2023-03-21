using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System;
using System.Windows.Data;

namespace Imagin.Core.Data;

public class Bind : Binding, IGlobalSource
{
    public Type Convert { set { Converter = (IValueConverter)Conversion.Converter.Instances[value]; } }

    ///

    Type convertFrom = null;
    public Type ConvertFrom { get => convertFrom; set { convertFrom = value; if (ConvertFrom != null && ConvertTo != null) Converter = Conversion.Converter.Get(ConvertFrom, ConvertTo); } }

    Type convertTo = null;
    public Type ConvertTo { get => convertTo; set { convertTo = value; if (ConvertFrom != null && ConvertTo != null) Converter = Conversion.Converter.Get(ConvertFrom, ConvertTo); } }

    ///

    public GlobalSource GlobalSource { set => Source = this.GetGlobalSource(value); }

    public RelativeSourceMode From { set => RelativeSource = value == RelativeSourceMode.FindAncestor ? new(RelativeSourceMode.FindAncestor) { AncestorType = fromType } : new(value); }

    Type fromType = null;
    public Type FromType { get => fromType; set { fromType = value; RelativeSource = new(RelativeSourceMode.FindAncestor) { AncestorType = value }; } }

    public object Parameter { set => ConverterParameter = value; }

    public UpdateSourceTrigger Trigger { set => UpdateSourceTrigger = value; }

    public int Way { set => Mode = value == 0 ? BindingMode.OneWayToSource : value == 1 ? BindingMode.OneWay : value == 2 ? BindingMode.TwoWay : throw new NotSupportedException(); }

    ///

    public Bind() : this(".") { }

    public Bind(string path) : base(path)
    {
        Trigger = UpdateSourceTrigger.PropertyChanged;
        Way = 1;
    }

    public Bind(string path, GlobalSource source) : this(path)
    {
        Converter = new ValueConverter<object, object>(i => i ?? Nothing.Do, i => i);
        GlobalSource = source;
    }

    public Bind(Type converter, object converterParameter) : this()
    {
        Convert = converter; Parameter = converterParameter;
    }
}

public class Options : Bind
{
    public Options() : this(".") { }

    public Options(string path) : base(path, GlobalSource.Options) { }
}