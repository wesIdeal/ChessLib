﻿using ChessLib.Data.Types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Data.MoveRepresentation;
using System.Text;
using System.Collections.Generic;

namespace ChessLib.Data.Helpers
{
    public static class BoardHelpers
    {
        #region Constant Piece Values for Indexing arrays
        // ReSharper disable InconsistentNaming
        public const int PAWN = (int)Piece.Pawn;
        public const int BISHOP = (int)Piece.Bishop;
        public const int KNIGHT = (int)Piece.Knight;
        public const int ROOK = (int)Piece.Rook;
        public const int QUEEN = (int)Piece.Queen;
        // ReSharper restore InconsistentNaming
        #endregion
        public static Color Toggle(this Color c) => c == Color.White ? Color.Black : Color.White;
        public const int KING = (int)Piece.King;
        public const int WHITE = (int)Color.White;
        public const int BLACK = (int)Color.Black;
        public static readonly Board IndividualSquares =
        new Board()
        {
            MoveBoard = new ulong[]{
            0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
            0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000,
            0x8000, 0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000,
            0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000, 0x8000000, 0x10000000,
            0x20000000, 0x40000000, 0x80000000, 0x100000000, 0x200000000, 0x400000000, 0x800000000,
            0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000, 0x10000000000, 0x20000000000, 0x40000000000,
            0x80000000000, 0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000, 0x1000000000000, 0x2000000000000,
            0x4000000000000, 0x8000000000000, 0x10000000000000, 0x20000000000000, 0x40000000000000, 0x80000000000000, 0x100000000000000,
            0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,
        }
        };

        public static ulong[] RankMasks = {
            0xff,               //R1
            0xff00,             //R2
            0xff0000,           //R3
            0xff000000,         //R4
            0xff00000000,       //R5
            0xff0000000000,     //R6
            0xff000000000000,   //R7
            0xff00000000000000  //R8
        };

        public static ulong[] FileMasks = {
            0x101010101010101,  //A
            0x202020202020202,  //B
            0x404040404040404,  //C
            0x808080808080808,  //D
            0x1010101010101010, //E
            0x2020202020202020, //F
            0x4040404040404040, //G
            0x8080808080808080  //H
        };

        private static readonly ulong[,] ArrInBetween = new ulong[64, 64];

        static BoardHelpers()
        {
            InitializeInBetween();
        }



        private static void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
            {
                for (var t = f; t < 64; t++)
                {
                    const long m1 = (-1);
                    const long aFileBorder = (0x0001010101010100);
                    const long b2DiagonalBorder = (0x0040201008040200);
                    const long hFileBorder = (0x0002040810204080);

                    var between = (m1 << f) ^ (m1 << t);
                    long file = (t & 7) - (f & 7);
                    long rank = ((t | 7) - f) >> 3;
                    var line = ((file & 7) - 1) & aFileBorder;
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2DiagonalBorder; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & hFileBorder; /* h1b7 if same anti-diagonal */
                    line *= between & -between; /* mul acts like shift by smaller square */
                    ArrInBetween[f, t] = (ulong)(line & between);   /* return the bits on that line in-between */
                }
            }
        }

