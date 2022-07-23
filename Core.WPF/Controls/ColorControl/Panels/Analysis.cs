using Imagin.Core.Colors;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Imagin.Core.Controls;

#region (class) ColorAnalysis

public abstract class ColorAnalysis : Base
{
    string model = null;
    public string Model
    {
        get => model;
        set => this.Change(ref model, value);
    }

    public ColorAnalysis() : base() { }

    public ColorAnalysis(string model) : base()
    {
        Model = model;
    }

    public override string ToString()
    {
        return $"{model}\n";
    }
}

#endregion

#region (enum) ColorAnalysisType

public enum ColorAnalysisType
{
    [DisplayName("XYZ 🡪 RGB 🡪 XYZ")]
    Accuracy,
    [DisplayName("XYZ 🡪 RGB")]
    Range,
    [DisplayName("RGB 🡪 XYZ")]
    RangeInverse
}

#endregion

//...

#region (class) ColorAccuracyAnalysis

public class ColorAccuracyAnalysis : ColorAnalysis
{
    string accuracy = null;
    public string Accuracy
    {
        get => accuracy;
        set => this.Change(ref accuracy, value);
    }

    Quality status = Quality.Average;
    public Quality Status
    {
        get => status;
        private set => this.Change(ref status, value);
    }

    public ColorAccuracyAnalysis(string model, object accuracy, Quality status) : base(model)
    {
        Accuracy = $"{accuracy}"; Status = status;
    }

    public override string ToString()
    {
        return $"{base.ToString()}{accuracy}";
    }
}

#endregion

#region (class) ColorRangeAnalysis

public class ColorRangeAnalysis : ColorAccuracyAnalysis
{
    string maximum = null;
    public string Maximum
    {
        get => maximum;
        set => this.Change(ref maximum, value);
    }

    string minimum = null;
    public string Minimum
    {
        get => minimum;
        set => this.Change(ref minimum, value);
    }

    public ColorRangeAnalysis(string model, object accuracy, Quality status, object minimum, object maximum) : base(model, accuracy, status)
    {
        Minimum = Infinite($"{minimum}"); Maximum = Infinite($"{maximum}");
    }

    public override string ToString()
    {
        return $"{base.ToString()}\n= > {minimum}, {maximum}";
    }

    protected string Infinite(string input) => input.Replace("999", "∞").Replace("-999", "∞");
}

#endregion

#region (class) ColorRangeInverseAnalysis

public class ColorRangeInverseAnalysis : ColorRangeAnalysis
{
    string targetMaximum = null;
    public string TargetMaximum
    {
        get => targetMaximum;
        set => this.Change(ref targetMaximum, value);
    }

    string targetMinimum = null;
    public string TargetMinimum
    {
        get => targetMinimum;
        set => this.Change(ref targetMinimum, value);
    }

    public ColorRangeInverseAnalysis(string model, object accuracy, Quality status, object minimum, object maximum, object targetMinimum, object targetMaximum) : base(model, accuracy, status, minimum, maximum)
    {
        TargetMinimum = Infinite($"{targetMinimum}"); TargetMaximum = Infinite($"{targetMaximum}");
    }

    public override string ToString()
    {
        return $"{base.ToString()}\nTarget > {targetMinimum}, {targetMaximum}";
    }
}

#endregion

//...

#region ColorAnalysisPanel

[DisplayName("Analysis"), Explicit]
public class ColorAnalysisPanel : Panel
{
    public override Uri Icon => Resources.InternalImage(Images.LineGraph);

    public override string Title => "Analysis";
        
    int depth = 10;
    [DisplayName("# of colors"), SliderUpDown, Setter(nameof(MemberModel.RightText), "^3"), Range(1, 255, 1)]
    [Lock, Tool, Visible]
    public int Depth
    {
        get => depth;
        set => this.Change(ref depth, value);
    }

    object results = null;
    public object Results
    {
        get => results;
        set => this.Change(ref results, value);
    }

