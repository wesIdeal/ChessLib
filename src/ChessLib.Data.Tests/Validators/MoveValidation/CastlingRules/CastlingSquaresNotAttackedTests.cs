using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.MoveValidation.CastlingRules
{
    [TestFixture]
    public class CastlingSquaresNotAttacked : ChessLib.Validators.MoveValidation.CastlingRules.CastlingSquaresNotAttacked
    {
        [Test(Description = "Should return correct error when castle's path is attacked")]
        public void Validate_ShouldReturnCorrectErrorWhenPathIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = new BoardInfo("4k2r/8/8/8/8/8/8/4KR2 b kq - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(position.GetPiecePlacement(), position.ActivePlayer, move);
            Assert.AreEqual(MoveExceptionType.Castle_ThroughCheck, Validate(position, postMoveBoard, move), "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
        }

        [Test(Description = "Should return null when castle's path is not attacked")]
        public void Validate_ShouldReturnNullWhenPathIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = new BoardInfo("4k2r/8/8/8/8/8/8/4K3 b kq - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(position.GetPiecePlacement(), position.ActivePlayer, move);
            Assert.IsNull(Validate(position, postMoveBoard, move), "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
        }
        [Test]
        public static void IsKingsPathInCheck_ShouldReturnFalse_IfKingUnaffectedByAnyPiece()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = new BoardInfo("4k2r/8/8/8/8/8/8/4K3 b kq - 1 2");
            Assert.IsFalse(IsKingsPathInCheck(position.OpponentColor(), position.GetPiecePlacement(), move), "IsKingsPathInCheck() should return false when nothing blocks castling privilege.");
        }

        [Test]
        public static void IsKingsPathInCheck_ShouldReturnTrue_IfAnySquareIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var pos1 = new BoardInfo("4k2r/8/8/8/8/8/8/4KR2 b kq - 1 2");
            var pos2 = new BoardInfo("4k2r/8/8/8/8/8/8/4K1R1 b kq - 1 2");
            var pos3 = new BoardInfo("4k2r/8/8/8/8/8/4R3/4K3 b kq - 1 2");

            Assert.IsTrue(IsKingsPathInCheck(pos1.OpponentColor(), pos1.GetPiecePlacement(), move), "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos2.OpponentColor(), pos1.GetPiecePlacement(), move), "IsKingsPathInCheck() should return true when Rook on g1 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos3.OpponentColor(), pos1.GetPiecePlacement(), move), "IsKingsPathInCheck() should return true when Rook on e2 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos3.OpponentColor(), pos1.GetPiecePlacement(), move), "IsKingsPathInCheck() should return true when Rooks on f1-h1 block castling privilege.");

        }
        [Test]
        public static void IsKingsPathInCheck_ShouldReturnFalse_IfKingUnaffectedByOpposingRook()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = new BoardInfo("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            Assert.IsFalse(IsKingsPathInCheck(position.OpponentColor(), position.GetPiecePlacement(), move), "IsKingsPathInCheck() should return false when Rook on h1 doesn't block castling privilege.");
        }

    }
}
