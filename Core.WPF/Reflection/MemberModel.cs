using Imagin.Core.Analytics;
using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using Imagin.Core.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Reflection;

#region ValueModel

public abstract class ValueModel : Base
{
    public abstract void SafeUpdate(object input);
}

#endregion

#region ValueModel<X>

public abstract class ValueModel<X> : ValueModel
{
    protected readonly Handle Handle = false;

    public X Value { get; set; }

    public ValueModel(X value) : base() => Handle.Invoke(() => Value = value);
}

#endregion

#region ValueModel<X, Y>

public abstract class ValueModel<X, Y> : ValueModel<X>
{
    public readonly MemberModel Source;

    public ValueModel(MemberModel source, X input) : base(input) => Source = source;

    public abstract Y GetValue();

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        Handle.SafeInvoke(() => Source.Value = GetValue());
    }

    public sealed override void SafeUpdate(object input) => Handle.SafeInvoke(() => input.If<Y>(SafeUpdate));

    public abstract void SafeUpdate(Y input);
}

///

#endregion

#region DataGridLengthModel

public class DataGridLengthModel : ValueModel<double, DataGridLength>
{
    public System.Windows.Controls.DataGridLengthUnitType Type { get; set; }

    public DataGridLengthModel(MemberModel source, double input, System.Windows.Controls.DataGridLengthUnitType type) : base(source, input) => Type = type;

    public override DataGridLength GetValue() => new(Value, Type);

    public override void SafeUpdate(DataGridLength input)
    {
        Value = input.Value; Type = input.UnitType;
    }
}

#endregion

#region GridLengthModel

public class GridLengthModel : ValueModel<double, GridLength>
{
    public System.Windows.GridUnitType Type { get; set; }

    public GridLengthModel(MemberModel source, double input, System.Windows.GridUnitType type) : base(source, input) => Type = type;

    public override GridLength GetValue() => new(Value, Type);

    public override void SafeUpdate(GridLength input)
    {
        Value = input.Value; Type = input.GridUnitType;
    }
}

#endregion

///

#region MemberModel

public abstract partial class MemberModel : Namable, IComparable
{
    public const string DefaultCategory = "General";

    public const string DefaultTab = "Other";

    #region Attributes

    static MemberAttribute<T> For<T>(Action<MemberModel, T> action) where T : Attribute => new((i, j) => j.ForEach(k => action(i, k)));

    static MemberAttribute<T> ForAll<T>(Action<MemberModel, IEnumerable<T>> action) where T : Attribute => new(action);

    static readonly MemberAttributeHandler AttributeHandler = new()
    {
        For<AssignAttribute>((i, j) 
            => {
            i.IsAssignable = true;

            if (j.Types is Type[] m && m.Length > 0)
            {
                i.AssignableTypes = new(m);
                return;
            }
            if (i.Type != null && MemberCollection.AssignableTypes.ContainsKey(i.Type))
            {
                i.AssignableTypes = new(MemberCollection.AssignableTypes[i.Type]);
                return;
            }
            if (j.Values != null)
                Try.Invoke(() => i.AssignableValues = i.Source.FirstInstance.GetPropertyValue(j.Values), e => Log.Write<MemberModel>(e));
        }),
        For<CaptionAttribute>((i, j) 
            => i.Caption = j.Caption),
        For<CategoryAttribute>((i, j) 
            => { i.Category = $"{j.Category}"; i.CategoryIndex = j.Category.As<Enum>()?.GetAttribute<IndexAttribute>()?.Index ?? 0; }),
        For<System.ComponentModel.CategoryAttribute>((i, j) 
            => i.Category = j.Category),
        For<CheckedImageAttribute>((i, j) 
            => i.CheckedImage = j.SmallImage),
        For<CollectionStyleAttribute>((i, j)
            =>
        {
            i.Style = j.Style;
            i.StyleAttribute = j;

            i.ItemCommand = Try.Return(() => i.Source.FirstInstance.GetPropertyValue(j.ItemCommand) as ICommand);

            i.ItemAddCommand = Try.Return(() => i.Source.FirstInstance.GetPropertyValue(j.AddCommand) as ICommand);

            if (j.AddItems != null)
                Try.Invoke(() => i.ItemAddItems = i.Source.FirstInstance.GetPropertyValue(j.AddItems), e => Log.Write<MemberModel>(e));
            
            i.ItemAddType = j.AddType;

            if (j.AddTypes is Type[] m && m.Length > 0)
                i.ItemAddTypes = m;

            else if (i.Type != null && MemberCollection.AssignableTypes.ContainsKey(i.Type))
                i.ItemAddTypes = MemberCollection.AssignableTypes[i.Type];
        }),
        For<ColorAttribute>((i, j)
            => i.Color = j.Color),
        For<ColorStyleAttribute>((i, j)
            =>
        {
            i.Style = j.Style;
            i.StyleAttribute = j;

            if (j.Style.Equals(ColorStyle.Model))
            {
                i.ColorModel = NormalColorModel.Get(j.Model);
                i.ColorModel.Normalize = j.Normalize;
                i.ColorModel.Precision = j.Precision;
            }
        }),
        For<ContentAttribute>((i, j) 
            => i.Content = j.Value),
        For<DescriptionAttribute>((i, j)
            => i.Description = j.Description),
        For<System.ComponentModel.DescriptionAttribute>((i, j)
            => i.Description = j.Description),
        For<HideNameAttribute>((i, j)
            => i.HideName = true),
        For<HorizontalAttribute>((i, j)
            => i.Orientation = System.Windows.Controls.Orientation.Horizontal),
        For<ImageAttribute>((i, j)
            => i.Icon = j.SmallImage),
        For<ImageColorAttribute>((i, j)
            => i.IconColor = j.Color),
        For<IndexAttribute>((i, j)
            => i.Index = j.Index),
        For<Int32StyleAttribute>((i, j) 
            =>
        {
            i.Style = j.Style;
            i.StyleAttribute = j;

            i.ItemPath = j.ItemPath;
            i.ItemSource = Try.Return(() => i.Source.FirstInstance.GetPropertyValue(j.ItemSource));
            i.SelectedItemPropertyName = j.ItemName;
        }),
        For<LeftTextAttribute>((i, j)
            => i.LeftText = j.Text),
        For<LockAttribute>((i, j)
            => i.IsLockable = j.Locked),
        For<NameAttribute>((i, j)
            => i.DisplayName = j.Name),
        For<System.ComponentModel.DisplayNameAttribute>((i, j)
            => i.DisplayName = j.DisplayName),
        For<PlaceholderAttribute>((i, j) 
            => i.Placeholder = j.Value),
        For<RangeGradientAttribute>((i, j)
            => i.RangeGradient = j.Colors.Select(l => XColor.Convert(new ByteVector4($"{l}"))).ToArray()),
        For<RightTextAttribute>((i, j)
            => i.RightText = j.Text),
        For<StringFormatAttribute>((i, j)
            => i.StringFormat = j.Format),
        For<StringStyleAttribute>((i, j)
            =>
        {
            i.Style = j.Style;
            i.StyleAttribute = j;

            i.Suggestions
                = Try.Return(() => i.Source.FirstInstance.GetPropertyValue(j.Suggestions));
            i.SuggestionCommand
                = Try.Return(() => (ICommand)i.Source.Instance.GetPropertyValue(j.SuggestionCommand));
        }),
        For<StyleAttribute>((i, j)
            => { i.Style = j.Style; i.StyleAttribute = j; }),
        For<TabAttribute>((i, j) 
            => i.Tab = j.Name),
        For<ValidateAttribute>((i, j)
            => 
        {
            IValidate result = null;
            Try.Invoke(() => result = j.Type.Create<IValidate>(), e => Log.Write<MemberModel>(e));
            i.ValidateHandler = result;
        }),
        For<VerticalAttribute>((i, j)
            => i.Orientation = System.Windows.Controls.Orientation.Vertical),

        ForAll<EnableTriggerAttribute>((i, j)
            => i.Source.EachSource(k => j.ForEach(l => i.OnEnableTrigger(k, l)))),
        ForAll<StyleTriggerAttribute>((i, j)
            => i.Source.EachSource(k => j.ForEach(l => i.OnStyleTrigger(k, l)))),
        ForAll<TriggerAttribute>((i, j)
            => i.Source.EachSource(k => j.ForEach(l => i.OnTrigger(k, l)))),
        ForAll<VisibilityTriggerAttribute>((i, j)
            => i.Source.EachSource(k => j.ForEach(l => i.OnVisibilityTrigger(k, l)))),
    };

