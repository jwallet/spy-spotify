using System;

namespace EspionSpotify.Exceptions
{
    public class DestinationPathNotFoundException: Exception
    {
        public DestinationPathNotFoundException()
        {
        }

        public DestinationPathNotFoundException(string message)
            : base(message)
        {
        }

        public DestinationPathNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
