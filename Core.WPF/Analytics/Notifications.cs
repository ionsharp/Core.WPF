using Imagin.Core.Config;
using System;

namespace Imagin.Core.Analytics;

public static class Notifications
{
    public static void Add(string title, Result result, TimeSpan? expiration = null)
    {
        var i = new Notification(title, result, expiration ?? TimeSpan.Zero);
        Current.Get<BaseApplication>().Notifications.Add(i);
    }
}