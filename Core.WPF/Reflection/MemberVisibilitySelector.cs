using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace Imagin.Core.Reflection;

public class MemberVisibilitySelector : FrameworkElement
{
    public static readonly DependencyProperty GroupValueProperty = DependencyProperty.Register(nameof(GroupValue), typeof(bool), typeof(MemberVisibilitySelector), new FrameworkPropertyMetadata(false, OnGroupValueChanged));
    public bool GroupValue
    {
        get => (bool)GetValue(GroupValueProperty);
        set => SetValue(GroupValueProperty, value);
    }
    static void OnGroupValueChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberVisibilitySelector>().OnGroupValueChanged(e);
    void OnGroupValueChanged(ReadOnlyValue<bool> input) => IsGroupVisible = input.New && GroupVisibility;

    public static readonly DependencyProperty GroupVisibilityProperty = DependencyProperty.Register(nameof(GroupVisibility), typeof(bool), typeof(MemberVisibilitySelector), new FrameworkPropertyMetadata(false, OnGroupVisibilityChanged));
    public bool GroupVisibility
    {
        get => (bool)GetValue(GroupVisibilityProperty);
        set => SetValue(GroupVisibilityProperty, value);
    }
    static void OnGroupVisibilityChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberVisibilitySelector>().OnGroupVisibilityChanged(e);
    void OnGroupVisibilityChanged(ReadOnlyValue<bool> input) => IsGroupVisible = input.New && GroupValue;

    static readonly DependencyPropertyKey IsGroupVisibleKey = DependencyProperty.RegisterReadOnly(nameof(IsGroupVisible), typeof(bool), typeof(MemberVisibilitySelector), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty IsGroupVisibleProperty = IsGroupVisibleKey.DependencyProperty;
    public bool IsGroupVisible
    {
        get => (bool)GetValue(IsGroupVisibleProperty);
        private set => SetValue(IsGroupVisibleKey, value);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(MemberVisibilitySelector), new FrameworkPropertyMetadata(null, OnSourceChanged));
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    static void OnSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberVisibilitySelector>().OnSourceChanged(e);
    void OnSourceChanged(ReadOnlyValue<object> input) => Check();

    ///

    public MemberVisibilitySelector() : base() { }

    public MemberVisibilitySelector(object source) : this() => SetCurrentValue(SourceProperty, source);

    ///

    void Check()
    {
        this.Unbind(GroupValueProperty);
        this.Unbind(GroupVisibilityProperty);

        if (Source is IList items)
        {
            Check(items);

            Subscribe(items, GroupValueProperty, nameof(MemberModel.Value), new MultiConverter<bool>(i =>
            {
                if (i.Values?.Length > 0)
                {
                    var m = 0; var n = 0;
                    foreach (var j in i.Values)
                    {
                        if (Source is IList y && n >= 0 && n < y.Count && y[n] is IAssignableMemberModel x && x.HideIfNull && j == null) { m++; }
                        n++;
                    }
                    return m < i.Values.Length;
                }
                return false;
            }));
            Subscribe(items, GroupVisibilityProperty, nameof(MemberModel.IsVisible), new MultiConverter<bool>(i =>
            {
                if (i?.Values?.Length > 0)
                {
                    var l = 0;
                    foreach (var j in i.Values)
                    {
                        if (j is bool k)
                        {
                            if (!k) l++;
                        }
                    }
                    return l < i.Values.Length;
                }
                return false;
            }));
        }
    }

    void Check(IList input)
    {
        IsGroupVisible = true;

        var j = 0; var k = 0;
        foreach (var i in input)
        {
            if (i is MemberModel item && item is IAssignableMemberModel assignable)
            {
                if (!item.IsVisible || (!item.IsIndeterminate && assignable.HideIfNull && item.Value == null))
                    k++;

                j++;
            }
        }

        if (j == k)
            IsGroupVisible = false;
    }

    void Subscribe(IList input, DependencyProperty property, string propertyName, MultiConverter converter)
    {
        var bindings = new Binding[input.Count];

        var j = 0;
        foreach (var i in input)
        {
            if (i is MemberModel item)
            {
                bindings[j] = new Binding(propertyName) { Source = item };
                j++;
            }
        }

        this.MultiBind(property, converter, null, bindings);
    }
}