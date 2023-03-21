using Imagin.Core.Analytics;
using Imagin.Core.Collections;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Log"), Image(SmallImages.Log), Serializable]
public class LogPanel : DataPanel
{
    public static readonly ResourceKey ImageTemplateKey = new();

    public static readonly ResourceKey MessageTemplateKey = new();

    public static readonly ResourceKey ResultTemplateKey = new();

    public static readonly ResourceKey TemplateKey = new();

    public static readonly ResourceKey TextStyleKey = new();

    public static readonly ResourceKey ToolTipKey = new();
        
    enum Category { Category0, Commands0, Group, Open, Sort, Text, View }

    [Serializable]
    public enum Views { Default, Details }

    #region Converters

    static IMultiValueConverter countConverter;
    public static IMultiValueConverter CountConverter => countConverter ??= new MultiConverter<string>(i =>
    {
        if (i.Values?.Length == 3)
        {
            if (i.Values[0] is ICollectionChanged a)
            {
                if (i.Values[2] is ResultTypes b)
                    return $"{a.Count<LogEntry>(i => i.Result.Type == b)}";
            }
        }
        return $"0";
    });

    static IMultiValueConverter visibilityConverter;
    public static IMultiValueConverter VisibilityConverter => visibilityConverter ??= new MultiConverter<Visibility>(i =>
    {
        if (i.Values?.Length >= 3)
        {
            if (i.Values[0] is LogEntry logEntry)
            {
                if (i.Values[1] is ResultTypes filterType)
                {
                    if (i.Values[2] is ResultLevel filterLevel)
                    {
                        if (filterType == ResultTypes.None)
                            return Visibility.Collapsed;

                        if (logEntry.Result.Type != ResultTypes.All)
                        {
                            if (!filterType.HasFlag(logEntry.Result.Type))
                                return Visibility.Collapsed;
                        }
                        if (!filterLevel.HasFlag(logEntry.Level))
                            return Visibility.Collapsed;

                        if (i.Values.Length >= 4)
                        {
                            if (i.Values[3] is string search)
                            {
                                if (!search.NullOrEmpty())
                                {
                                    if (!logEntry.Result.Text.ToLower().StartsWith(search.ToLower()))
                                        return Visibility.Collapsed;
                                }
                            }
                        }
                    }
                }
            }
        }
        return Visibility.Visible;
    });

    #endregion

    #region Properties

    [Hide]
    public string ErrorCount => $"Errors ({Data.Count<LogEntry>(i => i.Result is Error)})";

    [Hide]
    public string MessageCount => $"Messages ({Data.Count<LogEntry>(i => i.Result is Message)})";

    [Hide]
    public string SuccessCount => $"Success ({Data.Count<LogEntry>(i => i.Result is Success)})";

    [Hide]
    public string WarningCount => $"Warnings ({Data.Count<LogEntry>(i => i.Result is Warning)})";

    ///

    [Hide]
    public ResultTypes Filter
    {
        get
        {
            var result = ResultTypes.None;
            if (FilterError)
                result = result.AddFlag(ResultTypes.Error);

            if (FilterMessage)
                result = result.AddFlag(ResultTypes.Message);

            if (FilterSuccess)
                result = result.AddFlag(ResultTypes.Success);

            if (FilterWarning)
                result = result.AddFlag(ResultTypes.Warning);

            return result;
        }
    }

    [Description("Show with level")]
    [Header, Pin(Pin.AboveOrLeft), HideName, Horizontal, Index(-1), Name("Level")]
    public ResultLevel FilterLevel { get => Get(ResultLevel.Normal); set => Set(value); }

    [Description("Show errors.")]
    [Pin(Pin.AboveOrLeft), HideName, Image(SmallImages.XRound), ImageColor(ThemeKeys.Red), Name("Show errors"), Style(BooleanStyle.Button)]
    [Trigger(nameof(MemberModel.Content), nameof(ErrorCount)), Header]
    public bool FilterError { get => Get(true); set => Set(value); }

