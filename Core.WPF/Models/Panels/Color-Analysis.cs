using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Imagin.Core.Controls;

#region (class) ColorAnalysis

public abstract class ColorAnalysis : Base
{
    public string Model { get => Get<string>(); set => Set(value); }

    public ColorAnalysis() : base() { }

    public ColorAnalysis(string model) : base()
    {
        Model = model;
    }

    public override string ToString()
    {
        return $"{Model}\n";
    }
}

#endregion

#region (enum) ColorAnalysisType

public enum ColorAnalysisType
{
    [Name("Accuracy (XYZ 🡪 RGB 🡪 XYZ)")]
    Accuracy,
    [Name("Range (XYZ 🡪 RGB)")]
    Range,
    [Name("Range (RGB 🡪 XYZ)")]
    RangeInverse
}

#endregion

///

#region (class) ColorAccuracyAnalysis

public class ColorAccuracyAnalysis : ColorAnalysis
{
    public string Accuracy { get => Get<string>(); set => Set(value); }

    public Quality Quality { get => Get(Quality.Average); set => Set(value); }

    public ColorAccuracyAnalysis(string model, object accuracy, Quality status) : base(model)
    {
        Accuracy = $"{accuracy}"; Quality = status;
    }

    public override string ToString()
    {
        return $"{base.ToString()}{Accuracy}";
    }
}

#endregion

#region (class) ColorRangeAnalysis

public class ColorRangeAnalysis : ColorAccuracyAnalysis
{
    public string Maximum { get => Get<string>(); set => Set(value); }

    public string Minimum { get => Get<string>(); set => Set(value); }

    public ColorRangeAnalysis(string model, object accuracy, Quality status, object minimum, object maximum) : base(model, accuracy, status)
    {
        Minimum = Infinite($"{minimum}"); Maximum = Infinite($"{maximum}");
    }

    public override string ToString()
    {
        return $"{base.ToString()}\n= > {Minimum}, {Maximum}";
    }

    protected string Infinite(string input) => input.Replace("999", "∞").Replace("-999", "∞");
}

#endregion

#region (class) ColorRangeInverseAnalysis

public class ColorRangeInverseAnalysis : ColorRangeAnalysis
{
    public string TargetMaximum { get => Get(""); set => Set(value); }

    public string TargetMinimum { get => Get(""); set => Set(value); }

    public ColorRangeInverseAnalysis(string model, object accuracy, Quality status, object minimum, object maximum, object targetMinimum, object targetMaximum) : base(model, accuracy, status, minimum, maximum)
    {
        TargetMinimum = Infinite($"{targetMinimum}"); TargetMaximum = Infinite($"{targetMaximum}");
    }

    public override string ToString()
    {
        return $"{base.ToString()}\nTarget > {TargetMinimum}, {TargetMaximum}";
    }
}

#endregion

///

#region ColorAnalysisPanel

[Name("Analysis"), Explicit]
public class ColorAnalysisPanel : Panel
{
    public Collections.Serialization.IGroupWriter Profiles { get; set; }

    public override Uri Icon => Resource.GetImageUri(SmallImages.LineGraph);

    public override string Title => "Analysis";
        
    [Lock, Header, Show]
    [Name("# of colors"), Range(1, 255, 1, Style = RangeStyle.Both), RightText("^3")]
    public int Depth { get => Get(10); set => Set(value); }

    public object Results { get => Get<object>(); set => Set(value); }

    [Index(-1), Lock, Header, Show]
    public GroupItemForm Profile { get => Get<GroupItemForm>(); set => Set(value); }

    public ObservableCollection<ColorAccuracyAnalysis> 
        AResults { get; private set; } = new();

    public ObservableCollection<ColorRangeAnalysis> 
        BResults { get; private set; } = new();

    public ObservableCollection<ColorRangeInverseAnalysis> 
        CResults { get; private set; } = new();

    [Range(1, 8, 1, Style = RangeStyle.Both)]
    [Lock, Header, Show]
    public int Precision { get => Get(3); set => Set(value); }

    [Pin(Pin.AboveOrLeft), HideName, Lock, Header, Show]
    public ColorAnalysisType Type { get => Get(ColorAnalysisType.Accuracy); set => Set(value); }

