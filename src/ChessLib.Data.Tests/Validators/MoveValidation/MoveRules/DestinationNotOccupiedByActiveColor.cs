using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Services;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
{
    [TestFixture]
    internal class DestinationNotOccupiedByActiveColor : Core.Validation.Validators.MoveValidation.MoveRules.
        DestinationNotOccupiedByActiveColor
    {
        private static readonly FenReader FenReader = new FenReader();

        [Test]
        public void ShouldReturnNullIfTargetUnoccupied()
        {
            var board = FenReader.GetBoard("4k3/8/8/8/8/5N2/8/4K3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }

        [Test]
        public void ShouldReturnNullIfTargetOccupiedByOpponent()
        {
            var fen = "4k3/8/8/8/8/5N2/8/4K1b1 w - - 0 1";
            var board = new FenReader().GetBoard(fen);
            Console.WriteLine(board.CurrentFEN);
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }

        [Test]
        public void ShouldReturnErrorIfTargetOccupiedByActivePlayer()
        {
            var board = FenReader.GetBoard("4k3/8/8/8/8/5N2/8/4K1B1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.ActiveColorPieceAtDestination, Validate(board, postMove, move));
        }
    }
}