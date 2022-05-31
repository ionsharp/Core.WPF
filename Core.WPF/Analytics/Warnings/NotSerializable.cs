using Imagin.Core.Analytics;

namespace Imagin.Core
{
    public class NotSerializableWarning : Warning
    {
        public NotSerializableWarning(object input) : base($"'{input.GetType().FullName}' is not marked as serializable.") { }
    }
}