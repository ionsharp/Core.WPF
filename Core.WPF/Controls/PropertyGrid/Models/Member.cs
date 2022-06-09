using Imagin.Core.Analytics;
using Imagin.Core.Converters;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using Imagin.Core.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Controls;

public abstract class MemberModel : BaseNamable, IComparable
{
    readonly internal Handle Handle = false;

    System.Timers.Timer timer;

    #region Fields

    public readonly MemberCollection Collection;

    #endregion

    #region Properties

        
    /// <summary>[Attribute type] => [Member (i), First attribute (j), All attributes (k)]</summary>
    readonly MemberAttributeHandler attributes = new()
    {
        { typeof(AssignableAttribute),
            new((i, j, k) =>
            {
                i.Assignable = true;

                var a = j.As<AssignableAttribute>();
                if (a.Types is Type[] m && m.Length > 0)
                {
                    i.AssignableTypes = new(m);
                    return;
                }
                if (i.Type != null && PropertyGrid.AssignableTypes.ContainsKey(i.Type))
                {
                    i.AssignableTypes = new(PropertyGrid.AssignableTypes[i.Type]);
                    return;
                }
                if (a.Values != null)
                    Try.Invoke(() => i.AssignableValues = i.Source.First.GetPropertyValue(a.Values), e => Log.Write<MemberModel>(e));
            })
        },
        { typeof(CategoryAttribute),
            new((i, j, k) => i.Category = j.As<CategoryAttribute>().Category) },
        { typeof(System.ComponentModel.CategoryAttribute),
            new((i, j, k) => i.Category = j.As<System.ComponentModel.CategoryAttribute>().Category) },
        { typeof(ClearAttribute),
            new((i, j, k) => i.ClearText = j.As<ClearAttribute>().Value) },
        { typeof(CommandAttribute),
            new((i, j, k) =>
            {
                if (j.As<CommandAttribute>()?.CommandName is string commandName)
                    Try.Invoke(() => i.Command = i.Source.First.GetPropertyValue(commandName) as ICommand, e => Log.Write<MemberModel>(e));
            })
        },
        { typeof(ContentAttribute),
            new((i, j, k) => i.Content = j.As<ContentAttribute>().Content) },
        { typeof(ContentTriggerAttribute), 
            new((i, j, k) => i.OnContentTrigger(i.Source.First, new(j.As<ContentTriggerAttribute>().PropertyName))) },
        { typeof(ConvertAttribute),
            new((i, j, k) =>
            {
                Try.Invoke(() => i.Converter = j.As<ConvertAttribute>().Converter.Create<IValueConverter>(), e => Log.Write<MemberModel>(e));
                i.ConverterParameter = j.As<ConvertAttribute>().ConverterParameter;
            })
        },
        { typeof(DelimitAttribute),
            new((i, j, k) => i.Delimiter = j.As<DelimitAttribute>().Character) },
        { typeof(DescriptionAttribute),
            new((i, j, k) => i.Description = j.As<DescriptionAttribute>().Description) },
        { typeof(System.ComponentModel.DescriptionAttribute),
            new((i, j, k) => i.Description = j.As<System.ComponentModel.DescriptionAttribute>().Description) },
        { typeof(DisplayNameAttribute),
            new((i, j, k) => i.DisplayName = j.As<DisplayNameAttribute>().DisplayName) },
        { typeof(System.ComponentModel.DisplayNameAttribute),
            new((i, j, k) => i.DisplayName = j.As<System.ComponentModel.DisplayNameAttribute>().DisplayName) },
        { typeof(EnableTriggerAttribute),
            new((i, j, k) => i.OnEnableTrigger(i.Source.First, new(j.As<EnableTriggerAttribute>().Property))) },
        { typeof(FeaturedAttribute),
            new((i, j, k) => i.IsFeatured = j.As<FeaturedAttribute>().Featured) },
        { typeof(FormatAttribute),
            new((i, j, k) => i.Format = j.As<FormatAttribute>().Format) },
        { typeof(HeightAttribute),
            new((i, j, k) =>
            {
                i.MaximumHeight = j.As<HeightAttribute>().MaximumHeight; i.MinimumHeight = j.As<HeightAttribute>().MinimumHeight; i.Height = j.As<HeightAttribute>().Height;
            })
        },
        { typeof(IconAttribute),
            new((i, j, k) =>
            {
                i.Icon = j.As<IconAttribute>().Icon; i.IconColor = j.As<IconAttribute>().Color;
            })
        },
        { typeof(IndexAttribute),
            new((i, j, k) => i.Index = j.As<IndexAttribute>().Index) },
        { typeof(ItemStyleAttribute),
            new((i, j, k) => i.ItemStyle = j.As<ItemStyleAttribute>().Style) },
        { typeof(ItemTypeAttribute),
            new((i, j, k) => i.ItemType = j.As<ItemTypeAttribute>().ItemType) },
        { typeof(LabelAttribute),
            new((i, j, k) => i.Label = j.As<LabelAttribute>().Label) },
        { typeof(LocalizeAttribute),
            new((i, j, k) => i.Localize = j.As<LocalizeAttribute>().Localize) },
        { typeof(LockedAttribute),
            new((i, j, k) => i.IsLockable = j.As<LockedAttribute>().Locked) },
        { typeof(PlaceholderAttribute),
            new((i, j, k) => i.Placeholder = j.As<PlaceholderAttribute>().Placeholder) },
        { typeof(PropertyTriggerAttribute),
            new((i, j, k) =>
            {
                if (k?.Count() > 0)
                {
                    foreach (var m in k)
                        i.OnPropertyTrigger(i.Source.First, new(m.As<PropertyTriggerAttribute>().SourceName));
                }
            })
        },
        { typeof(RangeAttribute),
            new((i, j, k) =>
            {
                i.Increment = j.As<RangeAttribute>().Increment; i.Maximum = j.As<RangeAttribute>().Maximum; i.Minimum = j.As<RangeAttribute>().Minimum;
            })
        },
        { typeof(ReadOnlyAttribute),
            new((i, j, k) => i.IsReadOnly = j.As<ReadOnlyAttribute>().ReadOnly == true) },
        { typeof(System.ComponentModel.ReadOnlyAttribute),
            new((i, j, k) => i.IsReadOnly = j.As<System.ComponentModel.ReadOnlyAttribute>().IsReadOnly == true) },
        { typeof(SourceAttribute), 
            new((i, j, k) =>
            {
                i.ItemPath = j.As<SourceAttribute>().ItemPath;
                j.As<SourceAttribute>()?.ItemSource.If(m => Try.Invoke(() => i.ItemSource = i.Source.First.GetPropertyValue(m), e => Log.Write<MemberModel>(e)));
            }) 
        },
        { typeof(StringFormatAttribute), 
            new((i, j, k) => i.StringFormat = j.As<StringFormatAttribute>().Format) },
        { typeof(StyleAttribute), 
            new((i, j, k) => i.Style = j.As<StyleAttribute>().Style) },
        { typeof(SuggestionsAttribute),
            new((i, j, k) =>
            {
                if (j.As<SuggestionsAttribute>().SourceName is string suggestions)
                {
                    i.Suggestions = i.Source.First.GetPropertyValue(suggestions);
                    if (i.Suggestions != null)
                    {
                        if (j.As<SuggestionsAttribute>().CommandName is string suggestionCommand)
                            i.SuggestionCommand = i.Source.First.GetPropertyValue(suggestionCommand) as ICommand;
                    }
                }
            })
        },
        { typeof(ToolAttribute), 
            new((i, j, k) => i.IsTool = j.As<ToolAttribute>() != null) },
        { typeof(UpdateSourceTriggerAttribute), 
            new((i, j, k) => i.UpdateSourceTrigger = j.As<UpdateSourceTriggerAttribute>().UpdateSourceTrigger) },
        { typeof(ValidateAttribute), 
            new((i, j, k) =>
            {
                IValidate validateHandler = null;
                Try.Invoke(() => validateHandler = j.As<ValidateAttribute>()?.Type.Create<IValidate>(), e => Log.Write<MemberModel>(e));
                i.ValidateHandler = validateHandler;

            }) 
        },
        { typeof(WidthAttribute),
            new((i, j, k) =>
            {
                i.MaximumWidth = j.As<WidthAttribute>().MaximumWidth; i.MinimumWidth = j.As<WidthAttribute>().MinimumWidth; i.Width = j.As<WidthAttribute>().Width;
            }) 
        },
    };
    public MemberAttributes Attributes { get; private set; }

