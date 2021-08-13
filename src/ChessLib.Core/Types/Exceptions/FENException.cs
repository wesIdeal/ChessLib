using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ChessLib.Core.Types.Enums;
using EnumsNET;

namespace ChessLib.Core.Types.Exceptions
{
    [Serializable]
    public class FENException : Exception
    {
        public FENException(string fen, FENError fenError)
            : base(FormatFENError(fen, fenError))
        {
            FENError = fenError;
        }

        public FENException(string fen, FENError fenError, Exception innerException)
            : base(FormatFENError(fen, fenError), innerException)
        {
            FENError = fenError;
        }

        protected FENException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo,
            context)
        {
        }

        public FENException()
        {
        }

        public FENException(string message) : base(message)
        {
        }

        public FENException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public readonly FENError FENError;

        private static string GetFormattedMessage(FENError e)
        {
            return "* " + e.AsString(EnumFormat.Description);
        }

        private static string FormatFENError(string fen, FENError e)
        {
            if (e == FENError.None) return "";
            var sb = new StringBuilder($"PremoveFEN Errors Found ({fen}):\r\n");
            foreach (var error in e.GetFlags().Select(x => new {error = x, message = GetFormattedMessage(x)}))
            {
                sb.AppendLine(error.message);
            }

            return sb.ToString();
        }
    }
}