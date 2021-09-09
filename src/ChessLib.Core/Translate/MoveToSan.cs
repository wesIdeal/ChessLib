using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Translate
{
    public class MoveToSan : MoveDto<string>
    {
        public override string Translate(Move move, Board preMoveBoard, Board postMoveBoard)
        {
            string capture = "", promotionPiece = "", checkInfo = "", result = "";
            if (move.MoveType == MoveType.Castle)
            {
                return CastleToSan(move);
            }


            var strSrcPiece = GetSANSourceString(preMoveBoard, move);

            if (postMoveBoard.PieceCaptured.HasValue)
            {
                capture = "x";
            }

            if (move.MoveType == MoveType.Promotion)
            {
                promotionPiece = $"={PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece)}";
            }

            if (BoardHelpers.IsColorInCheck(postMoveBoard.Occupancy, postMoveBoard.ActivePlayer))
            {
                checkInfo = "+";
                if (postMoveBoard.GameState == GameState.Checkmate)
                {
                    checkInfo = "#";
                }
            }

            //Get piece representation
            var san =
                $"{strSrcPiece}{capture}{move.DestinationIndex.ToSquareString()}{promotionPiece}{checkInfo} {result}"
                    .Trim();
            return san;
        }

        private string GetSANSourceString(Board preMoveBoard, Move move)
        {
            var src = BoardHelpers.GetPieceAtIndex(preMoveBoard.Occupancy, move.SourceIndex);
            Debug.Assert(src.HasValue);
            if (src == Piece.King)
            {
                return "K";
            }

            if (src == Piece.Pawn)
            {
                var source = "";

                //if the move was an En Passant or a capture, return the file letter
                if (move.MoveType == MoveType.EnPassant ||
                    move.SourceIndex.GetFile() != move.DestinationIndex.GetFile())
                {
                    source = move.SourceIndex.IndexToFileDisplay().ToString();
                }

                return source;
            }

            var strSrcPiece = src.Value.GetCharRepresentation().ToString().ToUpper();
            var otherLikePieces = preMoveBoard.Occupancy.Occupancy(preMoveBoard.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                var legalMoves = Bitboard.Instance.GetLegalMoves(attackerIndex,
                    preMoveBoard.Occupancy, preMoveBoard.EnPassantIndex, preMoveBoard.CastlingAvailability);
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

            return strSrcPiece + move.SourceIndex.ToSquareString();
        }


        private static string CastleToSan(Move move)
        {
            var destinationFile = move.DestinationIndex;
            if (destinationFile.GetFile() == 2)
            {
                return "O-O-O";
            }

            return "O-O";
        }
    }
}