    [Description("Show messages.")]
    [Pin(Pin.AboveOrLeft), HideName, Image(SmallImages.Info), ImageColor(ThemeKeys.Blue), Name("Show messages"), Style(BooleanStyle.Button)]
    [Trigger(nameof(MemberModel.Content), nameof(MessageCount)), Header]
    public bool FilterMessage { get => Get(true); set => Set(value); }

    [Description("Show successes.")]
    [Pin(Pin.AboveOrLeft), HideName, Image(SmallImages.CheckmarkRound), ImageColor(ThemeKeys.Green), Name("Show successes"), Style(BooleanStyle.Button)]
    [Trigger(nameof(MemberModel.Content), nameof(SuccessCount)), Header]
    public bool FilterSuccess { get => Get(true); set => Set(value); }

    [Description("Show warnings.")]
    [Pin(Pin.AboveOrLeft), HideName, Image(SmallImages.Warning), ImageColor(ThemeKeys.Orange), Name("Show warnings"), Style(BooleanStyle.Button)]
    [Trigger(nameof(MemberModel.Content), nameof(WarningCount)), Header]
    public bool FilterWarning { get => Get(true); set => Set(value); }

    [Hide]
    public override IList GroupNames => new Collections.ObjectModel.StringCollection()
    {
        "None",
        nameof(LogEntry.Added),
        nameof(LogEntry.Level),
        nameof(LogEntry.Member),
        nameof(LogEntry.Result.Text),
        nameof(LogEntry.Result.Type),
        nameof(LogEntry.Sender),
    };

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Log);

    [Pin(Pin.BelowOrRight), Header, HideName, Image(SmallImages.Search), Index(int.MinValue), Placeholder("Search...")]
    [StringStyle(StringStyle.Search, EnterCommand = nameof(SearchCommand), Suggestions = nameof(SearchHistory), SuggestionCommand = nameof(SearchSuggestionCommand))]
    [UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged), Width(180)]
    public string Search { get => Get(string.Empty); set => Set(value); }

    [Hide]
    public Collections.ObjectModel.StringCollection SearchHistory { get => Get<Collections.ObjectModel.StringCollection>(new()); private set => Set(value); }

    [Hide]
    public override IList SortNames => new Collections.ObjectModel.StringCollection()
    {
        nameof(LogEntry.Added),
        nameof(LogEntry.Level),
        nameof(LogEntry.Member),
        nameof(LogEntry.Result.Text),
        nameof(LogEntry.Result.Type),
        nameof(LogEntry.Sender),
    };

    [Hide]
    public string Text { get => Get<string>(null); set => Set(value); }

    [Category(Category.Text), HideName, Image(SmallImages.ArrowDownLeft), Index(int.MaxValue - 1), Header, Style(BooleanStyle.Button)]
    public bool TextWrap { get => Get(false); set => Set(value); }

    [Hide]
    public override string TitleKey => "Log";

    [Category(Category.View), Option]
    public Views View { get => Get(Views.Details); set => Set(value); }

    #endregion

    #region LogPanel

    public LogPanel(ICollectionChanged input) : base(input) { }

    #endregion

    #region Methods

    void OnLogChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateText();

    ///

    string Format(LogEntry i)
    {
        var result = $"{Bullet.ToString(Data.As<IList>().IndexOf(i) + 1)} ";
        if (i.Result is Error parent)
        {
            result += $"{parent.Name} ({i.Sender}.{i.Member}, Line {i.Line}): {parent.Text}";
            result += $"{parent.StackTrace}";
            if (parent.Inner is Error firstChild)
            {
                result += $" {firstChild.Name}: {firstChild.Text}";
                result += $" {firstChild.StackTrace}";
                if (firstChild.Inner is Error secondChild)
                {
                    result += $" {secondChild.Name}: {secondChild.Text}";
                    result += $" {secondChild.StackTrace}";
                }
            }
        }
        else result += i.Result.Text;
        return result;
    }

    void UpdateSearch(string input)
    {
        if (!input.NullOrEmpty())
        {
            if (SearchHistory.Contains(input))
                SearchHistory.Remove(input);

            SearchHistory.Insert(0, input);
        }
    }

    string GetText() => Data.Select<object, LogEntry>(i => i as LogEntry).Where(i => Filter.HasFlag(i.Result.Type) && (Search.NullOrEmpty() || i.Result.Text.ToLower().StartsWith(Search.ToLower())))?.ToString("\n", Format);

    void UpdateText()
    {
        if (View == Views.Default)
        {
            Text = Data?.Count > 0
            ? GetText()
            : string.Empty;
        }
    }

    ///

    void Subscribe(ICollectionChanged input)
    {
        Unsubscribe(input);
        if (input != null)
        {
            UpdateText();
            input.CollectionChanged += OnLogChanged;
        }
    }

    void Unsubscribe(ICollectionChanged input)
    {
        if (input != null)
        {
            Text = string.Empty;
            input.CollectionChanged -= OnLogChanged;
        }
    }

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Data):
                Unsubscribe(Data);
                Subscribe(Data);
                break;

            case nameof(Count):
                Update(() => ErrorCount);
                Update(() => MessageCount);
                Update(() => SuccessCount);
                Update(() => WarningCount);
                break;

            case nameof(Filter):
                UpdateText();
                break;

            case nameof(FilterError):
            case nameof(FilterMessage):
            case nameof(FilterSuccess):
            case nameof(FilterWarning):
                Update(() => Filter);
                break;

            case nameof(View):
                UpdateText();
                break;
        }
    }

    ///

    [HeaderOption, Image(SmallImages.Open), Show]
    public void Open() => OpenCommand.Execute();

    [HeaderOption, Image(SmallImages.Save), Show]
    public void Save() => SaveCommand.Execute();

    #endregion

    #region Commands

    [Category(Category.Commands0), Name("Clear")]
    [Header, Image(SmallImages.XRound), Index(2)]
    new public ICommand ClearCommand => base.ClearCommand;

    ICommand cutCommand;
    [Category(Category.Commands0)]
    [Name("Cut")]
    [Image(SmallImages.Cut)]
    [Index(0)]
    [Header]
    public ICommand CutCommand
        => cutCommand ??= new RelayCommand(() => { CopyCommand.Execute(); ClearCommand.Execute(); }, () => Data?.Count > 0);

    ICommand cutSingleCommand;
    [Hide]
    public ICommand CutSingleCommand
        => cutSingleCommand ??= new RelayCommand<LogEntry>(i => { CopySingleCommand.Execute(i); Data.Remove(i); }, i => i != null);

    ICommand copyCommand;
    [Category(Category.Commands0)]
    [Name("Copy")]
    [Image(SmallImages.Copy)]
    [Index(1)]
    [Header]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
    {
        switch (View)
        {
            case Views.Details:
                Clipboard.SetText(GetText());
                break;
            case Views.Default:
                Clipboard.SetText(Text);
                break;
        }
    },
    () => Data?.Count > 0);

    ICommand copySingleCommand;
    [Hide]
    public ICommand CopySingleCommand
        => copySingleCommand ??= new RelayCommand<LogEntry>(i => Clipboard.SetText(Format(i)), i => i != null);

    [field: NonSerialized]
    ICommand openCommand;
    [Hide]
    public virtual ICommand OpenCommand => openCommand ??= new RelayCommand(() => Storage.File.Long.Open(Current.Get<Config.BaseApplication>().Log.FilePath).If(i => { if (i is Error e) { Dialog.ShowError("Open log", e, Controls.Buttons.Ok); } }));

    [Hide]
    public override ICommand RefreshCommand => base.RefreshCommand;

    [field: NonSerialized]
    ICommand saveCommand;
    [Hide]
    public virtual ICommand SaveCommand => saveCommand ??= new RelayCommand(() => Current.Get<ILog>().Save(), () => Current.Get<ILog>() != null);

    ICommand searchCommand;
    [Hide]
    public ICommand SearchCommand => searchCommand ??= new RelayCommand<string>(i =>
    {
        UpdateSearch(i);
        UpdateText();
    },
    i => !i.NullOrEmpty());

    ICommand searchSuggestionCommand;
    [Hide]
    public ICommand SearchSuggestionCommand => searchSuggestionCommand ??= new RelayCommand<string>(i =>
    {
        UpdateSearch(i);
        UpdateText();
    });

    #endregion
}