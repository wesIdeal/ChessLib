using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using MagicBitboard.SlidingPieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MagicBitboard
{
    public class Bitboard
    {
        public readonly ulong[][] PiecesOnBoard = new ulong[2][];
        public readonly MovePatternStorage Bishop;
        public readonly MovePatternStorage Rook;
        private readonly char[] allowedFENChars = new char[] { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };
        public Bitboard()
        {
            Bishop = new MovePatternStorage();
            Bishop.Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
            Rook = new MovePatternStorage();
            Rook.Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
            PiecesOnBoard[0] = new ulong[6];
            PiecesOnBoard[1] = new ulong[6];
        }

        public Bitboard(string fen, bool chess960 = false)
        {
            var fenPieces = fen.Split(' ');

            if (fenPieces.Count() != 6)
            {
                throw new FENException($"Invalid FEN passed in. FEN needs 6 pieces to be valid.\r\nSee https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation");
            }
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            string[] invalidChars;
            if ((invalidChars = piecePlacement.Select(x => x).Where(x => !allowedFENChars.Contains(x)).Select(x => x.ToString()).ToArray()).Any())
            {
                throw new FENException($"Invalid characters in FEN string.\r\nReceived {fen} with the following invalid characters:\r\n{string.Join(", ", invalidChars)}");
            }
            var characterTotal = 0;
            var rankTotal = 0;
            var ranks = piecePlacement.Split('/').Reverse();
            if (ranks.Count() != 8)
            {
                throw new FENException($"Invalid number of ranks in FEN string.\r\nReceived {fen} with {ranks.Count()} ranks.");
            }
            var ranksValidation = ranks.Select((r, idx) => new { Rank = idx + 1, Count = getStringRepForRank(r).Count() });
            var badRanks = ranksValidation.Where(x => x.Count != 8);
            if (badRanks.Any())
            {
                throw new FENException($"Invalid Rank{(badRanks.Count() > 1 ? "s" : "")} in FEN {fen}.\r\n{string.Join("\r\n", badRanks.Select(r => "Rank " + r.Rank + " has " + r.Count + " pieces"))}");
            }
            Color activeColor = FENHelpers.GetActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            var enPassentSquareIndex = MoveHelpers.SquareTextToIndex(fenPieces[(int)FENPieces.EnPassentSquare]);
            var halfmoveClock = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.HalfmoveClock]);
            var fullMoveCount = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.FullMoveNumber]);
            
            foreach (var rank in ranks)
            {

                foreach (var f in rank)
                {
                    switch (Char.IsDigit(f))
                    {
                        case true:
                            var emptySquares = Convert.ToUInt16(f);
                            rankTotal += emptySquares;
                            characterTotal += emptySquares;
                            break;
                        case false:
                            var pieceOfColor = PieceOfColor.GetPieceOfColor(f);
                            PiecesOnBoard[(int)pieceOfColor.Color][(int)pieceOfColor.Piece] |= (1ul << characterTotal);
                            characterTotal++;
                            rankTotal++;
                            break;
                    }
                }
            }
        }



        private string getStringRepForRank(string rank)
        {
            var rv = "";
            foreach (var c in rank)
            {
                if (char.IsDigit(c))
                {
                    rv += new string('x', UInt16.Parse(c.ToString()));
                }
                else rv += "x";
            }
            return rv;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int Rank(ushort idx) => idx / 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int File(ushort idx) => idx % 8;

        public ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color color = Color.White)
        {
            var r = Rank(pieceIndex);
            var f = File(pieceIndex);
            var bishopSquares = Bishop.GetLegalMoves(pieceIndex, occupancy);
            var rookSquares = Rook.GetLegalMoves(pieceIndex, occupancy);
            switch (piece)
            {
                case Piece.Bishop:
                    return bishopSquares;
                case Piece.Rook:
                    return rookSquares;
                case Piece.Queen:
                    return bishopSquares | rookSquares;
                case Piece.Pawn:
                    return PieceAttackPatternHelper.PawnAttackMask[color.ToInt(), r, f];
                case Piece.King:
                    return PieceAttackPatternHelper.KingMoveMask[r, f];
                case Piece.Knight:
                    return PieceAttackPatternHelper.KnightAttackMask[r, f];
                default:
                    throw new Exception("Piece not supported for GetAttackSquares().");
            }
        }

        public ulong[] RookOccupancyBoards(ushort index) => Rook.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
        public ulong[] BishopOccupancyBoards(ushort index) => Bishop.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
    }
}