    #endregion

    #region Fields

    System.Timers.Timer timer;

    ///

    public readonly MemberAttributes Attributes;

    public readonly MemberCollection ParentCollection;

    public readonly IComparer Sort;

    #endregion

    #region DefaultRanges

    public class Range
    {
        public readonly object Increment; public readonly object Maximum; public readonly object Minimum;

        public Range(object minimum, object maximum, object increment) : base() { Minimum = minimum; Maximum = maximum; Increment = increment; }
    }

    static readonly Dictionary<Type, Range> DefaultRanges = new()
    {
        { typeof(byte),
            new(byte.MinValue, byte.MaxValue, (byte)1) },
        { typeof(DateTime),
            new(DateTime.MinValue, DateTime.MaxValue, DateTime.Now) },
        { typeof(decimal),
            new(decimal.MinValue, decimal.MaxValue, (decimal)1) },
        { typeof(double),
            new(double.MinValue, double.MaxValue, (double)1) },
        { typeof(short),
            new(short.MinValue, short.MaxValue, (short)1) },
        { typeof(int),
            new(int.MinValue, int.MaxValue, (int)1) },
        { typeof(long),
            new(long.MinValue, long.MaxValue, (long)1) },
        { typeof(float),
            new(float.MinValue, float.MaxValue, (float)1) },
        { typeof(TimeSpan),
            new(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromSeconds(1)) },
        { typeof(UDouble),
            new(UDouble.MinValue, UDouble.MaxValue, (UDouble)1) },
        { typeof(ushort),
            new(ushort.MinValue, ushort.MaxValue, (ushort)1) },
        { typeof(uint),
            new(uint.MinValue, uint.MaxValue, (uint)1) },
        { typeof(ulong),
            new(ulong.MinValue, ulong.MaxValue, (ulong)1) },
        { typeof(USingle),
            new(USingle.MinValue, USingle.MaxValue, (USingle)1) },
    };

    #endregion

    #region Properties

    public enum PropertyNames
    {
        Increment,
        Maximum,
        Minimum,

        IsEditingText,

        RangeStyle
    }

    public virtual Type BaseType => Type?.BaseType;

    public abstract Type TemplateType { get; }

    public abstract Type Type { get; }

    ///

    public int Depth { get => Get(0); set => Set(value); }

    public MemberCollection Members { get => Get<MemberCollection>(); set => Set(value); }

    public MemberModel Parent { get; private set; }

    public PropertyDictionary Properties { get => Get<PropertyDictionary>(new()); set => Set(value); }

    public ObservableCollection<Type> Route
    {
        get
        {
            var result = new ObservableCollection<Type>() { Source.Type };

            MemberModel i = this;
            while (i.Parent != null)
            {
                i = i.Parent;
                result.Insert(0, i.Source.Type);
            }

            return result;
        }
    }

    public virtual bool ShowOptions { get; set; } = false;

    public MemberSource Source { get => Get<MemberSource>(); internal set => Set(value); }

    public virtual object Value { get; set; }

    public virtual Type ValueType { get => Get<Type>(); private set => Set(value); }

    ///

    public ICommand AssignableCommand { get => Get<ICommand>(); private set => Set(value); }

    public Type AssignableType { get => Get<Type>(); private set => Set(value); }

    public ObservableCollection<Type> AssignableTypes { get => Get<ObservableCollection<Type>>(); private set => Set(value); }

    public object AssignableValues { get => Get<object>(); private set => Set(value); }

    ///

    public string Caption { get => Get<string>(null); set => Set(value); }

    public virtual string Category { get => Get(DefaultCategory); set => Set(value); }

    public int CategoryIndex { get => Get(0); set => Set(value); }