    public MemberSource Source { get; private set; }

    //...

    public abstract bool CanWrite { get; }

    //...

    /// <summary>
    /// To do: Use <see cref="Imagin.Core.Converters.NullConverter"/> to prevent changing <see cref="UpDown{T}.Increment"/> if <see langword="null"/>. It does not currently work as intended...
    /// </summary>
    object DefaultIncrement
    {
        get
        {
            if (Type == typeof(byte))
                return (byte)1;

            if (Type == typeof(decimal))
                return (decimal)1;

            if (Type == typeof(double))
                return (double)1;

            if (Type == typeof(short))
                return (short)1;

            if (Type == typeof(int))
                return (int)1;

            if (Type == typeof(long))
                return (long)1;

            if (Type == typeof(float))
                return (float)1;

            if (Type == typeof(TimeSpan))
                return 1.Seconds();

            if (Type == typeof(UDouble))
                return (UDouble)1;

            if (Type == typeof(ushort))
                return (ushort)1;

            if (Type == typeof(uint))
                return (uint)1;

            if (Type == typeof(ulong))
                return (ulong)1;

            return null;
        }
    }

    /// <summary>
    /// To do: Use <see cref="Imagin.Core.Converters.NullConverter"/> to prevent changing <see cref="UpDown{T}.Maximum"/> if <see langword="null"/>. It does not currently work as intended...
    /// </summary>
    object DefaultMaximum
    {
        get
        {
            if (Type == typeof(byte))
                return byte.MaxValue;

            if (Type == typeof(decimal))
                return decimal.MaxValue;

            if (Type == typeof(double))
                return double.MaxValue;

            if (Type == typeof(short))
                return short.MaxValue;

            if (Type == typeof(int))
                return int.MaxValue;

            if (Type == typeof(long))
                return long.MaxValue;

            if (Type == typeof(float))
                return float.MaxValue;

            if (Type == typeof(TimeSpan))
                return TimeSpan.MaxValue;

            if (Type == typeof(UDouble))
                return UDouble.MaxValue;

            if (Type == typeof(ushort))
                return ushort.MaxValue;

            if (Type == typeof(uint))
                return uint.MaxValue;

            if (Type == typeof(ulong))
                return ulong.MaxValue;

            return null;
        }
    }

