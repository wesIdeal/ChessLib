using System;
using System.Runtime.Serialization;
using System.Text;
using ChessLib.Types.Enums;
using EnumsNET;

namespace ChessLib.Types.Exceptions
{
    [Serializable]
    public class BoardException : Exception
    {
        public readonly BoardExceptionType ExceptionType = BoardExceptionType.None;
        public static BoardException MakeBoardException(BoardExceptionType exceptionType)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var validationIssue in exceptionType.GetFlags())
            {
                sb.AppendLine($"* {validationIssue.AsString(EnumFormat.Description)}");
            }
            return new BoardException(exceptionType, sb.ToString());
        }

        protected BoardException(BoardExceptionType exceptionType, string message) : base(message)
        {
            ExceptionType = exceptionType;
        }

        protected BoardException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BoardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
