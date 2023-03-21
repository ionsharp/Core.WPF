using Imagin.Core.Analytics;
using Imagin.Core.Collections;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Name("Notifications"), Image(SmallImages.Bell), Serializable]
public class NotificationsPanel : DataPanel
{
    public static readonly ResourceKey TemplateKey = new();

    enum Category { Commands0, Text }

    #region Properties

    [Name("ClearAfter")]
    [Option]
    public TimeSpan ClearAfter { get => Get(TimeSpan.Zero); set => Set(value); }

    [Hide]
    public override IList GroupNames => new StringCollection()
    {
        "None",
        nameof(Notification.Type)
    };

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Bell);

    [Hide]
    public IList<Notification> Notifications => Data as IList<Notification>;

    [Hide]
    public override IList SortNames => new StringCollection()
    {
        nameof(Notification.Type)
    };

    [Hide]
    public override int TitleCount => Data?.Count<Notification>(i => !i.IsRead) ?? 0;

    [Hide]
    public override string TitleKey => "Notifications";

    [Category(Category.Text)]
    [Header, HideName, Image(SmallImages.ArrowDownLeft), Index(int.MaxValue - 1), Style(BooleanStyle.Button)]
    public bool TextWrap { get => Get(true); set => Set(value); }

    ///

    readonly Updatable update = new(1.Seconds());

    #endregion

    #region NotificationsPanel

    public NotificationsPanel(ICollectionChanged input) : base()
    {
        Data = input;

        Notifications.ForEach<Notification>(i => { Unsubscribe(i); Subscribe(i); });
        update.Updated += OnUpdate;
    }

    #endregion

    #region Methods

    void Subscribe(Notification input) => input.PropertyChanged += OnNotificationChanged;

    void Unsubscribe(Notification input) => input.PropertyChanged -= OnNotificationChanged;

    ///

    void OnUpdate(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (ClearAfter > TimeSpan.Zero)
        {
            for (var i = Notifications.Count - 1; i >= 0; i--)
            {
                var j = Notifications[i].As<Notification>();
                if (DateTime.Now > j.Created + ClearAfter)
                    Notifications.RemoveAt(i);
            }
        }
    }

    ///

    protected override void OnItemAdded(object input)
        => Subscribe(input as Notification);

    protected override void OnItemRemoved(object input)
        => Unsubscribe(input as Notification);

    ///

    void OnNotificationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Notification.IsRead):
                Update(() => Title);
                break;
        }
    }

    ///

    ICommand markAllCommand;
    [Category(Category.Commands0)]
    [Name("MarkAll")]
    [Image(SmallImages.Read)]
    [Header]
    public ICommand MarkAllCommand => markAllCommand
        ??= new RelayCommand(() => Notifications.ForEach<Notification>(i => i.IsRead = true), () => Notifications?.Contains<Notification>(i => !i.IsRead) == true);

    ICommand unmarkAllCommand;
    [Category(Category.Commands0)]
    [Name("UnmarkAll")]
    [Image(SmallImages.Unread)]
    [Header]
    public ICommand UnmarkAllCommand => unmarkAllCommand
        ??= new RelayCommand(() => Notifications.ForEach<Notification>(i => i.IsRead = false), () => Notifications?.Contains<Notification>(i => i.IsRead) == true);

    #endregion
}