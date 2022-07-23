using Imagin.Core;
using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Demo;

public class MainViewModel : DockViewModel<MainWindow, Document>
{
    #region Properties

    ColorDocument color;
    public ColorDocument Color
    {
        get => color;
        set => this.Change(ref color, value);
    }

    string downloadWindowSource = "";
    public string DownloadWindowSource
    {
        get => downloadWindowSource;
        set => this.Change(ref downloadWindowSource, value);
    }

    bool flipContent;
    public bool FlipContent
    {
        get => flipContent;
        set => this.Change(ref flipContent, value);
    }
        
    double gridLineZoom;
    public double GridLineZoom
    {
        get => gridLineZoom;
        set => this.Change(ref gridLineZoom, value);
    }

    Imagin.Core.Storage.ItemCollection items = new(Environment.SpecialFolder.UserProfile.Path(), Filter.Default);
    public Imagin.Core.Storage.ItemCollection Items
    {
        get => items;
        set => this.Change(ref items, value);
    }

    object propertyGridSource;
    public object MemberGridSource
    {
        get => propertyGridSource;
        set => this.Change(ref propertyGridSource, value);
    }

    string resultText;
    public string ResultText
    {
        get => resultText;
        set => this.Change(ref resultText, value);
    }

    ResultTypes resultType;
    public ResultTypes ResultType
    {
        get => resultType;
        set => this.Change(ref resultType, value);
    }

    Notification swipeButtonNotification = new("This is a title", new Message("This is a simple message.This is where your message goes."), TimeSpan.Zero);
    public Notification SwipeButtonNotification
    {
        get => swipeButtonNotification;
        set => this.Change(ref swipeButtonNotification, value);
    }

    StringCollection textBoxSuggestions = new();
    public StringCollection TextBoxSuggestions
    {
        get => textBoxSuggestions;
        set => this.Change(ref textBoxSuggestions, value);
    }

    #endregion

    #region MainViewModel

    public MainViewModel() : base()
    {
        Color = new(Colors.White, null);

        if (Get.Current<App>().Notifications.Count == 0)
        {
            for (var i = 0; i < 5; i++)
                Get.Current<App>().Notifications.Add(new("This is a title", new Message("This is a message."), TimeSpan.Zero));
        }

        _ = Items.RefreshAsync();
        for (var i = 0; i < 32; i++)
            TextBoxSuggestions.Add($"Suggestion {i}");
    }

    #endregion

    #region Methods

    public override IEnumerable<Imagin.Core.Models.Panel> GetPanels()
    {
        yield return
            new InheritsPanel();
        yield return 
            new ControlsPanel();
        yield return 
            new PropertiesPanel();
        yield return 
            new TestPanel();
    }

    #endregion

    #region Commands

    ICommand propertyGridSourceCommand;
    public ICommand MemberGridSourceCommand => propertyGridSourceCommand ??= new RelayCommand<string>(i =>
    {
        switch (i)
        {
            case "0":
                MemberGridSource = new
                {
                    Field1 = true,
                    Field2 = 3,
                    Field3 = "Hello",
                    Field4 = System.Windows.Visibility.Visible
                };
                break;

            case "1":
                MemberGridSource = Get.Current<Options>();
                break;

            case "2":
                MemberGridSource = Get.Current<Options>();
                break;

            case "3":
                MemberGridSource = new System.Windows.Point();
                break;
        }
    },
    i => i is string);

    //...

    Result GetResult()
    {
        Result result = default;
        switch (resultType)
        {
            case ResultTypes.Error:
                result = new Error(resultText);
                break;
            case ResultTypes.Message:
                result = new Message(resultText);
                break;
            case ResultTypes.Success:
                result = new Success() { Text = resultText };
                break;
            case ResultTypes.Warning:
                result = new Warning(resultText);
                break;
        }
        return result;
    }

    ICommand addResultCommand;
    public ICommand AddResultCommand => addResultCommand ??= new RelayCommand(() =>
    {
        var result = GetResult();
        if (result != null)
            Get.Current<App>().Notifications.Add(new("This is a result", result, TimeSpan.Zero));
    },
    () => resultText is string i && i.Length > 0 && resultType != ResultTypes.All && resultType != ResultTypes.None);

    ICommand logResultCommand;
    public ICommand LogResultCommand => logResultCommand ??= new RelayCommand(() =>
    {
        var result = GetResult();
        if (result != null)
            Log.Write<MainWindow>(result);
    },
    () => resultText is string i && i.Length > 0 && resultType != ResultTypes.All && resultType != ResultTypes.None);
        
    ICommand showColorWindowCommand;
    public ICommand ShowColorWindowCommand => showColorWindowCommand 
        ??= new RelayCommand(() => new ColorWindow("Select color", Colors.Red).ShowDialog());

    ICommand showDialogWindowCommand;
    public ICommand ShowDialogWindowCommand => showDialogWindowCommand 
        ??= new RelayCommand(() => Dialog.Show("Title", "Message", DialogImage.Information, Buttons.ContinueCancel));

    ICommand showDialogPresenterCommand;
    public ICommand ShowDialogPresenterCommand => showDialogPresenterCommand ??= new RelayCommand<object>(i =>
    {
        switch ($"{i}")
        {
            case "0":
                _ = Dialog.Show(View, "This is the title", "This is a message.", DialogImage.Information, Buttons.ContinueCancel);
                break;
            case "1":
                _ = Dialog.Show(View, "This is the title", new ProgressBar() { Height = 20, IsIndeterminate = true }, DialogImage.Information, Buttons.Cancel);
                break;
        }
    });

    ICommand showDownloadWindowCommand;
    public ICommand ShowDownloadWindowCommand => showDownloadWindowCommand ??= new RelayCommand(() => new DownloadWindow("Downloading...", "Downloading stuff now...", downloadWindowSource, Environment.SpecialFolder.Desktop.Path()).ShowDialog());
        
    ICommand showExplorerWindowCommand;
    public ICommand ShowExplorerWindowCommand => showExplorerWindowCommand
        ??= new RelayCommand(() => StorageWindow.Show(out string path, "Select something", StorageWindowModes.Open, null, null));

    ICommand showGradientWindowCommand;
    public ICommand ShowGradientWindowCommand => showGradientWindowCommand
        ??= new RelayCommand(() => new GradientWindow("Select gradient", Imagin.Core.Media.Gradient.Default).ShowDialog());

    ICommand showLoadWindowCommand;
    public ICommand ShowLoadWindowCommand => showLoadWindowCommand 
        ??= new RelayCommand(() => new LoadWindow().ShowDialog());

    ICommand showMemberWindowCommand;
    public ICommand ShowMemberWindowCommand => showMemberWindowCommand 
        ??= new RelayCommand(() => MemberWindow.ShowDialog(null, View));

    #endregion
}