    public string CheckedImage { get => Get<string>(null); set => Set(value); }

    public ByteVector4? Color { get => Get<ByteVector4?>(null); private set => Set(value); }

    public NormalColorModel ColorModel { get => Get<NormalColorModel>(); private set => Set(value); }

    public object Content { get => Get<object>(); set => Set(value); }

    public string Description { get => Get<string>(null); private set => Set(value); }

    public virtual string DisplayName { get => Get<string>(null); set => Set(value); }

    public double Height { get => Get(double.NaN); private set => Set(value); }

    public bool HideName { get => Get(false); protected set => Set(value); }

    public string Icon { get => Get<string>(null); private set => Set(value); }

    public string IconColor { get => Get<string>(null); private set => Set(value); }

    public int Index { get => Get(0); private set => Set(value); }

    public bool IsAssignable { get => Get(false); private set => Set(value); }

    public bool IsEditable { get => Get(false); set => Set(value); }

    public bool IsIndeterminate { get => Get(false); protected set => Set(value); }

    public bool IsLockable { get => Get(false); private set => Set(value); }

    public bool IsLocked { get => Get(false); internal set => Set(value); }

    public virtual bool IsReadOnly { get => Get(false); protected set => Set(value); }

    public bool IsTemplateIndeterminable => HasIndeterminableTemplate(TemplateType);

    public bool IsVisible { get => Get(true); private set => Set(value); }

    public string LeftText { get => Get<string>(null); set => Set(value); }

    public bool Localize { get => Get(true); private set => Set(value); }

    public double MaximumHeight { get => Get(double.NaN); private set => Set(value); }

    public double MaximumWidth { get => Get(double.NaN); private set => Set(value); }

    public double MinimumHeight { get => Get(double.NaN); private set => Set(value); }

    public double MinimumWidth { get => Get(double.NaN); private set => Set(value); }

    public Orientation Orientation { get => Get(Orientation.Vertical); set => Set(value); }

    public string Placeholder { get => Get<string>(null); private set => Set(value); }

    public System.Windows.Media.Color[] RangeGradient { get => Get<System.Windows.Media.Color[]>(null); set => Set(value); }

    public string RightText { get => Get<string>(null); set => Set(value); }

    public string StringFormat { get => Get<string>(null); private set => Set(value); }

    public object Suggestions { get => Get<object>(); private set => Set(value); }

    public ICommand SuggestionCommand { get => Get<ICommand>(null); private set => Set(value); }

    public Enum Style { get => Get<Enum>(null); set => Set(value); }

    public StyleAttribute StyleAttribute { get => Get<StyleAttribute>(null); private set => Set(value); }

    public ObjectDictionary StyleProperties { get => Get<ObjectDictionary>(null); private set => Set(value); }

    public string Tab { get => Get<string>(DefaultTab); set => Set(value); }

    public ValueModel TemplateModel { get => Get<ValueModel>(); set => Set(value); }

    public IValidate ValidateHandler { get => Get<IValidate>(null); set => Set(value); }

    public double Width { get => Get(double.NaN); private set => Set(value); }

    #region CollectionStyle

    public ICommand ItemCommand { get => Get<ICommand>(null); private set => Set(value); }

    public string ItemPath { get => Get("."); set => Set(value); }

    public object ItemSource { get => Get<object>(); set => Set(value); }

    public Enum ItemStyle { get => Get<Enum>(null); set => Set(value); }

    public ICommand ItemAddCommand { get => Get<ICommand>(); private set => Set(value); }

    public object ItemAddItems { get => Get<object>(); set => Set(value); }

    public Type ItemAddType { get => Get<Type>(); set => Set(value); }

    public Type[] ItemAddTypes { get => Get<Type[]>(); set => Set(value); }

    public int SelectedIndex { get => Get(-1); set => Set(value); }

    public object SelectedItem { get => Get<object>(); set => Set(value); }

    public string SelectedItemPropertyName { get => Get<string>(); private set => Set(value); }

    #endregion

    #endregion

    #region PropertyDictionary

    public class PropertyDictionary : Dictionary<string, object>
    {
        public object this[PropertyNames input]
        {
            get
            {
                if (input.ToString() is string key)
                {
                    if (ContainsKey(key))
                        return base[key];
                }
                return null;
            }
            set
            {
                var key = input.ToString();
                if (!this.ContainsKey(key))
                    Add(key, null);

                base[key] = value;
            }
        }

        public void Remove(params PropertyNames[] items)
        {
            if (items?.Length > 0)
            {
                foreach (var i in XList.Select(items, j => j.ToString()))
                    this.Remove(i);
            }
        }
    }

    #endregion

    #region MemberModel

    protected MemberModel(MemberCollection collection, MemberModel parent, MemberSource source, MemberAttributes attributes, int depth, IComparer sort) : base()
    {
        ParentCollection = collection; Parent = parent; Source = source; Attributes = attributes; Depth = depth; Sort = sort;
    }

    #endregion

    #region Methods

    #region Internal

    /// <summary>A very generic way of mass-applying attributes.</summary>
    internal void HandleAttributes()
    {
        foreach (var i in AttributeHandler)
        {
            var j = Attributes?.GetAll(i.Type);
            if (j?.Count() > 0)
                Try.Invoke(() => i.Execute(this, j), e => Log.Write<MemberModel>(e));
        }
    }

