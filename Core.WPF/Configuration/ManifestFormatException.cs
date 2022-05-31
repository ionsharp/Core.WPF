using System.IO;

namespace Imagin.Core.Configuration
{
    public class ManifestFormatException : FileFormatException
    {
        public readonly bool IsRemote;

        public ManifestFormatException(bool isRemote, string message) : base(message)
        {
            IsRemote = isRemote;
        }
    }
}