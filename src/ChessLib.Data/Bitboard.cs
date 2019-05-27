using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChessLib.Data
{
    public static class Bitboard
    {

        public static readonly MovePatternStorage Bishop = new MovePatternStorage();
        public static readonly MovePatternStorage Rook = new MovePatternStorage();

        static Bitboard()
        {
            Bishop.Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
            Rook.Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank(ushort idx) => idx / 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int File(ushort idx) => idx % 8;

        private static bool IsCastlingMove(Piece p, ushort source, ushort destination) => (p == Piece.King && ((source == 50 && (destination == 62 || destination == 58)) ||
                                        (source == 4 && (destination == 6 || destination == 2))));

        private static bool IsEnPassantCapture(Piece p, ushort source, ushort destination, ushort? enPassantSq) => (enPassantSq == null) || (destination != enPassantSq.Value) || p != Piece.Pawn ? false : true;

        private static bool IsPromotion(Piece p, ushort source, ushort destination)
        {
            var sRank = source.RankFromIdx();
            var dRank = destination.RankFromIdx();
            return p == Piece.Pawn && ((sRank == 1 && dRank == 0) || (sRank == 6 && dRank == 7));
        }

        public static IEnumerable<MoveExt> BoardValueToMoves(Piece p, ushort source, ulong destinations,
            ushort? enPassantSq, CastlingAvailability ca)
        {
            var rv = new List<MoveExt>();
            foreach (var destination in destinations.GetSetBits())
            {
                var moveType = IsCastlingMove(p, source, destination) ? MoveType.Castle :
                    IsEnPassantCapture(p, source, destination, enPassantSq) ? MoveType.EnPassant :
                    IsPromotion(p, source, destination) ? MoveType.Promotion : MoveType.Normal;
                var promotionTypes = moveType == MoveType.Promotion
                    ? (PromotionPiece[])Enum.GetValues(typeof(PromotionPiece))
                    : new[] { PromotionPiece.Knight };
                rv.AddRange(promotionTypes.Select(pp => MoveHelpers.GenerateMove(source, destination, moveType, pp)));

            }

            return rv;
        }


        /// <summary>
        /// Gets both moves and attacks/captures for a piece
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pieceSquare"></param>
        /// <param name="activeOcc"></param>
        /// <param name="oppOcc"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ulong GetPseudoLegalMoves(Piece piece, ushort pieceSquare, ulong activeOcc, ulong oppOcc, Color color, ushort? enPassantIndex, CastlingAvailability ca, out List<MoveExt> moves)
        {
            var lMoves = new List<MoveExt>();
            var totalOccupancy = activeOcc | oppOcc;
            ulong possibleMoves;
            switch (piece)
            {
                case Piece.Pawn:
                    var opponentOccupancy = oppOcc | (enPassantIndex ?? 0);
                    var pawnMoves = PieceAttackPatternHelper.PawnMoveMask[(int)color][pieceSquare] & ~(totalOccupancy);
                    var pawnAttacks = PieceAttackPatternHelper.PawnAttackMask[(int)color][pieceSquare] & opponentOccupancy;
                    possibleMoves = pawnMoves | pawnAttacks;
                    break;
                case Piece.Knight:
                    var totalAttacks = PieceAttackPatternHelper.KnightAttackMask[pieceSquare];
                    possibleMoves = totalAttacks & ~(activeOcc);
                    break;
                case Piece.Bishop:
                    possibleMoves = Bishop.GetLegalMoves(pieceSquare, totalOccupancy) & ~(activeOcc);
                    break;
                case Piece.Rook:
                    possibleMoves = Rook.GetLegalMoves(pieceSquare, totalOccupancy) & ~(activeOcc);
                    break;
                case Piece.Queen:
                    possibleMoves = (Bishop.GetLegalMoves(pieceSquare, totalOccupancy) | Rook.GetLegalMoves(pieceSquare, totalOccupancy)) & ~(activeOcc);
                    break;
                case Piece.King:

                    possibleMoves = PieceAttackPatternHelper.KingMoveMask[pieceSquare] & ~(activeOcc);
                    if (ca != CastlingAvailability.NoCastlingAvailable)
                    {
                        if (color == Color.Black)
                        {
                            if (ca.HasFlag(CastlingAvailability.BlackKingside))
                            {
                                possibleMoves |= (1ul << 62);
                            }
                            if (ca.HasFlag(CastlingAvailability.BlackQueenside))
                            {
                                possibleMoves |= (1ul << 58);
                            }
                        }
                        if (color == Color.White)
                        {
                            if (ca.HasFlag(CastlingAvailability.WhiteKingside))
                            {
                                possibleMoves |= (1ul << 6);
                            }
                            if (ca.HasFlag(CastlingAvailability.WhiteQueenside))
                            {
                                possibleMoves |= (1ul << 2);
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Piece argument passed to GetPossibleMoves()");
            }

            moves = BoardValueToMoves(piece, pieceSquare, possibleMoves, enPassantIndex, ca).ToList();
            return possibleMoves;
        }

        public static ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color color = Color.White)
        {
            var r = Rank(pieceIndex);
            var f = File(pieceIndex);
            switch (piece)
            {
                case Piece.Bishop:
                    return Bishop.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Rook:
                    return Rook.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Queen:
                    return Bishop.GetLegalMoves(pieceIndex, occupancy) | Rook.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Pawn:
                    return PieceAttackPatternHelper.PawnAttackMask[color.ToInt()][pieceIndex];
                case Piece.King:
                    return PieceAttackPatternHelper.KingMoveMask[r, f];
                case Piece.Knight:
                    return PieceAttackPatternHelper.KnightAttackMask[r, f];
                default:
                    throw new Exception("Piece not supported for GetAttackSquares().");
            }
        }

        public static bool CanPieceMove<T>(this IBoard board, ushort square) where T : MoveValidatorBase
        {
            var canMove = false;
            var p = BoardHelpers.GetPieceOfColorAtIndex(board.PiecePlacement, square);
            return GetLegalMoves<T>(board, square).Any();
        }

        public static IMoveExt[] GetLegalMoves<T>(this IBoard boardInfo, ushort src)
            where T : MoveValidatorBase =>
        GetLegalMoves<T>(boardInfo.PiecePlacement, boardInfo.ActivePlayer, src,
            boardInfo.EnPassantSquare, boardInfo.CastlingAvailability);

        public static IMoveExt[] GetLegalMoves<T>(this ulong[][] boardInfo, Color activeColor, ushort src, ushort? enPassantIndex, CastlingAvailability ca)
        where T : MoveValidatorBase
        {
            var rv = new List<MoveExt>();
            var piece = BoardHelpers.GetTypeOfPieceAtIndex(src, boardInfo);
            var legalMoves = GetPseudoLegalMoves(piece.Value, src, boardInfo.Occupancy(activeColor),
                boardInfo.Occupancy(activeColor.Toggle()), activeColor, enPassantIndex, ca,
                out List<MoveExt> pseudoMoves);

            foreach (var mv in pseudoMoves)
            {
                var moveValidator = (T)Activator.CreateInstance(typeof(T), new object[] { (IBoard)(new BoardFENInfo(boardInfo, activeColor, ca, enPassantIndex, null, 0)), mv });
                if (moveValidator.Validate() == null)
                {
                    rv.Add(mv);
                }
            }

            return rv.ToArray();
        }

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="color"/>
        /// </summary>
        /// <param name="squareIndex">Index of possible attack target</param>
        /// <param name="color">Color of attacker</param>
        /// <param name="piecesOnBoard">Occupancy arrays for both colors, indexed as [color_enum][piece_enum]</param>
        /// <returns>true if <paramref name="squareIndex"/> is attacked by any piece of <paramref name="color"/></returns>
        public static bool IsSquareAttackedByColor(this ushort squareIndex, Color color, ulong[][] piecesOnBoard)
        {

            var nColor = (int)color;
            var notNColor = nColor ^ 1;
            var r = squareIndex / 8;
            var f = squareIndex % 8;
            var totalOcc = 0ul;
            var oppositeOccupancy = piecesOnBoard[(int)color.Toggle()].Aggregate((x, y) => x |= y);
            var activeOccupancy = piecesOnBoard[(int)color].Aggregate((x, y) => x |= y);
            totalOcc = oppositeOccupancy | activeOccupancy;
            var bishopAttack = GetAttackedSquares(Piece.Bishop, squareIndex, totalOcc);
            var rookAttack = GetAttackedSquares(Piece.Rook, squareIndex, totalOcc);
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor][squareIndex] & piecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & piecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (piecesOnBoard[nColor][Piece.Bishop.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (piecesOnBoard[nColor][Piece.Rook.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatternHelper.KingMoveMask[r, f] & piecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="attackingColor"/>
        /// </summary>
        /// <param name="board">A board representation</param>
        /// <param name="squareIndex">Index of square to test for being under attack</param>
        /// <param name="attackingColor">color of attacker</param>
        /// <returns></returns>
        public static bool IsSquareAttackedByColor(this IBoard board, ushort squareIndex, Color attackingColor) => IsSquareAttackedByColor(squareIndex, attackingColor, board.PiecePlacement);


        public static ulong XRayRookAttacks(this IBoard board, ushort squareIndex)
        {
            var rookMovesFromSquare = PieceAttackPatternHelper.RookMoveMask[squareIndex];
            //blockers &= rookMovesFromSquare;
            return rookMovesFromSquare ^ Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, board.PiecePlacement.Occupancy());
        }

        public static ulong XRayBishopAttacks(this IBoard board, ushort squareIndex)
        {
            var bishopMovesFromSquare = PieceAttackPatternHelper.BishopMoveMask[squareIndex];
            //blockers &= bishopMovesFromSquare;
            return bishopMovesFromSquare ^ Bitboard.GetAttackedSquares(Piece.Bishop, squareIndex, board.PiecePlacement.Occupancy());
        }

        public static ulong GetAbsolutePins(this IBoard board)
        {
            ulong pinned = 0;
            var kingIndex = board.ActiveKingIndex();
            var xRayBishopAttacks = board.XRayBishopAttacks(kingIndex);
            var xRayRookAttacks = board.XRayRookAttacks(kingIndex);
            var bishopPinnedPieces = (board.PiecePlacement.Occupancy(board.OpponentColor(), Piece.Bishop) | board.PiecePlacement.Occupancy(board.OpponentColor(), Piece.Queen)) & xRayBishopAttacks;
            var rookPinnedPieces = (board.PiecePlacement.Occupancy(board.OpponentColor(), Piece.Rook) | board.PiecePlacement.Occupancy(board.OpponentColor(), Piece.Queen)) &
                                   xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, kingIndex);
                var piecesBetween = squaresBetween & board.PiecePlacement.Occupancy(board.ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }
            return pinned;
        }


    }
}
