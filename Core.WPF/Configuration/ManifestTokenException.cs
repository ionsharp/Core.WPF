using System;

namespace Imagin.Core.Configuration
{
    public class ManifestTokenException : Exception
    {
        public ManifestTokenException() : base() { }

        public ManifestTokenException(string message) : base(message) { }
    }
}