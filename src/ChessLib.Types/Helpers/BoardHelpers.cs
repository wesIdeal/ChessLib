using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.BoardValidation;

namespace ChessLib.Core.Helpers
{
    public static class BoardHelpers
    {
        public enum CheckType
        {
            None,
            Single,
            Double
        }

        public static readonly ulong[][] InitialBoard;


        private static readonly ulong[,] ArrInBetween = new ulong[64, 64];

        static BoardHelpers()
        {
            InitialBoard = new ulong[2][];
            InitialBoard[0] = new ulong[6];
            InitialBoard[0][0] = 65280;
            InitialBoard[0][1] = 66;
            InitialBoard[0][2] = 36;
            InitialBoard[0][3] = 129;
            InitialBoard[0][4] = 8;
            InitialBoard[0][5] = 16;
            InitialBoard[1] = new ulong[6];
            InitialBoard[1][0] = 71776119061217280;
            InitialBoard[1][1] = 4755801206503243776;
            InitialBoard[1][2] = 2594073385365405696;
            InitialBoard[1][3] = 9295429630892703744;
            InitialBoard[1][4] = 576460752303423488;
            InitialBoard[1][5] = 1152921504606846976;

            InitializeInBetween();
        }

        #region Initialization

        private static void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
                for (var t = f; t < 64; t++)
                {
                    const long m1 = -1;
                    const long aFileBorder = 0x0001010101010100;
                    const long b2DiagonalBorder = 0x0040201008040200;
                    const long hFileBorder = 0x0002040810204080;

                    var between = (m1 << f) ^ (m1 << t);
                    long file = (t & 7) - (f & 7);
                    long rank = ((t | 7) - f) >> 3;
                    var line = ((file & 7) - 1) & aFileBorder;
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2DiagonalBorder; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & hFileBorder; /* h1b7 if same anti-diagonal */
                    line *= between & -between; /* mul acts like shift by smaller boardIndex */
                    ArrInBetween[f, t] = (ulong)(line & between); /* return the bits on that line in-between */
                }
        }

        #endregion

        /// <summary>
        ///     Gets the opposite color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color Toggle(this Color c)
        {
            return c == Color.White ? Color.Black : Color.White;
        }

        /// <summary>
        ///     Gets the squares in between two squares, returns 0 for squares not linked diagonally or by file or rank
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ulong InBetween(int from, int to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return ArrInBetween[square1, square2];
        }


        /// <summary>
        ///     Gets the occupancy of a pieceLayout by color and/or piece (or neither = TotalOccupancy)
        /// </summary>
        /// <param name="board"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns>
        ///     <see cref="ulong" /> of pieces on board, optionally by
        ///     <param name="c">color</param>
        ///     and
        ///     <param name="p">piece</param>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Occupancy(this ulong[][] board, Color? c = null, Piece? p = null)
        {
            if (c == null && p == null)
                return board.Select(x => x.Aggregate((acc, val) => acc | val)).Aggregate((acc, val) => acc | val);
            if (c == null)
                return board[(int)Color.White][(int)p] | board[(int)Color.Black][(int)p];
            if (p == null) return board[(int)c].Aggregate((current, val) => current | val);

            return board[(int)c][(int)p];
        }

        /// <summary>
        ///     Gets the <see cref="Piece" /> object occupying the supplied
        ///     <param name="boardIndex">pieceLayout index</param>
        ///     on the current
        ///     <param name="occupancy">pieceLayout</param>
        /// </summary>
        /// <param name="occupancy">occupancy arrays in [white][black] format</param>
        /// <param name="boardIndex">pieceLayout index</param>
        /// <returns>Type of Piece, if found, otherwise null</returns>
        /// <exception cref="ArgumentException">
        ///     if
        ///     <param name="boardIndex">index</param>
        ///     is not in range
        /// </exception>
        public static Piece? GetPieceAtIndex(in ulong[][] occupancy, in ushort boardIndex)
        {
            boardIndex.ValidateIndex();
            var pocAtIndex = GetPieceOfColorAtIndex(occupancy, boardIndex);
            return pocAtIndex?.Piece;
        }


