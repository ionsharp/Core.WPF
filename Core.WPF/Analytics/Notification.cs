using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Runtime.Serialization;
using System.Timers;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Imagin.Core.Analytics;

[Name(nameof(Notification)), XmlType(nameof(Notification))]
[Serializable]
public class Notification : Updatable
{
    [field: NonSerialized]
    public event EventHandler<EventArgs> Expired;

    [XmlIgnore]
    public ICommand Command { get => Get<ICommand>(null, false); set => Set(value, false); }

    [XmlAttribute]
    public DateTime Created { get => Get(DateTime.Now); set => Set(value); }

    public TimeSpan Expiration { get => Get(TimeSpan.Zero); set => Set(value); }

    [XmlIgnore]
    public string Icon { get => Get(""); set => Set(value); }

    [XmlAttribute]
    public bool IsRead { get => Get(false); set => Set(value); }

    [XmlIgnore]
    public string Message => Result?.Text;

    [XmlElement]
    public Result Result { get => Get<Result>(); set => Set(value); }

    [XmlAttribute]
    public string Title { get => Get(""); set => Set(value); }

    public string Type => $"{Result?.Type}";

    ///

    public Notification() : base() { }

    public Notification(string title, Result result, TimeSpan expiration) : base()
    {
        Title
            = title;
        Result
            = result;
        Expiration
            = expiration;

        if (expiration > TimeSpan.Zero)
            timer.Enabled = true;
    }

    ///

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext input)
    {
        if (Expiration > TimeSpan.Zero)
            Reset(1.Seconds(), true);
    }

    protected override void OnUpdate(ElapsedEventArgs e)
    {
        base.OnUpdate(e);
        XPropertyChanged.Update(this, () => Created);
        if (e.SignalTime - Created >= Expiration)
        {
            OnExpired();
            return;
        }
    }

    protected virtual void OnExpired() => Expired?.Invoke(this, EventArgs.Empty);

    ///

    [field: NonSerialized]
    ICommand markCommand;
    [XmlIgnore]
    public ICommand MarkCommand => markCommand ??= new RelayCommand<object>(i => IsRead = true, i => i != null);
}