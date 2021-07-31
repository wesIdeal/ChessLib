using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation;

namespace ChessLib.Core.Translate
{
    internal class SanToMove
    {
        protected readonly string CastleKingSide = "O-O";
        protected readonly string CastleQueenSide = "O-O-O";


        public Move GetMoveFromSAN(Board board, string sanMove)
        {
            Move moveExt;
            var strMove = StripNonMoveInfoFromMove(sanMove);
            if (strMove.Length < 2)
            {
                throw new MoveException("Invalid move. Must have at least 2 characters.");
            }

            var colorMoving = board.ActivePlayer;

            if (char.IsLower(strMove[0]))
            {
                moveExt = GetPawnMoveDetails(board, strMove);
            }
            else if (strMove == CastleKingSide || strMove == CastleQueenSide)
            {
                if (colorMoving == Color.White)
                {
                    return strMove == CastleKingSide
                        ? MoveHelpers.WhiteCastleKingSide
                        : MoveHelpers.WhiteCastleQueenSide;
                }

                return strMove == CastleKingSide ? MoveHelpers.BlackCastleKingSide : MoveHelpers.BlackCastleQueenSide;
            }
            else
            {
                var pieceMoving = PieceHelpers.GetPiece(strMove[0]);
                var destinationSquare = strMove.Substring(strMove.Length - 2, 2).SquareTextToIndex();
                var applicableBlockerBoard = board.Occupancy.Occupancy(colorMoving, pieceMoving);
                var squaresAttackingTarget =
                    Bitboard.Instance.PiecesAttackingSquareByColor(board.Occupancy, destinationSquare, colorMoving);
                var activePieces = squaresAttackingTarget & applicableBlockerBoard;
                if (activePieces == 0)
                {
                    throw new MoveException(
                        $"No pieces on any squares are attacking the square {destinationSquare.IndexToSquareDisplay()}",
                        board);
                }

               

                var attackerIndices = activePieces.GetSetBits();
                

                if (attackerIndices.Length == 0)
                {
                    throw new MoveException(
                        $"Error with move {sanMove}:No pieces of type {pieceMoving.ToString()} are attacking the square {destinationSquare.IndexToSquareDisplay()}",
                        board);
                }

                if (attackerIndices.Length == 1)
                {
                    moveExt = MoveHelpers.GenerateMove(attackerIndices[0], destinationSquare);
                }
                else
                {
                    moveExt = DetermineWhichPieceMovesToSquare(board, strMove, attackerIndices,
                        applicableBlockerBoard,
                        destinationSquare);
                }
            }

            if (moveExt == null)
            {
                throw new NoNullAllowedException(
                    $"MoveTranslatorService: Move should not be null after translation. Error in PGN or application for move {sanMove}.");
            }

            return moveExt;
        }

        private string StripNonMoveInfoFromMove(in string move)
        {
            var nmi = new[] {"#", "+", "1/2-1/2", "1-0", "0-1"};
            var mv = (string) move.Clone();
            foreach (var s in nmi)
            {
                mv = mv.Replace(s, "");
            }

            return mv;
        }

