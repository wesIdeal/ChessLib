using ChessLib.Data.Exceptions;
using ChessLib.Data.Types;
using EnumsNET;
using System;
using System.Linq;

namespace ChessLib.Data.Helpers
{
    public static class FENHelpers
    {
        public const string FENInitial = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static readonly char[] ValidCastlingStringChars = new char[] { 'k', 'K', 'q', 'Q', '-' };
        public static readonly char[] ValidFENChars = new char[] { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };

        /// <summary>
        /// Gets a rank from a validated fen string
        /// </summary>
        /// <param name="fen">FEN string</param>
        /// <param name="rank">Board Rank (*not bitboard index rank*)</param>
        /// <returns></returns>
        public static string GetRankFromFen(this string fen, int rank)
        {
            var r = Math.Abs(rank - 8);
            var ranks = GetRanksFromFen(fen);
            return ranks[rank];
        }

        public static string[] GetRanksFromFen(this string fen)
        {
            var pp = GetFENPiece(fen, FENPieces.PiecePlacement);
            var ranks = pp.Split('/');
            return ranks;
        }

        public static void ValidateFENStructure(string fen)
        {
            if (string.IsNullOrEmpty(fen)
                || fen.Split(' ').Length != 6)
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
            var ranks = piecePlacement.Split('/').Reverse().ToArray();
            if (ranks.Length != 8)
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
            if ((piecePlacement.Select(x => x).Where(x => !ValidFENChars.Contains(x)).Select(x => x.ToString()).ToArray()).Any())
            {
                return FENError.PiecePlacementInvalidChars;
            }
            return FENError.Null;
        }

        public static FENError ValidateActiveColor(string activeColor) => (new[] { "w", "b" }).Contains(activeColor) ? FENError.Null : FENError.InvalidActiveColor;

        public static FENError ValidateCastlingAvailabilityString(string castleAvailability)
        {
            FENError fenError = FENError.Null;
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
                if (castleAvailability.Contains('-'))
                {
                    fenError |= FENError.CastlingStringBadSequence;
                }
                else
                {
                    var castleAvailabilityArray = castleAvailability.ToArray();
                    if (castleAvailabilityArray.Length != castleAvailabilityArray.Distinct().Count())
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
            return uint.TryParse(moveCounter, out _);
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

        public static string GetFENPiece(this string fen, FENPieces piece)
        {
            var fenPieces = fen.Split(' ');
            return fenPieces[(int)piece];
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
                default: throw new FENException(v, FENError.InvalidActiveColor);
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
            if (caBitFlags == 0) s = CastlingAvailability.NoCastlingAvailable.AsString(EnumFormat.Description);
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
            var rankOffset = ((ushort)(idx / 8)).RankCompliment();
            return (rankOffset * 8) + (idx % 8);
        }
    }
}
