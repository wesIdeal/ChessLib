using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Parse;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;

namespace ChessLib.Core.IO
{
    public class MoveDisplayService
    {
        protected Board Board;

        public MoveDisplayService(Board board)
        {
            Initialize(board);
        }

        public void Initialize()
        {
            Board = new Board();
        }

        public void Initialize(string fen)
        {
            Board = new FenTextToBoard().Translate(fen);
        }

        public void Initialize(Board board)
        {
            Board = (Board) board.Clone();
        }

        public static string MoveToLan(Move move)
        {
            return
                $"{move.SourceIndex.IndexToSquareDisplay()}{move.DestinationIndex.IndexToSquareDisplay()}{PromotionPieceChar(move)}";
        }

        private static char? PromotionPieceChar(Move move)
        {
            return move.MoveType == MoveType.Promotion
                ? char.ToLower(PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece))
                : (char?) null;
        }

        /// <summary>
        ///     Gets a Standard Algebraic Notation string from a move.
        /// </summary>
        /// <param name="move">move to convert</param>
        /// <param name="recordResult">if true, the game result is included with the SAN (ie. 1-0, 1/2-1/2, 0-1)</param>
        public string MoveToSAN(Move move, bool recordResult = true)
        {
            if (move.MoveType == MoveType.Castle)
            {
                var destinationFile = move.DestinationIndex;
                if (destinationFile.GetFile() == 2)
                {
                    return "O-O-O";
                }

                return "O-O";
            }

            var activePlayer = Board.ActivePlayer;
            var preMoveBoard = Board.Occupancy;

            var srcPiece = preMoveBoard.GetPieceOfColorAtIndex(move.SourceIndex)?.Piece;
            if (srcPiece == null)
                throw new MoveException("No piece at source index.", MoveError.ActivePlayerHasNoPieceOnSourceSquare,
                    move, activePlayer);
            var strSrcPiece = GetSANSourceString(move, srcPiece.Value);
            move.DestinationIndex.IndexToSquareDisplay();

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


            var board = Board.ApplyMoveToBoard(move);
            if (BoardHelpers.IsColorInCheck(board.Occupancy, board.ActivePlayer))
            {
                checkInfo = "+";
                if (board.GameState == GameState.Checkmate)
                {
                    checkInfo = "#";
                    if (recordResult)
                    {
                        result = activePlayer == Color.White ? "1-0" : "0-1";
                    }
                }
            }
            else if (recordResult)
            {
                result = BoardHelpers.IsStalemate(board.Occupancy, board.ActivePlayer, board.EnPassantIndex,
                    board.CastlingAvailability)
                    ? "1/2-1/2"
                    : "";
            }

            //Get piece representation
            return
                $"{strSrcPiece}{capture}{move.DestinationIndex.IndexToSquareDisplay()}{promotionInfo}{checkInfo} {result}"
                    .Trim();
        }

        internal string GetSANSourceString(Move move, Piece src)
        {
            if (src == Piece.King)
            {
                return "K";
            }

            if (src == Piece.Pawn)
            {
                //if the move was an En Passant or a capture, return the file letter
                return move.MoveType == MoveType.EnPassant ||
                       move.SourceIndex.GetFile() != move.DestinationIndex.GetFile()
                    ? move.SourceIndex.IndexToFileDisplay().ToString()
                    : "";
            }

            var strSrcPiece = src.GetCharRepresentation().ToString().ToUpper();
            var otherLikePieces = Board.Occupancy.Occupancy(Board.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                var legalMoves = Bitboard.Instance.GetLegalMoves(attackerIndex,
                    Board.Occupancy, Board.EnPassantIndex, Board.CastlingAvailability);
                var canPieceMoveToDestination = legalMoves.Any(x => x.DestinationIndex == move.DestinationIndex);
                if (canPieceMoveToDestination)
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
                return strSrcPiece + move.SourceIndex.IndexToFileDisplay();
            }

            if (!duplicateRanks)
            {
                return strSrcPiece + move.SourceIndex.IndexToRankDisplay();
            }

            return strSrcPiece + move.SourceIndex.IndexToSquareDisplay();
        }
    }
}