using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Data
{
    public class MoveDisplayService
    {
        protected IBoard Board;

        public MoveDisplayService()
        {
            Initialize();
        }

        public MoveDisplayService(string fen)
        {
            Initialize(fen);
        }

        public MoveDisplayService(IBoard board)
        {
            Initialize(board);
        }

        public void Initialize()
        {
            Board = new BoardInfo();
        }

        public void Initialize(string fen)
        {
            Board = new BoardInfo(fen);
        }

        public void Initialize(IBoard board)
        {
            Board = (IBoard)board.Clone();
        }
        public static string MoveToLan(MoveExt move) => $"{move.SourceIndex.IndexToSquareDisplay()}{move.DestinationIndex.IndexToSquareDisplay()}{PromotionPieceChar(move)}";

        private static char? PromotionPieceChar(MoveExt move) =>
            move.MoveType == MoveType.Promotion ?
                char.ToLower(PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece)) : (char?)null;

        /// <summary>
        /// Gets a Standard Algebraic Notation string from a move.
        /// </summary>
        /// <param name="move">move to convert</param>
        /// <param name="recordResult">if true, the game result is included with the SAN (ie. 1-0, 1/2-1/2, 0-1)</param>
        public string MoveToSAN(MoveExt move, bool recordResult = true)
        {
            var activePlayer = Board.ActivePlayer;
            var preMoveBoard = Board.GetPiecePlacement();
            var postMoveBoard = BoardHelpers.GetBoardPostMove(Board, move);
            var srcPiece = Board.GetPiecePlacement().GetPieceOfColorAtIndex(move.SourceIndex)?.Piece;
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

            var board = Board.ApplyMoveToBoard(move, true);
            if (board.IsActivePlayerInCheck())
            {
                checkInfo = "+";
                if (board.IsCheckmate())
                {
                    checkInfo = "#";
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
            var otherLikePieces = Board.GetPiecePlacement().Occupancy(Board.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                if (Board.CanPieceMoveToDestination(attackerIndex, move.DestinationIndex))
                {
                    duplicateAttackerIndexes.Add(attackerIndex);
                }
            }

            if (duplicateAttackerIndexes.Count == 1) return strSrcPiece;
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
