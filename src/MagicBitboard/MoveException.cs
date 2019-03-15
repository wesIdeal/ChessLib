using System;


namespace MagicBitboard
{
    using System.Runtime.Serialization;

    [Serializable]
    public class MoveException : Exception
    {
        public MoveException()
        {
        }

        public MoveException(string message) : base(message)
        {
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
