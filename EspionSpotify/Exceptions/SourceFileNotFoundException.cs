using System;

namespace EspionSpotify.Exceptions
{
    public class SourceFileNotFoundException : Exception
    {
        public SourceFileNotFoundException()
        {
        }

        public SourceFileNotFoundException(string message)
            : base(message)
        {
        }

        public SourceFileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
