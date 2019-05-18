﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.Data.Boards
{
    public abstract class BoardInformationService<T> : IBoardInformationService<T> where T : MoveStorage
    {
        public abstract MoveTree<T> MoveTree { get; set; }

        public MoveTree<T> Moves
        {
            get { return MoveTree; }
        }

        /// <summary>
        ///     Creates a BoardInfo object given FEN notation.
        /// </summary>
        /// <param name="fen">string of FEN notation.</param>
        /// <param name="chess960">[unused right now] Is this a chess960 position (possible non-standard starting position)</param>
        /// <returns>A BoardInfo object representing the board state after parsing the FEN</returns>
        public BoardInformationService(string fen, bool chess960 = false)
        {
            MoveTree = new MoveTree<T>(null);
            PiecesOnBoard = fen.BoardFromFen(out Color activePlayer, out CastlingAvailability castlingAvailability, out ushort? enPassantSquareIndex, out uint halfmoveClock, out uint fullMoveCount);
            ActivePlayerColor = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantIndex = enPassantSquareIndex;
            HalfmoveClock = halfmoveClock;
            MoveCounter = fullMoveCount;
            Chess960 = chess960;
            FEN = fen;
            MoveTree.FENStart = fen;
            ValidateFields();
        }





        public abstract void ApplyMove(string moveText);
        public abstract ulong GetAttackedSquares(Piece p, ushort index, ulong occupancy);
        #region General Board Information

        public ulong[][] PiecesOnBoard;
        public string FEN { get; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantIndex { get; set; }

        public readonly bool Chess960;

        #region Player Color Properties and conversions
        public Color ActivePlayerColor { get; set; }
        public Color OpponentColor => ActivePlayerColor.Toggle();
        protected int NActiveColor => (int)ActivePlayerColor;
        protected int NOpponentColor => (int)OpponentColor;


        #endregion

        #endregion

        #region Occupancy / Index Properties for shorthand access to occupancy board arrays

        /// <summary>
        ///     Occupancy of side-to-move's pawns
        /// </summary>
        public ulong ActivePawnOccupancy => PiecesOnBoard[NActiveColor][PAWN];

        /// <summary>
        ///     Occupancy of side-to-move's Knights
        /// </summary>
        public ulong ActiveKnightOccupancy => PiecesOnBoard[NActiveColor][KNIGHT];

        /// <summary>
        ///     Occupancy of side-to-move's Bishops
        /// </summary>
        public ulong ActiveBishopOccupancy => PiecesOnBoard[NActiveColor][BISHOP];

        /// <summary>
        ///     Occupancy of side-to-move's Rooks
        /// </summary>
        public ulong ActiveRookOccupancy => PiecesOnBoard[NActiveColor][ROOK];

        /// <summary>
        ///     Occupancy of side-to-move's Queen(s)
        /// </summary>
        public ulong ActiveQueenOccupancy => PiecesOnBoard[NActiveColor][QUEEN];

        /// <summary>
        ///     Index of side-to-move's King
        /// </summary>
        public ushort ActivePlayerKingIndex => KingIndex(ActivePlayerColor);

        /// <summary>
        ///     Value (occupancy board) of side-to-move's King
        /// </summary>
        public ulong ActivePlayerKingOccupancy => PiecesOnBoard[NActiveColor][KING];

        /// <summary>
        ///     Opponent's King's square index
        /// </summary>
        public ushort OpposingPlayerKingIndex => KingIndex(OpponentColor);


        #endregion


        public ulong Occupancy(Color? c = null)
        {
            if (c.HasValue)
            {
                var occ = PiecesOnBoard[(int)c];
                return occ.Aggregate<ulong, ulong>(0, (current, t) => current | t);
            }

            return PiecesOnBoard
                .Select(color => color.Aggregate((current, x) => current |= x))
                .Aggregate((current, x) => current |= x);
        }

        

        /// <summary>
        ///     Occupancy of side-to-move's pieces
        /// </summary>
        public ulong ActiveTotalOccupancy
        {
            get { return Occupancy(ActivePlayerColor); }
        }

        /// <summary>
        ///     Occupancy of opponent's pieces
        /// </summary>
        public ulong OpponentTotalOccupancy
        {
            get { return Occupancy(OpponentColor); }
        }

        /// <summary>
        ///     Total board occupancy
        /// </summary>
        public ulong TotalOccupancy => Occupancy();

        public ulong PawnOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][PAWN];
            return PiecesOnBoard[BLACK][PAWN] | PiecesOnBoard[WHITE][PAWN];
        }

        public ulong KnightOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][KNIGHT];
            return PiecesOnBoard[WHITE][KNIGHT] | PiecesOnBoard[BLACK][KNIGHT];
        }

        public ulong BishopOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][BISHOP];
            return PiecesOnBoard[WHITE][BISHOP] | PiecesOnBoard[BLACK][BISHOP];
        }

        public ulong RookOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][ROOK];
            return PiecesOnBoard[WHITE][ROOK] | PiecesOnBoard[BLACK][ROOK];
        }

        public ulong QueenOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][QUEEN];
            return PiecesOnBoard[WHITE][QUEEN] | PiecesOnBoard[BLACK][QUEEN];
        }

        public ulong KingOccupancy(Color? c = null)
        {
            if (c.HasValue) return PiecesOnBoard[(int)c][KING];
            return PiecesOnBoard[WHITE][KING] | PiecesOnBoard[BLACK][KING];
        }

        public ushort KingIndex(Color c)
        {
            return PiecesOnBoard[(int)c][KING].GetSetBits()[0];
        }


        private void ValidateFields()
        {
            var errors = new StringBuilder();
            errors.AppendLine(ValidateNumberOfPiecesOnBoard());
            errors.AppendLine(ValidateEnPassantSquare());
            errors.AppendLine(ValidateCastlingRights());
            errors.AppendLine(ValidateChecks());
        }

        public string ValidateNumberOfPiecesOnBoard(ulong[][] piecesOnBoard)
        {
            var message = new StringBuilder("");
            if (piecesOnBoard[Color.White.ToInt()].Sum(x => x.CountSetBits()) > 16)
                message.AppendLine("White has too many pieces on the board.");
            if (piecesOnBoard[Color.Black.ToInt()].Sum(x => x.CountSetBits()) > 16)
                message.AppendLine("Black has too many pieces on the board.");
            return message.ToString();
        }
        public static string ValidateEnPassantSquare(ulong[][] piecesOnBoard, ushort? enPassantSquare,
            Color activePlayer)
        {
            if (enPassantSquare == null) return "";
            if (activePlayer == Color.White && (enPassantSquare < 40 || enPassantSquare > 47)
                ||
                activePlayer == Color.Black && (enPassantSquare < 16 || enPassantSquare > 23))
                return "Bad En Passant Square detected.";
            return "";
        }


        public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability,
            bool chess960 = false)
        {
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return "";
            var message = new StringBuilder();
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var rook = ROOK;
            var king = (int)Piece.King;
            //Check for Rook placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) &&
                !piecesOnBoard[white][rook].IsBitSet(0))
                message.AppendLine("White cannot castle long with no Rook on a1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside) &&
                !piecesOnBoard[white][rook].IsBitSet(7))
                message.AppendLine("White cannot castle short with no Rook on h1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) &&
                !piecesOnBoard[black][rook].IsBitSet(56))
                message.AppendLine("Black cannot castle long with no Rook on a8.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside) &&
                !piecesOnBoard[black][rook].IsBitSet(63))
                message.AppendLine("Black cannot castle short with no Rook on h8.");

            //Check for King placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) ||
                castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside)
                && !piecesOnBoard[white][king].IsBitSet(4))
                message.AppendLine("White cannot castle without the King on e1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) ||
                castlingAvailability.HasFlag(CastlingAvailability.BlackKingside)
                && !piecesOnBoard[black][king].IsBitSet(60))
                message.AppendLine("Black cannot castle without the King on e1.");
            return message.ToString();
        }


        private string ValidateNumberOfPiecesOnBoard()
        {
            return ValidateNumberOfPiecesOnBoard(PiecesOnBoard);
        }

        private string ValidateEnPassantSquare()
        {
            return ValidateEnPassantSquare(PiecesOnBoard, EnPassantIndex, ActivePlayerColor);
        }

        private string ValidateCastlingRights()
        {
            return ValidateCastlingRights(PiecesOnBoard, CastlingAvailability, Chess960);
        }

        public string ValidateChecks()
        {
            var c = GetChecks(ActivePlayerColor);
            if (c.HasFlag(Check.Double))
            {
                return "Both Kings are in check.";
            }

            if (c.HasFlag(Check.Opposite)) return "Active side is in check.";
            return "";
        }

        protected abstract Check GetChecks(Color activePlayerColor);



        /// <summary>
        ///     Instance method to find if <paramref name="squareIndex" /> is attacked by a piece of <paramref name="color" />
        /// </summary>
        /// <param name="color">Color of possible attacker</param>
        /// <param name="squareIndex">Square which is possibly under attack</param>
        /// <returns>true if <paramref name="squareIndex" /> is attacked by <paramref name="color" />. False if not.</returns>
        public abstract bool IsAttackedBy(Color color, ushort squareIndex);

        public abstract bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece);

        #region Constant Piece Values for Indexing arrays
        // ReSharper disable InconsistentNaming
        protected const int PAWN = (int)Piece.Pawn;
        protected const int BISHOP = (int)Piece.Bishop;
        protected const int KNIGHT = (int)Piece.Knight;
        protected const int ROOK = (int)Piece.Rook;
        protected const int QUEEN = (int)Piece.Queen;
        protected const int KING = (int)Piece.King;

        protected const int BLACK = (int)Color.Black;

        protected const int WHITE = (int)Color.White;
        // ReSharper restore InconsistentNaming

        #endregion

        /// <summary>
        ///     Clears appropriate castling availability flag when <paramref name="movingPiece">piece moving</paramref> is a
        ///     <see cref="Piece.Rook">Rook</see> or <see cref="Piece.King">King</see>
        /// </summary>
        /// <param name="move">Move object</param>
        /// <param name="movingPiece">Piece that is moving</param>
        public void UnsetCastlingAvailability(MoveExt move, Piece movingPiece)
        {
            switch (movingPiece)
            {
                case Piece.Rook:
                    if (move.SourceIndex == 56) CastlingAvailability &= ~CastlingAvailability.BlackQueenside;
                    if (move.SourceIndex == 63) CastlingAvailability &= ~CastlingAvailability.BlackKingside;
                    if (move.SourceIndex == 0) CastlingAvailability &= ~CastlingAvailability.WhiteQueenside;
                    if (move.SourceIndex == 7) CastlingAvailability &= ~CastlingAvailability.WhiteKingside;
                    break;
                case Piece.King:
                    if (move.SourceIndex == 60)
                        CastlingAvailability &=
                            ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    if (move.SourceIndex == 4)
                        CastlingAvailability &=
                            ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    break;
            }
        }

        /// <summary>
        ///     Sets EnPassant flag appropriately, clearing it if no En Passant is available
        /// </summary>
        /// <param name="move"></param>
        /// <param name="pocSource"></param>
        public void SetEnPassantFlag(MoveExt move, PieceOfColor? pocSource)
        {
            if (pocSource.HasValue)
            {
                var startRank = pocSource.Value.Color == Color.White ? 1 : 6;
                var endRank = pocSource.Value.Color == Color.White ? 3 : 4;
                var enPassantIndexOffset = pocSource.Value.Color == Color.White ? 8 : -8;
                if (pocSource.Value.Piece == Piece.Pawn)
                    if ((move.SourceValue & BoardHelpers.RankMasks[startRank]) != 0
                        && (move.DestinationValue & BoardHelpers.RankMasks[endRank]) != 0)
                    {
                        EnPassantIndex = (ushort)(move.SourceIndex + enPassantIndexOffset);
                        return;
                    }
            }

            EnPassantIndex = null;
        }

    }
}