        /// <summary>
        ///     Gets a piece of color object for the index
        /// </summary>
        /// <param name="occupancy">Board's piece occupancy</param>
        /// <param name="boardIndex">Index on pieceLayout</param>
        /// <returns>
        ///     The object representing the piece at an index, or null if no piece occupies the supplied
        ///     <param name="boardIndex">index</param>
        ///     .
        /// </returns>
        public static PieceOfColor? GetPieceOfColorAtIndex(this ulong[][] occupancy, ushort boardIndex)
        {
            boardIndex.ValidateIndex();
            var val = 1ul << boardIndex;
            for (var c = 0; c < 2; c++)
            {
                var color = (Color)c;
                var piecePosition = occupancy[c]
                    .Select((placementValue, arrIdx) => new
                    { Color = color, PlacementValue = placementValue, Piece = (Piece)arrIdx })
                    .FirstOrDefault(p => (p.PlacementValue & val) != 0);

                if (piecePosition != null)
                {
                    return new PieceOfColor
                    {
                        Piece = piecePosition.Piece,
                        Color = piecePosition.Color
                    };
                }
            }

            return null;
        }



        /// <summary>
        ///     Method to validate if index is in range.
        /// </summary>
        /// <param name="index">pieceLayout index to validate</param>
        /// <exception cref="ArgumentException">if index is out of range (0...63)</exception>
        public static void ValidateIndex(this ushort index)
        {
            if (index >= 64) throw new ArgumentException($"Board index {index} is out of range.");
        }


        public static CastlingAvailability GetCastlingAvailabilityPostMove(IBoard board, IMove move)
        {
            var ca = board.CastlingAvailability;
            var movingPiece = GetPieceAtIndex(board.Occupancy, move.SourceIndex);
            switch (movingPiece)
            {
                case Piece.Rook:
                    if (move.SourceIndex == 56) ca &= ~CastlingAvailability.BlackQueenside;
                    if (move.SourceIndex == 63) ca &= ~CastlingAvailability.BlackKingside;
                    if (move.SourceIndex == 0) ca &= ~CastlingAvailability.WhiteQueenside;
                    if (move.SourceIndex == 7) ca &= ~CastlingAvailability.WhiteKingside;
                    break;
                case Piece.King:
                    if (move.SourceIndex == 60)
                        ca &=
                            ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    if (move.SourceIndex == 4)
                        ca &=
                            ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    break;
            }

            return ca;
        }

        public static bool IsEnPassantCaptureAvailable(this Board board)
        {
            var epSquare = board.EnPassantSquare;
            if (!epSquare.HasValue)
            {
                return false;
            }

            var epAttackFromSquares = Bitboard.Instance.GetPseudoLegalMoves(epSquare.Value, Piece.Pawn,
                board.OpponentColor,
                board.Occupancy.Occupancy());
            return (epAttackFromSquares & board.Occupancy[(int)board.ActivePlayer][(int)Piece.Pawn]) != 0;
        }

        /// <summary>
        ///     Sets EnPassant flag appropriately, clearing it if no En Passant is available
        /// </summary>
        /// <param name="move"></param>
        /// <param name="pocSource"></param>
        public static ushort? GetEnPassantIndex(IMove move, PieceOfColor? pocSource)
        {
            ushort? rv = null;
            if (pocSource.HasValue)
            {
                var startRank = pocSource.Value.Color == Color.White ? 1 : 6;
                var endRank = pocSource.Value.Color == Color.White ? 3 : 4;
                var enPassantIndexOffset = pocSource.Value.Color == Color.White ? 8 : -8;
                if (pocSource.Value.Piece == Piece.Pawn)
                    if ((move.SourceValue & BoardConstants.RankMasks[startRank]) != 0
                        && (move.DestinationValue & BoardConstants.RankMasks[endRank]) != 0)
                        rv = (ushort)(move.SourceIndex + enPassantIndexOffset);
            }

            return rv;
        }