    /// <summary>
    /// To do: Use <see cref="Imagin.Core.Converters.NullConverter"/> to prevent changing <see cref="UpDown{T}.Minimum"/> if <see langword="null"/>. It does not currently work as intended...
    /// </summary>
    object DefaultMinimum
    {
        get
        {
            if (Type == typeof(byte))
                return byte.MinValue;

            if (Type == typeof(decimal))
                return decimal.MinValue;

            if (Type == typeof(double))
                return double.MinValue;

            if (Type == typeof(short))
                return short.MinValue;

            if (Type == typeof(int))
                return int.MinValue;

            if (Type == typeof(long))
                return long.MinValue;

            if (Type == typeof(float))
                return float.MinValue;

            if (Type == typeof(TimeSpan))
                return TimeSpan.MinValue;

            if (Type == typeof(UDouble))
                return UDouble.MinValue;

            if (Type == typeof(ushort))
                return ushort.MinValue;

            if (Type == typeof(uint))
                return uint.MinValue;

            if (Type == typeof(ulong))
                return ulong.MinValue;

            return null;
        }
    }

    //...

    Type actualType;
    public Type ActualType
    {
        get => actualType;
        private set => this.Change(ref actualType, value);
    }

    public Type BaseType 
        => Type?.BaseType;

    public virtual Type DeclaringType 
        => Member?.DeclaringType;