        public static ulong InBetween(int from, int to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return ArrInBetween[square1, square2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Occupancy(this ulong[][] board, Color? c = null, Piece? p = null)
        {
            if (c == null && p == null)
                return board.Select(x => x.Aggregate((acc, val) => acc |= val)).Aggregate((acc, val) => acc |= val);
            if (c == null)
                return board[(int)Color.White][(int)p] | board[(int)Color.Black][(int)p];
            if (p == null)
            {
                return board[(int)c].Aggregate((curr, val) => curr |= val);
            }

            return board[(int)c][(int)p];
        }

        #region Initialization

        //private static void InitializeIndividualSquares()
        //{
        //    for (int i = 0; i < 64; i++)
        //    {
        //        IndividualSquares[i / 8, i % 8] = (ulong)1 << i;
        //    }
        //}

        #endregion

        #region Enum ToInt() methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Color c) => (int)c;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Piece p) => (int)p;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this File f) => (int)f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Rank r) => (int)r;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexDisplay(this ulong u, bool appendHexNotation = true, bool pad = false, int padSize = 64)
        {
            var str = Convert.ToString((long)u, 16);
            if (pad)
            {
                str = str.PadLeft(padSize, '0');
            }
            if (appendHexNotation)
            {
                str = "0x" + str;
            }
            return str;
        }
        #endregion

        #region Array Position to Friendly Position Helpers

        public static ushort? SquareTextToIndex(this string square)
        {
            if (square.Trim() == "-")
            {
                return null;
            }
            if (square.Length != 2)
            {
                throw new ArgumentException($"Square passed to SquareTextToIndex(), {square} has an invalid length.");
            }
            var file = Char.ToLower(square[0]);
            var rank = UInt16.Parse(square[1].ToString());
            if (!Char.IsLetter(file) || file < 'a' || file > 'h')
            {
                throw new ArgumentException("File portion of square-text should be a letter, between 'a' and 'h'.");
            }
            if (rank < 1 || rank > 8)
            {
                throw new ArgumentException("Rank portion of square-text should be a digit with a value between 1 and 8.");
            }
            var rankMultiplier = rank - 1;
            return (ushort)((rankMultiplier * 8) + file - 'a');
        }

        public static ushort RankAndFileToIndex(ushort rank, ushort file)
        {
            return (ushort)((rank * 8) + file);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static File GetFile(this int square)
        {
            return (File)(square % 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetFile(this ushort square)
        {
            return (ushort)(square % 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rank GetRank(this int square)
        {
            Debug.Assert(square >= 0 && square < 64);
            return (Rank)((ushort)square).GetRank();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetRank(this ushort square)
        {
            return (ushort)(square / 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankFromIdx(this ushort idx) => (ushort)(idx / 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(this ushort idx) => (ushort)(idx % 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankCompliment(this ushort rank) => (ushort)Math.Abs(rank - 7);

        #endregion

        /// <summary>
        /// Returns type of piece at the given index
        /// </summary>
        /// <param name="idx">board index</param>
        /// <param name="occupancy">occupancy arrays</param>
        /// <returns>Type of Piece, if found, otherwise null</returns>
        public static Piece? GetTypeOfPieceAtIndex(ushort idx, in ulong[][] occupancy)
        {
            var val = 1ul << idx;
            foreach (var color in Enum.GetValues(typeof(Color)))
            {
                foreach (var piece in (Piece[])Enum.GetValues(typeof(Piece)))
                {
                    if ((val & occupancy[(int)color][(int)piece]) != 0)
                        return piece;
                }
            }
            return null;
        }

        public static PieceOfColor? GetPieceOfColorAtIndex(ushort idx, in ulong[][] occupancy)
        {
            var val = 1ul << idx;
            foreach (var color in Enum.GetValues(typeof(Color)))
            {
                foreach (var piece in (Piece[])Enum.GetValues(typeof(Piece)))
                {
                    if ((val & occupancy[(int)color][(int)piece]) != 0)
                        return new PieceOfColor() { Color = (Color)color, Piece = piece };
                }
            }
            return null;
        }

        public static ulong FlipVertically(this ulong board)
        {
            var x = board;
            return (x << 56) |
                    ((x << 40) & 0x00ff000000000000) |
                    ((x << 24) & 0x0000ff0000000000) |
                    ((x << 8) & 0x000000ff00000000) |
                    ((x >> 8) & 0x00000000ff000000) |
                    ((x >> 24) & 0x0000000000ff0000) |
                    ((x >> 40) & 0x000000000000ff00) |
                    (x >> 56);
        }

        public static ushort FlipIndexVertically(this ushort idx)
        {
            var rank = idx.RankFromIdx();
            var file = idx.FileFromIdx();
            var rankCompliment = rank.RankCompliment();
            return (ushort)((rankCompliment * 8) + file);
        }

        public static ulong[][] GetBoardPostMove(in ulong[][] currentBoard, in Color activePlayerColor, in MoveExt move)
        {
            var nActiveColor = (int)activePlayerColor;
            var opponentColor = activePlayerColor.Toggle();
            var nOppColor = (int)opponentColor;
            var resultantBoard = new ulong[2][];
            var pieceMoving = GetTypeOfPieceAtIndex(move.SourceIndex, currentBoard);
            for (var i = 0; i < 2; i++)
            {
                resultantBoard[i] = new ulong[6];
                foreach (var p in Enum.GetValues(typeof(Piece)))
                {
                    resultantBoard[i][(int)p] = currentBoard[i][(int)p];
                    if (i == nActiveColor && (Piece)p == pieceMoving)
                    {
                        resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.SourceIndex);
                        resultantBoard[i][(int)p] = resultantBoard[i][(int)p].SetBit(move.DestinationIndex);
                    }
                    else if (i == (int)opponentColor)
                    {
                        resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.DestinationIndex);
                    }
                }
            }
            if (move.MoveType == MoveType.Castle)
            {
                resultantBoard[nActiveColor][ROOK] = GetRookBoardPostCastle(move, resultantBoard[nActiveColor][ROOK]);
            }
            else if (move.MoveType == MoveType.EnPassant)
            {
                var capturedPawnValue = 1ul << (opponentColor == Color.Black ? move.DestinationIndex - 8 : move.DestinationIndex + 8);
                resultantBoard[nOppColor][PAWN] &= ~(capturedPawnValue);
            }
            else if (move.MoveType == MoveType.Promotion)
            {
                resultantBoard[nActiveColor][PAWN] &= ~(move.DestinationValue);
                switch (move.PromotionPiece)
                {
                    case PromotionPiece.Knight:
                        resultantBoard[nActiveColor][KNIGHT] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Bishop:
                        resultantBoard[nActiveColor][BISHOP] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Rook:
                        resultantBoard[nActiveColor][ROOK] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Queen:
                        resultantBoard[nActiveColor][QUEEN] |= move.DestinationValue;
                        break;
                }
            }
            return resultantBoard;
        }

        private static ulong GetRookBoardPostCastle(MoveExt move, ulong rookBoard)
        {
            var rank = move.DestinationIndex.RankFromIdx();
            var file = move.DestinationIndex.FileFromIdx();
            var rookSource = rank == 7      // black castling
                ? file == 2
                    ? 0x100000000000000ul           // BLACK O-O-O
                    : 0x8000000000000000ul      // BLACK O-O
                : file == 2
                    ? 0x01ul                        // WHITE O-O-O
                    : 0x80ul;                   // WHITE O-O

            var rookDest = rank == 7        // black castling
                ? file == 2
                    ? 0x800000000000000ul           // BLACK O-O-O
                    : 0x2000000000000000ul      // BLACK O-O
                : file == 2
                    ? 0x08ul                        // WHITE O-O-O
                    : 0x20ul;                   // WHITE O-O

            return (rookBoard & ~(rookSource)) | rookDest;
        }


        public static bool CanEvadeThroughBlockOrCapture(Color kingColor, ulong[][] pieceOnBoard)
        {
            return GetEvasions(kingColor, pieceOnBoard).Any();
        }

        public static MoveExt[] GetEvasions(Color kingColor, ulong[][] piecesOnBoard)
        {
            var rv = new List<MoveExt>();
            var nColor = (int)kingColor;

            var nOppColor = (int)kingColor.Toggle();
            var kingIndex = piecesOnBoard[nColor][KING].GetSetBits()[0];
            var totalOcc = piecesOnBoard.Occupancy();
            var activeOccupancy = piecesOnBoard.Occupancy(kingColor);
            var oppOccupancy = piecesOnBoard.Occupancy((Color)nOppColor);
            var piecesAttacking = AttackersOn(kingIndex, piecesOnBoard) & piecesOnBoard.Occupancy(kingColor.Toggle());
            var attackerIndexes = piecesAttacking.GetSetBits();
            var pseudoLegalKingMoves = Bitboard.GetPseudoLegalMoves(Piece.King, kingIndex, activeOccupancy, oppOccupancy, kingColor);

            //double check - king must move
            foreach (var mv in pseudoLegalKingMoves.GetSetBits())
            {
                var move = MoveHelpers.GenerateMove(kingIndex, mv);
                var board = BoardHelpers.GetBoardPostMove(piecesOnBoard, kingColor, move);
                if (!Bitboard.IsAttackedBy((Color)nOppColor, mv, board))
                {
                    rv.Add(move);
                }
            }

            if (attackerIndexes.Length == 1)
            {
                foreach (var attackerIdx in attackerIndexes)
                {
                    var activeOccupancyNotKing = activeOccupancy & ~(kingIndex.GetBoardValueOfIndex());
                    foreach (var occupiedSquare in activeOccupancyNotKing.GetSetBits())
                    {
                        ulong destination;
                        var piece = BoardHelpers.GetPieceOfColorAtIndex(occupiedSquare, piecesOnBoard);
                        var attackedSquares =
                            Bitboard.GetPseudoLegalMoves(piece.Value.Piece, occupiedSquare, activeOccupancy, oppOccupancy, kingColor);
                        var squaresBetween = BoardHelpers.InBetween(kingIndex, attackerIdx);
                        if ((destination = (attackedSquares & squaresBetween)) != 0)
                        {
                            var destIdx = destination.GetSetBits();
                            Debug.Assert(destIdx.Length == 1);
                            var move = MoveHelpers.GenerateMove(occupiedSquare, destIdx[0]);
                            rv.Add(move);
                        }
                    }
                }
            }

            return rv.ToArray();
        }

        private static ulong AttackersOn(in ushort i, in ulong[][] piecesOnBoard)
        {

            var total = piecesOnBoard
                .Select(color => color.Aggregate((current, x) => current |= x))
                .Aggregate((current, x) => current |= x);
            var pawnWhite = piecesOnBoard[WHITE][PAWN];
            var pawnBlack = piecesOnBoard[BLACK][PAWN];
            var knight = piecesOnBoard[BLACK][KNIGHT] | piecesOnBoard[WHITE][KNIGHT];
            var bishop = piecesOnBoard[BLACK][BISHOP] | piecesOnBoard[WHITE][BISHOP];
            var rook = piecesOnBoard[BLACK][ROOK] | piecesOnBoard[WHITE][ROOK];
            var queen = piecesOnBoard[BLACK][QUEEN] | piecesOnBoard[WHITE][QUEEN];
            var king = piecesOnBoard[BLACK][KING] | piecesOnBoard[WHITE][KING];
            return (Bitboard.GetAttackedSquares(Piece.Pawn, i, total, Color.Black) & pawnWhite)
                   | (Bitboard.GetAttackedSquares(Piece.Pawn, i, total, Color.White) & pawnBlack)
                   | (Bitboard.GetAttackedSquares(Piece.Knight, i, total) & knight)
                   | (Bitboard.GetAttackedSquares(Piece.Bishop, i, total) & bishop)
                   | (Bitboard.GetAttackedSquares(Piece.Rook, i, total) & rook)
                   | (Bitboard.GetAttackedSquares(Piece.Queen, i, total) & (queen))
                   | (Bitboard.GetAttackedSquares(Piece.King, i, total) & king);

        }


        public static string GetPiecePlacement(this ulong[][] piecesOnBoard)
        {
            var pieceSection = new char[64];
            for (var iColor = 0; iColor < 2; iColor++)
                for (var iPiece = 0; iPiece < 6; iPiece++)
                {
                    var pieceArray = piecesOnBoard[iColor][iPiece];
                    var charRepForPieceOfColor = PieceHelpers.GetCharRepresentation((Color)iColor, (Piece)iPiece);
                    while (pieceArray != 0)
                    {
                        var squareIndex = BitHelpers.BitScanForward(pieceArray);
                        var fenIndex = FENHelpers.BoardIndexToFENIndex(squareIndex);
                        pieceSection[fenIndex] = charRepForPieceOfColor;
                        pieceArray &= pieceArray - 1;
                    }
                }

            var sb = new StringBuilder();
            for (var rank = 0; rank < 8; rank++) //start at FEN Rank of zero -> 7
            {
                var emptyCount = 0;
                for (var file = 0; file < 8; file++)
                {
                    var paChar = pieceSection[(rank * 8) + file];
                    if (paChar == 0)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount != 0)
                        {
                            sb.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }

                        sb.Append(paChar);
                    }
                }
                if (emptyCount != 0) sb.Append(emptyCount);
                if (rank != 7) sb.Append('/');
            }
            return sb.ToString();
        }
    }
}
