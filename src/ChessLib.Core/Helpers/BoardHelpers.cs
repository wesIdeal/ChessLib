using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation;
using EnumsNET;

namespace ChessLib.Core.Helpers
{
    public static class BoardHelpers
    {
        public static readonly ulong[][] InitialBoard;


        private static readonly ulong[,] ArrInBetween = new ulong[64, 64];

        public enum CheckType
        {
            None,
            Single,
            Double
        }

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
                ArrInBetween[f, t] = (ulong) (line & between); /* return the bits on that line in-between */
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
            {
                return board.Select(x => x.Aggregate((acc, val) => acc | val)).Aggregate((acc, val) => acc | val);
            }

            if (c == null)
            {
                return board[(int) Color.White][(int) p] | board[(int) Color.Black][(int) p];
            }

            if (p == null)
            {
                return board[(int) c].Aggregate((current, val) => current | val);
            }

            return board[(int) c][(int) p];
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
                var color = (Color) c;
                var piecePosition = occupancy[c]
                    .Select((placementValue, arrIdx) => new
                        {Color = color, PlacementValue = placementValue, Piece = (Piece) arrIdx})
                    .FirstOrDefault(p => (p.PlacementValue & val) != 0);

                if (piecePosition != null)
                {
                    return new PieceOfColor(piecePosition.Piece, piecePosition.Color);
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

        /// <summary>
        ///     Get new castling availability based on move and current availability
        /// </summary>
        /// <param name="occupancy"></param>
        /// <param name="move"></param>
        /// <param name="currentCastlingAvailability"></param>
        /// <returns></returns>
        public static CastlingAvailability GetCastlingAvailabilityPostMove(ulong[][] occupancy, IMove move,
            CastlingAvailability currentCastlingAvailability)
        {
            if (currentCastlingAvailability == CastlingAvailability.NoCastlingAvailable)
            {
                return CastlingAvailability.NoCastlingAvailable;
            }

            var movingPiece = GetPieceOfColorAtIndex(occupancy, move.SourceIndex);
            if (!movingPiece.HasValue)
            {
                return currentCastlingAvailability;
            }

            switch (movingPiece.Value.Piece)
            {
                case Piece.Rook:
                    switch (move.SourceIndex)
                    {
                        case 56:
                            currentCastlingAvailability &= ~CastlingAvailability.BlackQueenside;
                            break;
                        case 63:
                            currentCastlingAvailability &= ~CastlingAvailability.BlackKingside;
                            break;
                        case 0:
                            currentCastlingAvailability &= ~CastlingAvailability.WhiteQueenside;
                            break;
                        case 7:
                            currentCastlingAvailability &= ~CastlingAvailability.WhiteKingside;
                            break;
                    }

                    break;
                case Piece.King:
                    if (movingPiece.Value.Color == Color.Black)
                    {
                        currentCastlingAvailability &=
                            ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    }
                    else
                    {
                        currentCastlingAvailability &=
                            ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    }

                    break;
            }

            return currentCastlingAvailability;
        }

        /// <summary>
        ///     Get EnPassant capture availability
        /// </summary>
        /// <param name="board"></param>
        /// <returns>true if 1) en passant is available and 2) if a pawn attacks the en passant square. False otherwise.</returns>
        public static bool IsEnPassantCaptureAvailable(this Board board)
        {
            var epSquare = board.EnPassantIndex;
            if (!epSquare.HasValue)
            {
                return false;
            }

            var epAttackFromSquares = Bitboard.Instance.GetPseudoLegalMoves(epSquare.Value, Piece.Pawn,
                board.OpponentColor,
                board.Occupancy.Occupancy());
            return (epAttackFromSquares & board.Occupancy[(int) board.ActivePlayer][(int) Piece.Pawn]) != 0;
        }

        /// <summary>
        ///     Sets EnPassant flag appropriately, clearing it if no En Passant is available
        /// </summary>
        /// <param name="board"></param>
        /// <param name="move"></param>
        public static ushort? GetEnPassantIndex(Board board, IMove move)
        {
            var pieceOfColor = GetPieceOfColorAtIndex(board.Occupancy, move.SourceIndex);
            if (!pieceOfColor.HasValue || pieceOfColor.Value.Piece != Piece.Pawn)
            {
                return null;
            }

            switch (pieceOfColor.Value.Color)
            {
                case Color.Black:
                {
                    if ((move.SourceValue & BoardConstants.Rank7) != 0 &&
                        (move.DestinationValue & BoardConstants.Rank5) != 0)
                    {
                        return (ushort?) (move.SourceIndex - 8);
                    }

                    break;
                }
                default:
                {
                    if ((move.SourceValue & BoardConstants.Rank2) != 0 &&
                        (move.DestinationValue & BoardConstants.Rank4) != 0)
                    {
                        return (ushort?) (move.SourceIndex + 8);
                    }

                    break;
                }
            }

            return null;
        }


        /// <summary>
        ///     Applies a move to a pieceLayout
        /// </summary>
        /// <param name="currentBoard">Board to which move will be applied.</param>
        /// <param name="move"></param>
        /// <returns>The board after the move has been applied.</returns>
        /// <exception cref="MoveException">If no piece exists at source.</exception>
        public static Board ApplyMoveToBoard(this Board currentBoard, in Move move)
        {
            var board = (Board) currentBoard.Clone();

            var moveValidator = new MoveValidator(board, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                Debugger.Break();
            }

            var pieceMoving = GetPieceAtIndex(board.Occupancy, move.SourceIndex);
            var capturedPiece = GetPieceAtIndex(board.Occupancy, move.DestinationIndex);

            var halfMoveClock = capturedPiece != null || pieceMoving == Piece.Pawn
                ? 0
                : board.HalfMoveClock + 1;
            var fullMoveCounter =
                board.ActivePlayer == Color.Black ? board.FullMoveCounter + 1 : board.FullMoveCounter;

            var castlingAvailability =
                GetCastlingAvailabilityPostMove(board.Occupancy, move, board.CastlingAvailability);
            var enPassantIndex = GetEnPassantIndex(board, move);
            var activePlayer = board.ActivePlayer.Toggle();
            return new Board(moveValidator.PostMoveBoard, (byte) halfMoveClock, enPassantIndex, capturedPiece,
                castlingAvailability, activePlayer,
                (ushort) fullMoveCounter);
        }


        /// <summary>
        ///     Removes a move from a board, effectively rewinding the state to one more prior to <paramref name="currentBoard" />
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="previousState"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static Board UnapplyMoveFromBoard(this Board currentBoard, in BoardState previousState, in Move move)
        {
            var hmClock = previousState.HalfMoveClock;
            var epSquare = previousState.EnPassantIndex;
            var pieces = UnapplyMoveFromBoard(currentBoard.Occupancy, previousState, move, currentBoard.PieceCaptured);
            var fullMove = currentBoard.ActivePlayer == Color.White
                ? currentBoard.FullMoveCounter - 1
                : currentBoard.FullMoveCounter;
            var activeColor = previousState.ActivePlayer;
            var castlingAvailability = previousState.CastlingAvailability;
            var board = new Board(pieces, hmClock, epSquare, previousState.PieceCaptured,
                castlingAvailability,
                activeColor, (ushort) fullMove);
            return board;
        }

        private static ulong[][] UnapplyMoveFromBoard(in ulong[][] preMoveBoard, in BoardState previousBoardState,
            in Move move, Piece? capturedPieceType)
        {
            var piece = move.MoveType == MoveType.Promotion
                ? Piece.Pawn
                : GetPieceAtIndex(preMoveBoard, move.DestinationIndex);

            Debug.Assert(piece.HasValue, "Piece for un-apply() has no value.");
            var sourceSquareValue = move.DestinationValue;
            var destinationSquareValue = move.SourceValue;
            var activeColor = (int) previousBoardState.ActivePlayer;
            var opponentColor = activeColor ^ 1;
            var piecePlacement = preMoveBoard;


            piecePlacement[activeColor][(int) piece.Value] =
                piecePlacement[activeColor][(int) piece] | destinationSquareValue;
            piecePlacement[activeColor][(int) piece.Value] =
                piecePlacement[activeColor][(int) piece] & ~sourceSquareValue;


            if (capturedPieceType.HasValue)
            {
                var capturedPieceSrc = sourceSquareValue;
                if (move.MoveType == MoveType.EnPassant)
                {
                    capturedPieceSrc = (Color) activeColor == Color.White
                        ? ((ushort) (sourceSquareValue.GetSetBits()[0] - 8)).GetBoardValueOfIndex()
                        : ((ushort) (sourceSquareValue.GetSetBits()[0] + 8)).GetBoardValueOfIndex();
                }

                //    Debug.WriteLine(
                //        $"{board.ActivePlayer}'s captured {capturedPiece} is being replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
                piecePlacement[opponentColor][(int) capturedPieceType] ^= capturedPieceSrc;
                //    Debug.WriteLine(
                //        $"{board.ActivePlayer}'s captured {capturedPiece} was replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
            }

            if (move.MoveType == MoveType.Promotion)
            {
                var promotionPiece = (Piece) (move.PromotionPiece + 1);
                //Debug.WriteLine($"Un-applying promotion to {promotionPiece}.");
                //Debug.WriteLine($"{promotionPiece} ulong is {piecePlacement[active][(int)promotionPiece].ToString()}");
                piecePlacement[activeColor][(int) promotionPiece] &= ~sourceSquareValue;
                //Debug.WriteLine(
                //    $"{promotionPiece} ulong is now {piecePlacement[active][(int)promotionPiece].ToString()}");
            }
            else if (move.MoveType == MoveType.Castle)
            {
                var rookMove = MoveHelpers.GetRookMoveForCastleMove(move);
                piecePlacement[activeColor][(int) Piece.Rook] = piecePlacement[activeColor][(int) Piece.Rook] ^
                                                                (rookMove.SourceValue | rookMove.DestinationValue);
            }

            return piecePlacement;
        }

        /// <summary>
        ///     Gets the piece setup post-move
        /// </summary>
        /// <param name="boardInfo"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static ulong[][] GetBoardPostMove(in Board boardInfo, in IMove move)
        {
            var board = (Board) boardInfo.Clone();
            var pieces = board.Occupancy;
            var activeColor = (int) board.ActivePlayer;
            var oppColor = activeColor ^ 1;
            var oppOccupancy = Occupancy(board.Occupancy, board.ActivePlayer.Toggle());
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
                    $"{Environment.NewLine}FEN: {board.FENFromBoard()}",
                    boardInfo);
            }

