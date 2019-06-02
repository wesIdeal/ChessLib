using System.Linq;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;

namespace ChessLib.Validators.FENValidation.Rules
{
    public class CastlingAvailabilityRule : IFENRule
    {
        private static readonly char[] ValidCastlingStringChars = new char[] { 'k', 'K', 'q', 'Q' };
        public FENError Validate(in string fen)
        {
            var castlingAvailability = fen.GetFENPiece(FENPieces.CastlingAvailability);
            return ValidateCastlingAvailabilityString(castlingAvailability);
        }
        private static FENError ValidateCastlingAvailabilityString(string castleAvailability)
        {
            FENError fenError = FENError.None;
            if (string.IsNullOrWhiteSpace(castleAvailability))
            {
                return FENError.CastlingNoStringPresent;
            }
            if (castleAvailability == "-") return fenError;

            var castlingChars = castleAvailability.ToCharArray();
            var notAllowed = castlingChars.Where(c => !ValidCastlingStringChars.Contains(c));

            if (notAllowed.Any()) { fenError |= FENError.CastlingUnrecognizedChar; }
            else
            {
                var castleAvailabilityArray = castleAvailability.ToArray();
                if (castleAvailabilityArray.Length != castleAvailabilityArray.Distinct().Count())
                {
                    fenError |= FENError.CastlingStringRepetition;
                }
            }
            return fenError;
        }
    }
}

