using System;
using System.Runtime.Serialization;
using System.Text;
using ChessLib.Core.Types.Enums;
using EnumsNET;

namespace ChessLib.Core.Types.Exceptions
{
    [Serializable]
    public class BoardException : Exception
    {
        public BoardException(BoardExceptionType exceptionType, string message) : base(message)
        {
            ExceptionType = exceptionType;
        }

        protected BoardException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BoardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return $"{ExceptionType} received while validating a board.{Environment.NewLine}{Message}";
        }

        public readonly BoardExceptionType ExceptionType = BoardExceptionType.None;

        public static BoardException MakeBoardException(BoardExceptionType exceptionType)
        {
            var sb = new StringBuilder();
            foreach (var validationIssue in exceptionType.GetFlags())
            {
                sb.AppendLine($"* {validationIssue.AsString(EnumFormat.Description)}");
            }

            return new BoardException(exceptionType, sb.ToString());
        }
    }
}