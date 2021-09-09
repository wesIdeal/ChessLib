using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Translate
{
    public class MoveToLan : ChessDto<Move, string>
    {
        public override string Translate(Move move)
        {
            return
                $"{move.SourceIndex.ToSquareString()}{move.DestinationIndex.ToSquareString()}{PromotionPieceChar(move)}";
        }

        private static char? PromotionPieceChar(Move move)
        {
            return move.MoveType == MoveType.Promotion
                ? char.ToLower(PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece))
                : (char?)null;
        }
    }
}