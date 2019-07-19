using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic.Init;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.MoveValidation;

namespace ChessLib.Data
{
    public static class Bitboard
    {

        private static readonly MovePatternStorage Bishop = new BishopPatterns();
        private static readonly MovePatternStorage Rook = new RookPatterns();
        static Bitboard()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank(ushort idx) => idx / 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int File(ushort idx) => idx % 8;

        public static bool IsCastlingMove(Piece sourcePiece, ushort src, ushort dest)
        {
            if (sourcePiece == Piece.King)
            {
                if (src == 60)
                {
                    if (new[] { 62, 58 }.Contains(dest))
                    {
                        return true;
                    }

                }
                if (src == 4)
                {
                    if (new[] { 6, 2 }.Contains(dest))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsCastlingMove(this BoardInfo board, MoveExt unFilledMove)
        {
            var sourcePiece = BoardHelpers.GetPieceAtIndex(board.GetPiecePlacement(), unFilledMove.SourceIndex);
            if (sourcePiece == null)
            {
                return false;
            }
            return IsCastlingMove(sourcePiece.Value, unFilledMove.SourceIndex, unFilledMove.DestinationIndex);
        }

        public static bool IsEnPassantCapture(Piece sourcePiece, ushort src, ushort dest, ushort? enPassantSquare)
        {
            if (enPassantSquare != null)
            {
                if (sourcePiece == Piece.Pawn)
                {
                    if (dest == enPassantSquare.Value && dest.FileFromIdx() != src.FileFromIdx())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsEnPassantCapture(this BoardInfo board, MoveExt unFilledMove)
        {
            var sourcePiece = BoardHelpers.GetPieceAtIndex(board.GetPiecePlacement(), unFilledMove.SourceIndex);
            if (sourcePiece == null)
            {
                return false;
            }
            return IsEnPassantCapture(sourcePiece.Value, unFilledMove.SourceIndex, unFilledMove.DestinationIndex, board.EnPassantSquare);
        }

        public static bool IsPromotion(Piece p, ushort source, ushort destination)
        {
            var sRank = source.RankFromIdx();
            var dRank = destination.RankFromIdx();
            return p == Piece.Pawn && ((sRank == 1 && dRank == 0) || (sRank == 6 && dRank == 7));
        }

        /// <summary>
        /// Gets the type of move based on the current board and piece source/destination
        /// </summary>
        /// <param name="boardInfo">Board information for current position</param>
        /// <param name="source">Source Index</param>
        /// <param name="dest">Destination Index</param>
        /// <returns>The type of move represented by the given parameters</returns>
        /// <exception cref="PieceException">Thrown if there is no piece on the source square.</exception>
        public static MoveType GetMoveType(in IBoard boardInfo, ushort source, ushort dest)
        {
            var relevantPieces = new[] { Piece.Pawn, Piece.King };
            var sourcePieceColor = boardInfo.GetPieceOfColorAtIndex(source);
            if (sourcePieceColor == null)
            {
                var move = $"{source.IndexToSquareDisplay()}->{dest.IndexToSquareDisplay()}";
                throw new PieceException("Error getting piece on source in Bitboard.GetMoveType(...):" + move);
            }

            var piece = sourcePieceColor.Value.Piece;
            if (!relevantPieces.Contains(piece)) { return MoveType.Normal; }

            if (IsEnPassantCapture(piece, source, dest, boardInfo.EnPassantSquare))
            {
                return MoveType.EnPassant;
            }

            if (IsCastlingMove(piece, source, dest))
            {
                return MoveType.Castle;
            }

            if (IsPromotion(piece, source, dest))
            {
                return MoveType.Promotion;
            }

            return MoveType.Normal;
        }

        /// <summary>
        /// Gets a move based on given information
        /// </summary>
        /// <param name="board">Current board information</param>
        /// <param name="source">Source index for move</param>
        /// <param name="dest">Destination index for move</param>
        /// <param name="promotionPieceChar">Character for promotion</param>
        /// <returns>A move object</returns>
        /// <exception cref="MoveException">if promotion character is not [n|b|r|q|null], insensitive of case</exception>
        public static MoveExt GetMove(IBoard board, ushort source, ushort dest, PromotionPiece promotionPiece)
        {
            var moveType = GetMoveType(board, source, dest);
            return MoveHelpers.GenerateMove(source, dest, moveType, promotionPiece);
        }

        public static IEnumerable<MoveExt> BoardValueToMoves(Piece p, ushort source, ulong destinations, ushort? enPassantSq, CastlingAvailability ca)
        {
            var rv = new List<MoveExt>();
            foreach (var destination in destinations.GetSetBits())
            {
                var moveType = IsCastlingMove(p, source, destination) ? MoveType.Castle :
                    IsEnPassantCapture(p, source, destination, enPassantSq) ? MoveType.EnPassant :
                    IsPromotion(p, source, destination) ? MoveType.Promotion : MoveType.Normal;
                yield return MoveHelpers.GenerateMove(source, destination, moveType);

            }
        }

        public static ulong GetPseudoLegalMoves(IBoard board, ushort idx, out List<MoveExt> moves)
        {
            var piece = board.GetPieceAtIndex(idx);
            moves = new List<MoveExt>();
            if (piece == null) return 0;
            var pslMoves = GetPseudoLegalMoves(piece.Value, idx, board.ActiveOccupancy(), board.OpponentOccupancy(),
                board.ActivePlayer, board.EnPassantSquare, board.CastlingAvailability, out moves);
            var rv = new List<MoveExt>();
            foreach (var m in moves)
            {
                MoveValidator mv = new MoveValidator(board, m);
                var validationResult = mv.Validate();
                if (validationResult == MoveError.NoneSet)
                {
                    rv.Add(m);
                }
            }

            moves = rv;
            return pslMoves;
        }

        /// <summary>
        /// Gets both unvalidatedMoves and attacks/captures for a piece
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pieceSquare"></param>
        /// <param name="activeOcc"></param>
        /// <param name="oppOcc"></param>
        /// <param name="color"></param>
        /// <param name="enPassantIndex">En Passant index, if set</param>
        /// <param name="ca">Castling availability flags</param>
        /// <param name="unvalidatedMoves">Actual move objects</param>
        /// <returns></returns>
        public static ulong GetPseudoLegalMoves(Piece piece, ushort pieceSquare, ulong activeOcc, ulong oppOcc, Color color, ushort? enPassantIndex, CastlingAvailability ca, out List<MoveExt> unvalidatedMoves)
        {
            var lMoves = new List<MoveExt>();
            var totalOccupancy = activeOcc | oppOcc;
            ulong possibleMoves;

            switch (piece)
            {
                case Piece.Pawn:
                    possibleMoves = GetPawnPseudoLegalMoves(pieceSquare, activeOcc, oppOcc, color, enPassantIndex);
                    break;
                case Piece.Knight:
                    var totalAttacks = PieceAttackPatterns.Instance.KnightAttackMask[pieceSquare];
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

                    possibleMoves = PieceAttackPatterns.Instance.KingMoveMask[pieceSquare] & ~(activeOcc);
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
                    throw new Exception("Piece argument passed to GetPossibleMoves() not contained in switch statement.");
            }

            unvalidatedMoves = BoardValueToMoves(piece, pieceSquare, possibleMoves, enPassantIndex, ca).ToList();
            return possibleMoves;
        }

        public static ulong GetPawnPseudoLegalMoves(ushort pieceSquare, ulong activeOcc, ulong oppOcc, Color color, ushort? enPassantIndex)
        {
            var pieceValue = 1ul << pieceSquare;
            var enPassantValue = (1ul << enPassantIndex) ?? 0;
            var opponentOccupancyWithEnPassant = oppOcc | enPassantValue;
            var pMoves = PieceAttackPatterns.Instance.PawnMoveMask[(int)color][pieceSquare];
            var attacks = PieceAttackPatterns.Instance.PawnAttackMask[(int)color][pieceSquare] & opponentOccupancyWithEnPassant;
            var pawnMoves = pMoves & ~((activeOcc | oppOcc));
            var pawnAttacks = attacks & opponentOccupancyWithEnPassant;
            var isInitialPawnMove = ((pieceValue & BoardHelpers.RankMasks[1]) != 0 && color == Color.White) ||
                                    ((pieceValue & BoardHelpers.RankMasks[6]) != 0 && color == Color.Black);
            if (isInitialPawnMove)
            {
                var singleMoveSquare = (color == Color.White ? pieceValue << 8 : pieceValue >> 8);
                var doubleMoveSquare = (color == Color.White ? pieceValue << 16 : pieceValue >> 16);
                if ((pawnMoves & singleMoveSquare) == 0)
                {
                    // if the pawn can't move to the single-move square, it cannot go double
                    pawnMoves &= ~(doubleMoveSquare);
                }
            }
            return pawnMoves | pawnAttacks;
        }

        public static ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color color)
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
                    return PieceAttackPatterns.Instance.PawnAttackMask[color.ToInt()][pieceIndex];
                case Piece.King:
                    return PieceAttackPatterns.Instance.KingMoveMask[r, f];
                case Piece.Knight:
                    return PieceAttackPatterns.Instance.KnightAttackMask[r, f];
                default:
                    throw new Exception("Piece not supported for GetAttackSquares().");
            }
        }

        /// <summary>
        /// Returns value representing if piece can move.
        /// </summary>
        /// <param name="board">Current board configuration</param>
        /// <param name="square">Board index of piece</param>
        /// <returns>true if piece is mobile, false if not</returns>
        /// <exception cref="ArgumentException">if square is out of board range</exception>
        /// <exception cref="PieceException">if no piece exists on given <param name="square">index</param></exception>
        public static bool CanPieceMove(this IBoard board, ushort square)
        {
            square.ValidateIndex();
            var p = board.GetPiecePlacement().GetPieceOfColorAtIndex(square);
            if (p == null)
            {
                throw new PieceException("There is no piece occupying the index supplied.");
            }

            _ = GetPseudoLegalMoves(p.Value.Piece, square, board.GetPiecePlacement().Occupancy(p.Value.Color),
                board.GetPiecePlacement().Occupancy(p.Value.Color.Toggle()), p.Value.Color, board.EnPassantSquare,
                board.CastlingAvailability, out List<MoveExt> moves);
            foreach (var mv in moves)
            {
                var postMove = BoardHelpers.GetBoardPostMove(board, mv);
                if (!postMove.IsPlayerInCheck((int)board.ActivePlayer))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanPieceMoveToDestination(this IBoard boardInfo, ushort src, ushort dst) =>
        CanPieceMoveToDestination(boardInfo.GetPiecePlacement(), boardInfo.ActivePlayer, src, dst,
            boardInfo.EnPassantSquare, boardInfo.CastlingAvailability);

        /// <summary>
        /// Determines if piece can move to the supplied <param name="destinationIndex">destination</param>
        /// </summary>
        /// <param name="pieceOccupancyArrays">Current piece location arrays</param>
        /// <param name="activeColor">Color moving</param>
        /// <param name="sourceIndex">Source square index</param>
        /// <param name="destinationIndex">Destination square index</param>
        /// <param name="enPassantIndex">En Passant index, if available</param>
        /// <param name="castlingAvailability">Castling availability</param>
        /// <returns>true if piece can move to destination index</returns>
        /// <exception cref="ArgumentException">if any index is out of range</exception>
        /// <exception cref="PieceException">if no piece is found at <param name="sourceIndex">source index</param></exception>
        public static bool CanPieceMoveToDestination(this ulong[][] pieceOccupancyArrays, Color activeColor, ushort sourceIndex, ushort destinationIndex, ushort? enPassantIndex, CastlingAvailability castlingAvailability)
        {
            BoardHelpers.ValidateIndices(sourceIndex, destinationIndex, enPassantIndex ?? 0);

            var piece = BoardHelpers.GetPieceAtIndex(pieceOccupancyArrays, sourceIndex);
            if (piece == null)
            {
                throw new PieceException($"No piece found at index {sourceIndex}.");
            }
            var activeOccupancy = pieceOccupancyArrays.Occupancy(activeColor);
            var oppOccupancy = pieceOccupancyArrays.Occupancy(activeColor.Toggle());
            var dstValue = destinationIndex.GetBoardValueOfIndex();
            var legalMoves = GetPseudoLegalMoves(piece.Value, sourceIndex, activeOccupancy,
               oppOccupancy, activeColor, enPassantIndex, castlingAvailability,
                out List<MoveExt> pseudoMoves);
            if (piece == Piece.Pawn)
            {
                ulong moveSq1, moveSq2;
                var occ = activeOccupancy | oppOccupancy;
                if (activeColor == Color.White)
                {
                    moveSq1 = ((ushort)(sourceIndex + 8)).GetBoardValueOfIndex();
                    moveSq2 = ((ushort)(sourceIndex + 16)).GetBoardValueOfIndex();
                }
                else
                {
                    moveSq1 = ((ushort)(sourceIndex - 8)).GetBoardValueOfIndex();
                    moveSq2 = ((ushort)(sourceIndex - 16)).GetBoardValueOfIndex();
                }
                if ((moveSq1 & occ) != 0)
                {
                    legalMoves &= ~(moveSq1 | moveSq2);
                }
                else if((moveSq2 & occ) != 0)
                {
                    legalMoves &= ~(moveSq2);
                }
            }
            return ((legalMoves & dstValue) != 0);
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
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Debug.WriteLine(methodBase.Name);

            var nColor = (int)color;
            var notNColor = nColor ^ 1;
            var r = squareIndex / 8;
            var f = squareIndex % 8;
            var oppositeOccupancy = piecesOnBoard[(int)color.Toggle()].Aggregate((x, y) => x | y);
            var activeOccupancy = piecesOnBoard[(int)color].Aggregate((x, y) => x | y);
            var totalOcc = oppositeOccupancy | activeOccupancy;
            var bishopAttack = GetAttackedSquares(Piece.Bishop, squareIndex, totalOcc, Color.White);
            var rookAttack = GetAttackedSquares(Piece.Rook, squareIndex, totalOcc, Color.White);
            if ((PieceAttackPatterns.Instance.PawnAttackMask[notNColor][squareIndex] & piecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatterns.Instance.KnightAttackMask[r, f] & piecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (piecesOnBoard[nColor][Piece.Bishop.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (piecesOnBoard[nColor][Piece.Rook.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatterns.Instance.KingMoveMask[r, f] & piecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="attackingColor"/>
        /// </summary>
        /// <param name="board">A board representation</param>
        /// <param name="squareIndex">Index of square to test for being under attack</param>
        /// <param name="attackingColor">color of attacker</param>
        /// <returns></returns>
        public static bool IsSquareAttackedByColor(this IBoard board, ushort squareIndex, Color attackingColor) => IsSquareAttackedByColor(squareIndex, attackingColor, board.GetPiecePlacement());


        public static ulong XRayRookAttacks(this IBoard board, ushort squareIndex)
        {
            var rookMovesFromSquare = PieceAttackPatterns.Instance.RookMoveMask[squareIndex];
            //blockers &= rookMovesFromSquare;
            return rookMovesFromSquare ^ GetAttackedSquares(Piece.Rook, squareIndex, board.GetPiecePlacement().Occupancy(), Color.White);
        }

        public static ulong XRayBishopAttacks(this IBoard board, ushort squareIndex)
        {
            var bishopMovesFromSquare = PieceAttackPatterns.Instance.BishopMoveMask[squareIndex];
            //blockers &= bishopMovesFromSquare;
            return bishopMovesFromSquare ^ GetAttackedSquares(Piece.Bishop, squareIndex, board.GetPiecePlacement().Occupancy(), Color.White);
        }

        public static ulong GetAbsolutePins(this IBoard board)
        {
            ulong pinned = 0;
            var kingIndex = board.ActiveKingIndex();
            var xRayBishopAttacks = board.XRayBishopAttacks(kingIndex);
            var xRayRookAttacks = board.XRayRookAttacks(kingIndex);
            var bishopPinnedPieces = (board.GetPiecePlacement().Occupancy(board.OpponentColor(), Piece.Bishop) | board.GetPiecePlacement().Occupancy(board.OpponentColor(), Piece.Queen)) & xRayBishopAttacks;
            var rookPinnedPieces = (board.GetPiecePlacement().Occupancy(board.OpponentColor(), Piece.Rook) | board.GetPiecePlacement().Occupancy(board.OpponentColor(), Piece.Queen)) &
                                   xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, kingIndex);
                var piecesBetween = squaresBetween & board.GetPiecePlacement().Occupancy(board.ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }
            return pinned;
        }


    }
}
