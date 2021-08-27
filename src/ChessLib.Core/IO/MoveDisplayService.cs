using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.IO
{
    public class MoveDisplayService
    {
        public MoveDisplayService(Board board)
        {
            Initialize(board);
        }

        protected Board Board;


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
    }
}