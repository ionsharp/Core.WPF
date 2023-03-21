using Imagin.Core.Collections.Generic;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using Imagin.Core.Text;
using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

public class FindResult : Base
{
    public int Column { get => Get(0); set => Set(value); }

    public int Index { get => Get(0); set => Set(value); }

    public int Line { get => Get(0); set => Set(value); }

    public IFind File { get => Get<IFind>(); set => Set(value); }

    public string Text { get => Get(""); set => Set(value); }

    public FindResult(IFind target, int index, string text, int line, int column) : base()
    {
        File
            = target;
        Index
            = index;
        Text
            = text;
        Line
            = line;
        Column
            = column;
    }
}

public class FindResultCollection : ObservableCollection<FindResult>
{
    public string FindText { get; private set; }

    public FindResultCollection(string findText)
    {
        FindText = findText;
    }
}

public class FindResultsPanel : DataPanel
{
    public static readonly ResourceKey TemplateKey = new();

    #region (IMultiValueConverter) VisibilityConverter

    public static readonly IMultiValueConverter VisibilityConverter = new MultiConverter<Visibility>(i =>
    {
        if (i.Values?.Length >= 3)
        {
            if (i.Values[0] is FindResultsPanel panel)
            {
                if (i.Values[1] is FindResult result)
                {
                    if (i.Values[2] is FindSource source)
                    {
                        if (source == FindSource.CurrentDocument)
                        {
                            var activeDocument = panel.ViewModel.ActiveContent as Document;
                            if (!ReferenceEquals(result.File, activeDocument))
                                return Visibility.Collapsed;
                        }
                        if (i.Values.Length >= 3)
                        {
                            if (i.Values[2] is string search)
                            {
                                if (search.Length > 0)
                                {
                                    if (!result.Text.ToLower().Contains(search.ToLower()))
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

    [HideName]
    [Header]
    public FindSource FilterSource { get => Get<FindSource>(); set => Set(value); }

    [Hide]
    public override IList GroupNames => new StringCollection()
    {
        "None",
        nameof(FindResult.File)
    };

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Search);

    [Header]
    public bool KeepResults { get => Get(false); set => Set(value); }

    [Hide, XmlIgnore]
    public FindResultCollection Results { get => Get<FindResultCollection>(null, false); set => Set(value, false); }

    [Pin(Pin.BelowOrRight), Header, HideName, Image(SmallImages.Search), Index(int.MaxValue), Placeholder("Search...")]
    [StringStyle(StringStyle.Search, EnterCommand = nameof(SearchCommand), Suggestions = nameof(SearchHistory), SuggestionCommand = nameof(SearchSuggestionCommand))]
    [UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Width(300)]
    public string Search { get => Get(""); set => Set(value); }

    [Hide]
    public StringCollection SearchHistory { get => Get(new StringCollection()); set => Set(value); }

    [Hide]
    public override IList SortNames => new StringCollection()
    {
        nameof(FindResult.File),
        nameof(FindResult.Line),
        nameof(FindResult.Text)
    };

    [Header, HideName, Image(SmallImages.ArrowDownLeft), Index(int.MaxValue - 1), Style(BooleanStyle.Button)]
    public bool TextWrap { get => Get(true); set => Set(value); }

    [Hide]
    public override string TitleKey => "Find";

    [Hide]
    public override string TitleSuffix => $" \"{Results.FindText}\"";

    #endregion

    #region FindResultsPanel

    public FindResultsPanel() : base() { }

    public FindResultsPanel(FindResultCollection results) : this() => Results = results;

    #endregion

    #region Methods

    void UpdateSearch(string input)
    {
        if (!input.NullOrEmpty())
        {
            if (SearchHistory.Contains(input))
                SearchHistory.Remove(input);

            SearchHistory.Insert(0, input);
        }
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Results))
        {
            Data = Results;
            Update(() => Title);
        }
    }

    #endregion

    #region Commands

    [Hide]
    public override ICommand ClearCommand => base.ClearCommand;

    ICommand copyCommand;
    [Name("Copy")]
    [Image(SmallImages.Copy)]
    [Header]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
    {
        var result = new StringBuilder();
        foreach (var i in Results)
            result.AppendLine($"{i.Line}: {i.Text}");

        Clipboard.SetText(result.ToString());
    }, 
    () => Results.Count > 0);

    ICommand openResultCommand;
    [Hide]
    public ICommand OpenResultCommand => openResultCommand ??= new RelayCommand<FindResult>(i =>
    {
        ViewModel.ActiveContent = i.File as Content;
        //Scroll to and select matched text
    },
    i => i != null);

    ICommand searchCommand;
    [Hide]
    public ICommand SearchCommand => searchCommand ??= new RelayCommand(() => UpdateSearch(Search), () => !Search.NullOrEmpty());

    ICommand searchSuggestionCommand;
    [Hide]
    public ICommand SearchSuggestionCommand => searchSuggestionCommand ??= new RelayCommand<string>(i => UpdateSearch(i));

    #endregion
}