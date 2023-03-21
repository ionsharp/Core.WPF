using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imagin.Core.Effects;

/// <summary>For displaying multiple effects on the same <see cref="UIElement"/>! :)</summary>
public static class EffectExtensions
{
    static readonly Dictionary<Border, EffectCollection> targets = new();

    ///

    public static readonly DependencyProperty EffectsProperty = DependencyProperty.RegisterAttached("Effects", typeof(EffectCollection), typeof(EffectExtensions), new FrameworkPropertyMetadata(null, OnEffectsChanged));
    public static EffectCollection GetEffects(Border i) => (EffectCollection)i.GetValue(EffectsProperty);
    public static void SetEffects(Border i, EffectCollection input) => i.SetValue(EffectsProperty, input);
    static void OnEffectsChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Border border)
        {
            border.RegisterHandlerAttached(e.NewValue != null, EffectsProperty, i =>
            {
                var effects = (EffectCollection)e.NewValue;
                if (!targets.ContainsKey(border))
                    targets.Add(border, effects);

                Unsubscribe(effects);
                Subscribe(effects);

                Update(border);
            }, i =>
            {
                if (targets.ContainsKey(border))
                {
                    Unsubscribe(targets[border]);
                    targets.Remove(border);
                }
            });
        }
    }

    static void OnEffectsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var result = targets.First(i => ReferenceEquals(i.Value, (EffectCollection)sender)).Key;
        Update(result);
    }

    ///

    static void Subscribe(EffectCollection effects)
    {
        effects.CollectionChanged += OnEffectsChanged;
    }

    static void Unsubscribe(EffectCollection effects)
    {
        effects.CollectionChanged -= OnEffectsChanged;
    }

    ///

    static void Update(Border border)
    {
        var effects = GetEffects(border);

        Border parent = border, lastParent = null;

        UIElement content = null;
        while (parent != null)
        {
            content = parent.Child;
            lastParent = parent;
            parent = content as Border;
        }

        if (lastParent != null)
            lastParent.Child = null;

        border.Child = null;
        border.Child = content;

        if (effects?.Count() > 0)
        {
            Border a = new(), b = null;

            var c = border.Child;
            border.Child = null;

            a.Child = c;
            foreach (var i in effects)
            {
                a.Effect = i;

                var converter = new MultiConverter<ImageEffect>(j => j.Values?.Length == 2 && j.Values[0] is bool a && j.Values[1] is bool b && a && b ? j.Parameter as ImageEffect : null);
                a.MultiBind(UIElement.EffectProperty, converter, i, new Binding(nameof(EffectCollection.IsVisible)) { Source = effects }, new Binding(nameof(ImageEffect.IsVisible)) { Source = i });

                b = new Border { Child = a };
                a = b;
            }

            border.Child = a;
        }
    }
}

[Editable, Explicit, Name("Effects"), Serializable]
public class EffectCollection : ObservableCollection<ImageEffect>
{
    [Float(Float.Above), CheckedImage(SmallImages.Hidden), Name("Show/hide"), Image(SmallImages.Visible), HideName, Show, Style(BooleanStyle.Button)]
    public bool IsVisible { get => this.Get(true); set => this.Set(value); }

    public static IEnumerable<Type> Types
    {
        get
        {
            var result = new List<Type>();
            typeof(ImageEffect).Assembly.GetDerivedTypes<ImageEffect>(AssemblyPath.Effects, true, true).ForEach(i => result.Add(i));
            return result;
        }
    }

    public EffectCollection() : base() { }

    public EffectCollection(IEnumerable<ImageEffect> input) : base(input) { }

    public IEnumerable<Type> GetTypes() => Types;
}