    /// <summary>
    /// Allows low level objects to inherit or override attributes from top level ones by applying attributes in this order:
    /// <para><b>(a)</b> Source/parent member > <b>(b)</b> Member type > <b>(c)</b> Member value type > <b>(d)</b> Member declaration</para>
    /// </summary>
    internal void OverrideAttributes()
    {
        var attributes = new Dictionary<Type, string>()
        {
            { typeof(EditableAttribute),
                nameof(IsEditable) },
            { typeof(OptionsAttribute),
                nameof(ShowOptions) },
            { typeof(System.ComponentModel.ReadOnlyAttribute),
                nameof(IsReadOnly) },
            { typeof(ReadOnlyAttribute),
                nameof(IsReadOnly) }
        };

        void doAttribute(Type attributeType, string propertyName)
        {
            if (Parent == null)
            {
                //(a) Source
                if (Source.Type.HasAttribute(attributeType))
                {
                    this.SetPropertyValue(propertyName, true);
                }
            }
            else
            {
                //(a) Parent member
                this.SetPropertyValue(propertyName, Parent.GetPropertyValue(propertyName));
            }

            if (Type?.HasAttribute(attributeType) == true)
            {
                //(b) Member type
                this.SetPropertyValue(propertyName, true);
            }

            if (ValueType?.HasAttribute(attributeType) == true)
            {
                //(c) Member value type
                this.SetPropertyValue(propertyName, true);
            }

            if (Attributes?.GetFirst(attributeType) != null)
            {
                //(d) Member declaration
                this.SetPropertyValue(propertyName, true);
            }
        }

        attributes.ForEach(i => doAttribute(i.Key, i.Value));
    }

    /// <summary>There are currently (3) ways attributes are getting applied.</summary>
    internal virtual void ApplyAttributes()
    {
        //(1) 
        HandleAttributes();

        //(2) A more tailored way of applying attributes (relatively new)
        Attributes.ForEach(i => OnAttributeApplied(i.Value.First()));

        //(3) The newest system that 
        OverrideAttributes();
    }

    #endregion

    #region Private

    int IComparable.CompareTo(object a)
    {
        if (a is MemberModel b)
            return Index.CompareTo(b.Index);

        return 0;
    }

    ///

    object CreateItem(Type type = null)
    {
        type ??= ItemAddType;
        return type == typeof(System.Windows.Media.Color) ? System.Windows.Media.Colors.Transparent : type == typeof(string) ? "" : type.Create<object>();
    }

    object GetSelectedItem()
        => Value is IList i && i.Count > SelectedIndex && SelectedIndex >= 0 ? i[SelectedIndex] : null;

    ///