    GroupItemModel profile = null;
    [Index(-1), Lock, Tool, Visible]
    public GroupItemModel Profile
    {
        get => profile;
        set => this.Change(ref profile, value);
    }

    public ObservableCollection<ColorAccuracyAnalysis> 
        AResults { get; private set; } = new();

    public ObservableCollection<ColorRangeAnalysis> 
        BResults { get; private set; } = new();

    public ObservableCollection<ColorRangeInverseAnalysis> 
        CResults { get; private set; } = new();

    int precision = 3;
    [SliderUpDown, Range(1, 8, 1)]
    [Lock, Tool, Visible]
    public int Precision
    {
        get => precision;
        set => this.Change(ref precision, value);
    }

    ColorAnalysisType type = ColorAnalysisType.Accuracy;
    [Above, Label(false), Lock, Localize(false), Tool, Visible]
    public ColorAnalysisType Type
    {
        get => type;
        set => this.Change(ref type, value);
    }

    readonly Threading.CancelTask refresh;

    public ColorAnalysisPanel(Collections.Serialization.IGroupWriter profiles) : base() 
    {
        Results = AResults;
        refresh = new(Refresh, false);

        Profile = new(profiles, 0, 0);
    }

    async Task Refresh(CancellationToken token)
    {
        IsBusy = true; IsLocked = true;

        var results = this.results as IList;
        results.Clear();

        var profile = Profile?.SelectedItem.As<WorkingProfileModel>()?.Value ?? WorkingProfile.Default;

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
                        var a = Colour.Analysis.GetAccuracy(i, profile, (uint)depth, precision, true);
                        result = new ColorAccuracyAnalysis(i.Name, $"{a}%", GetStatus(a));
                        break;

                    case ColorAnalysisType.Range:
                        var b = Colour.Analysis.GetRange(i, profile, out double x, true, (uint)depth, precision, true);
                        result = new ColorRangeAnalysis(i.Name, $"{x.Round(precision)}%", GetStatus(x), b.Minimum, b.Maximum);
                        break;

                    case ColorAnalysisType.RangeInverse:
                        var c = Colour.Analysis.GetRange(i, profile, out double y, false, (uint)depth, precision, true);
                        result = new ColorRangeInverseAnalysis(i.Name, $"{y.Round(precision)}%", GetStatus(y), c.Minimum, c.Maximum, Colour.Minimum(i), Colour.Maximum(i));
                        break;
                }
                Dispatch.Invoke(() => results.Add(result));
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

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
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
    [Below, DisplayName("Cancel"), Image(Images.StopRound), Index(1), Tool, Trigger(nameof(MemberModel.IsVisible), nameof(IsBusy)), Visible]
    public ICommand CancelCommand => cancelCommand ??= new RelayCommand(() => refresh.Cancel(), () => refresh.Started);

    ICommand copyCommand;
    [Below, DisplayName("Copy"), Image(Images.Copy), Index(1), Lock, Tool, Visible]
    public ICommand CopyCommand => copyCommand ??= new RelayCommand(() =>
    {
        var result = new StringBuilder();
        Results.As<IList>().ForEach(i => result.AppendLine($"{i}\n"));
        Clipboard.SetText(result.ToString());
    },
    () => Results?.As<IList>().Count > 0);

    ICommand clearCommand;
    [DisplayName("Clear"), Below, Image(Images.XRound), Index(0), Lock, Tool, Visible]
    public ICommand ClearCommand => clearCommand ??= new RelayCommand(() => Results?.As<IList>().Clear(), () => Results?.As<IList>().Count > 0);

    ICommand refreshCommand;
    [DisplayName("Refresh"), Below, Image(Images.Refresh), Index(2), Lock, Tool, Trigger(nameof(MemberModel.IsVisible), nameof(IsNotBusy)), Visible]
    public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(() => _ = refresh.Start(), () => !refresh.Started);
}

#endregion