        /// <summary>
        ///     Applies a move to a pieceLayout
        /// </summary>
        /// <param name="currentBoard">Board to which move will be applied.</param>
        /// <param name="move"></param>
        /// <param name="bypassMoveValidation">Bypass validation; useful when move was previously validated</param>
        /// <returns>The board after the move has been applied.</returns>
        /// <exception cref="MoveException">If no piece exists at source.</exception>
        public static IBoard ApplyMoveToBoard(this IBoard currentBoard, in IMove move,
            bool bypassMoveValidation = false)
        {
            var board = (IBoard)currentBoard.Clone();
            if (!bypassMoveValidation)
            {
                var boardValidator = new BoardValidator(board);
                boardValidator.Validate(true);
            }

            var pieceMoving = GetPieceOfColorAtIndex(board.Occupancy, move.SourceIndex);
            if (pieceMoving == null)
                throw new MoveException("No piece at current source to apply move to.",
                    MoveError.ActivePlayerHasNoPieceOnSourceSquare, move, board.ActivePlayer);


            var capturedPiece = GetPieceAtIndex(board.Occupancy, move.DestinationIndex);


            var isPawnMove = IsPawnMove(board, move);

            var halfMoveClock = capturedPiece != null || isPawnMove ? 0 : board.HalfMoveClock + 1;
            var fullMoveCounter =
                board.ActivePlayer == Color.Black ? board.FullMoveCounter + 1 : board.FullMoveCounter;

            var piecePlacement = GetBoardPostMove(board, move);
            var castlingAvailability = GetCastlingAvailabilityPostMove(board, move);
            var enPassantSquare = GetEnPassantIndex(move, pieceMoving.Value);
            var activePlayer = board.ActivePlayer.Toggle();
            return new Board(piecePlacement, (ushort)halfMoveClock, enPassantSquare, capturedPiece,
                castlingAvailability, activePlayer,
                (ushort)fullMoveCounter);
        }

        private static bool IsPawnMove(in IBoard board, in IMove move)
        {
            return (board.Occupancy[(int)board.ActivePlayer][BoardConstants.Pawn] & move.SourceValue) != 0;
        }


        /// <summary>
        ///     Gets the piece setup post-move
        /// </summary>
        /// <param name="boardInfo"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static ulong[][] GetBoardPostMove(in IBoard boardInfo, in IMove move)
        {
            var board = (IBoard)boardInfo.Clone();
            var pieces = board.Occupancy;
            var activeColor = (int)board.ActivePlayer;
            var oppColor = activeColor ^ 1;
            var oppOccupancy = Occupancy(board.Occupancy, board.OpponentColor());
            var piece = GetPieceOfColorAtIndex(board.Occupancy, move.SourceIndex);
            if (piece == null)
            {
                throw new MoveException(
                    $"No piece is present at the source indicated: {move.SourceIndex.IndexToSquareDisplay()}",
                    boardInfo);
            }

            if (piece.Value.Color != board.ActivePlayer)
            {
                throw new MoveException(
                    $"Piece found at square index {move.SourceIndex} was {piece.Value.Color} when it is {board.ActivePlayer}'s move." +
                    $"{Environment.NewLine}FEN: {board.ToFEN()}",
                    boardInfo);
            }

            var nPiece = (int)piece.Value.Piece;
            pieces[activeColor][nPiece] ^= move.SourceValue ^ move.DestinationValue;
            if ((oppOccupancy & move.DestinationValue) != 0)
            {
                for (var idx = 0; idx < pieces[oppColor].Length; idx++)
                {
                    pieces[oppColor][idx] &= ~move.DestinationValue;
                }
            }

            switch (move.MoveType)
            {
                case MoveType.Promotion:
                    pieces[activeColor][nPiece] ^= move.DestinationValue;
                    var promotionPiece = (int)move.PromotionPiece + 1;
                    pieces[activeColor][promotionPiece] ^= move.DestinationValue;
                    break;
                case MoveType.EnPassant:
                    if (!board.EnPassantSquare.HasValue)
                    {
                        throw new MoveException(
                            "En Passant is not available, but move was flagged as En Passant capture.",
                            MoveError.EpNotAvailable, move, board.ActivePlayer);
                    }

                    var enPassantSqIdx = board.EnPassantSquare.Value;
                    var captureSquare = board.ActivePlayer == Color.White ? enPassantSqIdx - 8 : enPassantSqIdx + 8;
                    pieces[oppColor][BoardConstants.Pawn] &= ~((ushort)captureSquare).GetBoardValueOfIndex();
                    break;
                case MoveType.Castle:
                    var rookMove = MoveHelpers.GetRookMoveForCastleMove(move);
                    pieces[activeColor][BoardConstants.Rook] ^= rookMove.SourceValue ^ rookMove.DestinationValue;
                    break;
            }

            return pieces;
            //return GetBoardPostMove(board.GetPiecePlacement(), board.ActivePlayer, move);
        }