    void InsertAbove(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i => Value.As<IList>().Insert(index == -1 ? 0 : index, i)), e => Log.Write<MemberModel>(e));

    void InsertBelow(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i =>
        {
            if (Value is IList list)
            {
                var newIndex = index + 1;
                if (index != -1 && newIndex < list.Count)
                    list.Insert(newIndex, i);

                else list.Add(i);
            }
        }), e => Log.Write<MemberModel>(e));

    ///

    void OnUpdate(object sender, System.Timers.ElapsedEventArgs e) => OnUpdate(e);

    ///

    void OnTrigger(object sender, TriggerAttribute i)
    {
        object result = null;
        Try.Invoke(() =>
        {
            result = sender.GetPropertyValue(i.SourceName);
            this.SetPropertyValue(i.TargetName, result);
        },
        e => Log.Write<MemberModel>(e));
    }

    void OnTrigger(object sender, PropertyChangedEventArgs e)
    {
        var trigger = Attributes.GetAll<TriggerAttribute>()?.FirstOrDefault(i => e.PropertyName == i.SourceName);
        trigger.If(i => OnTrigger(sender, i));
    }

    ///

    void OnOperatorTrigger(object sender, OperatorTriggerAttribute i, Action<bool> action)
    {
        object result = null;
        Try.Invoke(() =>
        {
            static bool? Do(object x, Operators z, object y)
            {
                switch (z)
                {
                    case Operators.Greater:
                        return x is int a0 && y is int b0 ? a0 > b0 : x is double c0 && y is double d0 ? c0 > d0 : null;

                    case Operators.GreaterOrEqual:
                        return x is int a1 && y is int b1 ? a1 > b1 : x is double c1 && y is double d1 ? c1 > d1 : null;

                    case Operators.Lesser:
                        return x is int a2 && y is int b2 ? a2 > b2 : x is double c2 && y is double d2 ? c2 > d2 : null;

                    case Operators.LesserOrEqual:
                        return x is int a3 && y is int b3 ? a3 > b3 : x is double c3 && y is double d3 ? c3 > d3 : null;
                }
                return null;
            }

            result = sender.GetPropertyValue(i.Name);
            switch (i.Operator)
            {
                case Operators.Equal:
                    action(result == i.Value || result?.Equals(i.Value) == true);
                    break;

                case Operators.Greater:
                case Operators.GreaterOrEqual:
                case Operators.Lesser:
                case Operators.LesserOrEqual:
                    var nextResult = Do(result, i.Operator, i.Value);
                    if (nextResult != null)
                        action(nextResult.Value);

                    break;

                case Operators.NotEqual:
                    action(result != i.Value || result?.Equals(i.Value) == false);
                    break;
            }
        },
        e => Log.Write<MemberModel>(e));
    }

    ///

    void OnEnableTrigger(object sender, EnableTriggerAttribute attribute)
    {
        OnOperatorTrigger(sender, attribute, i => IsReadOnly = !i);
    }

    void OnEnableTrigger(object sender, PropertyChangedEventArgs e)
    {
        var triggers = Attributes.GetAll<EnableTriggerAttribute>();
        if (triggers?.Count() > 0)
        {
            foreach (var i in triggers)
            {
                if (e.PropertyName == i.Name)
                    OnEnableTrigger(sender, i);
            }
        }
    }

    ///

    void OnStyleTrigger(object sender, StyleTriggerAttribute attribute)
    {
        if (StyleProperties != null)
        {
            MemberInfo member = Source.FirstInstance.GetField(attribute.LocalName) ?? (MemberInfo)Source.FirstInstance.GetProperty(attribute.LocalName);
            if (member != null)
            {
                var value = Source.FirstInstance.GetMemberValue(member);
                StyleProperties.SetOrAdd(attribute.TargetName, value);
            }
        }
    }

    void OnStyleTrigger(object sender, PropertyChangedEventArgs e)
    {
        var triggers = Attributes.GetAll<StyleTriggerAttribute>();
        if (triggers?.Count() > 0)
        {
            foreach (var i in triggers)
            {
                if (e.PropertyName == i.LocalName)
                    OnStyleTrigger(sender, i);
            }
        }
    }
    
    ///

    void OnVisibilityTrigger(object sender, VisibilityTriggerAttribute attribute)
    {
        OnOperatorTrigger(sender, attribute, i => IsVisible = i);
    }

    void OnVisibilityTrigger(object sender, PropertyChangedEventArgs e)
    {
        var triggers = Attributes.GetAll<VisibilityTriggerAttribute>();
        if (triggers?.Count() > 0)
        {
            foreach (var i in triggers)
            {
                if (e.PropertyName == i.Name)
                    OnVisibilityTrigger(sender, i);
            }
        }
    }

    ///

    void UpdateRange()
    {
        if (Type != null && DefaultRanges.ContainsKey(Type))
        {
            if (Attributes?.GetFirst<RangeAttribute>() is RangeAttribute rangeAttribute)
            {
                Properties[PropertyNames.Increment]
                    = rangeAttribute.Increment;
                Properties[PropertyNames.Maximum]
                    = rangeAttribute.Maximum;
                Properties[PropertyNames.Minimum]
                    = rangeAttribute.Minimum;

                Properties[PropertyNames.RangeStyle]
                    = rangeAttribute.Style;
            }
            else
            {
                var defaultRange = DefaultRanges[Type];

                Properties[PropertyNames.Increment]
                    = defaultRange.Increment;
                Properties[PropertyNames.Maximum]
                    = defaultRange.Maximum;
                Properties[PropertyNames.Minimum]
                    = defaultRange.Minimum;
            }
        }
        else
        {
            Properties.Remove(PropertyNames.Increment, PropertyNames.Maximum, PropertyNames.Minimum);
        }
    }

    #endregion

    #region Protected

    protected virtual bool HasIndeterminableTemplate(Type input)
    {
        if (input == typeof(Boolean))
            return Style == null;

        if (input == typeof(Enum))
            return Style == null;

        return MemberCollection.IndeterminableTemplateTypes.Contains(input);
    }

    protected virtual void OnAttributeApplied(Attribute attribute) 
    {
        if (attribute is HeightAttribute heightAttribute)
        {
            Height = heightAttribute.Value;
            MaximumHeight = heightAttribute.Maximum;
            MinimumHeight = heightAttribute.Minimum;
        }
        else if(attribute is WidthAttribute widthAttribute)
        {
            Width = widthAttribute.Value;
            MaximumWidth = widthAttribute.Maximum;
            MinimumWidth = widthAttribute.Minimum;
        }
    }

    protected virtual void OnUpdate(System.Timers.ElapsedEventArgs e) { }

    #endregion

    #region Public

    public T GetAttribute<T>() where T : Attribute => Attributes?.GetFirst<T>();

    public IEnumerable<T> GetAttributes<T>() where T : Attribute => Attributes?.GetAll<T>();

    ///

    public bool IsCollectionType(Type input)
        => (TemplateType == typeof(INotifyCollectionChanged) || GetTemplateType(input) == typeof(INotifyCollectionChanged)) && Style == null;

    public bool IsObjectType(Type input)
        => (TemplateType == typeof(object) || GetTemplateType(input) == typeof(object)) && Style == null;

    ///

    public bool HasAttribute<T>() where T : Attribute => Attributes?.GetFirst<T>() != null;

    public bool HasAttribute(Type type) => Attributes?.GetFirst(type) != null;

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(IsEditable):
                if (IsEditable)
                {
                    if (ColorModel != null || (Value != null && (IsCollectionType(ValueType) || IsObjectType(ValueType)) && Source.Length == 1))
                    {
                        Members ??= new(this, Depth + 1, ParentCollection?.GroupName ?? Controls.MemberGroupName.None, Sort);
                        Members.Load(ColorModel ?? Value);
                    }
                    else
                    {
                        Members?.Clear();
                        Members = null;
                    }
                }
                break;

            case nameof(SelectedItem):
                if (SelectedItemPropertyName != null)
                    Try.Invoke(() => Source.FirstInstance.SetPropertyValue(SelectedItemPropertyName, SelectedItem), e => Log.Write<MemberModel>(e));

                break;

            case nameof(StyleAttribute):
                if (StyleAttribute != null)
                {
                    StyleProperties?.Clear();
                    StyleProperties ??= new ObjectDictionary();

                    StyleAttribute.GetType().GetMembers(BindingFlags.Public, MemberTypes.Field | MemberTypes.Property, false, null, null, false)
                        .ForEach(i => StyleProperties.Add(i.Name, StyleAttribute.GetMemberValue(i)));
                }
                break;

            case nameof(TemplateType):
                Update(() => IsTemplateIndeterminable);

                if (TemplateType == typeof(ICommand))
                    HideName = true;

                break;

            case nameof(Type):
                if (Type == typeof(ICommand))
                    HideName = true;

                UpdateRange();
                break;

            case nameof(Value):
                Update(() => TemplateType);

                ValueType = Value?.GetType();

                if (Value is System.Windows.Controls.DataGridLength dataGridLength)
                {
                    TemplateModel ??= new DataGridLengthModel(this, dataGridLength.Value, dataGridLength.UnitType);
                    TemplateModel.SafeUpdate(dataGridLength);
                }
                else if (Value is System.Windows.GridLength gridLength)
                {
                    TemplateModel ??= new GridLengthModel(this, gridLength.Value, gridLength.GridUnitType);
                    TemplateModel.SafeUpdate(gridLength);
                }
                goto case nameof(IsEditable);

            case nameof(ValueType):
                UpdateRange();
                break;
        }
    }

    ///

    public virtual void Subscribe()
    {
        Source.EachSource(i => i.If<INotifyPropertyChanged>(j =>
        {
            GetAttributes<TriggerAttribute>()?.ForEach(k => OnTrigger(i, k));
            GetAttributes<VisibilityTriggerAttribute>()?.ForEach(k => OnVisibilityTrigger(i, k));
            
            j.PropertyChanged -= OnTrigger; j.PropertyChanged += OnTrigger;
            j.PropertyChanged -= OnEnableTrigger; j.PropertyChanged += OnEnableTrigger;
            j.PropertyChanged -= OnStyleTrigger; j.PropertyChanged += OnStyleTrigger;
            j.PropertyChanged -= OnVisibilityTrigger; j.PropertyChanged += OnVisibilityTrigger;
        }));

        if (GetAttribute<UpdateAttribute>() is UpdateAttribute update)
        {
            timer = new() { Interval = update.Seconds * 1000 };
            timer.Elapsed += OnUpdate;
            timer.Start();
        }

        Members?.Subscribe();
    }

    public virtual void Unsubscribe()
    {
        Source.EachSource(i => i.If<INotifyPropertyChanged>(j => { j.PropertyChanged -= OnTrigger; j.PropertyChanged -= OnEnableTrigger; j.PropertyChanged -= OnStyleTrigger; j.PropertyChanged -= OnVisibilityTrigger; }));

        if (timer != null)
        {
            timer.Stop();
            timer.Elapsed -= OnUpdate;
            timer.Dispose();
        }

        Members?.Unsubscribe();
    }

    ///

    public virtual void UpdateSource(MemberSource source)
    {
        Source = source;
        ApplyAttributes();
    }

    ///

    public static Type GetTemplateType(Type input)
    {
        if (input == null)
            return null;

        if (input.IsNullable())
            input = Enumerable.FirstOrDefault(input.GetGenericArguments());

        if (MemberCollection.TemplateTypes.Contains(input))
            return input;

        //The order here is important!

        if (input.IsArray)
            return typeof(Array);

        if (input.IsEnum)
            return typeof(Enum);

        if (input.Implements<ICommand>())
            return typeof(ICommand);

        if (input.Implements<INotifyCollectionChanged>())
            return typeof(INotifyCollectionChanged);

        if (input.Implements<IList>())
            return typeof(IList);

        if (input.Implements<IEnumerable>())
            return typeof(IEnumerable);

        return typeof(object);
    }

    #endregion

    #endregion

    #region Commands

    ICommand clearCommand;
    public ICommand ClearCommand => clearCommand
        ??= new RelayCommand(() => Value.As<IList>().Clear(), () => Value is IList);

    ICommand insertAboveCommand;
    public ICommand InsertAboveCommand => insertAboveCommand ??= new RelayCommand<object>(i =>
    {
        if (i is Type j)
            InsertAbove(SelectedIndex, j);

        else if (i is ICloneable k)
            Value.As<IList>().Add(k.Clone());
    },
    i => Value is IList && (i is Type || i is ICloneable));

    ICommand insertBelowCommand;
    public ICommand InsertBelowCommand
        => insertBelowCommand
        ??= new RelayCommand<Type>(i => InsertBelow(SelectedIndex, i),
            i => Value is IList);

    ICommand invokeMethodCommand;
    public ICommand InvokeMethodCommand => invokeMethodCommand
        ??= new RelayCommand<MethodInfo>(i =>
        {
            Try.Invoke(() =>
            {
                if (i.ReturnType == typeof(void))
                    i.Invoke(Value, i.GetParameters()?.Length == 0 ? null : new object[] { Copy.Get(i.GetParameters()[0].ParameterType) });

                else if (i.Invoke(Value, null) is object result)
                {
                    Copy.Set(result);
                    Log.Write<MemberModel>(new Success($"Copied '{result.GetType().Name}'"));
                }
            },
            e => Log.Write<MemberSource>(e));

        },
        i => i != null && (i.ReturnType != typeof(void) || i.GetParameters()?.Length == 0 || Copy.Contains(i.GetParameters()[0].ParameterType)));

    ICommand moveDownCommand;
    public ICommand MoveDownCommand
        => moveDownCommand
        ??= new RelayCommand(() => Try.Invoke(() => Value.As<IList>().MoveDown(SelectedIndex, true)),
            () => Value is IList && GetSelectedItem() != null);
    
    ICommand moveUpCommand;
    public ICommand MoveUpCommand
        => moveUpCommand
        ??= new RelayCommand(() => Try.Invoke(() => Value.As<IList>().MoveUp(SelectedIndex, true)),
            () => Value is IList && GetSelectedItem() != null);

    ICommand removeCommand;
    public ICommand RemoveCommand
        => removeCommand
        ??= new RelayCommand(() => Try.Invoke(() => Value.As<IList>().RemoveAt(SelectedIndex)),
            () => Value is IList && GetSelectedItem() != null);

    #endregion
}

