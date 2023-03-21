using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Linq;
using System.Collections.Specialized;
using System.Linq;

namespace Imagin.Core.Analytics;

public class NotificationWriter : XmlWriter<Notification>
{
    public static Limit DefaultLimit = new(50, Limit.Actions.ClearAndArchive);

    public int Read => this.Where(i => i.IsRead).Count();

    public int Unread => this.Where(i => !i.IsRead).Count();

    public NotificationWriter(string folderPath, Limit limit) : base("Notifications", folderPath, "Notifications", "xml", "xml", limit, typeof(Notification), typeof(Result), typeof(Error), typeof(Message), typeof(Success), typeof(Warning)) { }
        
    void OnExpired(object sender, System.EventArgs e)
    {
        Remove(sender as Notification);
    }

    void OnNotificationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Notification.IsRead))
        {
            XPropertyChanged.Update(this, () => Read);
            XPropertyChanged.Update(this, () => Unread);
        }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        XPropertyChanged.Update(this, () => Read);
        XPropertyChanged.Update(this, () => Unread);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems[0] is Notification i)
                {
                    i.Expired += OnExpired;
                    i.PropertyChanged += OnNotificationChanged;
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems[0] is Notification j)
                {
                    j.Expired -= OnExpired;
                    j.PropertyChanged -= OnNotificationChanged;
                }
                break;
        }
    }
}