    public Type TemplateType => ActualType is Type i ? GetTemplateType(i) : null;

    public abstract Type Type { get; }

    //...

    bool assignable = false;
    public bool Assignable
    {
        get => assignable;
        private set => this.Change(ref assignable, value);
    }
    
    ObservableCollection<Type> assignableTypes = new();
    public ObservableCollection<Type> AssignableTypes
    {
        get => assignableTypes;
        private set => this.Change(ref assignableTypes, value);
    }

    object assignableValues = null;
    public object AssignableValues
    {
        get => assignableValues;
        private set => this.Change(ref assignableValues, value);
    }

    string category = null;
    public string Category
    {
        get => category;
        set => this.Change(ref category, value);
    }

    bool clearText = true;
    public bool ClearText
    {
        get => clearText;
        set => this.Change(ref clearText, value);
    }

    ICommand command = null;
    public ICommand Command
    {
        get => command;
        private set => this.Change(ref command, value);
    }

    object content = null;
    public object Content
    {
        get => content;
        private set => this.Change(ref content, value);
    }

    IValueConverter converter = null;
    public IValueConverter Converter
    {
        get => converter;
        private set => this.Change(ref converter, value);
    }

    object converterParameter = null;
    public object ConverterParameter
    {
        get => converterParameter;
        private set => this.Change(ref converterParameter, value);
    }

    char delimiter = ';';
    public char Delimiter
    {
        get => delimiter;
        set => this.Change(ref delimiter, value);
    }

    int depth = 0;
    public int Depth
    {
        get => depth;
        set => this.Change(ref depth, value);
    }

    string description = null;
    public string Description
    {
        get => description;
        private set => this.Change(ref description, value);
    }

    string displayName = null;
    public virtual string DisplayName
    {
        get => displayName;
        set => this.Change(ref displayName, value);
    }

    FileSizeFormat fileSizeFormat = FileSizeFormat.BinaryUsingSI;
    public FileSizeFormat FileSizeFormat
    {
        get => fileSizeFormat;
        set => this.Change(ref fileSizeFormat, value);
    }

    object format = default;
    public virtual object Format
    {
        get => format;
        private set => this.Change(ref format, value);
    }

    double height = double.NaN;
    public double Height
    {
        get => height;
        private set => this.Change(ref height, value);
    }

    string icon = null;
    public string Icon
    {
        get => icon;
        private set => this.Change(ref icon, value);
    }

    string iconColor = null;
    public string IconColor
    {
        get => iconColor;
        private set => this.Change(ref iconColor, value);
    }

    dynamic increment = default;
    public object Increment
    {
        get => increment;
        set => this.Change(ref increment, value);
    }

    int index = 0;
    public int Index
    {
        get => index;
        private set => this.Change(ref index, value);
    }

    bool isEnabled = true;
    public virtual bool IsEnabled
    {
        get => isEnabled;
        private set => this.Change(ref isEnabled, value);
    }

    bool isFeatured = false;
    public bool IsFeatured
    {
        get => isFeatured;
        private set => this.Change(ref isFeatured, value);
    }
        
    bool isIndeterminate = false;
    public bool IsIndeterminate
    {
        get => isIndeterminate;
        private set => this.Change(ref isIndeterminate, value);
    }

    public bool IsIndeterminable 
        => PropertyGrid.IndeterminableTypes.Contains(TemplateType);

    bool isLockable = false;
    public bool IsLockable
    {
        get => isLockable;
        private set => this.Change(ref isLockable, value);
    }