#endregion

#region MemberModel<Info>

public abstract class MemberModel<Info> : MemberModel where Info : MemberInfo
{
    public virtual Type DeclaringType => Member.DeclaringType;

    public Info Member { get => Get<Info>(); private set => Set(value); }

    public virtual MemberTypes MemberType => Member.MemberType;

    public override string Name => Member.Name;

    protected MemberModel(MemberCollection parentCollection, MemberModel parent, MemberSource source, Info member, MemberAttributes attributes, int depth, IComparer sort) : base(parentCollection, parent, source, attributes, depth, sort)
    {
        Member = member;
        if (parent == null)
        {
            Orientation
                = DeclaringType.HasAttribute<HorizontalAttribute>() || !DeclaringType.HasAttribute<VerticalAttribute>() ? Orientation.Horizontal : Orientation.Vertical;
        }
        else
        {
            bool has = DeclaringType.HasAttribute<HorizontalAttribute>();
            Orientation = has ? Orientation.Horizontal : Orientation.Vertical;
        }

        DisplayName = Name;
        ApplyAttributes();
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Member))
        {
            Update(() => DeclaringType);
            Update(() => MemberType);
            Update(() => Name);
            Update(() => Type);
        }
    }
}

#endregion

///

#region EventModel : MemberModel<EventInfo>

public class EventModel : MemberModel<EventInfo>
{
    public override Type TemplateType => typeof(EventInfo);

    public override Type Type => null;

    public EventModel(MemberCollection collection, MemberModel parent, MemberSource source, EventInfo member, MemberAttributes attributes, int depthIndex, IComparer sort) : base(collection, parent, source, member, attributes, depthIndex, sort) { }

    protected override bool HasIndeterminableTemplate(Type input) => false;
}

#endregion

#region MethodModel : MemberModel<MethodInfo>

public class MethodModel : MemberModel<MethodInfo>
{
    public override Type TemplateType => typeof(MethodInfo);

