﻿using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    [TestFixture]
    internal class DestinationNotOccupiedByActiveColor : Core.Validation.Validators.MoveValidation.MoveRules.
        DestinationNotOccupiedByActiveColor
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [Test]
        public void ShouldReturnNullIfTargetUnoccupied()
        {
            var board = FenReader.Translate("4k3/8/8/8/8/5N2/8/4K3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }

        [Test]
        public void ShouldReturnNullIfTargetOccupiedByOpponent()
        {
            var fen = "4k3/8/8/8/8/5N2/8/4K1b1 w - - 0 1";
            var board = FenReader.Translate(fen);
            Console.WriteLine(board.Fen);
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }

        [Test]
        public void ShouldReturnErrorIfTargetOccupiedByActivePlayer()
        {
            var board = FenReader.Translate("4k3/8/8/8/8/5N2/8/4K1B1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.ActiveColorPieceAtDestination, Validate(board, postMove, move));
        }
    }
}