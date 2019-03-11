using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicBitboard.Helpers
{
    public static class FENHelpers
    {
        public static readonly char[] ValidCastlingStringChars = new char[] { 'k', 'K', 'q', 'Q', '-' };
        public static readonly char[] ValidFENChars = new char[] { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };
        public static Color GetActiveColor(string v)
        {
            switch (v)
            {
                case "w": return Color.White;
                case "b": return Color.Black;
                default: throw new FENActiveColorException("Invalid active color in FEN. Color character received was " + v + ".");
            }
        }

        public static uint GetMoveNumberFromString(string v)
        {
            if (!uint.TryParse(v, out uint result))
            {
                throw new FENMoveNumberException($"Could not parse Halfmove Clock portion of FEN. Received {v}.");
            }
            return result;
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

        public static void ValidateCastlingAvailabilityString(string castleAvailability)
        {
            if (castleAvailability.Trim() == "") { throw new FENCastlingAvailabilityException($"Cannot get catling availability from empty string."); }
            var castlingChars = castleAvailability.ToCharArray();
            var notAllowed = castlingChars.Where(c => !ValidCastlingStringChars.Contains(c));
            if (notAllowed.Any()) { throw new FENCastlingAvailabilityException($"Found unallowed characters in castling string: {string.Join(", ", notAllowed.Select(x => x.ToString()))}."); }
            var castleAvailabilityArray = castleAvailability.ToArray();
            if (castleAvailabilityArray.Count() != castleAvailabilityArray.Distinct().Count())
            {
                throw new FENCastlingAvailabilityException($"Found a repeat of the same castling availability in FEN.\r\nCastle string was {castleAvailability}.");
            }
        }

        public static void ValidateFENStructure(string fen)
        {
            var fenPieces = fen.Split(' ');
            if (fenPieces.Count() != 6)
            {
                throw new FENException($"Invalid FEN passed in. FEN needs 6 pieces to be valid.\r\nSee https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation");
            }
        }

        public static void ValidateFENPiecePlacement(string piecePlacement)
        {
            ValidateFENPiecePlacementCharacters(piecePlacement);
            ValidateFENPiecePlacementRanks(piecePlacement);
            ValidateFENPiecePlacementKing(piecePlacement);
            ValidateFENPiecePlacementPawns(piecePlacement);
            ValidateNumberOfPieces(piecePlacement);
        }

        private static void ValidateNumberOfPieces(string piecePlacement)
        {

            var whitePieceCount = piecePlacement.Count(p => char.IsLetter(p) && char.IsUpper(p)) > 16;
            var blackPieceCount = piecePlacement.Count(p => char.IsLetter(p) && char.IsLower(p)) > 16;
            if (whitePieceCount || blackPieceCount)
            {
                var message = "Invalid FEN Piece Placement. ";
                if (whitePieceCount)
                {
                    message += "\r\nWhite has too many pieces, max is 16";
                }
                if (blackPieceCount)
                {
                    message += "\r\nBlack has too many pieces, max is 16";
                }
                message += $"Piece Placement: {piecePlacement}";
                throw new FENPieceCountTooHighException(message);
            }

        }

        public static void ValidateFENPiecePlacementKing(string piecePlacement)
        {
            var wkCount = piecePlacement.Count(p => p == 'K');
            var bkCount = piecePlacement.Count(p => p == 'k');
            if (wkCount != 1 || bkCount != 1)
            {
                var sideSpecificMessage = "";
                if (wkCount != 1 && bkCount != 1) sideSpecificMessage = "White and Black must have";
                else if (wkCount != 1) sideSpecificMessage = "White must have";
                else sideSpecificMessage = "Black must have";
                throw new FENPiecePlacementKingException($"Invalid King setup in piece placement portion of FEN. {sideSpecificMessage} one and only one king. Piece Placement: {piecePlacement}");
            }
        }

        public static void ValidateFENPiecePlacementPawns(string piecePlacement)
        {
            var wpCount = piecePlacement.Count(p => p == 'P');
            var bpCount = piecePlacement.Count(p => p == 'p');
            if (wpCount > 8 || bpCount > 8)
            {
                var sideSpecificMessage = "";
                if (wpCount > 8 && bpCount > 8) sideSpecificMessage = "White and Black have";
                else if (wpCount > 8) sideSpecificMessage = "White has";
                else sideSpecificMessage = "Black has";
                throw new FENPiecePlacementPawnException($"Invalid Pawn setup in piece placement portion of FEN. {sideSpecificMessage} more than 8 pawns on the board.\r\nPiece Placement: {piecePlacement}");
            }
            var ranks = piecePlacement.Split('/');
            if (ranks[0].Any(x => char.ToLower(x) == 'p') || ranks[7].Any(x => char.ToLower(x) == 'p'))
            {
                throw new FENPiecePlacementPawnException($"Invalid Pawn setup in piece placement portion of FEN. Pawns cannot be on the first or eighth ranks. Piece Placement: {piecePlacement}");

            }
        }

        private static void ValidateFENPiecePlacementRanks(string piecePlacement)
        {
            var ranks = piecePlacement.Split('/').Reverse();
            if (ranks.Count() != 8)
            {
                throw new FENPiecePlacementException($"Invalid number of ranks in FEN string.\r\nReceived {piecePlacement} with {ranks.Count()} ranks.");
            }
            var ranksValidation = ranks.Select((r, idx) => new { Rank = idx + 1, Count = getStringRepForRank(r) });
            var badRanks = ranksValidation.Where(x => x.Count != 8);

            if (badRanks.Any())
            {
                throw new FENPiecePlacementException($"Invalid Rank{(badRanks.Count() > 1 ? "s" : "")} in FEN {piecePlacement}.\r\n{string.Join("\r\n", badRanks.Select(r => "Rank " + r.Rank + " has " + r.Count + " pieces"))}");
            }
        }

        private static void ValidateFENPiecePlacementCharacters(string piecePlacement)
        {
            string[] invalidChars;
            if ((invalidChars = piecePlacement.Select(x => x).Where(x => !ValidFENChars.Contains(x)).Select(x => x.ToString()).ToArray()).Any())
            {
                throw new FENPiecePlacementException($"Invalid characters in FEN string.\r\nReceived {piecePlacement} with the following invalid characters:\r\n{string.Join(", ", invalidChars)}");
            }
        }

        public static BoardInfo BoardInfoFromFen(string fen, bool chess960 = false)
        {
            ValidateFENStructure(fen);
            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            ValidateFENPiecePlacement(fenPieces[(int)FENPieces.PiecePlacement]);

            ulong[][] pieces = new ulong[2][];
            pieces[(int)Color.White] = new ulong[6];
            pieces[(int)Color.Black] = new ulong[6];
            var ranks = piecePlacement.Split('/').Reverse();

            var activePlayer = FENHelpers.GetActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            var enPassentSquareIndex = MoveHelpers.SquareTextToIndex(fenPieces[(int)FENPieces.EnPassentSquare]);
            var halfmoveClock = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.HalfmoveClock]);
            var fullMoveCount = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.FullMoveNumber]);
            uint pieceIndex = 0;

            foreach (var rank in ranks)
            {
                foreach (var f in rank)
                {
                    switch (Char.IsDigit(f))
                    {
                        case true:
                            var emptySquares = uint.Parse(f.ToString());
                            pieceIndex += emptySquares;
                            break;
                        case false:
                            var pieceOfColor = PieceOfColor.GetPieceOfColor(f);
                            pieces[(int)pieceOfColor.Color][(int)pieceOfColor.Piece] |= (1ul << (int)pieceIndex);
                            pieceIndex++;
                            break;
                    }
                }
            }
            //if (m_stm == White && (m_epSquare < a6 || m_epSquare > h6))
            //{
            //    return InvalidEnPassant;
            //}
            //if (m_stm == Black && (m_epSquare < a3 || m_epSquare > h3))
            //{
            //    return InvalidEnPassant;
            //}
            if ((activePlayer == Color.White && (enPassentSquareIndex < 40 || enPassentSquareIndex > 47))
                ||
                (activePlayer == Color.Black && (enPassentSquareIndex < 16 || enPassentSquareIndex > 23)))
            {
                throw new FENException("Bad En Passent square in FEN.");
            }
            return new BoardInfo(pieces, activePlayer, GetCastlingFromString(fenPieces[(int)FENPieces.CastlingRights]),
                enPassentSquareIndex, halfmoveClock, fullMoveCount);
        }

        private static int getStringRepForRank(string rank)
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
    }
}
