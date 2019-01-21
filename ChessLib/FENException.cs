using System;
using System.Runtime.Serialization;

namespace ChessLib
{
    [Serializable]
    public class FENException : Exception
    {
        public FENException()
        {
        }

        public FENException(string message) : base(message)
        {
        }

        public FENException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FENException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}