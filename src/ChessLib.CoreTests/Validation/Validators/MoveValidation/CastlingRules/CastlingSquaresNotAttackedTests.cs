using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    [TestFixture]
    public class
        CastlingSquaresNotAttacked : Core.Validation.Validators.MoveValidation.CastlingRules.CastlingSquaresNotAttacked
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [Test(Description = "Should return correct error when castle's path is attacked")]
        public void Validate_ShouldReturnCorrectErrorWhenPathIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = FenReader.Translate("4k2r/8/8/8/8/8/8/4KR2 b kq - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(position, move);
            Assert.AreEqual(MoveError.CastleThroughCheck, Validate(position, postMoveBoard, move),
                "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
        }

        [Test(Description = "Should return null when castle's path is not attacked")]
        public void Validate_ShouldReturnNullWhenPathIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = FenReader.Translate("4k2r/8/8/8/8/8/8/4K3 b kq - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(position, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(position, postMoveBoard, move),
                "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
        }

        [Test]
        public static void IsKingsPathInCheck_ShouldReturnFalse_IfKingUnaffectedByAnyPiece()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = FenReader.Translate("4k2r/8/8/8/8/8/8/4K3 b kq - 1 2");
            Assert.IsFalse(IsKingsPathInCheck(position.ActivePlayer.Toggle(), position.Occupancy, move, position.EnPassantIndex),
                "IsKingsPathInCheck() should return false when nothing blocks castling privilege.");
        }

        [Test]
        public static void IsKingsPathInCheck_ShouldReturnTrue_IfAnySquareIsAttacked()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var pos1 = FenReader.Translate("4k2r/8/8/8/8/8/8/4KR2 b kq - 1 2");
            var pos2 = FenReader.Translate("4k2r/8/8/8/8/8/8/4K1R1 b kq - 1 2");
            var pos3 = FenReader.Translate("4k2r/8/8/8/8/8/4R3/4K3 b kq - 1 2");

            Assert.IsTrue(IsKingsPathInCheck(pos1.ActivePlayer.Toggle(), pos1.Occupancy, move, pos1.EnPassantIndex),
                "IsKingsPathInCheck() should return true when Rook on f1 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos2.ActivePlayer.Toggle(), pos1.Occupancy, move, pos2.EnPassantIndex),
                "IsKingsPathInCheck() should return true when Rook on g1 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos3.ActivePlayer.Toggle(), pos1.Occupancy, move, pos1.EnPassantIndex),
                "IsKingsPathInCheck() should return true when Rook on e2 blocks castling privilege.");
            Assert.IsTrue(IsKingsPathInCheck(pos3.ActivePlayer.Toggle(), pos1.Occupancy, move, pos3.EnPassantIndex),
                "IsKingsPathInCheck() should return true when Rooks on f1-h1 block castling privilege.");
        }

        [Test]
        public static void IsKingsPathInCheck_ShouldReturnFalse_IfKingUnaffectedByOpposingRook()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            var position = FenReader.Translate("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            Assert.IsFalse(IsKingsPathInCheck(position.ActivePlayer.Toggle(), position.Occupancy, move, position.EnPassantIndex),
                "IsKingsPathInCheck() should return false when Rook on h1 doesn't block castling privilege.");
        }
    }
}