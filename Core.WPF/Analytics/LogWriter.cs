using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Models;

namespace Imagin.Core.Analytics;

public class LogWriter : XmlWriter<LogEntry>, ILog
{
    public static Limit DefaultLimit = new(5000, Limit.Actions.ClearAndArchive);

    public bool Enabled => Current.Get<MainViewOptions>()?.LogEnable == true;

    public LogWriter(string folderPath, Limit limit) : base(nameof(Log), folderPath, nameof(Log), "xml", "xml", limit, typeof(Error), typeof(LogEntry), typeof(Message), typeof(Success), typeof(Warning), typeof(Result), typeof(ResultTypes), typeof(ResultLevel)) { }

    ///

    void ILog.Add(LogEntry input) => _ = Dispatch.BeginInvoke(() => Add(input));

    protected override void ClearItems() => _ = Dispatch.BeginInvoke(() => base.ClearItems());
        
    protected override void InsertItem(int index, LogEntry item) => _ = Dispatch.BeginInvoke(() => base.InsertItem(index, item));

    protected override void RemoveItem(int index) => _ = Dispatch.BeginInvoke(() => base.RemoveItem(index));
}