    bool isLocked = false;
    public bool IsLocked
    {
        get => isLocked;
        internal set => this.Change(ref isLocked, value);
    }

    bool isReadOnly = false;
    public virtual bool IsReadOnly
    {
        get => isReadOnly;
        private set => this.Change(ref isReadOnly, value);
    }

    bool isTool = false;
    public bool IsTool
    {
        get => isTool;
        private set => this.Change(ref isTool, value);
    }

    string itemPath = ".";
    public string ItemPath
    {
        get => itemPath;
        private set => this.Change(ref itemPath, value);
    }

    object itemSource = null;
    public object ItemSource
    {
        get => itemSource;
        private set => this.Change(ref itemSource, value);
    }

    Enum itemStyle = null;
    public Enum ItemStyle
    {
        get => itemStyle;
        private set => this.Change(ref itemStyle, value);
    }

    Type itemType = null;
    public Type ItemType
    {
        get => itemType;
        private set => this.Change(ref itemType, value);
    }

    ListCollectionView itemTypes = null;
    public ListCollectionView ItemTypes
    {
        get => itemTypes;
        private set => this.Change(ref itemTypes, value);
    }

    bool label = true;
    public bool Label
    {
        get => label;
        private set => this.Change(ref label, value);
    }

    string leftText = null;
    public string LeftText
    {
        get => leftText;
        set => this.Change(ref leftText, value);
    }

    bool localize = true;
    public bool Localize
    {
        get => localize;
        private set => this.Change(ref localize, value);
    }

    dynamic maximum = default;
    public object Maximum
    {
        get => maximum;
        set => this.Change(ref maximum, value);
    }

    double maximumHeight = double.NaN;
    public double MaximumHeight
    {
        get => maximumHeight;
        private set => this.Change(ref maximumHeight, value);
    }

    double maximumWidth = double.NaN;
    public double MaximumWidth
    {
        get => maximumWidth;
        private set => this.Change(ref maximumWidth, value);
    }

    MemberInfo member;
    public virtual MemberInfo Member
    {
        get => member;
        private set
        {
            this.Change(ref member, value);
            MemberType = member?.MemberType ?? MemberTypes.Custom;
            Name = member?.Name;
        }
    }

    MemberCollection members = null;
    public MemberCollection Members
    {
        get => members;
        set => this.Change(ref members, value);
    }

    MemberTypes memberType;
    public MemberTypes MemberType
    {
        get => memberType;
        private set => this.Change(ref memberType, value);
    }

    dynamic minimum = default;
    public object Minimum
    {
        get => minimum;
        set => this.Change(ref minimum, value);
    }

    double minimumHeight = double.NaN;
    public double MinimumHeight
    {
        get => minimumHeight;
        private set => this.Change(ref minimumHeight, value);
    }

    double minimumWidth = double.NaN;
    public double MinimumWidth
    {
        get => minimumWidth;
        private set => this.Change(ref minimumWidth, value);
    }

    string placeholder = null;
    public string Placeholder
    {
        get => placeholder;
        private set => this.Change(ref placeholder, value);
    }

    string rightText = null;
    public string RightText
    {
        get => rightText;
        set => this.Change(ref rightText, value);
    }

    string stringFormat = null;
    public string StringFormat
    {
        get => stringFormat;
        private set => this.Change(ref stringFormat, value);
    }

    object style = null;
    public virtual object Style
    {
        get => style;
        private set => this.Change(ref style, value);
    }

    object suggestions = null;
    public object Suggestions
    {
        get => suggestions;
        private set => this.Change(ref suggestions, value);
    }

    ICommand suggestionCommand = null;
    public ICommand SuggestionCommand
    {
        get => suggestionCommand;
        private set => this.Change(ref suggestionCommand, value);
    }

    UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default;
    public UpdateSourceTrigger UpdateSourceTrigger
    {
        get => updateSourceTrigger;
        private set => this.Change(ref updateSourceTrigger, value);
    }

