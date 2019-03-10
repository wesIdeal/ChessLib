using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MagicBitboard
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

        protected FENException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }
    }
}