    public override Type Type => null;

    public MethodModel(MemberCollection collection, MemberModel parent, MemberSource source, MethodInfo member, MemberAttributes attributes, int depthIndex, IComparer sort) : base(collection, parent, source, member, attributes, depthIndex, sort) { }

    protected override bool HasIndeterminableTemplate(Type input) => true;
}

#endregion

///

#region IAssignableMemberModel

public interface IAssignableMemberModel
{
    MemberCollection Members { get; }

    bool HideIfNull { get; }

    object Value { get; }

    Type ValueType { get; }

    void UpdateValueFromSource();
}

#endregion

#region AssignableMemberModel<Info>

public abstract class AssignableMemberModel<Info> : MemberModel<Info>, IAssignableMemberModel where Info : MemberInfo
{
    #region Properties

    public object DefaultValue { get => Get<object>(); private set => Set(value); }

    public bool HasDefaultValue { get => Get(false); private set => Set(value); }

    public bool HideIfNull { get => Get(false); set => Set(value); }

    protected object InitialValue { get; set; } = null;

    public bool IsNameFromValue { get => Get(false); private set => Set(value); }

    public bool IsNullable { get => Get(false); private set => Set(value); }

    public abstract bool IsWritable { get; }

    public ObservableCollection<MethodInfo> Methods { get => Get<ObservableCollection<MethodInfo>>(); private set => Set(value); }

    public ObservableCollection<MethodInfo> CopyMethods { get => Get<ObservableCollection<MethodInfo>>(); private set => Set(value); }

    public ObservableCollection<MethodInfo> PasteMethods { get => Get<ObservableCollection<MethodInfo>>(); private set => Set(value); }

    public override Type TemplateType => ValueType is Type i ? GetTemplateType(i) : null;

    public UpdateSourceTrigger UpdateSourceTrigger { get => Get(UpdateSourceTrigger.Default); private set => Set(value); }

    public override object Value
    {
        get => Get<object>();
        set => UpdateSourceWithValue(value);
    }

    #endregion

    #region AssignableMemberModel

    protected AssignableMemberModel(MemberCollection collection, MemberModel parent, MemberSource source, Info member, MemberAttributes attributes, int depth, IComparer sort) : base(collection, parent, source, member, attributes, depth, sort)
    {
        UpdateValueFromSource();

        InitialValue
            = Value;
    }

    #endregion

    #region Methods

    protected void AssignValueToSource(object value) => Source.EachSource(i => SetValue(i, value));

    protected abstract object GetValue(object input);

    protected abstract void SetValue(object source, object value);

    protected override void OnAttributeApplied(Attribute attribute)
    {
        base.OnAttributeApplied(attribute);
        if (attribute is DefaultValueAttribute defaultValue)
        {
            HasDefaultValue = true;
            DefaultValue = defaultValue.Value;
        }

        if (attribute is HideIfNullAttribute)
            HideIfNull = true;

        if (attribute is NameFromValueAttribute)
            IsNameFromValue = true;

        if (attribute is NullableAttribute)
            IsNullable = true;

        if (attribute is UpdateSourceTriggerAttribute updateSourceTrigger)
            UpdateSourceTrigger = updateSourceTrigger.Value;
    }

    protected override void OnUpdate(System.Timers.ElapsedEventArgs e)
    {
        base.OnUpdate(e);
        UpdateValueFromSource();
    }

    protected virtual void OnValueChanged(object input) { }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(ValueType):
                /*
                if (e.OldValue != e.NewValue)
                {
                    ValueType?.Get_Methods()
                        .If(i => i.Any(), i => Methods = new(i));
                    ValueType?.GetCopyMethods()
                        .If(i => i.Any(), i => CopyMethods = new(i));
                    ValueType?.GetPasteMethods()
                        .If(i => i.Any(), i => PasteMethods = new(i));
                }
                */

                break;

            case nameof(Value):
                if (IsNameFromValue)
                    DisplayName = Conversion.Converter.Get<NameAttributeConverter>().Convert(Value ?? Type);

                break;
        }
    }

    public override void UpdateSource(MemberSource source)
    {
        base.UpdateSource(source);
        UpdateValueFromSource();
    }

    public virtual object GetValueFromSource()
    {
        object result = null;

        var allEqual = true;
        object last = null;

        Try.Invoke(() =>
        {
            last = GetValue(Source.FirstInstance);
            return;

            var index = 0;
            Source.EachSource(i =>
            {
                var j = GetValue(i);

                if (index > 0)
                {
                    if (last != j)
                        allEqual = false;
                }

                last = j;
                index++;
            });
        });

        if (allEqual)
        {
            result = last;
            IsIndeterminate = false;
        }
        else
        {
            result = null;
            IsIndeterminate = true;
        }

        return result;
    }

    public void UpdateSourceWithValue(object newValue)
    {
        if (!IsWritable || IsReadOnly)
            return;

        Set(() => Value, newValue);
        AssignValueToSource(newValue);

        switch (Source.DataType)
        {
            case Types.Value:
                Parent.If<AssignableMemberModel<Info>>(i => i.SetValue(i.Source.Instance, Source.Instance));
                break;
        }
        OnValueChanged(newValue);
    }

    public virtual void UpdateValueFromSource() => Set(() => Value, GetValueFromSource());

    #endregion

    #region Commands

    ICommand copyCommand;
    public ICommand CopyCommand => copyCommand ??= new RelayCommand
        (() => Copy.Set(Value), () => Value != null);

    ICommand pasteCommand;
    public ICommand PasteCommand => pasteCommand ??= new RelayCommand
        (() => Value = Copy.Get<object>(), () => (!IsReadOnly && IsWritable) && Copy.Get<object>() is object i && i.GetType().Inherits(TemplateType ?? Type, true));

    ICommand defaultCommand;
    public ICommand DefaultCommand => defaultCommand
        ??= new RelayCommand<AssignableMemberModel<Info>>(i =>
        {
            if (i.HasDefaultValue)
            {
                i.Value = i.DefaultValue;
            }
            else
            {
                var type = i.Value?.GetType() ?? i.Type;
                i.Value = type.GetDefaultValue();
            }
        },
        i => i is not null && (!i.IsReadOnly && i.IsWritable) && (i.HasDefaultValue || !(i.Value?.GetType() ?? i.Type).IsAbstract));

    ICommand executeAllCommand;
    public ICommand ExecuteAllCommand => executeAllCommand ??= new RelayCommand(() =>
    {
        if (TemplateType == typeof(ICommand))
            Source.EachSource(i => (GetValue(i) as ICommand)?.Execute());
    },
    () =>
    {
        var result = false;
        if (TemplateType == typeof(ICommand))
        {
            result = true;
            Source.EachSource(i => result = result && (GetValue(i) as ICommand)?.CanExecute(null) == true);
        }
        return result;
    });

    ICommand resetCommand;
    public ICommand ResetCommand => resetCommand
        ??= new RelayCommand<MemberModel>(i => i.Value.If<IReset>(j => j.Reset()), i => i?.Value is IReset);

    ICommand revertCommand;
    public ICommand RevertCommand => revertCommand
        ??= new RelayCommand<MemberModel>(i => i.Value = InitialValue, i => InitialValue != null);

    ICommand unsetCommand;
    public ICommand UnsetCommand => unsetCommand
        ??= new RelayCommand<AssignableMemberModel<Info>>
        (
            i => i.Value = null,
            i => i != null
                && (!i.IsReadOnly && i.IsWritable)
                && (i.Type?.IsValueType == false || i.Type?.IsNullable() == true)
                && i.Value is not null
                && i.IsNullable
        );

    #endregion
}

