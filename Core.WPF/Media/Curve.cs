using Imagin.Core.Analytics;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Imagin.Core.Media;

#region (class) Curve

[Description("The rate of change of a parameter over time.")]
[Base(typeof(Curve)), Name("Curve"), Categorize(false), Image(AssemblyType.Current, "Curve.png"), Serializable]
public class Curve : Base, ICloneable
{
    public static string GetCode(string code)
    {
        static string Sanitize(string input)
        {
            return input;
        }

        var result = new StringBuilder();
        result.Append("using System;");
        result.Append("\r\nnamespace Imagin.Core.Media");
        result.Append("\r\n{");
        result.Append("\r\npublic sealed class CustomCurve");
        result.Append("\r\n{");
        result.Append("\r\npublic static double Execute(double start, double end, double value)");
        result.Append("\r\n{");
        result.AppendFormat("\r\n{0}", Sanitize(code));
        result.Append("\r\n}");
        result.Append("\r\n}");
        result.Append("\r\n}");
        return result.ToString();
    }

    ///

    public static CSharpCodeProvider CompileCode(string code, out MethodInfo method)
    {
        var codeProvider = new CSharpCodeProvider();

        var compilerParameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
        compilerParameters.ReferencedAssemblies.AddRange(new[]
        {
            "System.dll",
            "mscorlib.dll",
        });

        string sourceCode = Curve.GetCode(code);
        var compilerResults
            = codeProvider.CompileAssemblyFromSource(compilerParameters, sourceCode);

        method = compilerResults.CompiledAssembly.GetType("Imagin.Core.Media.CustomCurve").GetMethod("Execute");
        return codeProvider;
    }

    /// <summary>Defines a custom curve using typed C# code that gets compiled in memory.</summary>
    public static double RunCode(double start, double end, double value, string code)
    {
        double result = 0;
        using (var provider = CompileCode(code, out MethodInfo method))
            result = method?.Invoke(null, new object[] { start, end, value }) is double i ? i : 0;

        return result;
    }

    ///

    const double NATURAL_LOG_OF_2 = 0.693147181d;

    [Caption("<p>Code to perform arithmetic (C#). Available variables are <b>start</b>, <b>end</b>, and <b>value</b>.</p><p><b>Example</b> (Variation of 'ease in quad'):</p> <code>end -= start; start -= 0.02; return end * (end - value) * value + start;</code>")]
    [StringStyle(StringStyle.MultiLine)]
    [UpdateSourceTrigger(System.Windows.Data.UpdateSourceTrigger.LostFocus)]
    public virtual string Code { get => Get(""); set => Set(value); }

    ///

    public virtual double Execute(double x) => Execute(0, 1, x);

    public virtual double Execute(double start, double end, double value) => Try.Return(() => RunCode(start, end, value, Code), e => Log.Write<Curve>(e));

    public virtual object Clone() => GetType().Create<Curve>();
}

#endregion

#region (class) DefaultCurve

[Serializable]
public class DefaultCurve : Curve
{
    /// <summary>Natural log of 2</summary>
    const double Nl2 = 0.693147181d;

    ///

    public static Dictionary<DefaultCurves, MethodInfo> Functions = new();

    public static Dictionary<DefaultCurves, MethodInfo> FunctionsD = new();

    ///

    [Hide]
    public override string Code { get => base.Code; set => base.Code = value; }

    [Hide]
    public readonly DefaultCurves Type;

    ///

    static DefaultCurve()
    {

        typeof(DefaultCurves).GetEnumValues().ForEach(i
            => Functions.Add((DefaultCurves)i, typeof(DefaultCurve).GetMethod($"{i}")));
        typeof(DefaultCurves).GetEnumValues().ForEach(i
            => FunctionsD.Add((DefaultCurves)i, typeof(DefaultCurve).GetMethod($"{i}D")));
    }

    public DefaultCurve(DefaultCurves type) : base() => Type = type;

    ///

    public override object Clone() => new DefaultCurve(Type);

    public override double Execute(double x) => Execute(0, 1, x);

    public override double Execute(double start, double end, double value) => Get(Type).Invoke(start, end, value);

    ///

    /// <summary>Returns the function associated to the easingFunction enum. This value returned should be cached as it allocates memory to return.</summary>
    public static Func<double, double, double, double> Get(DefaultCurves type)
    {
        var method = Functions[type];
        return new Func<double, double, double, double>((a, b, c) => (double)method.Invoke(null, new object[] { a, b, c }));
    }

    /// <summary>Gets the derivative function of the appropriate easing function. If you use an easing function for position then this function can get you the speed at a given time (normalized).</summary>
    public static Func<double, double, double, double> GetInverse(DefaultCurves type)
    {
        var method = FunctionsD[type];
        return new Func<double, double, double, double>((a, b, c) => (double)method.Invoke(null, new object[] { a, b, c }));
    }

