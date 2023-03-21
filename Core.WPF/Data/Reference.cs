using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows;

namespace Imagin.Core.Data;

public abstract class Reference<T> : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(T), typeof(Reference<T>), new FrameworkPropertyMetadata(default(T)));
    public T Data
    {
        get => (T)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new Reference();

    public Reference() : base() { }

    public Reference(T data) : this() => SetCurrentValue(DataProperty, data);
}

public class Reference2<T1, T2> : Freezable
{
    public static readonly DependencyProperty FirstProperty = DependencyProperty.Register(nameof(First), typeof(T1), typeof(Reference2<T1, T2>), new FrameworkPropertyMetadata(default(T1)));
    public T1 First
    {
        get => (T1)GetValue(FirstProperty);
        set => SetValue(FirstProperty, value);
    }

    public static readonly DependencyProperty SecondProperty = DependencyProperty.Register(nameof(Second), typeof(T2), typeof(Reference2<T1, T2>), new FrameworkPropertyMetadata(default(T2)));
    public T2 Second
    {
        get => (T2)GetValue(SecondProperty);
        set => SetValue(SecondProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new Reference2<T1, T2>();

    public Reference2() : base() { }

    public Reference2(T1 first, T2 second) : this() 
    {
        SetCurrentValue(FirstProperty, first); SetCurrentValue(SecondProperty, second);
    }
}

public class Reference : Reference<object>
{
    public Reference() : this(null) { }

    public Reference(object data) : base(data) { }
}

public class Reference2 : Reference2<object, object>
{
    public Reference2() : this(null, null) { }

    public Reference2(object first, object second) : base(first, second) { }
}

public class BooleanReference : Reference<bool>
{
    public BooleanReference() : this(false) { }

    public BooleanReference(bool data) : base(data) { }
}

public class ItemModelReference : Reference
{
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(object), typeof(ItemModelReference), new FrameworkPropertyMetadata(null, OnItemChanged));
    public object Item
    {
        get => (object)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
    static void OnItemChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ItemModelReference>().OnItemChanged(e);
    protected virtual void OnItemChanged(ReadOnlyValue<object> input)
    {
        UpdateModel();
    }

    public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Enum), typeof(ItemModelReference), new FrameworkPropertyMetadata(null, OnItemStyleChanged));
    static void OnItemStyleChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ItemModelReference>().OnItemStyleChanged(e);
    protected virtual void OnItemStyleChanged(ReadOnlyValue<Enum> input)
    {
        Data.If<MemberModel>(i => i.Style = input.New);
    }

    public static readonly DependencyProperty MemberProperty = DependencyProperty.Register(nameof(Member), typeof(MemberModel), typeof(ItemModelReference), new FrameworkPropertyMetadata(null, OnMemberChanged));
    public MemberModel Member
    {
        get => (MemberModel)GetValue(MemberProperty);
        set => SetValue(MemberProperty, value);
    }
    static void OnMemberChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ItemModelReference>().OnMemberChanged(e);
    protected virtual void OnMemberChanged(ReadOnlyValue<MemberModel> input)
    {
        UpdateModel();

        if (input.Old != null)
            this.Unbind(ItemStyleProperty);

        if (input.New != null)
            this.Bind(ItemStyleProperty, nameof(MemberModel.ItemStyle), input.New);
    }

    public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(ItemModelReference), new FrameworkPropertyMetadata(-1, OnIndexChanged));
    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
    static void OnIndexChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ItemModelReference>().OnIndexChanged(e);
    protected virtual void OnIndexChanged(ReadOnlyValue<int> input)
    {
        Data.If<ItemModel>(i => i.Key = input.New);
    }

    public ItemModelReference() : base() { }

    ///

    void UpdateModel()
    {
        if (Member != null)
        {
            if (Item != null)
                Data = new ItemModel(Member.ParentCollection, Member, Item) { Style = Member.ItemStyle };
        }
    }
}