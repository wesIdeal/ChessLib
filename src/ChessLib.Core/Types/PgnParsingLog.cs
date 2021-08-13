using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types
{
    public readonly struct PgnParsingLog : IEquatable<PgnParsingLog>
    {
        public ParsingErrorLevel ParsingErrorLevel { get; }
        public string Message { get; }
        public string ParseInput { get; }

        public PgnParsingLog(ParsingErrorLevel parsingErrorLevel, string message, string parseInput = "")
        {
            ParsingErrorLevel = parsingErrorLevel;
            Message = message;
            ParseInput = parseInput;
        }

        public bool Equals(PgnParsingLog other)
        {
            return ParsingErrorLevel == other.ParsingErrorLevel && Message == other.Message &&
                   ParseInput == other.ParseInput;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((PgnParsingLog)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ParsingErrorLevel;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ParseInput != null ? ParseInput.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            var parserInput = string.IsNullOrWhiteSpace(ParseInput) ? "" : $"{Environment.NewLine}\t{ParseInput}";
            return $"{ParsingErrorLevel} | {Message} {parserInput}";
        }

        public static bool operator ==(PgnParsingLog left, PgnParsingLog right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PgnParsingLog left, PgnParsingLog right)
        {
            return !Equals(left, right);
        }
    }
}