    IValidate validateHandler = null;
    public IValidate ValidateHandler
    {
        get => validateHandler;
        set => this.Change(ref validateHandler, value);
    }

    dynamic value = default;
    public object Value
    {
        get => value;
        set => OnValueChanging(this.value, value);
    }

    double width = double.NaN;
    public double Width
    {
        get => width;
        private set => this.Change(ref width, value);
    }

    #endregion

    #region MemberModel

    public MemberModel(MemberData data) : base()
    {
        Collection
            = data.Collection;
        Source
            = data.Source;
        Member 
            = data.Member;

        DisplayName = Name;

        Increment = DefaultIncrement;
        Maximum = DefaultMaximum; Minimum = DefaultMinimum;

        Attributes = data.Attributes;
        Attributes.Apply(this, attributes);

        IsReadOnly = !CanWrite;

        if (GetValue() is ITypes i)
        {
            ItemTypes = new(new ObservableCollection<Type>(i.GetTypes()));
            ItemTypes.CustomSort = TypeComparer.Default;
            ItemTypes.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = CategoryConverter.Default });
        }
    }

    #endregion

    #region Methods

    void OnContentTrigger(object sender, PropertyChangedEventArgs e)
    {
        var trigger = Attributes.GetFirst<ContentTriggerAttribute>();
        if (e.PropertyName == trigger.PropertyName)
        {
            Try.Invoke(() =>
            {
                var result = sender.GetPropertyValue(e.PropertyName);
                Content = trigger.Format?.F(result) ?? result;
            });
        }
    }

    void OnPropertyTrigger(object sender, PropertyChangedEventArgs e)
    {
        var triggers = Attributes.GetAll<PropertyTriggerAttribute>();
        if (triggers?.Count() > 0)
        {
            foreach (var i in triggers)
            {
                if (e.PropertyName == i.SourceName)
                {
                    Try.Invoke(() =>
                    {
                        var result = sender.GetPropertyValue(e.PropertyName);
                        this.SetPropertyValue(i.TargetName, result);
                    },
                    e => Log.Write<MemberModel>(e));
                }
            }
        }
    }
        
    void OnEnableTrigger(object sender, PropertyChangedEventArgs e)
    {
        var trigger = Attributes.GetFirst<EnableTriggerAttribute>();
        if (e.PropertyName == trigger.Property)
        {
            Try.Invoke(() =>
            {
                var result = sender.GetPropertyValue(e.PropertyName);
                IsEnabled = result?.Equals(trigger.Value) == true;
            });
        }
    }

    void OnItemSourceChanged(object sender, PropertyChangedEventArgs e)
    {
        var source = Attributes.GetFirst<SourceAttribute>();
        if (e.PropertyName == source.ItemSource)
            Try.Invoke(() => ItemSource = sender.GetPropertyValue(e.PropertyName));
    }

    void OnUpdate(object sender, System.Timers.ElapsedEventArgs e)
    {
        UpdateValue();
    }

    //...

    int IComparable.CompareTo(object a)
    {
        if (a is MemberModel b)
            return Index.CompareTo(b.Index);

        return 0;
    }

    //...

    protected T Info<T>() where T : MemberInfo => (T)Member;

    //...

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Value):
                ActualType = value?.GetType();
                this.Changed(() => TemplateType);
                break;
        }
    }

    //...

    void OnValueChanging(object oldValue, object newValue)
    {
        if (!isReadOnly)
        {
            Unsubscribe();
            SetValue(newValue);

            UpdateValue(newValue);
            switch (Source.DataType)
            {
                case Types.Value:
                    Collection.ParentMember.If(i => i.SetValue(i.Source.First, Source.First));
                    break;
            }

            OnValueChanged(newValue);
            Subscribe();
        }
    }

    protected virtual void OnValueChanged(object input) { }

    //...

    public object GetValue()
    {
        object result = null;
        Dispatch.Invoke(() =>
        {
            Try.Invoke(() =>
            {
                IsIndeterminate = false;
                result = GetValue(Source.First);
                for (var i = 1; i < Source.Count; i++)
                {
                    var next = GetValue(Source[i]);
                    if (result?.Equals(next) == false)
                    {
                        IsIndeterminate = true;
                        result = null;
                        break;
                    }
                }
            });
        });
        return result;
    }

    protected abstract object GetValue(object input);

    public IEnumerable<object> GetValues()
    {
        foreach (var i in Source)
            yield return i;

        yield break;
    }

    //...

    protected void SetValue(object value) => Handle.Invoke(() =>
    {
        foreach (var i in Source)
            SetValue(i, value);
    });

    protected abstract void SetValue(object source, object value);

    //...

    internal void UpdateValue() => UpdateValue(GetValue());

    internal void UpdateValue(object input) => this.Change(ref value, input, () => Value);

    //...

    internal void UpdateSource(MemberSource input)
    {
        Unsubscribe();
        Source = input;

        UpdateValue();
        Subscribe();
    }

    //...

    void SubscribeTriggers(bool subscribe)
    {
        foreach (var i in Source)
        {
            if (i is INotifyPropertyChanged j)
            {
                if (Attributes.GetFirst<ContentTriggerAttribute>() != null)
                    subscribe.If(true, () => j.PropertyChanged += OnContentTrigger, () => j.PropertyChanged -= OnContentTrigger);

                if (Attributes.GetFirst<EnableTriggerAttribute>() != null)
                    subscribe.If(true, () => j.PropertyChanged += OnEnableTrigger, () => j.PropertyChanged -= OnEnableTrigger);

                if (Attributes.GetFirst<PropertyTriggerAttribute>() != null)
                    subscribe.If(true, () => j.PropertyChanged += OnPropertyTrigger, () => j.PropertyChanged -= OnPropertyTrigger);

                if (Attributes.GetFirst<SourceAttribute>() != null)
                    subscribe.If(true, () => j.PropertyChanged += OnItemSourceChanged, () => j.PropertyChanged -= OnItemSourceChanged);
            }
        }
    }

    public virtual void Subscribe()
    {
        var a = TemplateType == typeof(object) || GetTemplateType(Type) == typeof(object);
        var b = Style?.Equals(ObjectStyle.Default) == true;

        var value = Value;
        if (a && b && value != null)
        {
            Members ??= new(Collection.ParentControl, this, Depth + 1);
            _ = Members.Reload(MemberCollection.LoadType.Recreate, new(new MemberRouteSource(value)), MemberFilter.Field | MemberFilter.Property, null, false, null);
        }

        SubscribeTriggers(true);

        if (Attributes.GetFirst<UpdateAttribute>() is UpdateAttribute updateAttribute)
        {
            timer = new() { Interval = updateAttribute.Seconds * 1000 };
            timer.Elapsed += OnUpdate;
            timer.Start();
        }
    }

    public virtual void Unsubscribe()
    {
        SubscribeTriggers(false);

        if (timer != null)
        {
            timer.Stop();
            timer.Elapsed -= OnUpdate;
            timer.Dispose();
        }

        Members?.ForEach(i => i.Unsubscribe());
    }

    //...

    public static Type GetTemplateType(Type input)
    {
        if (input == null)
            return null;

        if (input.IsNullable())
            input = Enumerable.FirstOrDefault(input.GetGenericArguments());

        if (PropertyGrid.DefaultTypes.Contains(input))
            return input;

        if (input.IsArray)
            return typeof(Array);

        if (input.IsEnum)
            return typeof(Enum);

        if (input.Implements<ICommand>())
            return typeof(ICommand);

        //The order here is important!
        if (input.Implements<INotifyCollectionChanged>())
            return typeof(INotifyCollectionChanged);

        if (input.Implements<IList>())
            return typeof(IList);

        if (input.Implements<IEnumerable>())
            return typeof(IEnumerable);

        return typeof(object); //input.IsClass || input.IsValueType
    }

    #endregion
}