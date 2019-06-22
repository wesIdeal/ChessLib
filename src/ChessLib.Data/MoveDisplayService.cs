using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data
{
    internal class MoveDisplayService
    {
        private readonly IBoard _board;

        public MoveDisplayService(IBoard board)
        {
            _board = board;
        }
        /// <summary>
        /// Gets a Standard Algebraic Notation string from a move.
        /// </summary>
        /// <param name="recordResult">if true, the game result is included with the SAN (ie. 1-0, 1/2-1/2, 0-1)</param>
        public string MoveToSAN(MoveExt move, bool recordResult = true)
        {
            var activePlayer = _board.ActivePlayer;
            var preMoveBoard = _board.GetPiecePlacement();
            var postMoveBoard = _board.GetPiecePlacement().GetBoardPostMove(activePlayer, move);
            var srcPiece = _board.GetPiecePlacement().GetPieceOfColorAtIndex(move.SourceIndex)?.Piece;
            if (srcPiece == null) throw new MoveException("No piece at source index.", MoveError.ActivePlayerHasNoPieceOnSourceSquare, move, activePlayer);
            var strSrcPiece = GetSANSourceString(move, srcPiece.Value);
            var strDstSquare = move.DestinationIndex.IndexToSquareDisplay();
            string checkInfo = "", result = "", promotionInfo = "", capture = "";

            var opponentOcc = preMoveBoard.Occupancy(activePlayer.Toggle());
            if ((opponentOcc & move.DestinationValue) != 0 || move.MoveType == MoveType.EnPassant)
            {
                capture = "x";
            }

            if (move.MoveType == MoveType.Promotion)
            {
                promotionInfo = $"={PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece)}";
            }

            var board = BoardHelpers.ApplyMoveToBoard(_board, move);
            if (board.IsActivePlayerInCheck())
            {
                checkInfo = "+";
                if (board.IsCheckmate())
                {
                    checkInfo = $"#";
                    if (recordResult)
                    {
                        result = (activePlayer == Color.White ? "1-0" : "0-1");
                    }
                }
            }
            else if (recordResult)
            {

                result = board.IsStalemate() ? "1/2-1/2" : "";
            }

            //Get piece representation
            return $"{strSrcPiece}{capture}{move.DestinationIndex.IndexToSquareDisplay()}{promotionInfo}{checkInfo} {result}".Trim();
        }

        internal string GetSANSourceString(MoveExt move, Piece src)
        {
            if (src == Piece.King)
            {
                return "K";
            }
            if (src == Piece.Pawn)
            {
                //if the move was an En Passant or a capture, return the file letter
                return move.MoveType == MoveType.EnPassant || (move.SourceIndex.GetFile() != move.DestinationIndex.GetFile())
                    ? move.SourceIndex.IndexToFileDisplay().ToString() : "";
            }

            var strSrcPiece = src.GetCharRepresentation().ToString().ToUpper();
            var otherLikePieces = _board.GetPiecePlacement().Occupancy(_board.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                if (_board.CanPieceMoveToDestination(attackerIndex, move.DestinationIndex))
                {
                    duplicateAttackerIndexes.Add(attackerIndex);
                }
            }

            if (duplicateAttackerIndexes.Count() == 1) return strSrcPiece;
            var duplicateFiles = duplicateAttackerIndexes.Select(x => x.GetFile()).GroupBy(x => x)
                .Any(x => x.Count() > 1);
            var duplicateRanks = duplicateAttackerIndexes.Select(x => x.GetRank()).GroupBy(x => x)
                .Any(x => x.Count() > 1);

            if (!duplicateFiles)
            {
                return strSrcPiece += move.SourceIndex.IndexToFileDisplay();
            }
            else if (!duplicateRanks)
            {
                return strSrcPiece += move.SourceIndex.IndexToRankDisplay();
            }
            else
            {
                return strSrcPiece += move.SourceIndex.IndexToSquareDisplay();
            }
        }
    }
}
