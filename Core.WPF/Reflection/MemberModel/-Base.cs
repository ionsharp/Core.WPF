using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Numerics;
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

namespace Imagin.Core.Reflection;

public abstract partial class MemberModel : BaseNamable, IComparable
{
    readonly MemberAttributes Attributes;

    readonly Handle Handle = false;

    System.Timers.Timer timer;

    #region Attributes

    /// <summary>[Attribute type] => [Member (i), First attribute (j), All attributes (k)]</summary>
    readonly MemberAttributeHandler AttributeHandler = new()
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
            typeof(AssignAttribute),
            new((i, j, k) =>
            {
                i.Assignable = true;

                var a = j.As<AssignAttribute>();
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
            typeof(CaptionAttribute),
            new((i, j, k) => i.Caption = j.As<CaptionAttribute>().Caption)
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
            typeof(CopyAttribute),
            new((i, j, k) => i.Assignable = true)
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
            typeof(LockAttribute),
            new((i, j, k) => i.IsLockable = j.As<LockAttribute>().Locked)
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

    public MemberModel Parent { get; private set; }

    public MemberSource Source { get; private set; }

    //...

    public abstract bool IsWritable { get; }

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

    public virtual IList Collection => Value as IList;

    public int CollectionLength
    {
        get => Collection.Count;
        set
        {
            if (value == Collection?.Count)
                return;

            Try.Invoke(() =>
            {
                if (value == 0)
                    Collection.Clear();

                else if (value > Collection.Count)
                {
                    var j = value - Collection.Count;
                    for (var i = 0; i < j; i++)
                        Collection.Add(CreateItem());
                }
                else
                {
                    var j = Collection.Count - value;
                    for (var i = Collection.Count - 1; i >= value; i--)
                        Collection.RemoveAt(i);
                }
            },
            e => Log.Write<MemberModel>(e));
            this.Changed(() => CollectionLength);
        }
    }

    readonly Type[] CollectionTypes = XArray.New(typeof(Array), typeof(IEnumerable), typeof(IList), typeof(INotifyCollectionChanged));

    public bool IsCollectionType => TemplateType.EqualsAny(CollectionTypes) || GetTemplateType(Type).EqualsAny(CollectionTypes);

    //...

    readonly Type[] ReferenceTypes = XArray.New(typeof(Array), typeof(IEnumerable), typeof(IList), typeof(INotifyCollectionChanged), typeof(object));

    public bool IsReferenceType => TemplateType.EqualsAny(ReferenceTypes) || GetTemplateType(Type).EqualsAny(ReferenceTypes);

    //...

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

    //...

    object defaultValue = null;
    public object DefaultValue
    {
        get => defaultValue;
        private set => this.Change(ref defaultValue, value);
    }

    dynamic value = default;
    public object Value
    {
        get => value;
        set => OnValueChanging(this.value, value);
    }

    //...

    #region Height/Width

    double height = double.NaN;
    public double Height
    {
        get => height;
        private set => this.Change(ref height, value);
    }

    double width = double.NaN;
    public double Width
    {
        get => width;
        private set => this.Change(ref width, value);
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

    #endregion

    #region Is

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

    bool isVisible = true;
    public bool IsVisible
    {
        get => isVisible;
        set => this.Change(ref isVisible, value);
    }

    #endregion

    #region Left/RightText

    string leftText = null;
    public string LeftText
    {
        get => leftText;
        set => this.Change(ref leftText, value);
    }

    string rightText = null;
    public string RightText
    {
        get => rightText;
        set => this.Change(ref rightText, value);
    }

    #endregion

    #region Other

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
        set => this.Change(ref format, value);
    }

    System.Windows.Media.Color[] gradient = null;
    public System.Windows.Media.Color[] Gradient
    {
        get => gradient;
        set => this.Change(ref gradient, value);
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

    int index = 0;
    public int Index
    {
        get => index;
        private set => this.Change(ref index, value);
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

    object itemClones = null;
    public object ItemClones
    {
        get => itemClones;
        set => this.Change(ref itemClones, value);
    }

    bool label = true;
    public bool Label
    {
        get => label;
        private set => this.Change(ref label, value);
    }

    bool localize = true;
    public bool Localize
    {
        get => localize;
        private set => this.Change(ref localize, value);
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

    int selectedIndex = -1;
    public int SelectedIndex
    {
        get => selectedIndex;
        set => this.Change(ref selectedIndex, value);
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

    #endregion

    #region Range

    static readonly Dictionary<Type, Func<object>> Increments = new()
    {
        { typeof(byte),     () => (byte)1     },
        { typeof(decimal),  () => (decimal)1  },
        { typeof(double),   () => (double)1   },
        { typeof(short),    () => (short)1    },
        { typeof(int),      () => (int)1      },
        { typeof(long),     () => (long)1     },
        { typeof(float),    () => (float)1    },
        { typeof(TimeSpan), () => 1.Seconds() },
        { typeof(UDouble),  () => (UDouble)1  },
        { typeof(ushort),   () => (ushort)1   },
        { typeof(uint),     () => (uint)1     },
        { typeof(ulong),    () => (ulong)1    }
    };

    static readonly Dictionary<Type, Func<object>> Maximums = new()
    {
        { typeof(byte),     () => byte.MaxValue       },
        { typeof(decimal),  () => decimal.MaxValue    },
        { typeof(double),   () => double.MaxValue     },
        { typeof(short),    () => short.MaxValue      },
        { typeof(int),      () => int.MaxValue        },
        { typeof(long),     () => long.MaxValue       },
        { typeof(float),    () => float.MaxValue      },
        { typeof(TimeSpan), () => TimeSpan.MaxValue   },
        { typeof(UDouble),  () => UDouble.MaxValue    },
        { typeof(ushort),   () => ushort.MaxValue     },
        { typeof(uint),     () => uint.MaxValue       },
        { typeof(ulong),    () => ulong.MaxValue      },
    };

    static readonly Dictionary<Type, Func<object>> Minimums = new()
    {
        { typeof(byte),     () => byte.MinValue       },
        { typeof(decimal),  () => decimal.MinValue    },
        { typeof(double),   () => double.MinValue     },
        { typeof(short),    () => short.MinValue      },
        { typeof(int),      () => int.MinValue        },
        { typeof(long),     () => long.MinValue       },
        { typeof(float),    () => float.MinValue      },
        { typeof(TimeSpan), () => TimeSpan.MinValue   },
        { typeof(UDouble),  () => UDouble.MinValue    },
        { typeof(ushort),   () => ushort.MinValue     },
        { typeof(uint),     () => uint.MinValue       },
        { typeof(ulong),    () => ulong.MinValue      },
    };

    object DefaultIncrement 
        => Type != null && Increments.ContainsKey(Type) 
        ? Increments[Type]() : null;

    object DefaultMaximum 
        => Type != null && Maximums.ContainsKey(Type) 
        ? Maximums[Type]() : null;

    object DefaultMinimum 
        => Type != null && Minimums.ContainsKey(Type) 
        ? Minimums[Type]() : null;

    dynamic increment = default;
    public object Increment
    {
        get => increment;
        set => this.Change(ref increment, value);
    }

    dynamic maximum = default;
    public object Maximum
    {
        get => maximum;
        set => this.Change(ref maximum, value);
    }

    dynamic minimum = default;
    public object Minimum
    {
        get => minimum;
        set => this.Change(ref minimum, value);
    }

    #endregion

    #endregion

    #region MemberModel

    protected MemberModel(MemberModel parent, MemberSource source, MemberInfo member, MemberAttributes attributes, int depth)
    {
        Parent = parent; Source = source; Member = member; Attributes = attributes; Depth = depth;

        //Try.Invoke(() => Dispatch.Invoke(() => Category = Parent?.Control?.DefaultCategoryName));

        DisplayName 
            = Name;
        Increment 
            = DefaultIncrement; Maximum = DefaultMaximum; Minimum = DefaultMinimum;
        Orientation 
            = DeclaringType.GetAttribute<HorizontalAttribute>() != null ? System.Windows.Controls.Orientation.Horizontal : Orientation;
        
        Attributes?.Apply(this, AttributeHandler);
        IsReadOnly = !IsWritable || IsReadOnly || Parent?.IsReadOnly == true;

        UpdateValue();
        RefreshHard();
    }

    #endregion

    #region Methods

    #region Get/HasAttribute

    public T GetAttribute<T>() where T : Attribute => Attributes?.GetFirst<T>();

    public IEnumerable<T> GetAttributes<T>() where T : Attribute => Attributes?.GetAll<T>();

    public bool HasAttribute<T>() where T : Attribute => Attributes?.GetFirst<T>() != null;
 
    #endregion

    #region GetValue

    public virtual object GetValue()
    {
        object result = null;
        Try.Invoke(() => result = GetValue(Source.Instance)); //Dispatch.Invoke(() => );
        return result;
    }

    protected abstract object GetValue(object input);

    #endregion

    #region SetValue

    protected void SetValue(object value) => Handle.Invoke(() => SetValue(Source.Instance, value));

    protected abstract void SetValue(object source, object value);

    #endregion

    #region Other

    int IComparable.CompareTo(object a)
    {
        if (a is MemberModel b)
            return Index.CompareTo(b.Index);

        return 0;
    }

    //...

    object CreateItem(Type type = null)
    {
        type ??= ItemType;
        return type == typeof(string) ? "" : type.Create<object>();
    }

    void EachParent(MemberModel input, Action<MemberModel> action)
    {
        MemberModel result = input;
        while (result != null)
        {
            result = result.Parent;
            result.If(action);
        }
    }

    object GetSelectedItem()
        => Collection is IList i && i.Count > SelectedIndex && SelectedIndex >= 0 ? i[SelectedIndex] : null;

    //...

    void InsertAbove(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i => Collection.Insert(index == -1 ? 0 : index, i)), e => Log.Write<MemberModel>(e));

    void InsertBelow(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i =>
        {
            var newIndex = index + 1;
            if (index != -1 && newIndex < Collection.Count)
                Collection.Insert(newIndex, i);

            else Collection.Add(i);
        }), e => Log.Write<MemberModel>(e));

    //...

    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.Changed(() => CollectionLength);
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
                    object result = null;
                    Try.Invoke(() =>
                    {
                        result = sender.GetPropertyValue(e.PropertyName);
                        this.SetPropertyValue(i.TargetName, result);
                    },
                    e => Log.Write<MemberModel>(e));
                }
            }
        }
    }

