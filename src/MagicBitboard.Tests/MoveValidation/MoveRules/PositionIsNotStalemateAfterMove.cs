using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.MoveRules
{
    [TestFixture()]
    public class PositionIsNotStalemateAfterMove : ChessLib.MagicBitboard.MoveValidation.MoveRules.PositionIsNotStalemateAfterMove
    {

        [TestCase("5r2/6Pk/1R6/7P/6K1/8/8/8 w - - 0 62", 54, 61, PromotionPiece.Queen, MoveType.Promotion)]
        [TestCase("4k1K1/6P1/8/7q/8/8/8/8 w - - 9 56", 51, 60)]
        [TestCase("7k/7P/7K/8/8/8/8/8 b - - 2 65", 46, 47)]
        public void IsStalemate(string fen, int f, int t, PromotionPiece p = PromotionPiece.Knight, MoveType type = MoveType.Normal)
        {
            var board = new BoardInfo(fen);
            var move = MoveHelpers.GenerateMove((ushort)f, (ushort)t, type, p);
            var postMoveBoard = BoardHelpers.GetBoardPostMove(board.PiecesOnBoard, board.ActivePlayerColor, move);
            var fen2 = board.FEN;
            var validation = Validate(board, postMoveBoard, move);
            Assert.AreEqual(MoveExceptionType.Stalemate, validation);

        }

    }
}