        private Move DetermineWhichPieceMovesToSquare(in Board board, in string move,
            IEnumerable<ushort> possibleAttackersOfType,
            ulong applicableBb, ushort destinationSquare)
        {
            var mv = move;
            mv = mv.Substring(1);
            mv = mv.Substring(0, mv.Length - 2);
            mv = mv.Replace("x", "");
            ushort source;
            if (mv.Length == 2)
            {
                var sourceIdx = mv.SquareTextToIndex();
                source = sourceIdx;
            }
            else if (mv.Length == 1)
            {
                ushort[] sourceSquares;
                if (char.IsDigit(mv[0]))
                {
                    var sourceRank = ushort.Parse(mv) - 1;
                    sourceSquares = possibleAttackersOfType.Where(s => s.GetRank() == sourceRank).ToArray();
                }
                else
                {
                    var sourceFile = mv[0] - 'a';
                    sourceSquares = possibleAttackersOfType.Where(s => s.GetFile() == sourceFile).ToArray();
                }

                if (!sourceSquares.Any())
                {
                    throw new MoveException($"Problem finding attacking piece from move {move}.");
                }

                if (sourceSquares.Length > 1)
                {
                    throw new MoveException($"Problem finding only one attacking piece from move {move}.");
                }

                source = sourceSquares[0];
            }

            else
            {
                var narrowedSquares = new List<ushort>();
                foreach (var square in possibleAttackersOfType)
                {
                    var testMove = MoveHelpers.GenerateMove(square, destinationSquare);
                    var moveValidator = new MoveValidator(board, testMove);
                    if (moveValidator.Validate() == MoveError.NoneSet)
                    {
                        narrowedSquares.Add(square);
                    }
                }

                if (narrowedSquares.Count != 1)
                {
                    throw new MoveException($"Problem finding only one attacking piece from move {move}.");
                }

                source = narrowedSquares[0];
            }

            var sourceVal = source.GetBoardValueOfIndex();
            if ((sourceVal & applicableBb) == 0)
            {
                throw new MoveException(
                    $"No pieces attack {destinationSquare.IndexToSquareDisplay()} from move {move}.");
            }

            return MoveHelpers.GenerateMove(source, destinationSquare);
        }

        private Move GetPawnMoveDetails(Board preMoveBoard, string move)
        {
            var promotionPiece = PromotionPiece.Knight;
            var colorMoving = preMoveBoard.ActivePlayer;
            var pawnBitBoard = preMoveBoard.Occupancy.Occupancy(colorMoving);

            var moveLength = move.Length;
            var isCapture = move.Contains("x");

            var isPromotion = move.Contains("=");
            ushort startingFile = 0;
            var moveType = MoveType.Normal;
            if (isCapture)
            {
                startingFile = (ushort) (move[0] - 'a');
                move = move.Substring(2, moveLength - 2);
                if (preMoveBoard.EnPassantIndex.HasValue)
                {
                }
            }

            if (isPromotion)
            {
                moveType = MoveType.Promotion;
                var equalIndex = move.IndexOf('=');
                var promotionPieceStr = move.Substring(equalIndex + 1, 1);
                promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceStr[0]);
                move = move.Substring(0, equalIndex);
            }

            ushort? destIndex = move.SquareTextToIndex();
            if (!destIndex.HasValue)
            {
                throw new NoNullAllowedException("MoveTranslatorService: destIndex should not be null.");
            }

            var likelyStartingSq = (ushort) (colorMoving == Color.White ? destIndex.Value - 8 : destIndex.Value + 8);
            var sourceIndex = likelyStartingSq;
            Debug.Assert(destIndex.HasValue);

            var dpRankMask = colorMoving == Color.Black ? BoardConstants.Rank5 : BoardConstants.Rank4;
            var isPossibleInitialMove = (destIndex.Value.GetBoardValueOfIndex() & dpRankMask) != 0;
            if (isCapture)
            {
                var destinationFile = destIndex.Value.GetFile();
                if (colorMoving == Color.White)
                {
                    var modifier = destinationFile - startingFile + 8;
                    sourceIndex = (ushort) (destIndex.Value - modifier);
                }
                else
                {
                    var modifier = startingFile - destinationFile + 8;
                    sourceIndex = (ushort) (destIndex.Value + modifier);
                }
            }
            else if (isPossibleInitialMove)
            {
                Func<ulong, ulong> Shift = BitHelpers.ShiftS;
                if (colorMoving == Color.Black)
                {
                    Shift = BitHelpers.ShiftN;
                }

                var possibleStart = destIndex.Value.GetBoardValueOfIndex();
                for (var i = 0; i < 2; i++)
                {
                    possibleStart = Shift(possibleStart);
                    if ((possibleStart & preMoveBoard.Occupancy.Occupancy(colorMoving, Piece.Pawn)) != 0)
                    {
                        sourceIndex = possibleStart.GetSetBits()[0];
                    }
                }
            }

            if (isCapture && preMoveBoard.EnPassantIndex.HasValue &&
                destIndex.Value == preMoveBoard.EnPassantIndex.Value)
            {
                moveType = MoveType.EnPassant;
            }

            return MoveHelpers.GenerateMove(sourceIndex, destIndex.Value, moveType, promotionPiece);
        }
    }
}