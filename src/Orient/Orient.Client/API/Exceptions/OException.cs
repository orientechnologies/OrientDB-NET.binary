using System;

namespace Orient.Client
{
    public class OException : Exception
    {
        public OExceptionType Type { get; set; }

        public OException()
        {
        }

        public OException(OExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public OException(OExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }
    }
}
