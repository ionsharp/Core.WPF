using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using Imagin.Core.Conversion;
using Imagin.Core.Data;
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
            typeof(FilePathAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(FolderPathAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(TokensAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ThumbnailAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SearchAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(PasswordAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(MultipleAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(AngleAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(UnitAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ProgressAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SelectedIndexAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(BulletsAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(HexadecimalAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SliderUpDownAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(SliderAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(UpDownAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(CommasAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(CollectionAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ButtonAttribute),
            new((i, j, k) => i.Style = j.GetType())
        },
        {
            typeof(ToggleButtonAttribute),
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
            typeof(CommandAttribute),
            new((i, j, k) =>
            {
                if (j.As<CommandAttribute>()?.CommandName is string commandName)
                    Try.Invoke(() => i.Command = i.Source.Instance.GetPropertyValue(commandName) as ICommand, e => Log.Write<MemberModel>(e));
            })
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
            typeof(ToolAttribute),
            new((i, j, k) => i.IsTool = j.As<ToolAttribute>() != null)
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

        Try.Invoke(() =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(i => CreateItem(e.NewStartingIndex, i));
                    break;

                case NotifyCollectionChangedAction.Move:
                    var item = SourceCollection[e.OldStartingIndex];
                    SourceCollection.RemoveAt(e.OldStartingIndex);
                    SourceCollection.Insert(e.NewStartingIndex, item);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems?.ForEach(i =>
                    {
                        Items[e.OldStartingIndex].Unsubscribe();
                        Items.RemoveAt(e.OldStartingIndex);
                    });
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (!ItemType.IsValueType && ItemType != typeof(string))
                    {
                        e.OldItems?.ForEach(i =>
                        {
                            Items[e.OldStartingIndex].Unsubscribe();
                            Items.RemoveAt(e.OldStartingIndex);
                        });
                        goto case NotifyCollectionChangedAction.Add;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Items.ForEach(i => i.Unsubscribe());
                    Items.Clear();
                    break;
            }
        },
        e => Log.Write<MemberModel>(e));
    }

    void OnValueChanging(object oldValue, object newValue)
    {
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
            case nameof(Value):
                ActualType = value?.GetType();
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
        var source = Value;
        if (source != null)
        {
            if (TemplateType == typeof(object) || GetTemplateType(Type) == typeof(object))
            {
                Members ??= new(Parent.Control, this, DepthIndex + 1);
                Members.Load(source);

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
            return;
            Items ??= new();
            Items.ForEach(i => i.Unsubscribe());
            Items.Clear();

            SourceCollection.ForEach(i => CreateItem(-1, i));
            collection.CollectionChanged -= OnCollectionChanged;
            collection.CollectionChanged += OnCollectionChanged;

            this.Changed(() => CollectionLength);
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

        if (Items != null)
        {
            return;
            Items.ForEach(i => i.Unsubscribe());
            Items.Clear();

            (SourceCollection as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
        }
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
}