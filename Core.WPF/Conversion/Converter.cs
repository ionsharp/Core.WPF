using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Conversion;

public static class Converter
{
    public static readonly IEnumerable<IValueConverter> DefaultConverters = new List<IValueConverter>
    {
        new ValueConverter<Double, One>(false, i => (One)i.Value, i => (double)i.Value),
        new ValueConverter<One, Double>(false, i => (double)i.Value, i => (One)i.Value),

        new ValueConverter<Matrix, DoubleMatrix>(i => new DoubleMatrix((double[,])i), i => new Matrix(i)),

        new ValueConverter<Controls.SelectionMode, System.Windows.Controls.SelectionMode>(i =>
        {
            return i switch
            {
                Controls.SelectionMode.Multiple => System.Windows.Controls.SelectionMode.Extended,
                Controls.SelectionMode.Single or Controls.SelectionMode.SingleOrNone => System.Windows.Controls.SelectionMode.Single,
                _ => throw new NotSupportedException(),
            };
        }),

        new ValueConverter<Result, Error>(false, i => (Error)i.Value, i => i.Value),
        new ValueConverter<Error, Result>(false, i => i.Value, i => (Error)i.Value),

        new ValueConverter<SolidColorBrush, ByteVector4>(false, i => { i.Value.Color.Convert(out ByteVector4 result); return result;}),

        new ValueConverter<Imagin.Core.Data.SortDirection, ListSortDirection>(i => i.Convert(), i => i.Convert()),

        new ValueConverter<string, FontFamily>(false, i =>
        {
            FontFamily result = null;
            Try.Invoke(() => result = new FontFamily(i.Value));
            return result;
        },
        i => i.Value.Source)
    };

    public static readonly Instances Instances = new();

    public static T Get<T>() where T : IValueConverter => (T)Get(typeof(T));

    public static ValueConverter<TInput, TOutput> Get<TInput, TOutput>()
        => (ValueConverter<TInput, TOutput>)DefaultConverters.FirstOrDefault(i => i is ValueConverter<TInput, TOutput>);

    public static IValueConverter Get(Type input, Type output)
        => DefaultConverters.FirstOrDefault(i => i is IConvert j && j.InputType == input && j.OutputType == output);

    public static IValueConverter Get(Type type) => (IValueConverter)Instances[type];
}