    readonly Threading.Method refresh;

    public ColorAnalysisPanel() : base()
    {
        Results = AResults;
        refresh = new(Refresh, false);
    }

    public ColorAnalysisPanel(Collections.Serialization.IGroupWriter profiles) : this()
    {
        Profile = new(profiles, 0, 0);
    }

    async Task Refresh(CancellationToken token)
    {
        IsBusy = true; IsLocked = true;

        var results = Results as IList;
        results.Clear();

        var profile = Profile?.SelectedItem.As<WorkingProfile>() ?? WorkingProfile.Default;

        await Task.Run(() =>
        {
            Colour.Types.ForEach(i =>
            {
                if (token.IsCancellationRequested)
                    return;

                ColorAnalysis result = null;
                switch (Type)
                {
                    case ColorAnalysisType.Accuracy:
                        var a = Colour.Analysis.GetAccuracy(i, profile, (uint)Depth, Precision, true);
                        result = new ColorAccuracyAnalysis(i.Name, $"{a}%", GetStatus(a));
                        break;

                    case ColorAnalysisType.Range:
                        var b = Colour.Analysis.GetRange(i, profile, out double x, true, (uint)Depth, Precision, true);
                        result = new ColorRangeAnalysis(i.Name, $"{x.Round(Precision)}%", GetStatus(x), b.Minimum, b.Maximum);
                        break;

                    case ColorAnalysisType.RangeInverse:
                        var c = Colour.Analysis.GetRange(i, profile, out double y, false, (uint)Depth, Precision, true);
                        result = new ColorRangeInverseAnalysis(i.Name, $"{y.Round(Precision)}%", GetStatus(y), c.Minimum, c.Maximum, Colour.Minimum(i), Colour.Maximum(i));
                        break;
                }
                _ = Dispatch.BeginInvoke(() => results.Add(result));
            });
        });

        IsLocked = false; IsBusy = false;
    }

    Quality GetStatus(double input)
    {
        if (input >= 100)
            return Quality.Perfect;

        if (input >= 75)
            return Quality.Excellent;

        if (input >= 50)
            return Quality.Average;

        if (input >= 25)
            return Quality.Fair;

        return Quality.Poor;
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Type):
                switch (Type)
                {
                    case ColorAnalysisType.Accuracy:
                        Results = AResults;
                        break;
                    case ColorAnalysisType.Range:
                        Results = BResults;
                        break;
                    case ColorAnalysisType.RangeInverse:
                        Results = CResults;
                        break;
                }
                break;
        }
    }

    ICommand cancelCommand;
    [Content("Cancel"), Header, Name("Cancel"), Pin(Pin.BelowOrRight), Image(SmallImages.StopRound), Index(int.MaxValue), Show, Style(Core.ButtonStyle.Cancel)]
    [VisibilityTrigger(nameof(IsBusy), true)]
    public ICommand CancelCommand => cancelCommand ??= new RelayCommand(() => refresh.Cancel(), () => refresh.Started);

    ICommand copyCommand;
    [Header, Name("Copy"), Image(SmallImages.Copy), Index(int.MaxValue), Lock, Show]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
    {
        var result = new StringBuilder();
        Results.As<IList>().ForEach(i => result.AppendLine($"{i}\n"));
        Clipboard.SetText(result.ToString());
    },
    () => Results?.As<IList>().Count > 0);

    ICommand clearCommand;
    [Header, Name("Clear"), Image(SmallImages.XRound), Index(int.MaxValue), Lock, Show]
    public ICommand ClearCommand => clearCommand ??= new RelayCommand(() => Results?.As<IList>().Clear(), () => Results?.As<IList>().Count > 0);

    ICommand startCommand;
    [Content("Start"), Header, Image(SmallImages.Play), Index(int.MaxValue), Lock, Name("Start"), Pin(Pin.BelowOrRight), Show, Style(Core.ButtonStyle.Default)]
    [VisibilityTrigger(nameof(IsBusy), false)]
    public ICommand StartCommand => startCommand ??= new RelayCommand(() => _ = refresh.Start(), () => !refresh.Started);
}

#endregion