            var nPiece = (int) piece.Value.Piece;
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
                    var promotionPiece = (int) move.PromotionPiece + 1;
                    pieces[activeColor][promotionPiece] ^= move.DestinationValue;
                    break;
                case MoveType.EnPassant:
                    if (!board.EnPassantIndex.HasValue)
                    {
                        throw new MoveException(
                            "En Passant is not available, but move was flagged as En Passant capture.",
                            MoveError.EpNotAvailable, move, board.ActivePlayer);
                    }

                    var enPassantSqIdx = board.EnPassantIndex.Value;
                    var captureSquare = board.ActivePlayer == Color.White ? enPassantSqIdx - 8 : enPassantSqIdx + 8;
                    pieces[oppColor][BoardConstants.Pawn] &= ~((ushort) captureSquare).GetBoardValueOfIndex();
                    break;
                case MoveType.Castle:
                    var rookMove = MoveHelpers.GetRookMoveForCastleMove(move);
                    pieces[activeColor][BoardConstants.Rook] ^= rookMove.SourceValue ^ rookMove.DestinationValue;
                    break;
            }

            return pieces;
        }


        public static CheckType GetCheckType(ulong[][] board, Color activeColor, out ushort[] attackingPieces)
        {
            var checkedColorKingIdx = board.Occupancy(activeColor, Piece.King).GetSetBits()[0];
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
            return GetCheckType(board, checkedColor, out _) != CheckType.None;
        }


        public static bool IsStalemate(ulong[][] occupancy, Color activeColor, ushort? enPassantIdx,
            CastlingAvailability castlingAvailability)
        {
            if (IsColorInCheck(occupancy, activeColor))
            {
                return false;
            }

            foreach (var piece in Enums.GetValues<Piece>())
            {
                var pieceSet = occupancy.Occupancy(activeColor, piece);
                foreach (var pieceIdx in pieceSet.GetSetBits())
                {
                    var legalMoves =
                        Bitboard.Instance.GetLegalMoves(pieceIdx, occupancy, enPassantIdx, castlingAvailability);
                    if (legalMoves.Any())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsDrawn(ulong[][] occupancy)
        {
            var isDrawn = true;

            foreach (var color in Enums.GetValues<Color>())
            {
                foreach (var piece in Enums.GetValues<Piece>().Where(p => p != Piece.King))
                {
                    if (occupancy.Occupancy(color, piece) != 0)
                    {
                        isDrawn = false;
                        break;
                    }
                }
            }

            return isDrawn;
        }

        /// <summary>
        ///     Determines if active player has been mated
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckmate(Board board)
        {
            var occupancy = board.Occupancy;
            var activeColor = board.ActivePlayer;
            var checkType = GetCheckType(occupancy, activeColor, out var attackingPieces);
            var kingIndex = occupancy.Occupancy(activeColor, Piece.King).GetSetBits()[0];
            if (checkType == CheckType.None)
            {
                return false;
            }

            var kingMoves = Bitboard.Instance.GetLegalMoves(kingIndex, occupancy, null,
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
                if (CanAttackBeBlocked(board, kingIndex, attackingPiece, occupancy, activeColor))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanAttackBeBlocked(Board board, ushort indexOfAttackedPiece, ushort attackingPieceIndex,
            ulong[][] occupancy, Color activeColor)
        {
            var squaresBetweenKingAndAttackingPiece = InBetween(indexOfAttackedPiece, attackingPieceIndex);
            foreach (var squareBetween in squaresBetweenKingAndAttackingPiece.GetSetBits())
            {
                //Can any piece block the check (besides the King).
                var attackedPieceValue = indexOfAttackedPiece.GetBoardValueOfIndex();
                var availableBlockers =
                    Bitboard.Instance.GetPiecesThatCanMoveToSquare(occupancy, squareBetween, activeColor) &
                    ~attackedPieceValue;
                foreach (var availableBlocker in availableBlockers.GetSetBits())
                {
                    var legalMovesForPotentialBlocker = Bitboard.Instance.GetLegalMoves(availableBlocker, occupancy,
                        board.EnPassantIndex, board.CastlingAvailability);
                    if (legalMovesForPotentialBlocker.Any(x => x.DestinationIndex == squareBetween))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static ushort[] GetPiecesAttackingSquare(ulong[][] pieces, ushort idx, Color attackerColor,
            ushort? blockerToRemove = null)
        {
            pieces = blockerToRemove.HasValue ? RemovePotentialBlocker(pieces, blockerToRemove.Value) : pieces;
            var piecesAttacking = Bitboard.Instance.PiecesAttackingSquareByColor(pieces, idx, attackerColor);
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

        /// <summary>
        ///     Gets the type of move based on the current board and piece source/destination
        /// </summary>
        /// <param name="boardInfo">Board information for current position</param>
        /// <param name="source">Source Index</param>
        /// <param name="dest">Destination Index</param>
        /// <returns>The type of move represented by the given parameters</returns>
        /// <exception cref="PieceException">Thrown if there is no piece on the source square.</exception>
        public static MoveType GetMoveType(in Board boardInfo, ushort source, ushort dest)
        {
            var relevantPieces = new[] {Piece.Pawn, Piece.King};
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

            if (IsEnPassantCapture(piece, source, dest, boardInfo.EnPassantIndex))
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


        private static bool IsCastlingMove(Color activeColor, ushort source, ushort destination)
        {
            var legalCastlingMovesForColor = activeColor == Color.White
                ? MoveHelpers.WhiteCastlingMoves
                : MoveHelpers.BlackCastlingMoves;
            return legalCastlingMovesForColor.Any(m =>
                m.SourceIndex == source && m.DestinationIndex == destination);
        }

        private static bool IsEnPassantCapture(Piece sourcePiece, ushort src, ushort dest, ushort? EnPassantIndex)
        {
            if (EnPassantIndex != null)
            {
                if (sourcePiece == Piece.Pawn)
                {
                    if (dest == EnPassantIndex.Value && dest.GetFile() != src.GetFile())
                    {
                        return true;
                    }
                }
            }

            return false;
        }


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
        public static ushort SquareTextToIndex(this string square)
        {
            var squareIndex = BoardConstants.SquareNames.Select((sq, idx) => new {square = sq, index = (ushort) idx})
                .FirstOrDefault(x => x.square == square)?.index;
            if (squareIndex == null)
            {
                throw new ArgumentException($"{square} is not recognized as a legal square.");
            }

            Debug.Assert(squareIndex >= 0 && squareIndex < 64);
            return squareIndex.Value;
        }


        /// <summary>
        ///     Gets a File basked on boardIndex index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetFile(this ushort square)
        {
            return (ushort) (square % 8);
        }

        /// <summary>
        ///     Gets a Rank basked on boardIndex index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rank GetRank(this int square)
        {
            return (Rank) ((ushort) square).GetRank();
        }

        /// <summary>
        ///     Gets a Rank basked on boardIndex index, 0-7 [ranks 1 - 8]
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
            return (ushort) (boardIndex / 8);
        }

        /// <summary>
        ///     Gets
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankCompliment(this ushort rank)
        {
            return (ushort) Math.Abs(rank - 7);
        }

        #endregion
    }
}