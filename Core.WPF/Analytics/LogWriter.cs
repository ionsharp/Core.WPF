using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Models;

namespace Imagin.Core.Analytics
{
    public class LogWriter : XmlWriter<LogEntry>, ILog
    {
        public static Limit DefaultLimit = new(5000, Limit.Actions.ClearAndArchive);

        public bool Enabled => Get.Where<IMainViewOptions>()?.LogEnable == true;

        public LogWriter(string folderPath, Limit limit) : base(nameof(Log), folderPath, nameof(Log), "xml", "xml", limit, typeof(Error), typeof(LogEntry), typeof(Message), typeof(Success), typeof(Warning), typeof(Result), typeof(ResultTypes), typeof(ResultLevel))
            => Get.Register<ILog>(this);
    }
}