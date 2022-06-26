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

namespace Imagin.Core.Models
{
    [Serializable]
    public class LogPanel : DataPanel
    {
        public static readonly ResourceKey TemplateKey = new();

        enum Category { Category0, Commands0, Group, Sort, Text, View }

        [Serializable]
        public enum Views { Rows, Text }

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

        [Hidden]
        public string ErrorCount 
            => $"Errors ({Data.Count<LogEntry>(i => i.Result is Error)})";

        [Hidden]
        public string MessageCount 
            => $"Messages ({Data.Count<LogEntry>(i => i.Result is Message)})";

        [Hidden]
        public string SuccessCount 
            => $"Success ({Data.Count<LogEntry>(i => i.Result is Success)})";

        [Hidden]
        public string WarningCount 
            => $"Warnings ({Data.Count<LogEntry>(i => i.Result is Warning)})";

        //...

        [Hidden]
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

        bool filterError = true;
        [Button, Feature]
        [Label(false)]
        [Image(Images.XRound, ThemeKeys.ResultError)]
        [Index(-5)]
        [Trigger(nameof(MemberModel.Content), nameof(ErrorCount))]
        [Tool]
        public bool FilterError
        {
            get => filterError;
            set => this.Change(ref filterError, value);
        }

        bool filterMessage = true;
        [Button, Feature]
        [Label(false)]
        [Image(Images.Info, ThemeKeys.ResultMessage)]
        [Index(-4)]
        [Trigger(nameof(MemberModel.Content), nameof(MessageCount))]
        [Tool]
        public bool FilterMessage
        {
            get => filterMessage;
            set => this.Change(ref filterMessage, value);
        }

        bool filterSuccess = true;
        [Button, Feature]
        [Label(false)]
        [Image(Images.CheckmarkRound, ThemeKeys.ResultSuccess)]
        [Index(-3)]
        [Trigger(nameof(MemberModel.Content), nameof(SuccessCount))]
        [Tool]
        public bool FilterSuccess
        {
            get => filterSuccess;
            set => this.Change(ref filterSuccess, value);
        }

        bool filterWarning = true;
        [Button, Feature]
        [Label(false)]
        [Image(Images.Warning, ThemeKeys.ResultWarning)]
        [Index(-2)]
        [Trigger(nameof(MemberModel.Content), nameof(WarningCount))]
        [Tool]
        public bool FilterWarning
        {
            get => filterWarning;
            set => this.Change(ref filterWarning, value);
        }

        [Hidden]
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

        [Hidden]
        public override Uri Icon => Resources.InternalImage(Images.Log);

        string search = string.Empty;
        [Command(nameof(SearchCommand))]
        [Label(false)]
        [Feature(AboveBelow.Below)]
        [Hidden, Image(Images.Search)]
        [Index(int.MaxValue)]
        [Search, Setter(nameof(MemberModel.Placeholder), "Search...")]
        [Suggestions(nameof(SearchHistory), nameof(SearchSuggestionCommand))]
        [Tool]
        [UpdateSourceTrigger(UpdateSourceTrigger.LostFocus)]
        [Width(180)]
        public string Search
        {
            get => search;
            set => this.Change(ref search, value);
        }

        Collections.ObjectModel.StringCollection searchHistory = new();
        [Hidden]
        public Collections.ObjectModel.StringCollection SearchHistory
        {
            get => searchHistory;
            private set => this.Change(ref searchHistory, value);
        }

        [Hidden]
        public override IList SortNames => new Collections.ObjectModel.StringCollection()
        {
            nameof(LogEntry.Added),
            nameof(LogEntry.Level),
            nameof(LogEntry.Member),
            nameof(LogEntry.Result.Text),
            nameof(LogEntry.Result.Type),
            nameof(LogEntry.Sender),
        };

        string text = null;
        [Hidden]
        public string Text
        {
            get => text;
            set => this.Change(ref text, value);
        }

        bool textWrap = false;
        [Button]
        [Category(Category.Text)]
        [Label(false)]
        [Image(Images.ArrowDownLeft)]
        [Index(int.MaxValue - 1)]
        [Tool]
        public bool TextWrap
        {
            get => textWrap;
            set => this.Change(ref textWrap, value);
        }

        [Hidden]
        public override string TitleKey => "Log";

        Views view = Views.Text;
        [Category(Category.View)]
        [Option]
        public Views View
        {
            get => view;
            set => this.Change(ref view, value);
        }

        #endregion

        #region LogPanel

        public LogPanel(ICollectionChanged input) : base(input) { }

        #endregion

        #region Methods

        void OnLogChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateText();

        //...
        
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
            if (View == Views.Text)
            {
                Text = Data?.Count > 0
                ? GetText()
                : string.Empty;
            }
        }

        //...

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

        //...

        public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Data):
                    Unsubscribe(Data);
                    Subscribe(Data);
                    break;

                case nameof(Count):
                    this.Changed(() => ErrorCount);
                    this.Changed(() => MessageCount);
                    this.Changed(() => SuccessCount);
                    this.Changed(() => WarningCount);
                    break;

                case nameof(Filter):
                    UpdateText();
                    break;

                case nameof(FilterError):
                case nameof(FilterMessage):
                case nameof(FilterSuccess):
                case nameof(FilterWarning):
                    this.Changed(() => Filter);
                    break;

                case nameof(View):
                    UpdateText();
                    break;
            }
        }

        [Category(Category.Commands0)]
        [DisplayName("Clear")]
        [Image(Images.Trash)]
        [Index(2)]
        [Tool]
        new public ICommand ClearCommand => base.ClearCommand;

        ICommand cutCommand;
        [Category(Category.Commands0)]
        [DisplayName("Cut")]
        [Image(Images.Cut)]
        [Index(0)]
        [Tool]
        public ICommand CutCommand
            => cutCommand ??= new RelayCommand(() => { CopyCommand.Execute(); ClearCommand.Execute(); }, () => Data?.Count > 0);

        ICommand cutSingleCommand;
        [Hidden]
        public ICommand CutSingleCommand
            => cutSingleCommand ??= new RelayCommand<LogEntry>(i => { CopySingleCommand.Execute(i); Data.Remove(i); }, i => i != null);

        ICommand copyCommand;
        [Category(Category.Commands0)]
        [DisplayName("Copy")]
        [Image(Images.Copy)]
        [Index(1)]
        [Tool]
        public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
        {
            switch (View)
            {
                case Views.Rows:
                    Clipboard.SetText(GetText());
                    break;
                case Views.Text:
                    Clipboard.SetText(Text);
                    break;
            }
        },
        () => Data?.Count > 0);

        ICommand copySingleCommand;
        [Hidden]
        public ICommand CopySingleCommand
            => copySingleCommand ??= new RelayCommand<LogEntry>(i => Clipboard.SetText(Format(i)), i => i != null);

        [Hidden]
        public override ICommand RefreshCommand => base.RefreshCommand;

        ICommand searchCommand;
        [Hidden]
        public ICommand SearchCommand => searchCommand ??= new RelayCommand<string>(i =>
        {
            UpdateSearch(i);
            UpdateText();
        },
        i => !i.NullOrEmpty());

        ICommand searchSuggestionCommand;
        [Hidden]
        public ICommand SearchSuggestionCommand => searchSuggestionCommand ??= new RelayCommand<string>(i =>
        {
            UpdateSearch(i);
            UpdateText();
        });

        #endregion
    }
}