#endregion

///

#region FieldModel : AssignableMemberModel<FieldInfo>

public class FieldModel : AssignableMemberModel<FieldInfo>
{
    public override bool IsWritable => Member.CanSet();

    public override Type Type => Member.FieldType;

    public FieldModel(MemberCollection collection, MemberModel parent, MemberSource source, FieldInfo member, MemberAttributes attributes, int depthIndex, IComparer sort) : base(collection, parent, source, member, attributes, depthIndex, sort) { }

    protected override object GetValue(object input) => Member.GetValue(input);

    protected override void SetValue(object source, object value) => Member.SetValue(source, value);
}

#endregion

#region PropertyModel : AssignableMemberModel<PropertyInfo>

public class PropertyModel : AssignableMemberModel<PropertyInfo>
{
    public override bool IsWritable => Member.CanSet();

    public override Type Type => Member.PropertyType;

    public PropertyModel(MemberCollection collection, MemberModel parent, MemberSource source, PropertyInfo member, MemberAttributes attributes, int depthIndex, IComparer sort) : base(collection, parent, source, member, attributes, depthIndex, sort) { }

    protected override object GetValue(object input) => Member.GetValue(input);

    protected override void SetValue(object source, object value) => Member.SetValue(source, value, null);
}

#endregion

///

#region IndexorPropertyModel<Indexor, Key> : PropertyModel

public abstract class IndexorPropertyModel<TIndexor, TKey> : PropertyModel
{
    public override Type DeclaringType => Indexor?.GetType();

    public override string DisplayName => Name;

    public TIndexor Indexor => (TIndexor)Source.FirstInstance;

    public override bool IsWritable => !IsReadOnly;

    public TKey Key { get => Get<TKey>(); set => Set(value); }

    public override MemberTypes MemberType => MemberTypes.Property;

    public override string Name => $"{Key}";

    public override Type TemplateType => GetTemplateType(Type);

    public override Type Type => Value?.GetType();

    public IndexorPropertyModel(MemberCollection collection, MemberModel parent, MemberSource source, PropertyInfo member, MemberAttributes attributes, int depthIndex, IComparer sort, TKey key) : base(collection, parent, source, member, attributes, depthIndex, sort) 
    {
        Key = key;
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Key):
                UpdateValueFromSource();
                InitialValue ??= Value;

                Update(() => Name);
                ApplyAttributes();
                break;

            case nameof(IsReadOnly):
                Update(() => IsWritable);
                break;

            case nameof(Name):
                Update(() => DisplayName);
                break;

            case nameof(Value):
                Update(() => Type);
                break;

            case nameof(Type):
                Update(() => TemplateType);
                break;
        }
    }

    internal override void ApplyAttributes() => OverrideAttributes();
}

#endregion

#region ItemModel : IndexorPropertyModel<IList, int>

public class ItemModel : IndexorPropertyModel<IList, int>
{
    public ItemModel(MemberCollection collection, MemberModel parent, object item) : base(collection, parent, new(parent.Value), null, null, 0, parent.Sort, -1) { }

    protected override object GetValue(object input)
    {
        return Indexor.ContainsKey(Key) ? Indexor[Key] : null;
    }

    protected override void SetValue(object source, object value)
    {
        Indexor.If(i => i.ContainsKey(Key), i => { i[Key] = value; Update(() => Value); });
    }
}

#endregion

#region ResourceModel : IndexorPropertyModel<ResourceDictionary, object>

public class ResourceModel : IndexorPropertyModel<ResourceDictionary, object>
{
    public override string Category => "Resources";

    public ResourceModel(MemberCollection collection, MemberSource source, IComparer sort, object key) : base(collection, null, source, null, null, 0, sort, key) { }

    protected override object GetValue(object input) => Indexor[Key];

    protected override void SetValue(object source, object value) => Indexor[Key] = value;
}

#endregion

///

#region ContainerMemberModel<T>

public abstract class ContainerMemberModel<T> : MemberModel
{
    public override Type TemplateType => Type;

    public override Type Type => typeof(T);

    public override object Value => Source.FirstInstance;

    public ContainerMemberModel(MemberCollection collection, MemberModel parent, MemberSource source, IComparer sort) : base(collection, parent, source, null, 0, sort) { }
}

#endregion

#region CollectionModel : ContainerMemberModel<INotifyCollectionChanged>

public class CollectionModel : ContainerMemberModel<INotifyCollectionChanged>
{
    public CollectionModel(MemberCollection members, MemberSource source, IComparer sort) : base(members, null, source, sort)
    {
        HideName = true;
    }
}

#endregion