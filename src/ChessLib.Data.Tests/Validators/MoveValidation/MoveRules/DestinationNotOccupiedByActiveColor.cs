﻿using System;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Data.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
{
    [TestFixture()]
    class DestinationNotOccupiedByActiveColor : Core.Validation.Validators.MoveValidation.MoveRules.DestinationNotOccupiedByActiveColor
    {
        [Test]
        public void ShouldReturnNullIfTargetUnoccupied()
        {
            var board = new Board("4k3/8/8/8/8/5N2/8/4K3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board,postMove,move));
        }
        [Test]
        public void ShouldReturnNullIfTargetOccupiedByOpponent()
        {
            var fen = "4k3/8/8/8/8/5N2/8/4K1b1 w - - 0 1";
            var board = new Board(fen);
            Console.WriteLine(board.CurrentFEN);
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }
        [Test]
        public void ShouldReturnErrorIfTargetOccupiedByActivePlayer()
        {
            var board = new Board("4k3/8/8/8/8/5N2/8/4K1B1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.ActiveColorPieceAtDestination, Validate(board, postMove, move));
        }
    }
}