        ///// <summary>
        /////     Gets a pieceLayout's piece setup after the specified player makes the specified move
        ///// </summary>
        ///// <param name="currentBoard"></param>
        ///// <param name="activePlayerColor"></param>
        ///// <param name="move"></param>
        ///// <returns></returns>
        //public static ulong[][] GetBoardPostMove(this ulong[][] currentBoard, in Color activePlayerColor,
        //    in IMove move)
        //{
        //    var nActiveColor = (int)activePlayerColor;
        //    var opponentColor = activePlayerColor.Toggle();
        //    var nOppColor = (int)opponentColor;
        //    var resultantBoard = new ulong[2][];
        //    var pieceMoving = GetPieceAtIndex(currentBoard, move.SourceIndex);
        //    for (var i = 0; i < 2; i++)
        //    {
        //        resultantBoard[i] = new ulong[6];
        //        foreach (var p in Enum.GetValues(typeof(Piece)))
        //        {
        //            resultantBoard[i][(int)p] = currentBoard[i][(int)p];
        //            if (i == nActiveColor && (Piece)p == pieceMoving)
        //            {
        //                resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.SourceIndex);
        //                resultantBoard[i][(int)p] = resultantBoard[i][(int)p].SetBit(move.DestinationIndex);
        //            }
        //            else if (i == (int)opponentColor)
        //            {
        //                resultantBoard[i][(int)p] =
        //                    BitHelpers.ClearBit(resultantBoard[i][(int)p], move.DestinationIndex);
        //            }
        //        }
        //    }

        //    if (move.MoveType == MoveType.Castle)
        //    {
        //        resultantBoard[nActiveColor][BoardConstants.Rook] =
        //            GetRookBoardPostCastle(move, resultantBoard[nActiveColor][BoardConstants.Rook]);
        //    }
        //    else if (move.MoveType == MoveType.EnPassant)
        //    {
        //        var capturedPawnValue = 1ul << (opponentColor == Color.Black
        //            ? move.DestinationIndex - 8
        //            : move.DestinationIndex + 8);
        //        resultantBoard[nOppColor][BoardConstants.Pawn] &= ~capturedPawnValue;
        //    }
        //    else if (move.MoveType == MoveType.Promotion)
        //    {
        //        resultantBoard[nActiveColor][BoardConstants.Pawn] &= ~move.DestinationValue;
        //        switch (move.PromotionPiece)
        //        {
        //            case PromotionPiece.Knight:
        //                resultantBoard[nActiveColor][BoardConstants.Knight] |= move.DestinationValue;
        //                break;
        //            case PromotionPiece.Bishop:
        //                resultantBoard[nActiveColor][BoardConstants.Bishop] |= move.DestinationValue;
        //                break;
        //            case PromotionPiece.Rook:
        //                resultantBoard[nActiveColor][BoardConstants.Rook] |= move.DestinationValue;
        //                break;
        //            case PromotionPiece.Queen:
        //                resultantBoard[nActiveColor][BoardConstants.Queen] |= move.DestinationValue;
        //                break;
        //        }
        //    }

        //    return resultantBoard;
        //}


        public static Color OpponentColor(this IBoard board)
        {
            return board.ActivePlayer.Toggle();
        }

        public static ushort ActiveKingIndex(this IBoard board)
        {
            return board.Occupancy[(int)board.ActivePlayer][BoardConstants.King].GetSetBits()[0];
        }

        public static bool IsActivePlayerInCheck(this IBoard board)
        {
            return IsColorInCheck(board.Occupancy, board.ActivePlayer);
        }

        public static bool IsOpponentInCheck(this IBoard board)
        {
            return IsColorInCheck(board.Occupancy, board.OpponentColor());
        }

        private static ushort GetKingIndex(this ulong[][] board, Color kingColor)
        {
            var indices = board.Occupancy(kingColor, Piece.King).GetSetBits();
            Debug.Assert(indices.Length == 1);
            return indices[0];
        }

        public static CheckType GetCheckType(ulong[][] board, Color activeColor, out ushort[] attackingPieces)
        {
            var checkedColorKingIdx = board.GetKingIndex(activeColor);
            var attackingColor = activeColor.Toggle();
            attackingPieces = GetPiecesAttackingSquare(board, checkedColorKingIdx, attackingColor);
            if (!attackingPieces.Any())
            {
                return CheckType.None;
            }

            if (attackingPieces.Length > 1)
            {
                return CheckType.Double;
            }

            return CheckType.Single;
        }

