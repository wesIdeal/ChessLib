using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;

namespace ChessLib.Data.Validators.FENValidation.Rules
{
    public class EnPassantRule : IFENRule
    {
        public FENError Validate(in string fen)
        {
            var ep = fen.GetFENPiece(FENPieces.EnPassantSquare);
            return ValidateEnPassantSquare(ep);
        }

        private static FENError ValidateEnPassantSquare(string v)
        {
            if (v == "-") return FENError.None;
            const FENError error = FENError.InvalidEnPassantSquare;
            bool valid = true;
            if (v.Length != 2) return error;
            valid &= (char.IsLetter(v[0]) && char.IsLower(v[0]) && (v[0] >= 'a' && v[0] <= 'h'));
            valid &= ushort.TryParse(v[1].ToString(), out ushort rank);
            if (valid)
            {
                valid &= (rank >= 1 && rank <= 8);
            }
            return valid ? FENError.None : error;
        }
    }
}