    void OnUpdate(object sender, System.Timers.ElapsedEventArgs e) => UpdateValue();

    //...

    void OnValueChanging(object oldValue, object newValue)
    {
        if (isReadOnly) return;

        Unsubscribe();
        SetValue(newValue);

        UpdateValue(newValue);
        switch (Source.DataType)
        {
            case Types.Value:
                Parent.If(i => i.SetValue(i.Source.Instance, Source.Instance));
                break;
        }

        OnValueChanged(newValue);

        Refresh(oldValue, newValue);
        Subscribe();
    }

    protected virtual void OnValueChanged(object input) { }

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

    #endregion

    #region Refresh

    internal void Refresh(object oldValue, object newValue)
    {
        var oldType = oldValue?.GetType();
        var newType = newValue?.GetType();

        if (oldType != newType)
            RefreshHard();

        else if (oldType == newType)
            RefreshSoft();
    }

    internal void RefreshSafe() => Handle.SafeInvoke(() => Refresh(Value, GetValue()));

    //...

    internal void RefreshHard()
    {
        UpdateValue();

        var newValue = Value;
        if (newValue != null && IsReferenceType)
        {
            Members ??= new(this, Depth + 1);
            Members.Load(newValue);

            if (SortedMembers == null)
            {
                var categorize = GetAttribute<CategorizeAttribute>()?.Categorize == true;

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

    //...

    internal void RefreshSoft()
    {
        UpdateValue();
        Members?.Refresh(GetValue());
    }

    internal void RefreshSoftSafe() => Handle.SafeInvoke(RefreshSoft);

    #endregion

    #region Subscribe, Unsubscribe

    public virtual void Subscribe()
    {
        if (Source.Instance is INotifyPropertyChanged notify)
        {
            var triggers = GetAttributes<TriggerAttribute>();
            if (triggers?.Count() > 0)
            {
                foreach (var trigger in triggers)
                    OnTrigger(notify, new(trigger.SourceName));
            }

            notify.PropertyChanged -= OnTrigger;
            notify.PropertyChanged += OnTrigger;
        }

        if (GetAttribute<UpdateAttribute>() is UpdateAttribute update)
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
        if (Source.Instance is INotifyPropertyChanged notify)
            notify.PropertyChanged -= OnTrigger;

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

    #endregion

    #endregion

    #region Commands

    ICommand insertAboveCommand;
    public ICommand InsertAboveCommand => insertAboveCommand ??= new RelayCommand<object>(i =>
    {
        if (i is Type j)
            InsertAbove(SelectedIndex, j);

        else if (i is ICloneable k)
            Collection.Add(k.Clone());
    },
    i => Collection != null && (i is Type || i is ICloneable));

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

    #endregion
}

public abstract class MemberModel<T> : MemberModel where T : MemberInfo
{
    new public T Member => (T)base.Member;

    protected MemberModel(MemberModel parent, MemberSource source, MemberInfo member, MemberAttributes attributes, int depthIndex) : base(parent, source, member, attributes, depthIndex) { }
}