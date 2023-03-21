using Imagin.Core.Linq;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Imagin.Core.Effects;

public abstract class BaseEffect : ShaderEffect
{
    protected const string DefaultFolder = $"-Default";

    protected string DefaultFilePath => $"{DefaultFolder}/{GetType().Name}.ps";

    protected virtual string FilePath => DefaultFilePath;

    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(BaseEffect), 0);
    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public BaseEffect() : base()
    {
        PixelShader = new() { UriSource = new Uri($"/{XAssembly.Name};component/Effects/{FilePath}", UriKind.Relative) };
        UpdateShaderValue(InputProperty);
    }
}