﻿using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.MoveValidation.CastlingRules
{
    [TestFixture]
    public class HasValidDestinationSquare : ChessLib.Validators.MoveValidation.CastlingRules.HasValidDestinationSquare
    {
        #region BadDestination

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            var BoardInfo = new BoardInfo("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveExceptionType.Castle_BadDestinationSquare, Validate(BoardInfo, postMoveBoard, move));
            }
        }
        #endregion
        #region GoodDestination

        [Test]
        public void Validate_ShouldReturnNoErrorForGoodDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = this.ValidDestinationSquares;
            var BoardInfo = new BoardInfo("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.IsNull(Validate(BoardInfo, postMoveBoard, move));
            }
        }
        #endregion
    }
}