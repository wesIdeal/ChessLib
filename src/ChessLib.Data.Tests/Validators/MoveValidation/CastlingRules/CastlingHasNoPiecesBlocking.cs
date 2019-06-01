using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.MoveValidation.CastlingRules
{
    /// <summary>
    /// Tests for castling through occupied and non-occupied squares between castling King and Rook
    /// </summary>
    [TestFixture(Description = "Tests for castling through occupied and non-occupied squares between castling King and Rook")]
    class CastlingHasNoPiecesBlocking : ChessLib.Validators.MoveValidation.CastlingRules.CastlingHasNoPiecesBlocking
    {
        BoardInfo _biOccupied, _biNonOccupied;
        [SetUp]
        public void Setup()
        {
            _biOccupied = new BoardInfo("r1N1k1Nr/8/8/8/8/8/8/R1B1K1BR w KQkq - 0 1");
            _biNonOccupied = new BoardInfo("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        }

        #region Occupancy Between Castle
        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_q()
        {
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_k()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_Q()
        {
            var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_K()
        {
            var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }
        #endregion

        #region No Occupancy Between Castle
        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_q()
        {
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_k()
        {
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_Q()
        {
            var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_K()
        {
            var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }
        #endregion
        #region BadDestination

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.Throws(typeof(MoveException), () =>
                {
                    try
                    {
                        Validate(_biNonOccupied, postMoveBoard, move);
                    }
                    catch (MoveException m)
                    {
                        Assert.AreEqual(m.ExceptionType, m.ExceptionType);
                        throw;
                    }
                });
            }
        }
        #endregion

        private void AssertOccupiedExceptionThrown(MoveExt move)
        {
            const MoveExceptionType expected = MoveExceptionType.Castle_OccupancyBetween;
            var result = Validate(_biOccupied,
                BoardHelpers.GetBoardPostMove(_biOccupied.GetPiecePlacement(), _biOccupied.ActivePlayer, move), move);
            Assert.AreEqual(expected, result);
        }

        private void AssertOccupiedExceptionNotThrown(MoveExt move)
        {
            var result = Validate(_biNonOccupied,
                BoardHelpers.GetBoardPostMove(_biOccupied.GetPiecePlacement(), _biOccupied.ActivePlayer, move), move);
            Assert.IsNull(result);
        }
    }
}
