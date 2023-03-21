using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Windows.Data;

namespace Imagin.Core.Data;

public class MultiBind : MultiBinding
{
    public Type Convert { set { Converter = (IMultiValueConverter)Conversion.Converter.Instances[value]; } }

    public MultiBind() : base() => Mode = BindingMode.OneWay;
}

public class UnitMultiBind : MultiBind
{
    public enum Types { Double, String }

    public Types Type { set => ConverterParameter = value; }

    public UnitMultiBind() : base()
    {
        Converter = new MultiConverter<object>(i =>
        {
            if (i.Values?.Length >= 4)
            {
                if (i.Values[0] is double value)
                {
                    if (i.Values[1] is float resolution)
                    {
                        if (i.Values[2] is Unit from)
                        {
                            if (i.Values[3] is Unit to)
                            {
                                var result = from.Convert(to, value, resolution);
                                if (i.Values.Length >= 5)
                                {
                                    if (i.Values[4] is int aRound)
                                    {
                                        result = result.Round(aRound);
                                        if (i.Values.Length >= 6)
                                        {
                                            if (i.Values[5] is double aScale)
                                                result *= aScale;
                                        }
                                    }
                                    else if (i.Values[4] is double bScale)
                                    {
                                        result *= bScale;
                                        if (i.Values.Length >= 6)
                                        {
                                            if (i.Values[5] is int bRound)
                                                result = result.Round(bRound);
                                        }
                                    }
                                }
                                return i.Parameter is Types x && x == Types.Double ? result : result.ToString();
                            }
                        }
                    }
                }
            }
            return Binding.DoNothing;
        });
        Type = Types.Double;
    }
}

public class VisibilityMultiBinding : MultiBind
{
    public VisibilityMultiBinding() : base() => Convert = typeof(BooleanToVisibilityMultiConverter);
}