    #region Functions

    public static double Linear(double start, double end, double value)
        => start + value * (end - start);

    public static double Spring(double start, double end, double value)
    {
        //value = Math.Clamp01(value);
        value = (Math.Sin(value * Math.PI * (0.2d + 2.5d * value * value * value)) * Math.Pow(1d - value, 2.2d) + value) * (1d + (1.2d * (1d - value)));
        return start + (end - start) * value;
    }

    public static double EaseInQuad(double start, double end, double value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static double EaseOutQuad(double start, double end, double value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static double EaseInOutQuad(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return end * 0.5d * value * value + start;
        value--;
        return -end * 0.5d * (value * (value - 2) - 1) + start;
    }

    public static double EaseInCubic(double start, double end, double value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static double EaseOutCubic(double start, double end, double value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static double EaseInOutCubic(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return end * 0.5d * value * value * value + start;
        value -= 2;
        return end * 0.5d * (value * value * value + 2) + start;
    }

    public static double EaseInQuart(double start, double end, double value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static double EaseOutQuart(double start, double end, double value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    public static double EaseInOutQuart(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return end * 0.5d * value * value * value * value + start;
        value -= 2;
        return -end * 0.5d * (value * value * value * value - 2) + start;
    }

    public static double EaseInQuint(double start, double end, double value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static double EaseOutQuint(double start, double end, double value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    public static double EaseInOutQuint(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return end * 0.5d * value * value * value * value * value + start;
        value -= 2;
        return end * 0.5d * (value * value * value * value * value + 2) + start;
    }

    public static double EaseInSine(double start, double end, double value)
    {
        end -= start;
        return -end * Math.Cos(value * (Math.PI * 0.5d)) + end + start;
    }

    public static double EaseOutSine(double start, double end, double value)
    {
        end -= start;
        return end * Math.Sin(value * (Math.PI * 0.5d)) + start;
    }

    public static double EaseInOutSine(double start, double end, double value)
    {
        end -= start;
        return -end * 0.5d * (Math.Cos(Math.PI * value) - 1) + start;
    }

    public static double EaseInExpo(double start, double end, double value)
    {
        end -= start;
        return end * Math.Pow(2, 10 * (value - 1)) + start;
    }

    public static double EaseOutExpo(double start, double end, double value)
    {
        end -= start;
        return end * (-Math.Pow(2, -10 * value) + 1) + start;
    }

    public static double EaseInOutExpo(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return end * 0.5d * Math.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end * 0.5d * (-Math.Pow(2, -10 * value) + 2) + start;
    }

    public static double EaseInCirc(double start, double end, double value)
    {
        end -= start;
        return -end * (Math.Sqrt(1 - value * value) - 1) + start;
    }

    public static double EaseOutCirc(double start, double end, double value)
    {
        value--;
        end -= start;
        return end * Math.Sqrt(1 - value * value) + start;
    }

    public static double EaseInOutCirc(double start, double end, double value)
    {
        value /= .5d;
        end -= start;
        if (value < 1) return -end * 0.5d * (Math.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end * 0.5d * (Math.Sqrt(1 - value * value) + 1) + start;
    }

    public static double EaseInBounce(double start, double end, double value)
    {
        end -= start;
        double d = 1d;
        return end - EaseOutBounce(0, end, d - value) + start;
    }

    public static double EaseOutBounce(double start, double end, double value)
    {
        value /= 1d;
        end -= start;
        if (value < (1 / 2.75d))
        {
            return end * (7.5625d * value * value) + start;
        }
        else if (value < (2 / 2.75d))
        {
            value -= (1.5d / 2.75d);
            return end * (7.5625d * (value) * value + .75d) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25d / 2.75d);
            return end * (7.5625d * (value) * value + .9375d) + start;
        }
        else
        {
            value -= (2.625d / 2.75d);
            return end * (7.5625d * (value) * value + .984375d) + start;
        }
    }

    public static double EaseInOutBounce(double start, double end, double value)
    {
        end -= start;
        double d = 1d;
        if (value < d * 0.5d) return EaseInBounce(0, end, value * 2) * 0.5d + start;
        else return EaseOutBounce(0, end, value * 2 - d) * 0.5d + end * 0.5d + start;
    }

    public static double EaseInBack(double start, double end, double value)
    {
        end -= start;
        value /= 1;
        double s = 1.70158d;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    public static double EaseOutBack(double start, double end, double value)
    {
        double s = 1.70158d;
        end -= start;
        value = (value) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    public static double EaseInOutBack(double start, double end, double value)
    {
        double s = 1.70158d;
        end -= start;
        value /= .5d;
        if ((value) < 1)
        {
            s *= (1.525d);
            return end * 0.5d * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525d);
        return end * 0.5d * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    public static double EaseInElastic(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        return -(a * Math.Pow(2, 10 * (value -= 1)) * Math.Sin((value * d - s) * (2 * Math.PI) / p)) + start;
    }

    public static double EaseOutElastic(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p * 0.25d;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        return (a * Math.Pow(2, -10 * value) * Math.Sin((value * d - s) * (2 * Math.PI) / p) + end + start);
    }

    public static double EaseInOutElastic(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (value == 0) return start;

        if ((value /= d * 0.5d) == 2) return start + end;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        if (value < 1) return -0.5d * (a * Math.Pow(2, 10 * (value -= 1)) * Math.Sin((value * d - s) * (2 * Math.PI) / p)) + start;
        return a * Math.Pow(2, -10 * (value -= 1)) * Math.Sin((value * d - s) * (2 * Math.PI) / p) * 0.5d + end + start;
    }

    //
    // These are derived functions that the motor can use to get the speed at a specific time.
    //
    // The easing functions all work with a normalized time (0 to 1) and the returned value here
    // reflects that. Values returned here should be divided by the actual time.
    //
    // TODO: These functions have not had the testing they deserve. If there is odd behavior around
    //       dash speeds then this would be the first place I'd look.

    public static double LinearD(double start, double end, double value)
    {
        return end - start;
    }

    public static double EaseInQuadD(double start, double end, double value)
    {
        return 2d * (end - start) * value;
    }

    public static double EaseOutQuadD(double start, double end, double value)
    {
        end -= start;
        return -end * value - end * (value - 2);
    }

    public static double EaseInOutQuadD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return end * value;
        }

        value--;

        return end * (1 - value);
    }

    public static double EaseInCubicD(double start, double end, double value)
    {
        return 3d * (end - start) * value * value;
    }

    public static double EaseOutCubicD(double start, double end, double value)
    {
        value--;
        end -= start;
        return 3d * end * value * value;
    }

    public static double EaseInOutCubicD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return (3d / 2d) * end * value * value;
        }

        value -= 2;

        return (3d / 2d) * end * value * value;
    }

    public static double EaseInQuartD(double start, double end, double value)
    {
        return 4d * (end - start) * value * value * value;
    }

    public static double EaseOutQuartD(double start, double end, double value)
    {
        value--;
        end -= start;
        return -4d * end * value * value * value;
    }

    public static double EaseInOutQuartD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return 2d * end * value * value * value;
        }

        value -= 2;

        return -2d * end * value * value * value;
    }

    public static double EaseInQuintD(double start, double end, double value)
    {
        return 5d * (end - start) * value * value * value * value;
    }

    public static double EaseOutQuintD(double start, double end, double value)
    {
        value--;
        end -= start;
        return 5d * end * value * value * value * value;
    }

    public static double EaseInOutQuintD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return (5d / 2d) * end * value * value * value * value;
        }

        value -= 2;

        return (5d / 2d) * end * value * value * value * value;
    }

    public static double EaseInSineD(double start, double end, double value)
    {
        return (end - start) * 0.5d * Math.PI * Math.Sin(0.5d * Math.PI * value);
    }

    public static double EaseOutSineD(double start, double end, double value)
    {
        end -= start;
        return (Math.PI * 0.5d) * end * Math.Cos(value * (Math.PI * 0.5d));
    }

    public static double EaseInOutSineD(double start, double end, double value)
    {
        end -= start;
        return end * 0.5d * Math.PI * Math.Cos(Math.PI * value);
    }

    public static double EaseInExpoD(double start, double end, double value)
    {
        return (10d * Nl2 * (end - start) * Math.Pow(2d, 10d * (value - 1)));
    }

    public static double EaseOutExpoD(double start, double end, double value)
    {
        end -= start;
        return 5d * Nl2 * end * Math.Pow(2d, 1d - 10d * value);
    }

    public static double EaseInOutExpoD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return 5d * Nl2 * end * Math.Pow(2d, 10d * (value - 1));
        }

        value--;

        return (5d * Nl2 * end) / (Math.Pow(2d, 10d * value));
    }

    public static double EaseInCircD(double start, double end, double value)
    {
        return ((end - start) * value) / Math.Sqrt(1d - value * value);
    }

    public static double EaseOutCircD(double start, double end, double value)
    {
        value--;
        end -= start;
        return (-end * value) / Math.Sqrt(1d - value * value);
    }

    public static double EaseInOutCircD(double start, double end, double value)
    {
        value /= .5d;
        end -= start;

        if (value < 1)
        {
            return (end * value) / (2d * Math.Sqrt(1d - value * value));
        }

        value -= 2;

        return (-end * value) / (2d * Math.Sqrt(1d - value * value));
    }

    public static double EaseInBounceD(double start, double end, double value)
    {
        end -= start;
        double d = 1d;

        return EaseOutBounceD(0, end, d - value);
    }

    public static double EaseOutBounceD(double start, double end, double value)
    {
        value /= 1d;
        end -= start;

        if (value < (1 / 2.75d))
        {
            return 2d * end * 7.5625d * value;
        }
        else if (value < (2 / 2.75d))
        {
            value -= (1.5d / 2.75d);
            return 2d * end * 7.5625d * value;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25d / 2.75d);
            return 2d * end * 7.5625d * value;
        }
        else
        {
            value -= (2.625d / 2.75d);
            return 2d * end * 7.5625d * value;
        }
    }

