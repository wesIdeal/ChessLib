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
        // ReSharper disable once CollectionNeverUpdated.Local - used as lock
        private static readonly List<string> LLock = new List<string>();
        private static Bitboard instance;

        public static Bitboard Instance
        {
            get
            {
                lock (LLock)
                {
                    return instance ??= new Bitboard();
                }
            }
        }

        private Bitboard()
        {
            Rook = new Rook();
            Bishop = new Bishop();
            _pawn = new Pawn();
            _king = new King();
            _knight = new Knight();
        }

        private readonly IMovingPiece _king;
        private readonly Knight _knight;
        private readonly Pawn _pawn;
        internal readonly SlidingPiece Bishop;
        internal readonly SlidingPiece Rook;

        public IEnumerable<IMove> GetLegalMoves(ushort squareIndex, ulong[][] occupancy, ushort? enPassantIdx,
            CastlingAvailability castlingAvailability)
        {
            var pieceOfColor = occupancy.GetPieceOfColorAtIndex(squareIndex);
            if (pieceOfColor == null)
            {
                yield break;
            }

            var piece = pieceOfColor.Value.Piece;
            var color = pieceOfColor.Value.Color;
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

            if (piece == Piece.King && IsKingSquare(squareIndex, color))
            {
                moves.AddRange(GetPseudoLegalCastlingMoves(color, castlingAvailability));
            }

            foreach (var move in moves)
            {
                var moveValidator =
                    new MoveValidator(
                        new Board(occupancy, 0, enPassantIdx, null, castlingAvailability, color, 0, false),
                        move);
                var validationResult = moveValidator.Validate();
                if (validationResult == MoveError.NoneSet)
                {
                    yield return move;
                }
            }
        }

        private IEnumerable<IMove> GetPseudoLegalCastlingMoves(Color color, CastlingAvailability castlingAvailability)
        {
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
        ///     Determines if piece on <paramref name="squareIndex" /> is attacked by <paramref name="color" />
        /// </summary>
        /// <param name="squareIndex">Index of possible attack target</param>
        /// <param name="color">Color of attacker</param>
        /// <param name="piecesOnBoard">Occupancy arrays for both colors, indexed as [color_enum][piece_enum]</param>
        /// <returns>true if <paramref name="squareIndex" /> is attacked by any piece of <paramref name="color" /></returns>
        public bool IsSquareAttackedByColor(ushort squareIndex, Color color, ulong[][] piecesOnBoard)
        {
            var occupancy = piecesOnBoard.Occupancy();
            foreach (var p in Enums.GetValues<Piece>())
            {
                var moves = GetPseudoLegalMoves(squareIndex, p, color.Toggle(), occupancy);
                if (p == Piece.Pawn)
                {
                    moves = moves & ~BoardConstants.FileMasks[squareIndex.GetFile()];
                }

                var squareAttackers = piecesOnBoard.Occupancy(color, p) & moves;
                if ((squareAttackers & moves) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color attackingColor)
        {
            var occupancyWithoutAttackingPiece = occupancy & ~pieceIndex.GetBoardValueOfIndex();
            var pseudoLegalMoves =
                GetPseudoLegalMoves(pieceIndex, piece, attackingColor, occupancyWithoutAttackingPiece);
            if (piece == Piece.Pawn)
            {
                pseudoLegalMoves &= ~_pawn.GetMovesFromSquare(pieceIndex, attackingColor);
            }

            return pseudoLegalMoves;
        }

        public ulong PiecesAttackingSquareByColor(in ulong[][] occupancy, in ushort squareIndex,
            Color? attackerColor = null)
        {
            var total = occupancy.Occupancy();
            var attackerColorMask = attackerColor.HasValue ? occupancy.Occupancy(attackerColor) : ulong.MaxValue;
            var pawnWhite = occupancy[BoardConstants.White][BoardConstants.Pawn];
            var pawnBlack = occupancy[BoardConstants.Black][BoardConstants.Pawn];
            var knight = occupancy[BoardConstants.Black][BoardConstants.Knight] |
                         occupancy[BoardConstants.White][BoardConstants.Knight];
            var bishop = occupancy[BoardConstants.Black][BoardConstants.Bishop] |
                         occupancy[BoardConstants.White][BoardConstants.Bishop];
            var rook = occupancy[BoardConstants.Black][BoardConstants.Rook] |
                       occupancy[BoardConstants.White][BoardConstants.Rook];
            var queen = occupancy[BoardConstants.Black][BoardConstants.Queen] |
                        occupancy[BoardConstants.White][BoardConstants.Queen];
            var king = occupancy[BoardConstants.Black][BoardConstants.King] |
                       occupancy[BoardConstants.White][BoardConstants.King];
            var blackPawnPseudoAttacks = _pawn.GetAttacksFromSquare(squareIndex, Color.White);
            var blackPawnAttacks = blackPawnPseudoAttacks & pawnBlack;
            var whitePawnPseudoAttacks = _pawn.GetAttacksFromSquare(squareIndex, Color.Black);
            var whitePawnAttacks = whitePawnPseudoAttacks & pawnWhite;
            var rookAttackedSquares = GetAttackedSquares(Piece.Rook, squareIndex, total, Color.White) & rook;
            var allAttacks = (blackPawnAttacks
                              | whitePawnAttacks
                              | (GetAttackedSquares(Piece.Knight, squareIndex, total, Color.White) & knight)
                              | (GetAttackedSquares(Piece.Bishop, squareIndex, total, Color.White) & bishop)
                              | rookAttackedSquares
                              | (GetAttackedSquares(Piece.Queen, squareIndex, total, Color.White) & queen)
                              | (GetAttackedSquares(Piece.King, squareIndex, total, Color.White) & king)) &
                             attackerColorMask;
            return allAttacks;
        }


        internal ulong GetPiecesThatCanMoveToSquare(ulong[][] occupancy, ushort squareIndex,
            Color pieceColorMovingToSquare)
        {
            var total = occupancy.Occupancy();
            var pawnBlack = occupancy[(int) pieceColorMovingToSquare][BoardConstants.Pawn];
            var knight = occupancy[(int) pieceColorMovingToSquare][BoardConstants.Knight];
            var bishop = occupancy[(int) pieceColorMovingToSquare][BoardConstants.Bishop];
            var rook = occupancy[(int) pieceColorMovingToSquare][BoardConstants.Rook];
            var queen = occupancy[(int) pieceColorMovingToSquare][BoardConstants.Queen];
            var king = occupancy[(int) pieceColorMovingToSquare][BoardConstants.King];

            var pawnPseudoAttacks = _pawn.GetMovesFromSquare(squareIndex, pieceColorMovingToSquare.Toggle());
            var pawnAttacks = pawnPseudoAttacks & pawnBlack;
            var rookAttackedSquares = GetAttackedSquares(Piece.Rook, squareIndex, total, Color.White) & rook;
            var allAttacks = pawnAttacks
                             | (GetAttackedSquares(Piece.Knight, squareIndex, total, Color.White) & knight)
                             | (GetAttackedSquares(Piece.Bishop, squareIndex, total, Color.White) & bishop)
                             | rookAttackedSquares
                             | (GetAttackedSquares(Piece.Queen, squareIndex, total, Color.White) & queen)
                             | (GetAttackedSquares(Piece.King, squareIndex, total, Color.White) & king);
            return allAttacks;
        }
    }
}