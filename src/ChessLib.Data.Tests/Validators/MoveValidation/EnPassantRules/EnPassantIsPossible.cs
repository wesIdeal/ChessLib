using ChessLib.Core.Helpers;
using ChessLib.Core.Services;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules.Tests
{
    [TestFixture]
    internal class SourceIsPawn : Core.Validation.Validators.MoveValidation.EnPassantRules.SourceIsPawn
    {
        private static readonly FenReader FenReader = new FenReader();

        private readonly ulong[][] _pmb = new ulong[2][];

        [Test]
        public void ShouldReturnErrorWhenSourceIsNotPawn()
        {
            var board = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/3Bp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.EpSourceIsNotPawn, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenSourceIsPawn()
        {
            var board = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, _pmb, move));
        }
    }

    [TestFixture]
    internal class SourceIsCorrectRank : Core.Validation.Validators.MoveValidation.EnPassantRules.SourceIsCorrectRank
    {
        private static readonly FenReader FenReader = new FenReader();

        private readonly ulong[][] _pmb = new ulong[2][];

        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank()
        {
            var board = FenReader.GetBoard("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(27, 44);
            Assert.AreEqual(MoveError.EpWrongSourceRank, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource()
        {
            var board = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank_Black()
        {
            var board = FenReader.GetBoard("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(35, 28);
            Assert.AreEqual(MoveError.EpWrongSourceRank, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource_Black()
        {
            var board = FenReader.GetBoard("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(27, 20);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, _pmb, move));
        }
    }

    [TestFixture]
    internal class EnPassantSquareIsAttackedBySource : Core.Validation.Validators.MoveValidation.EnPassantRules.
        EnPassantSquareIsAttackedBySource
    {
        private static readonly FenReader FenReader = new FenReader();
        private readonly ulong[][] _pmb = new ulong[2][];

        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotAttackedBySource()
        {
            var board = FenReader.GetBoard("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 45);
            var actual = Validate(board, _pmb, move);
            Assert.AreEqual(MoveError.EpNotAttackedBySource, actual);
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsAttackedBySource()
        {
            var board = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, _pmb, move));
        }
    }

    [TestFixture]
    internal class EnPassantIsPossible : Core.Validation.Validators.MoveValidation.EnPassantRules.EnPassantIsPossible
    {
        private static readonly FenReader FenReader = new FenReader();

        private readonly ulong[][] _pmb = new ulong[2][];

        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotPossible()
        {
            var board = FenReader.GetBoard("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.EpNotAvailable, Validate(board, _pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsPossible()
        {
            var board = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, _pmb, move));
        }
    }
}