        public static bool IsColorInCheck(ulong[][] board, Color checkedColor)
        {
            var nCheckedColor = (int)checkedColor;
            var kingOccupancy = board[nCheckedColor][BoardConstants.King];
            var setBits = kingOccupancy.GetSetBits();
            var checkedColorKingIdx = setBits[0];
            return Bitboard.Instance.IsSquareAttackedByColor(checkedColorKingIdx, checkedColor.Toggle(), board);
        }


        public static bool IsStalemate(ulong[][] occupancy, Color activeColor, ushort? enPassantIdx,
            CastlingAvailability castlingAvailability)
        {
            if (IsColorInCheck(occupancy, activeColor))
            {
                return false;
            }

            for (var p = 0; p <= 6; p++)
            {
                var piece = (Piece)p;
                var pieceSet = occupancy.Occupancy(activeColor);
                foreach (var pieceIdx in pieceSet.GetSetBits())
                {
                    var legalMoves = Bitboard.Instance.GetLegalMoves(pieceIdx, piece, activeColor, occupancy,
                        enPassantIdx, castlingAvailability);
                    if (legalMoves.Any())
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        ///     Determines if active player has been mated
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckmate(ulong[][] occupancy, Color activeColor)
        {
            var checkType = GetCheckType(occupancy, activeColor, out var attackingPieces);
            var kingIndex = GetKingIndex(occupancy, activeColor);
            if (checkType == CheckType.None)
            {
                return false;
            }

            var kingMoves = Bitboard.Instance.GetLegalMoves(kingIndex, Piece.King, activeColor, occupancy, null,
                CastlingAvailability.NoCastlingAvailable);
            if (kingMoves.Any())
            {
                return false;
            }

            if (checkType == CheckType.Single)
            {
                //Can checking piece be captured by piece
                var attackingPiece = attackingPieces.Single();
                var piecesAttackingCheckingPiece = GetPiecesAttackingSquare(occupancy, attackingPiece, activeColor);
                //if any piece other than the King attacks the checking piece (King attack was already checked above), then it is not mate.
                if (piecesAttackingCheckingPiece.Any(x => x != kingIndex))
                {
                    return false;
                }

                //Can check be blocked
                var squaresBetweenKingAndAttackingPiece = InBetween(kingIndex, attackingPiece);
                foreach (var squareBetween in squaresBetweenKingAndAttackingPiece.GetSetBits())
                {
                    var availableBlockers = GetPiecesAttackingSquare(occupancy, squareBetween, activeColor);
                    //Can any piece block the check (besides the King).
                    if (availableBlockers.Any(blocker => blocker != kingIndex))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        ///     Can the specified color's king evade an attack through a block or a capture
        /// </summary>
        /// <returns></returns>
        public static bool DoesKingHaveEvasions(ulong[][] occupancy, Color activeColor)
        {
            var kindIndex = occupancy[(int)activeColor][(int)Piece.King].GetSetBits()[0];
            var activeOccupancy = Occupancy(occupancy, activeColor);
            var opponentOccupancy = Occupancy(occupancy, activeColor.Toggle());
            var moves = Bitboard.Instance.GetPseudoLegalMoves(kindIndex, Piece.King,
                activeColor, activeOccupancy | opponentOccupancy);
            foreach (var move in moves.GetSetBits())
            {
                if (!Bitboard.Instance.IsSquareAttackedByColor(move, activeColor.Toggle(), occupancy))
                {
                    return true;
                }
            }

            return false;
        }


        private static ushort[] GetPiecesAttackingSquare(ulong[][] pieces, ushort idx, Color attackerColor,
            ushort? blockerToRemove = null)
        {
            pieces = blockerToRemove.HasValue ? RemovePotentialBlocker(pieces, blockerToRemove.Value) : pieces;
            var piecesAttacking = Bitboard.Instance.PiecesAttackingSquare(pieces, idx);
            var attackingColorOccupancy = pieces.Occupancy(attackerColor);
            var attackingPieces = piecesAttacking & attackingColorOccupancy;
            var attackerIndexes = attackingPieces.GetSetBits();
            return attackerIndexes;
        }

        private static ulong[][] RemovePotentialBlocker(ulong[][] pieces, ushort blocker)
        {
            var notPieceVal = ~blocker.GetBoardValueOfIndex();
            var pieceArrayRv = new ulong[2][];
            for (var i = 0; i < pieces.Length; i++)
            {
                var colorSet = pieces[i];
                pieceArrayRv[i] = new ulong[6];
                for (var pieceIdx = 0; pieceIdx < colorSet.Length; pieceIdx++)
                {
                    var piece = colorSet[pieceIdx];
                    pieceArrayRv[i][pieceIdx] = piece & notPieceVal;
                }
            }

            return pieceArrayRv;
        }

        public static IMove GetMove(IBoard board, ushort sourceIndex, ushort destinationIndex,
            PromotionPiece promotionPiece)
        {
            var moveType = GetMoveType(board, sourceIndex, destinationIndex);
            return MoveHelpers.GenerateMove(sourceIndex, destinationIndex, moveType, promotionPiece);
        }

        /// <summary>
        ///     Gets the type of move based on the current board and piece source/destination
        /// </summary>
        /// <param name="boardInfo">Board information for current position</param>
        /// <param name="source">Source Index</param>
        /// <param name="dest">Destination Index</param>
        /// <returns>The type of move represented by the given parameters</returns>
        /// <exception cref="PieceException">Thrown if there is no piece on the source square.</exception>
        public static MoveType GetMoveType(in IBoard boardInfo, ushort source, ushort dest)
        {
            var relevantPieces = new[] { Piece.Pawn, Piece.King };
            var sourcePiece = GetPieceOfColorAtIndex(boardInfo.Occupancy, source);
            if (sourcePiece == null)
            {
                var move = $"{source.IndexToSquareDisplay()}->{dest.IndexToSquareDisplay()}";
                throw new PieceException("Error getting piece on source in Bitboard.GetMoveType(...):" + move);
            }

            var piece = sourcePiece.Value.Piece;
            var color = sourcePiece.Value.Color;
            if (!relevantPieces.Contains(piece))
            {
                return MoveType.Normal;
            }

            if (IsEnPassantCapture(piece, source, dest, boardInfo.EnPassantSquare))
            {
                return MoveType.EnPassant;
            }

            if (piece == Piece.King && IsCastlingMove(color, source, dest))
            {
                return MoveType.Castle;
            }

            if (piece == Piece.Pawn && IsPromotion(color, source, dest))
            {
                return MoveType.Promotion;
            }

            return MoveType.Normal;
        }

        private static bool IsPromotion(Color color, ushort source, ushort dest)
        {
            var sourceValue = source.GetBoardValueOfIndex();
            var destValue = dest.GetBoardValueOfIndex();
            var validPromotionRankSource = color == Color.Black ? BoardConstants.Rank2 : BoardConstants.Rank7;
            var validPromotionRankDest = color == Color.Black ? BoardConstants.Rank1 : BoardConstants.Rank8;
            return (sourceValue & validPromotionRankSource) == sourceValue &&
                   (destValue & validPromotionRankDest) == destValue;
        }


        public static bool IsCastlingMove(Color activeColor, ushort source, ushort destination)
        {
            var legalCastlingMovesForColor = activeColor == Color.White
                ? MoveHelpers.WhiteCastlingMoves
                : MoveHelpers.BlackCastlingMoves;
            return legalCastlingMovesForColor.Any(m =>
                m.SourceIndex == source && m.DestinationIndex == destination);
        }

        public static bool IsEnPassantCapture(Piece sourcePiece, ushort src, ushort dest, ushort? enPassantSquare)
        {
            if (enPassantSquare != null)
            {
                if (sourcePiece == Piece.Pawn)
                {
                    if (dest == enPassantSquare.Value && dest.GetFile() != src.GetFile())
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        #region Enum ToInt() methods

        /// <summary>
        ///     Extension to get the int of a color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Color c)
        {
            return (int)c;
        }

        /// <summary>
        ///     Extension to get the int of a piece
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Piece p)
        {
            return (int)p;
        }

        /// <summary>
        ///     Extension to get the int of a file
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this File f)
        {
            return (int)f;
        }

        /// <summary>
        ///     Extension to get the int of a rank
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Rank r)
        {
            return (int)r;
        }

        /// <summary>
        ///     Gets the hex display of a long (for debugging, mainly)
        /// </summary>
        /// <param name="u">long to get display from</param>
        /// <param name="appendHexNotation">append '0x' to the representation</param>
        /// <param name="pad">pad length to a certain size</param>
        /// <param name="padSize">size to pad</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexDisplay(this ulong u, bool appendHexNotation = true, bool pad = false,
            int padSize = 64)
        {
            var str = Convert.ToString((long)u, 16);
            if (pad) str = str.PadLeft(padSize, '0');
            if (appendHexNotation) str = "0x" + str;
            return str;
        }

        #endregion

        #region Array Position to Friendly Position Helpers

        /// <summary>
        ///     Gets the index of a boardIndex
        /// </summary>
        /// <param name="square">
        ///     SAN boardIndex representation (A1, H5, E4, etc). Must be either '-' (Premove FEN En Passant) or 2
        ///     characters
        /// </param>
        /// <returns>Square index</returns>
        /// <exception cref="ArgumentException">Thrown if boardIndex length, File, or Rank is invalid.</exception>
        public static ushort? SquareTextToIndex(this string square)
        {
            if (square.Trim() == "-") return null;
            if (square.Length != 2)
                throw new ArgumentException($"Square passed to SquareTextToIndex(), {square} has an invalid length.");
            var file = char.ToLower(square[0]);
            var rank = ushort.Parse(square[1].ToString());
            if (!char.IsLetter(file) || file < 'a' || file > 'h')
                throw new ArgumentException("File portion of boardIndex-text should be a letter, between 'a' and 'h'.");
            if (rank < 1 || rank > 8)
                throw new ArgumentException(
                    "Rank portion of boardIndex-text should be a digit with a value between 1 and 8.");
            var rankMultiplier = rank - 1;
            return (ushort)(rankMultiplier * 8 + file - 'a');
        }


        /// <summary>
        ///     Gets a File basked on boardIndex index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static File GetFile(this int square)
        {
            return (File)(square % 8);
        }

        /// <summary>
        ///     Gets a File basked on boardIndex index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetFile(this ushort square)
        {
            return (ushort)(square % 8);
        }

        /// <summary>
        ///     Gets a Rank basked on boardIndex index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rank GetRank(this int square)
        {
            Debug.Assert(square >= 0 && square < 64);
            return (Rank)((ushort)square).GetRank();
        }

        /// <summary>
        ///     Gets a Rank basked on boardIndex index
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     if
        ///     <param name="boardIndex">index</param>
        ///     is not in range.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetRank(this ushort boardIndex)
        {
            boardIndex.ValidateIndex();
            return (ushort)(boardIndex / 8);
        }


        /// <summary>
        ///     Gets a file index from a boardIndex index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(this ushort idx)
        {
            return (ushort)(idx % 8);
        }

        /// <summary>
        ///     Gets
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankCompliment(this ushort rank)
        {
            return (ushort)Math.Abs(rank - 7);
        }

        #endregion


        #region PremoveFEN String Retrieval

        public static string GetFENPiecePlacement(this IBoard board)
        {
            return board.Occupancy.GetPiecePlacement();
        }

        public static string GetFENSideToMoveStrRepresentation(this IBoard board)
        {
            return board.ActivePlayer == Color.Black ? "b" : "w";
        }

        public static string GetFENCastlingAvailabilityString(this IBoard board)
        {
            return FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(board.CastlingAvailability);
        }

        public static string GetFENEnPassantString(this IBoard board)
        {
            return board.EnPassantSquare == null ? "-" : board.EnPassantSquare.Value.IndexToSquareDisplay();
        }

        public static string GetFENHalfMoveClockString(this IBoard board)
        {
            return board.HalfMoveClock.ToString();
        }

        public static string GetFENMoveCounterString(this IBoard board)
        {
            return board.FullMoveCounter.ToString();
        }

        public static string ToFEN(this IBoard b)
        {
            return
                $"{b.GetFENPiecePlacement()} {b.GetFENSideToMoveStrRepresentation()} {b.GetFENCastlingAvailabilityString()} {b.GetFENEnPassantString()} {b.GetFENHalfMoveClockString()} {b.GetFENMoveCounterString()}";
        }

        #endregion
    }
}