using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;

[assembly: InternalsVisibleTo("ChessLib.EngineInterface.UCI")]

namespace ChessLib.Core.Translate
{
    public class LanVariationToMoves : ChessDto<IEnumerable<string>, IEnumerable<Move>>
    {
        private static readonly LanToMove lanToMove = new LanToMove();

        public override IEnumerable<Move> Translate(IEnumerable<string> strMoves)
        {
            foreach (var move in strMoves)
            {
                yield return lanToMove.Translate(move);
            }
        }
    }

    public class LanToMove : ChessDto<string, Move>
    {
        /// <summary>
        ///     Gets a basic move with no SAN and no en passant information
        /// </summary>
        /// <param name="lan"></param>
        /// <returns></returns>
        public static Move BasicMoveFromLAN(string lan)
        {
            var length = lan.Length;
            if (length < 4 || length > 5)
            {
                throw new MoveException($"LAN move {lan} has invalid length.");
            }

            var sourceString = lan.Substring(0, 2);
            var destString = lan.Substring(2, 2);
            var source = sourceString.ToBoardIndex();
            var dest = destString.ToBoardIndex();
            var promotionChar = lan.Length == 5 ? lan[4] : (char?) null;
            var promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionChar);
            var isPromotion = length == 5;

            return MoveHelpers.GenerateMove(source, dest,
                isPromotion ? MoveType.Promotion : MoveType.Normal, promotionPiece);
        }

        public override Move Translate(string from)
        {
            return BasicMoveFromLAN(from);
        }
    }
}