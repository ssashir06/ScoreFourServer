using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.Exceptions
{
    public class GameException : Exception
    {
        public GameException(string message) : base(message)
        {
        }

        public GameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GameException()
        {
        }
    }
}
