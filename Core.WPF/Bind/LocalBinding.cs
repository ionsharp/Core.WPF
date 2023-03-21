using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Windows.Data;

namespace Imagin.Core.Data;

public class LocalBinding : MultiBind, IGlobalSource
{
    readonly Bind KeyBinding = null;

    ///

    public Type ConvertKey { set => KeyBinding.Convert = value; }

    ///

    public RelativeSourceMode From { set => KeyBinding.RelativeSource = new(value); }

    public Type FromType { set => KeyBinding.RelativeSource = new(RelativeSourceMode.FindAncestor) { AncestorType = value }; }

    ///

    public GlobalSource GlobalSource { set => KeyBinding.Source = this.GetGlobalSource(value); }

    ///

    public string Format { get; set; }

    ///

    public bool Lower { get; set; }

    public bool Upper { get; set; }

    ///

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    ///

    public LocalBinding() : this(".") { }

    public LocalBinding(string path) : base()
    {
        Converter = new MultiConverter<string>(i =>
        {
            if (i.Values?.Length > 0)
            {
                return Try.Return(() =>
                {
                    var result = i.Values[0]?.ToString()?.Translate(Prefix, Suffix, Format);
                    result = Lower ? result?.ToLower() : Upper ? result?.ToUpper() : result;

                    return result;
                });
            }
            return null;
        });

        KeyBinding = new Bind(path);
        Bindings.Add(KeyBinding);

        Bindings.Add(new Options(nameof(MainViewOptions.Language)));
    }

    ///

    public LocalBinding(Type converter) : this(".", converter) { }

    public LocalBinding(string path, Type converter) : this(path) => ConvertKey = converter;
}