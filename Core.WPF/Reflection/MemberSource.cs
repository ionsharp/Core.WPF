using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Imagin.Core.Reflection;

public class MemberSource
{
    public Types DataType => Type.IsValueType ? Types.Value : Types.Reference;

    public object FirstInstance => Length == 1 ? Instance : Instances[0];

    public object Instance { get; internal set; }

    public object[] Instances { get; private set; }

    public ObservableCollection<MethodInfo> Methods { get; private set; } = new();

    public ObservableCollection<MethodInfo> CopyMethods { get; private set; } = new();

    public ObservableCollection<MethodInfo> PasteMethods { get; private set; } = new();
    
    public int Length => Instance != null ? 1 : Instances.Length;

    public readonly Type Type;

    public MemberSource(object input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        Instance = input;
        Type = Instance.GetType();

        Type.Get_Methods()
            .If(i => i.Any(), i => Methods = new(i));
        Type.GetCopyMethods()
            .If(i => i.Any(), i => CopyMethods = new(i));
        Type.GetPasteMethods()
            .If(i => i.Any(), i => PasteMethods = new(i));
    }

    public MemberSource(object[] input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (input.Length <= 1)
            throw new ArgumentOutOfRangeException(nameof(input));

        Instances = input;
        Type = GetSharedType(XList.Select(Instances, i => i.GetType()));
    }

    Type GetSharedType(IEnumerable<Type> types)
    {
        if (types == null)
            return null;

        Type a = null;
        foreach (var i in types)
        {
            if (a == null)
            {
                a = i;
                continue;
            }

            var b = i;

            //Compare a and b
            while (a != typeof(object))
            {
                while (b != typeof(object))
                {
                    if (a == b)
                        goto next;

                    b = b.BaseType;
                }
                a = a.BaseType;
                b = i;
            }

            return null;
            next: continue;
        }
        return a;
    }

    public void EachSource(Action<object> action)
    {
        Instances.If(i => XList.ForEach(i, j => action(j)));
        Instance.If(i => action(i));
    }

    ICommand invokeMethodCommand;
    public ICommand InvokeMethodCommand => invokeMethodCommand
        ??= new RelayCommand<MethodInfo>(i =>
        {
            Try.Invoke(() =>
            {
                if (i.ReturnType == typeof(void))
                    i.Invoke(FirstInstance, i.GetParameters()?.Length == 0 ? null : new object[] { Copy.Get(i.GetParameters()[0].ParameterType) });

                else if (i.Invoke(FirstInstance, null) is object result)
                {
                    Copy.Set(result);
                    Log.Write<MemberSource>(new Success($"Copied '{result.GetType().Name}'"));
                }
            }, 
            e => Log.Write<MemberSource>(e));

        }, 
        i => i != null && (i.ReturnType != typeof(void) || i.GetParameters()?.Length == 0 || Copy.Contains(i.GetParameters()[0].ParameterType)));
}