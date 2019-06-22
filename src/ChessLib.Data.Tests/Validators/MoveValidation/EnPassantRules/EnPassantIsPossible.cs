using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.MoveValidation.EnPassantRules
{
    [TestFixture]
    class SourceIsPawn : ChessLib.Validators.MoveValidation.EnPassantRules.SourceIsPawn
    {
        private readonly ulong[][] _pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenSourceIsNotPawn()
        {
            var board = new BoardInfo("rnbqkbnr/pppp1ppp/8/3Bp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.EP_SourceIsNotPawn, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenSourceIsPawn()
        {
            var board = new BoardInfo("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, _pmb, move));
        }
    }

    [TestFixture]
    class SourceIsCorrectRank : ChessLib.Validators.MoveValidation.EnPassantRules.SourceIsCorrectRank
    {
        readonly ulong[][] _pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank()
        {
            var board = new BoardInfo("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(27, 44);
            Assert.AreEqual(MoveError.EP_WrongSourceRank, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource()
        {
            var board = new BoardInfo("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, _pmb, move));
        }
        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank_Black()
        {
            var board = new BoardInfo("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(35, 28);
            Assert.AreEqual(MoveError.EP_WrongSourceRank, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource_Black()
        {
            var board = new BoardInfo("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(27, 20);
            Assert.IsNull(Validate(board, _pmb, move));
        }
    }

    [TestFixture]
    class EnPassantSquareIsAttackedBySource : ChessLib.Validators.MoveValidation.EnPassantRules.EnPassantSquareIsAttackedBySource
    {
        readonly ulong[][] _pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotAttackedBySource()
        {
            var board = new BoardInfo("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 45);
            var actual = Validate(board, _pmb, move);
            Assert.AreEqual(MoveError.EP_NotAttackedBySource, actual);
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsAttackedBySource()
        {
            var board = new BoardInfo("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, _pmb, move));
        }
    }
    [TestFixture()]
    class EnPassantIsPossible : ChessLib.Validators.MoveValidation.EnPassantRules.EnPassantIsPossible
    {
        readonly ulong[][] _pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotPossible()
        {
            var board = new BoardInfo("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.Ep_NotAvailalbe, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsPossible()
        {
            var board = new BoardInfo("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, _pmb, move));
        }
    }
}
