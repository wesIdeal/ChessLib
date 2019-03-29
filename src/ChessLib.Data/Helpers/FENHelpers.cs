using ChessLib.Data.Exceptions;
using ChessLib.Data.Types;
using EnumsNET;
using System;
using System.Linq;

namespace ChessLib.Data.Helpers
{
    public static class FENHelpers
    {
        public static readonly char[] ValidCastlingStringChars = new char[] { 'k', 'K', 'q', 'Q', '-' };
        public static readonly char[] ValidFENChars = new char[] { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";



        public static void ValidateFENStructure(string fen)
        {
            if (string.IsNullOrEmpty(fen)
                || fen.Split(' ').Count() != 6)
            {
                throw new FENException(fen, FENError.InvalidFENString);
            }
        }

        public static FENError ValidatePiecePlacement(string piecePlacement)
        {
            FENError fenError = FENError.Null;
            fenError |= ValidatePiecePlacementRanks(piecePlacement);
            fenError |= ValidatePiecePlacementCharacters(piecePlacement);
            return fenError;
        }
        private static FENError ValidatePiecePlacementRanks(string piecePlacement)
        {
            FENError fenError = FENError.Null;
            var ranks = piecePlacement.Split('/').Reverse();
            if (ranks.Count() != 8)
            {
                fenError |= FENError.PiecePlacementRankCount;
            }
            var ranksValidation = ranks.Select((r, idx) => new { Rank = idx + 1, Count = GetStringRepForRank(r) });
            var badRanks = ranksValidation.Where(x => x.Count != 8);

            if (badRanks.Any())
            {
                fenError |= FENError.PiecePlacementPieceCountInRank;
            }
            return fenError;
        }
        private static FENError ValidatePiecePlacementCharacters(string piecePlacement)
        {
            string[] invalidChars;
            if ((invalidChars = piecePlacement.Select(x => x).Where(x => !ValidFENChars.Contains(x)).Select(x => x.ToString()).ToArray()).Any())
            {
                return FENError.PiecePlacementInvalidChars;
            }
            return FENError.Null;
        }

        public static FENError ValidateActiveColor(string activeColor) => (new[] { "w", "b" }).Contains(activeColor) ? FENError.Null : FENError.InvalidActiveColor;

        public static FENError ValidateCastlingAvailabilityString(string castleAvailability)
        {
            FENError fenError = FENError.Null;
            if (castleAvailability == "-") return fenError;

            if (string.IsNullOrWhiteSpace(castleAvailability)) { fenError |= FENError.CastlingUnrecognizedChar; }
            var castlingChars = castleAvailability.ToCharArray();
            var notAllowed = castlingChars.Where(c => !ValidCastlingStringChars.Contains(c));

            if (notAllowed.Any()) { fenError |= FENError.CastlingUnrecognizedChar; }
            else
            {
                if (castleAvailability.Contains('-'))
                {
                    fenError |= FENError.CastlingStringBadSequence;
                }
                else
                {
                    var castleAvailabilityArray = castleAvailability.ToArray();
                    if (castleAvailabilityArray.Count() != castleAvailabilityArray.Distinct().Count())
                    {
                        fenError |= FENError.CastlingStringRepetition;
                    }
                }
            }
            return fenError;
        }

        public static FENError ValidateEnPassantSquare(string v)
        {
            if (v == "-") return FENError.Null;
            const FENError error = FENError.InvalidEnPassantSquare;
            bool valid = true;
            if (v.Length != 2) return error;
            valid &= (char.IsLetter(v[0]) && char.IsLower(v[0]) && (v[0] >= 'a' && v[0] <= 'h'));
            valid &= ushort.TryParse(v[1].ToString(), out ushort rank);
            if (valid)
            {
                valid &= (rank >= 1 && rank <= 8);
            }
            return valid ? FENError.Null : error;
        }

        public static FENError ValidateHalfmoveClock(string n) => ValidateNumberFromString(n) ? FENError.Null : FENError.HalfmoveClock;
        public static FENError ValidateFullMoveCounter(string n) => ValidateNumberFromString(n) ? FENError.Null : FENError.FullMoveCounter;
        private static bool ValidateNumberFromString(string moveCounter)
        {
            return uint.TryParse(moveCounter, out uint result);
        }

        public static void ValidateFENString(string fen)
        {
            ValidateFENStructure(fen);
            FENError fenError = FENError.Null;
            var fenPieces = fen.Split(' ');

            fenError |= ValidatePiecePlacement(fenPieces[(int)FENPieces.PiecePlacement]);
            fenError |= ValidateActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            fenError |= ValidateCastlingAvailabilityString(fenPieces[(int)FENPieces.CastlingAvailability]);
            fenError |= ValidateEnPassantSquare(fenPieces[(int)FENPieces.EnPassantSquare]);
            fenError |= ValidateHalfmoveClock(fenPieces[(int)FENPieces.HalfmoveClock]);
            fenError |= ValidateFullMoveCounter(fenPieces[(int)FENPieces.FullMoveCounter]);
            if (fenError != FENError.Null)
            {
                throw new FENException(fen, fenError);
            }
        }



        public static CastlingAvailability GetCastlingFromString(string castleAvailability)
        {
            ValidateCastlingAvailabilityString(castleAvailability);
            var rv = 0;
            if (castleAvailability == "-") return CastlingAvailability.NoCastlingAvailable;
            if (castleAvailability.Contains('k')) { rv |= (int)CastlingAvailability.BlackKingside; }
            if (castleAvailability.Contains('K')) { rv |= (int)CastlingAvailability.WhiteKingside; }
            if (castleAvailability.Contains('q')) { rv |= (int)CastlingAvailability.BlackQueenside; }
            if (castleAvailability.Contains('Q')) { rv |= (int)CastlingAvailability.WhiteQueenside; }
            return (CastlingAvailability)rv;
        }

        public static Color GetActiveColor(string v)
        {
            switch (v)
            {
                case "w": return Color.White;
                case "b": return Color.Black;
                default: return 0;
            }
        }

        public static uint GetMoveNumberFromString(string v)
        {
            return uint.Parse(v);
        }



        private static int GetStringRepForRank(string rank)
        {
            var rv = 0;
            foreach (var c in rank)
            {
                if (char.IsDigit(c))
                {
                    rv += UInt16.Parse(c.ToString());
                }
                else rv++;
            }
            return rv;
        }

        public static string MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability caBitFlags)
        {
            var s = "";
            if (caBitFlags == (CastlingAvailability)0) s = CastlingAvailability.NoCastlingAvailable.AsString(EnumFormat.Description);
            else
            {
                foreach (var caFlag in caBitFlags.GetFlags().OrderBy(x => x))
                {
                    s += caFlag.AsString(EnumFormat.Description);
                }
            }
            return s;
        }

        public static int BoardIndexToFENIndex(ushort idx)
        {
            var rankOffset = BoardHelpers.RankCompliment((ushort)(idx / 8));
            return (rankOffset * 8) + (idx % 8);
        }
    }
}
