using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.MagicBitboard.MovingPieces;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation;
using EnumsNET;
[assembly: InternalsVisibleTo("ChessLib.Core.Tests.MagicBitboard.MovingPieces")]
namespace ChessLib.Core.MagicBitboard
{

    public sealed class Bitboard
    {
        private static readonly List<string> LLock = new List<string>();
        private static Bitboard instance;
        private readonly Pawn _pawn;
        private readonly Knight _knight;
        internal readonly SlidingPiece Bishop;
        internal readonly SlidingPiece Rook;
        private readonly IMovingPiece _king;

        private Bitboard()
        {
            Rook = new Rook();
            Bishop = new Bishop();
            _pawn = new Pawn();
            _king = new King();
            _knight = new Knight();
        }

        public static Bitboard Instance
        {
            get
            {
                lock (LLock)
                {
                    if (instance == null)
                    {
                        instance = new Bitboard();
                    }

                    return instance;
                }
            }
        }

        public IEnumerable<IMove> GetLegalMoves(ushort squareIndex, Piece piece, Color color, ulong[][] occupancy, ushort? enPassantIdx,
            CastlingAvailability castlingAvailability)
        {
            var pseudoLegalMoves = GetPseudoLegalMoves(squareIndex, piece, color, occupancy.Occupancy());
            var moves = new List<IMove>();
            var squareValue = squareIndex.GetBoardValueOfIndex();
            var moveType = IsPromotion(piece, color, squareValue) ? MoveType.Promotion : MoveType.Normal;
            foreach (var plMove in pseudoLegalMoves.GetSetBits())
            {
                moves.Add(MoveHelpers.GenerateMove(squareIndex, plMove, moveType));
            }

            if (piece == Piece.Pawn && enPassantIdx.HasValue &&
                (enPassantIdx.Value.GetBoardValueOfIndex() & pseudoLegalMoves) != 0)
            {
                moves.Add(MoveHelpers.GenerateMove(squareIndex, enPassantIdx.Value, MoveType.EnPassant));
            }

            if (piece == Piece.King && (IsKingSquare(squareIndex, color)))
            {
                moves.AddRange(GetPseudoLegalCastlingMoves(color, castlingAvailability));
            }

            foreach (var move in moves)
            {
                var moveValidator =
                    new MoveValidator(new Board(occupancy, 0, enPassantIdx, null, castlingAvailability, color, 0), move);
                var validationResult = moveValidator.Validate();
                if (validationResult == MoveError.NoneSet)
                {
                    yield return move;
                }
            }
        }

        private IEnumerable<IMove> GetPseudoLegalCastlingMoves(Color color, CastlingAvailability castlingAvailability)
        {
            var moves = new List<Move>(2);
            switch (color)
            {
                case Color.White:
                    if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside))
                    {
                        yield return MoveHelpers.WhiteCastleKingSide;
                    }

