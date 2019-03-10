using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicBitboard.Helpers
{
    public static class FENHelpers
    {
        public static Color GetActiveColor(string v)
        {
            switch (v)
            {
                case "w": return Color.White;
                case "b": return Color.Black;
                default: throw new FENException("Invalid active color in FEN. Color character received was " + v + ".");
            }
        }

        public static uint? GetMoveNumberFromString(string v)
        {
            if (!uint.TryParse(v, out uint result))
            {
                throw new FENException($"Could not parse Halfmove Clock portion of FEN. Received {v}.");
            }
            return result;
        }

        public static CastlingAvailability GetCastlingFromString(string castleAvailability)
        {
            char[] allowedChars = new char[] { 'k', 'K', 'q', 'Q' };
            if (castleAvailability.Trim() == "") { throw new FENException($"Cannot get catling availability from empty string."); }
            if (castleAvailability == "-") return CastlingAvailability.NoCastlingAvailable;
            var castlingChars = castleAvailability.ToCharArray();
            var notAllowed = castlingChars.Where(c => !allowedChars.Contains(c));
            if (notAllowed.Any()) { throw new FENException($"Found unallowed characters in castling string: {string.Join(", ", notAllowed.Select(x => x.ToString()))}."); }
            var rv = 0;
            if (castleAvailability.Contains('k')) { rv |= (int)CastlingAvailability.BlackKingside; }
            if (castleAvailability.Contains('K')) { rv |= (int)CastlingAvailability.WhiteKingside; }
            if (castleAvailability.Contains('q')) { rv |= (int)CastlingAvailability.BlackQueenside; }
            if (castleAvailability.Contains('Q')) { rv |= (int)CastlingAvailability.WhiteQueenside; }
            return (CastlingAvailability)rv;
        }
    }
}