    public static double EaseInOutBounceD(double start, double end, double value)
    {
        end -= start;
        double d = 1d;

        if (value < d * 0.5d)
        {
            return EaseInBounceD(0, end, value * 2) * 0.5d;
        }
        else
        {
            return EaseOutBounceD(0, end, value * 2 - d) * 0.5d;
        }
    }

    public static double EaseInBackD(double start, double end, double value)
    {
        double s = 1.70158d;

        return 3d * (s + 1d) * (end - start) * value * value - 2d * s * (end - start) * value;
    }

    public static double EaseOutBackD(double start, double end, double value)
    {
        double s = 1.70158d;
        end -= start;
        value = (value) - 1;

        return end * ((s + 1d) * value * value + 2d * value * ((s + 1d) * value + s));
    }

    public static double EaseInOutBackD(double start, double end, double value)
    {
        double s = 1.70158d;
        end -= start;
        value /= .5d;

        if ((value) < 1)
        {
            s *= (1.525d);
            return 0.5d * end * (s + 1) * value * value + end * value * ((s + 1d) * value - s);
        }

        value -= 2;
        s *= (1.525d);
        return 0.5d * end * ((s + 1) * value * value + 2d * value * ((s + 1d) * value + s));
    }

    public static double EaseInElasticD(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        double c = 2 * Math.PI;

        // From an online derivative calculator, kinda hoping it is right.
        return ((-a) * d * c * Math.Cos((c * (d * (value - 1d) - s)) / p)) / p -
            5d * Nl2 * a * Math.Sin((c * (d * (value - 1d) - s)) / p) *
            Math.Pow(2d, 10d * (value - 1d) + 1d);
    }