                    if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside))
                    {
                        yield return MoveHelpers.WhiteCastleQueenSide;
                    }
                    break;
                case Color.Black:
                    if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside))
                    {
                        yield return MoveHelpers.BlackCastleKingSide;
                    }

                    if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside))
                    {
                        yield return MoveHelpers.BlackCastleQueenSide;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        private bool IsKingSquare(ushort squareIndex, Color color)
        {
            switch (color)
            {
                case Color.White:
                    return squareIndex == BoardConstants.WhiteKingSquare;
                case Color.Black:
                    return squareIndex == BoardConstants.BlackKingSquare;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        private static bool IsPromotion(Piece piece, Color color, ulong squareValue)
        {
            return (squareValue & BoardConstants.Rank7) == squareValue && color == Color.White ||
                   (squareValue & BoardConstants.Rank2) == squareValue && color == Color.Black && piece == Piece.Pawn;
        }

        public ulong GetPseudoLegalMoves(ushort squareIndex, Piece bishop, Color color, ulong occupancy)
        {
            switch (bishop)
            {
                case Piece.Pawn:
                    return _pawn.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Knight:
                    return _knight.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Bishop:
                    return Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Rook:
                    return Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Queen:
                    var rookMoves = Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    var bishopMoves = Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    return rookMoves | bishopMoves;
                case Piece.King:
                    return _king.GetPseudoLegalMoves(squareIndex, color, occupancy);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bishop), bishop, null);
            }
        }

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="color"/>
        /// </summary>
        /// <param name="squareIndex">Index of possible attack target</param>
        /// <param name="color">Color of attacker</param>
        /// <param name="piecesOnBoard">Occupancy arrays for both colors, indexed as [color_enum][piece_enum]</param>
        /// <returns>true if <paramref name="squareIndex"/> is attacked by any piece of <paramref name="color"/></returns>
        public bool IsSquareAttackedByColor(ushort squareIndex, Color color, ulong[][] piecesOnBoard)
        {
            var occupancy = piecesOnBoard.Occupancy();
            var squareVal = squareIndex.GetBoardValueOfIndex();
            foreach (var p in Enums.GetValues<Piece>())
            {
                var moves = GetPseudoLegalMoves(squareIndex, p, color, occupancy);
                if ((moves & squareVal) == squareVal)
                {
                    return true;
                }
            }
            return false;
        }
        public  ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color attackingColor)
        {
            var occupancyWithoutAttackingPiece = occupancy & ~(pieceIndex.GetBoardValueOfIndex());
            var pseudoLegalMoves = GetPseudoLegalMoves(pieceIndex, piece, attackingColor, occupancyWithoutAttackingPiece );
            if (piece == Piece.Pawn)
            {
                pseudoLegalMoves &= ~(_pawn.GetMovesFromSquare(pieceIndex, attackingColor));
            }

            return pseudoLegalMoves;
        }
        public ulong PiecesAttackingSquare(in ulong[][] piecesOnBoard, in ushort squareIndex)
        {

            var total = piecesOnBoard.Occupancy();
            var pawnWhite = piecesOnBoard[BoardConstants.White][BoardConstants.Pawn];
            var pawnBlack = piecesOnBoard[BoardConstants.Black][BoardConstants.Pawn];
            var knight = piecesOnBoard[BoardConstants.Black][BoardConstants.Knight] | piecesOnBoard[BoardConstants.White][BoardConstants.Knight];
            var bishop = piecesOnBoard[BoardConstants.Black][BoardConstants.Bishop] | piecesOnBoard[BoardConstants.White][BoardConstants.Bishop];
            var rook = piecesOnBoard[BoardConstants.Black][BoardConstants.Rook] | piecesOnBoard[BoardConstants.White][BoardConstants.Rook];
            var queen = piecesOnBoard[BoardConstants.Black][BoardConstants.Queen] | piecesOnBoard[BoardConstants.White][BoardConstants.Queen];
            var king = piecesOnBoard[BoardConstants.Black][BoardConstants.King] | piecesOnBoard[BoardConstants.White][BoardConstants.King];
            var blackPawnPseudoAttacks = _pawn.GetAttacksFromSquare(squareIndex, Color.White);
            var blackPawnAttacks = blackPawnPseudoAttacks & pawnBlack;
            var whitePawnPseudoAttacks = _pawn.GetAttacksFromSquare(squareIndex, Color.Black);
            var whitePawnAttacks = whitePawnPseudoAttacks & pawnWhite;
            var rookAttackedSquares = (GetAttackedSquares(Piece.Rook, squareIndex, total, Color.White) & rook);
            var allAttacks = blackPawnAttacks
                             | whitePawnAttacks
                             | (GetAttackedSquares(Piece.Knight, squareIndex, total, Color.White) & knight)
                             | (GetAttackedSquares(Piece.Bishop, squareIndex, total, Color.White) & bishop)
                             | rookAttackedSquares
                             | (GetAttackedSquares(Piece.Queen, squareIndex, total, Color.White) & queen)
                             | (GetAttackedSquares(Piece.King, squareIndex, total, Color.White) & king);
            return allAttacks;
        }

        private bool IsSquareAttackedBySlidingPiece(ushort attackedSquare, Color attackerColor, ulong[][] piecesOnBoard)
        {
            var bishopAttackSquares = GetPseudoLegalMoves(attackedSquare, Piece.Bishop, attackerColor.Toggle(), piecesOnBoard.Occupancy());
            var rookAttackSquares = GetPseudoLegalMoves(attackedSquare, Piece.Bishop, attackerColor.Toggle(), piecesOnBoard.Occupancy());
            return
                (rookAttackSquares & (piecesOnBoard[(int)attackerColor][PieceHelpers.Rook] |
                                      piecesOnBoard[(int)attackerColor][PieceHelpers.Queen])) != 0
                ||
                (bishopAttackSquares & (piecesOnBoard[(int)attackerColor][PieceHelpers.Bishop] |
                                        piecesOnBoard[(int)attackerColor][PieceHelpers.Queen])) != 0;
        }


    }
}


