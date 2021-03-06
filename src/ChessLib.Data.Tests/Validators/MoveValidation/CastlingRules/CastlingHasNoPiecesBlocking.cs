﻿using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules.Tests
{
    /// <summary>
    /// Tests for castling through occupied and non-occupied squares between castling King and Rook
    /// </summary>
    [TestFixture(Description = "Tests for castling through occupied and non-occupied squares between castling King and Rook")]
    class CastlingHasNoPiecesBlocking : Data.Validators.MoveValidation.CastlingRules.CastlingHasNoPiecesBlocking
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
            _biOccupied.ActivePlayer = Color.Black;
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_k()
        {
            _biOccupied.ActivePlayer = Color.Black;
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertOccupiedExceptionThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldReturnErrorIfPiecesAreBetween_Q()
        {
            _biOccupied.ActivePlayer = Color.White;
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
            _biOccupied.ActivePlayer = Color.Black;
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_k()
        {
            _biOccupied.ActivePlayer = Color.Black;
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertOccupiedExceptionNotThrown(move);
        }

        [Test]
        public void ValidateMove_CastleOccupancyBetween_ShouldNotReturnErrorIfNoPiecesAreBetween_Q()
        {
            var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            _biNonOccupied.ActivePlayer = Color.Black;
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
                        Assert.AreEqual(m.Error, m.Error);
                        throw;
                    }
                });
            }
        }
        #endregion

        private void AssertOccupiedExceptionThrown(MoveExt move)
        {
            const MoveError expected = MoveError.CastleOccupancyBetween;
            var result = Validate(_biOccupied,
                BoardHelpers.GetBoardPostMove(_biOccupied, move), move);
            Assert.AreEqual(expected, result);
        }

        private void AssertOccupiedExceptionNotThrown(MoveExt move)
        {
            var result = Validate(_biNonOccupied,
                BoardHelpers.GetBoardPostMove(_biOccupied, move), move);
            Assert.AreEqual(MoveError.NoneSet, result);
        }
    }
}
