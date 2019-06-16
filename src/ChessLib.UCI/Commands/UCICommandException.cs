using System;
using System.Runtime.Serialization;

namespace ChessLib.UCI.Commands
{
    [Serializable]
    internal class UCICommandException : Exception
    {
        public UCICommandException()
        {
        }

        public UCICommandException(string message) : base(message)
        {
        }

        public UCICommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UCICommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}