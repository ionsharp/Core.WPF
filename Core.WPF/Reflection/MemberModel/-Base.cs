using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
using Imagin.Core.Storage;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Reflection;

public abstract partial class MemberModel : BaseNamable, IComparable
{
    readonly Handle Handle = false;

    System.Timers.Timer timer;

    #region Attributes

    public MemberAttributes Attributes { get; private set; }

    /// <summary>[Attribute type] => [Member (i), First attribute (j), All attributes (k)]</summary>
    readonly MemberAttributeHandler attributes = new()
    {
        {
            typeof(AboveAttribute),
            new((i, j, k) => i.IsFeatured = true)
        },
        {
            typeof(AngleAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(AssignableAttribute),
            new((i, j, k) =>
            {
                i.Assignable = true;

                var a = j.As<AssignableAttribute>();
                if (a.Types is Type[] m && m.Length > 0)
                {
                    i.AssignableTypes = new(m);
                    return;
                }
                if (i.Type != null && MemberGrid.AssignableTypes.ContainsKey(i.Type))
                {
                    i.AssignableTypes = new(MemberGrid.AssignableTypes[i.Type]);
                    return;
                }
                if (a.Values != null)
                    Try.Invoke(() => i.AssignableValues = i.Source.Instance.GetPropertyValue(a.Values), e => Log.Write<MemberModel>(e));
            })
        },
        {
            typeof(BelowAttribute),
            new((i, j, k) => i.IsFeatured = true)
        },
        {
            typeof(BulletsAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ButtonAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(CategoryAttribute),
            new((i, j, k) => i.Category = j.As<CategoryAttribute>().Category)
        },
        {
            typeof(System.ComponentModel.CategoryAttribute),
            new((i, j, k) => i.Category = j.As<System.ComponentModel.CategoryAttribute>().Category)
        },
        {
            typeof(ObjectAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(CommandAttribute),
            new((i, j, k) =>
            {
                if (j.As<CommandAttribute>()?.CommandName is string commandName)
                    Try.Invoke(() => i.Command = i.Source.Instance.GetPropertyValue(commandName) as ICommand, e => Log.Write<MemberModel>(e));
            })
        },
        {
            typeof(CommasAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ConvertAttribute),
            new((i, j, k) =>
            {
                Try.Invoke(() => i.Converter = j.As<ConvertAttribute>().Converter.Create<IValueConverter>(), e => Log.Write<MemberModel>(e));
                i.ConverterParameter = j.As<ConvertAttribute>().ConverterParameter;
            })
        },
        {
            typeof(DefaultValueAttribute),
            new((i, j, k) => i.DefaultValue = j.As<DefaultValueAttribute>().Value)
        },
        {
            typeof(DescriptionAttribute),
            new((i, j, k) => i.Description = j.As<DescriptionAttribute>().Description)
        },
        {
            typeof(System.ComponentModel.DescriptionAttribute),
            new((i, j, k) => i.Description = j.As<System.ComponentModel.DescriptionAttribute>().Description)
        },
        {
            typeof(DisplayNameAttribute),
            new((i, j, k) => i.DisplayName = j.As<DisplayNameAttribute>().DisplayName)
        },
        {
            typeof(System.ComponentModel.DisplayNameAttribute),
            new((i, j, k) => i.DisplayName = j.As<System.ComponentModel.DisplayNameAttribute>().DisplayName)
        },
        {
            typeof(FeatureAttribute),
            new((i, j, k) => i.IsFeatured = j.As<FeatureAttribute>().Featured)
        },
        {
            typeof(FilePathAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(FolderPathAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(GradientAttribute),
            new((i, j, k) => i.Gradient = j.As<GradientAttribute>().Colors.Select(l => XColor.Convert(new ByteVector4($"{l}"))).ToArray())
        },
        {
            typeof(HeightAttribute),
            new((i, j, k) =>
            {
                i.MaximumHeight = j.As<HeightAttribute>().MaximumHeight; i.MinimumHeight = j.As<HeightAttribute>().MinimumHeight; i.Height = j.As<HeightAttribute>().Height;
            })
        },
        {
            typeof(HexadecimalAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(HorizontalAttribute),
            new((i, j, k) => i.Orientation = System.Windows.Controls.Orientation.Horizontal)
        },
        {
            typeof(ImageAttribute),
            new((i, j, k) =>
            {
                i.Icon = j.As<ImageAttribute>().Icon; i.IconColor = j.As<ImageAttribute>().Color;
            })
        },
        {
            typeof(IndexAttribute),
            new((i, j, k) => i.Index = j.As<IndexAttribute>().Index)
        },
        {
            typeof(LabelAttribute),
            new((i, j, k) => i.Label = j.As<LabelAttribute>().Label)
        },
        {
            typeof(LocalizeAttribute),
            new((i, j, k) => i.Localize = j.As<LocalizeAttribute>().Localize)
        },
        {
            typeof(LockedAttribute),
            new((i, j, k) => i.IsLockable = j.As<LockedAttribute>().Locked)
        },
        {
            typeof(MultipleAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(NullableAttribute),
            new((i, j, k) => i.IsNullable = true)
        },
        {
            typeof(PasswordAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ProgressAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(RangeAttribute),
            new((i, j, k) =>
            {
                i.Increment = j.As<RangeAttribute>().Increment; i.Maximum = j.As<RangeAttribute>().Maximum; i.Minimum = j.As<RangeAttribute>().Minimum;
            })
        },
        {
            typeof(ReadOnlyAttribute),
            new((i, j, k) => i.IsReadOnly = j.As<ReadOnlyAttribute>().ReadOnly == true)
        },
        {
            typeof(System.ComponentModel.ReadOnlyAttribute),
            new((i, j, k) => i.IsReadOnly = j.As<System.ComponentModel.ReadOnlyAttribute>().IsReadOnly == true)
        },
        {
            typeof(SearchAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SelectedIndexAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SetterAttribute),
            new((i, j, k) =>
            {
                if (k?.Count() > 0)
                {
                    foreach (var m in k)
                    {
                        if (m is SetterAttribute n)
                            Try.Invoke(() => i.SetPropertyValue(n.PropertyName, n.Value), e => Log.Write<MemberModel>(e));
                    }
                }
            })
        },
        {
            typeof(SliderAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SliderUpDownAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(StringFormatAttribute),
            new((i, j, k) => i.StringFormat = j.As<StringFormatAttribute>().Format)
        },
        {
            typeof(SuggestionsAttribute),
            new((i, j, k) =>
            {
                if (j.As<SuggestionsAttribute>().SourceName is string suggestions)
                {
                    i.Suggestions = i.Source.Instance.GetPropertyValue(suggestions);
                    if (i.Suggestions != null)
                    {
                        if (j.As<SuggestionsAttribute>().CommandName is string suggestionCommand)
                            i.SuggestionCommand = i.Source.Instance.GetPropertyValue(suggestionCommand) as ICommand;
                    }
                }
            })
        },
        {
            typeof(ThumbnailAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ToggleButtonAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(TokensAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ToolAttribute),
            new((i, j, k) => i.IsTool = j.As<ToolAttribute>() != null)
        },
        {
            typeof(TriggerAttribute),
            new((i, j, k) =>
            {
                if (k?.Count() > 0)
                {
                    foreach (var m in k)
                        i.OnTrigger(i.Source.Instance, new(m.As<TriggerAttribute>().SourceName));
                }
            })
        },
        {
            typeof(UnitAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(UpDownAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(UpdateSourceTriggerAttribute),
            new((i, j, k) => i.UpdateSourceTrigger = j.As<UpdateSourceTriggerAttribute>().UpdateSourceTrigger)
        },
        {
            typeof(ValidateAttribute),
            new((i, j, k) =>
            {
                IValidate validateHandler = null;
                Try.Invoke(() => validateHandler = j.As<ValidateAttribute>()?.Type.Create<IValidate>(), e => Log.Write<MemberModel>(e));
                i.ValidateHandler = validateHandler;

            })
        },
        {
            typeof(VerticalAttribute),
            new((i, j, k) => i.Orientation = System.Windows.Controls.Orientation.Vertical)
        },
        {
            typeof(WidthAttribute),
            new((i, j, k) =>
            {
                i.MaximumWidth = j.As<WidthAttribute>().MaximumWidth; i.MinimumWidth = j.As<WidthAttribute>().MinimumWidth; i.Width = j.As<WidthAttribute>().Width;
            })
        },
    };

    #endregion

    #region Properties

    public MemberCollection Parent { get; private set; }

    public MemberModel ParentMember { get; private set; }

    public MemberSource Source { get; private set; }

    //...

    public abstract bool CanWrite { get; }

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

    public IList Collection => Value as IList;

    public int CollectionLength
    {
        get => Collection.Count;
        set
        {
            Resize(value);
            this.Changed(() => CollectionLength);
        }
    }

    readonly Type[] CollectionTypes = XArray.New(typeof(Array), typeof(IEnumerable), typeof(IList), typeof(INotifyCollectionChanged));

    public bool IsCollectionType => TemplateType.EqualsAny(CollectionTypes) || GetTemplateType(Type).EqualsAny(CollectionTypes);

    //...

    readonly Type[] ReferenceTypes = XArray.New(typeof(Array), typeof(IEnumerable), typeof(IList), typeof(INotifyCollectionChanged), typeof(object));

    public bool IsReferenceType => TemplateType.EqualsAny(ReferenceTypes) || GetTemplateType(Type).EqualsAny(ReferenceTypes);

    //...

    /// <summary>
    /// To do: Use <see cref="Imagin.Core.Conversion.NullConverter"/> to prevent changing <see cref="UpDown{T}.Increment"/> if <see langword="null"/>. It does not currently work as intended...
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
    /// To do: Use <see cref="Imagin.Core.Conversion.NullConverter"/> to prevent changing <see cref="UpDown{T}.Maximum"/> if <see langword="null"/>. It does not currently work as intended...
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
    /// To do: Use <see cref="Imagin.Core.Conversion.NullConverter"/> to prevent changing <see cref="UpDown{T}.Minimum"/> if <see langword="null"/>. It does not currently work as intended...
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

    string caption = null;
    public string Caption
    {
        get => caption;
        set => this.Change(ref caption, value);
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
        set => this.Change(ref content, value);
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

    object defaultValue = null;
    public object DefaultValue
    {
        get => defaultValue;
        private set => this.Change(ref defaultValue, value);
    }

    char delimiter = ';';
    public char Delimiter
    {
        get => delimiter;
        set => this.Change(ref delimiter, value);
    }

    int depthIndex = 0;
    public int DepthIndex
    {
        get => depthIndex;
        set => this.Change(ref depthIndex, value);
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
        set => this.Change(ref format, value);
    }

    System.Windows.Media.Color[] gradient = null;
    public System.Windows.Media.Color[] Gradient
    {
        get => gradient;
        set => this.Change(ref gradient, value);
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

    bool isNullable = false;
    public virtual bool IsNullable
    {
        get => isNullable;
        private set => this.Change(ref isNullable, value);
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

    bool isVisible = true;
    public bool IsVisible
    {
        get => isVisible;
        set => this.Change(ref isVisible, value);
    }

    string itemPath = ".";
    public string ItemPath
    {
        get => itemPath;
        set => this.Change(ref itemPath, value);
    }

    object itemSource = null;
    public object ItemSource
    {
        get => itemSource;
        set => this.Change(ref itemSource, value);
    }

    Enum itemStyle = null;
    public Enum ItemStyle
    {
        get => itemStyle;
        set => this.Change(ref itemStyle, value);
    }

    Type itemType = null;
    public Type ItemType
    {
        get => itemType;
        set => this.Change(ref itemType, value);
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

    ListCollectionView sortedMembers = null;
    public ListCollectionView SortedMembers
    {
        get => sortedMembers;
        set => this.Change(ref sortedMembers, value);
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

    System.Windows.Controls.Orientation orientation = System.Windows.Controls.Orientation.Vertical;
    public System.Windows.Controls.Orientation Orientation
    {
        get => orientation;
        set => this.Change(ref orientation, value);
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

    Type style = null;
    public Type Style
    {
        get => style;
        set => this.Change(ref style, value);
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

    internal MemberModel(MemberModel parent, MemberData data, int depthIndex)
    {
        ParentMember
            = parent;
        Parent
            = data.Collection; Source = data.Source; Member = data.Member;

        Dispatch.Invoke(() => Category = Parent.Control.DefaultCategoryName);

        DepthIndex
            = depthIndex;
        DisplayName
            = Name;

        Increment = DefaultIncrement; Maximum = DefaultMaximum; Minimum = DefaultMinimum;

        Attributes = data.Attributes;
        Attributes.Apply(this, attributes);

        IsReadOnly = !CanWrite || IsReadOnly || Parent.Parent?.IsReadOnly == true;

        if (GetValue() is ITypes i)
            GetItemTypes(i);

        else if (Source.Instance is ITypes j)
            GetItemTypes(j);

        UpdateValue();
        RefreshHard();
    }

    public MemberModel(MemberData data, int depthIndex) : this(null, data, depthIndex) { }

    #endregion

    #region Methods

    int IComparable.CompareTo(object a)
    {
        if (a is MemberModel b)
            return Index.CompareTo(b.Index);

        return 0;
    }

    void EachParent(MemberModel input, Action<MemberModel> action)
    {
        MemberModel parent = input;
        while (parent != null)
        {
            parent = parent.Parent.Parent;
            parent.If(i => action(i));
        }
    }

    void GetItemTypes(ITypes input)
    {
        ItemTypes = new(new ObservableCollection<Type>(input.GetTypes()));
        ItemTypes.CustomSort = TypeComparer.Default;
        ItemTypes.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = CategoryConverter.Default });
    }

    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.Changed(() => CollectionLength);
    }

    void OnValueChanging(object oldValue, object newValue)
    {
        if (Type == typeof(ByteVector4))
            Log.Write<MemberModel>($"Old value = {oldValue}, new value = {newValue}");

        if (!isReadOnly)
        {
            var oldType = oldValue?.GetType();
            var newType = newValue?.GetType();

            Unsubscribe();
            SetValue(newValue);

            UpdateValue(newValue);
            switch (Source.DataType)
            {
                case Types.Value:
                    Parent.Parent.If(i => i.SetValue(i.Source.Instance, Source.Instance));
                    break;
            }

            OnValueChanged(newValue);

            if (oldType != newType)
                RefreshHard();

            else if (oldType == newType)
                RefreshSoft();

            Subscribe();
        }
    }

    void OnTrigger(object sender, PropertyChangedEventArgs e)
    {
        var triggers = Attributes.GetAll<TriggerAttribute>();
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

    void OnUpdate(object sender, System.Timers.ElapsedEventArgs e) => UpdateValue();

    //...

    protected virtual void OnValueChanged(object input) { }

    protected T Info<T>() where T : MemberInfo => (T)Member;

    //...

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(TemplateType):
                this.Changed(() => IsCollectionType);
                this.Changed(() => IsReferenceType);
                break;

            case nameof(Value):
                ActualType = value?.GetType();
                this.Changed(() => Collection);
                this.Changed(() => TemplateType);
                break;
        }
    }

    //...

    public static Type GetTemplateType(Type input)
    {
        if (input == null)
            return null;

        if (input.IsNullable())
            input = Enumerable.FirstOrDefault(input.GetGenericArguments());

        if (MemberGrid.DefaultTypes.Contains(input))
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

    #region GetValue

    public object GetValue()
    {
        object result = null;
        Dispatch.Invoke(() => Try.Invoke(() => result = GetValue(Source.Instance)));
        return result;
    }

    protected abstract object GetValue(object input);

    #endregion

    #region SetValue

    protected void SetValue(object value) => Handle.Invoke(() => SetValue(Source.Instance, value));

    protected abstract void SetValue(object source, object value);

    #endregion

    #region Refresh

    internal void RefreshHard()
    {
        var currentValue = Value;
        if (currentValue != null && IsReferenceType)
        {
            Members ??= new(Parent.Control, this, DepthIndex + 1);
            Members.Load(currentValue);

            if (SortedMembers == null)
            {
                var categorize = Attributes.GetFirst<CategorizeAttribute>()?.Categorize == true;

                SortedMembers = new(Members);

                if (categorize)
                    SortedMembers.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Category), ListSortDirection.Ascending));

                SortedMembers.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Index), ListSortDirection.Ascending));
                //SortedMembers.CustomSort = new MemberSortComparer(Parent.Control);

                if (categorize)
                    SortedMembers.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = MemberGroupConverterSelector.Default.Select(MemberGroupName.Category) });
            }
        }
        else
        {
            Members?.Clear();
            Members = null;
        }
    }

    internal void RefreshSoft()
    {
        UpdateValue();
        if (Members != null)
        {
            var newValue = GetValue();
            foreach (var i in Members)
            {
                i.UpdateSource(new(new MemberPathSource(newValue)));
                i.RefreshSoft();
            }
        }
    }

    #endregion

    #region Subscribe, Unsubscribe

    public virtual void Subscribe()
    {
        if (Source.Instance is INotifyPropertyChanged notify)
        {
            var triggers = Attributes.GetAll<TriggerAttribute>();
            if (triggers?.Count() > 0)
            {
                foreach (var trigger in triggers)
                    OnTrigger(notify, new(trigger.SourceName));
            }

            notify.PropertyChanged += OnTrigger;
        }

        if (Attributes.GetFirst<UpdateAttribute>() is UpdateAttribute update)
        {
            timer = new() { Interval = update.Seconds * 1000 };
            timer.Elapsed += OnUpdate;
            timer.Start();
        }

        if (Value is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged -= OnCollectionChanged;
            collection.CollectionChanged += OnCollectionChanged;
        }
    }

    public virtual void Unsubscribe()
    {
        if (Source.Instance is INotifyPropertyChanged j)
        {
            var triggers = Attributes.GetAll<TriggerAttribute>();
            if (triggers?.Count() > 0)
                j.PropertyChanged -= OnTrigger;
        }

        if (timer != null)
        {
            timer.Stop();
            timer.Elapsed -= OnUpdate;
            timer.Dispose();
        }

        Members?.ForEach(i => i.Unsubscribe());

        if (Value is INotifyCollectionChanged collection)
            collection.CollectionChanged -= OnCollectionChanged;
    }

    #endregion

    #region UpdateSource

    internal void UpdateSource(MemberSource input)
    {
        Unsubscribe();
        Source = input;
        Subscribe();
    }

    #endregion

    #region UpdateValue

    internal void UpdateValue() => UpdateValue(GetValue());

    internal void UpdateValue(object input) => this.Change(ref value, input, () => Value);
    
    internal void UpdateValueSafe() => Handle.SafeInvoke(RefreshSoft);

    #endregion

    #endregion

    int selectedIndex = -1;
    public int SelectedIndex
    {
        get => selectedIndex;
        set => this.Change(ref selectedIndex, value);
    }

    //...

    object CreateItem(Type type)
    {
        type ??= ItemType;
        return type == typeof(string) ? "" : type.Create<object>();
    }

    object GetSelectedItem()
        => Collection is IList i && i.Count > SelectedIndex && SelectedIndex >= 0 ? i[SelectedIndex] : null;

    //...

    protected virtual void InsertAbove(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i => Collection.Insert(index == -1 ? 0 : index, i)), e => Log.Write<MemberModel>(e));

    protected virtual void InsertBelow(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i =>
        {
            var newIndex = index + 1;
            if (index != -1 && newIndex < Collection.Count)
                Collection.Insert(newIndex, i);

            else Collection.Add(i);
        }), e => Log.Write<MemberModel>(e));

    void Resize(int length)
    {
        if (length == Collection?.Count)
            return;

        Try.Invoke(() =>
        {
            if (length == 0)
                Collection.Clear();

            else if (length > Collection.Count)
            {
                var j = length - Collection.Count;
                for (var i = 0; i < j; i++)
                    Collection.Add(CreateItem(null));
            }
            else
            {
                var j = Collection.Count - length;
                for (var i = Collection.Count - 1; i >= length; i--)
                    Collection.RemoveAt(i);
            }
        },
        e => Log.Write<MemberModel>(e));
    }

    //...

    ICommand insertAboveCommand;
    public ICommand InsertAboveCommand
        => insertAboveCommand
        ??= new RelayCommand<Type>(i => InsertAbove(SelectedIndex, i),
            i => Collection != null);

    ICommand insertBelowCommand;
    public ICommand InsertBelowCommand
        => insertBelowCommand
        ??= new RelayCommand<Type>(i => InsertBelow(SelectedIndex, i),
            i => Collection != null);

    ICommand moveDownCommand;
    public ICommand MoveDownCommand
        => moveDownCommand
        ??= new RelayCommand(() => Try.Invoke(() => Collection.MoveDown(SelectedIndex, true)),
            () => GetSelectedItem() != null);

    ICommand moveUpCommand;
    public ICommand MoveUpCommand
        => moveUpCommand
        ??= new RelayCommand(() => Try.Invoke(() => Collection.MoveUp(SelectedIndex, true)),
            () => GetSelectedItem() != null);

    ICommand removeCommand;
    public ICommand RemoveCommand
        => removeCommand
        ??= new RelayCommand(() => Try.Invoke(() => Collection.RemoveAt(SelectedIndex)),
            () => GetSelectedItem() != null);

    ICommand resetCommand;
    public ICommand ResetCommand
        => resetCommand
        ??= new RelayCommand(() => Try.Invoke(() => GetSelectedItem().If<IReset>(i => i.Reset())),
            () => GetSelectedItem() is IReset);
}