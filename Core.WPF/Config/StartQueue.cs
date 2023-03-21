using Imagin.Core.Analytics;
using Imagin.Core.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imagin.Core.Config;

public class StartQueue : List<StartTask>
{
    public event EventHandler<EventArgs> Completed;

    public SplashWindow SplashWindow { get; private set; }

    public void Add(bool dispatch, string message, Action action)
    {
        Add(new StartTask(dispatch, message, action));
    }

    async Task Alert(string message, double progress)
    {
        await Dispatch.BeginInvoke(() =>
        {
            SplashWindow.Message = message;
            SplashWindow.Progress = progress;
        });
    }

    public async Task Invoke(SplashWindow splashWindow)
    {
        SplashWindow = splashWindow;

        var delay = splashWindow.Delay;
        await Task.Run(async () =>
        {
            double progress = 0;

            double a = 0;
            double b = Count;

            foreach (var i in this)
            {
                System.Threading.Thread.Sleep(delay);

                a++;
                progress = a / b;

                await Alert(i.Message, progress);
                Try.Invoke(() =>
                {
                    if (!i.Dispatch)
                    {
                        i.Action();
                        return;
                    }
                    Dispatch.Invoke(i.Action);
                },
                e => Log.Write<StartQueue>(e));
            }
        });
        Completed?.Invoke(this, EventArgs.Empty);
    }
}