using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MagicBitboard
{
    public class FENPiecePlacementPawnException : FENException
    {
        public FENPiecePlacementPawnException(string message) : base(message)
        {
        }
    }
    public class FENPiecePlacementKingException : FENException
    {
        public FENPiecePlacementKingException(string message) : base(message)
        {
        }
    }
    public class FENPiecePlacementException : FENException
    {
        public FENPiecePlacementException(string message) : base(message)
        {
        }
    }
    public class FENActiveColorException : FENException
    {
        public FENActiveColorException(string message) : base(message)
        {
        }
    }
    public class FENMoveNumberException : FENException
    {
        public FENMoveNumberException(string message) : base(message)
        {
        }
    }
    public class FENCastlingAvailabilityException : FENException
    {
        public FENCastlingAvailabilityException(string message) : base(message)
        {
        }
    }

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
