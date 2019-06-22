using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessLib.Data.Helpers
{
    public static class MoveHelpers
    {


        /// <summary>
        /// Creates a <see cref="MoveExt"/> object from move details.
        /// </summary>
        ///<remarks >Basically makes a ushort from the 4 details needed to make a move</remarks>
        /// <returns></returns>
        public static MoveExt GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new MoveExt((ushort)(mt | pp | origin | dest));
        }


        


        

       


        /// <summary>
        /// Gets a Standard Algebraic Notation string from a move.
        /// </summary>
        public static string MoveToSAN(this MoveExt move, IBoard boardInfo, bool recordResult = true)
        {
            var sideMoving = boardInfo.ActivePlayer;
            var preMoveBoard = boardInfo.GetPiecePlacement();
            var postMoveBoard = boardInfo.GetPiecePlacement().GetBoardPostMove(sideMoving, move);
            var srcPiece = boardInfo.GetPiecePlacement().GetPieceOfColorAtIndex(move.SourceIndex)?.Piece;
            if (srcPiece == null) throw new MoveException("No piece at source index.", MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, move, sideMoving);
            var strSrcPiece = GetSANSourceString(boardInfo, move, srcPiece.Value);
            var strDstSquare = move.DestinationIndex.IndexToSquareDisplay();
            string checkInfo = "", result = "", promotionInfo = "", capture = "";

            var opponentOcc = preMoveBoard.Occupancy(sideMoving.Toggle());
            if ((opponentOcc & move.DestinationValue) != 0 || move.MoveType == MoveType.EnPassant)
            {
                capture = "x";
            }

            if (move.MoveType == MoveType.Promotion)
            {
                promotionInfo = $"={PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece)}";
            }

            var board = boardInfo.ApplyMoveToBoard(move);
            if (board.IsActivePlayerInCheck())
            {
                checkInfo = "+";
                if (board.IsCheckmate())
                {
                    checkInfo = $"#";
                    if (recordResult)
                    {
                        result = (sideMoving == Color.White ? "1-0" : "0-1");
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

        public static string GetSANSourceString(IBoard board, MoveExt move, Piece src)
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
            var otherLikePieces = board.GetPiecePlacement().Occupancy(board.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                if (board.CanPieceMoveToDestination(attackerIndex, move.DestinationIndex))
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