    public static double EaseOutElasticD(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p * 0.25d;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        return (a * Math.PI * d * Math.Pow(2d, 1d - 10d * value) *
            Math.Cos((2d * Math.PI * (d * value - s)) / p)) / p - 5d * Nl2 * a *
            Math.Pow(2d, 1d - 10d * value) * Math.Sin((2d * Math.PI * (d * value - s)) / p);
    }

    public static double EaseInOutElasticD(double start, double end, double value)
    {
        end -= start;

        double d = 1d;
        double p = d * .3d;
        double s;
        double a = 0;

        if (a == 0d || a < Math.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Math.PI) * Math.Asin(end / a);
        }

        if (value < 1)
        {
            value -= 1;

            return -5d * Nl2 * a * Math.Pow(2d, 10d * value) * Math.Sin(2 * Math.PI * (d * value - 2d) / p) -
                a * Math.PI * d * Math.Pow(2d, 10d * value) * Math.Cos(2 * Math.PI * (d * value - s) / p) / p;
        }

        value -= 1;

        return a * Math.PI * d * Math.Cos(2d * Math.PI * (d * value - s) / p) / (p * Math.Pow(2d, 10d * value)) -
            5d * Nl2 * a * Math.Sin(2d * Math.PI * (d * value - s) / p) / (Math.Pow(2d, 10d * value));
    }

    public static double SpringD(double start, double end, double value)
    {
        //value = Math.Clamp01(value);
        end -= start;

        // Damn... Thanks http://www.derivative-calculator.net/
        return end * (6d * (1d - value) / 5d + 1d) * (-2.2d * Math.Pow(1d - value, 1.2d) *
            Math.Sin(Math.PI * value * (2.5d * value * value * value + 0.2d)) + Math.Pow(1d - value, 2.2d) *
            (Math.PI * (2.5d * value * value * value + 0.2d) + 7.5d * Math.PI * value * value * value) *
            Math.Cos(Math.PI * value * (2.5d * value * value * value + 0.2d)) + 1d) -
            6d * end * (Math.Pow(1 - value, 2.2d) * Math.Sin(Math.PI * value * (2.5d * value * value * value + 0.2d)) + value
            / 5d);

    }

    #endregion
}

#endregion

#region (enum) DefaultCurves

[Serializable]
public enum DefaultCurves
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    Spring,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic
}

#endregion