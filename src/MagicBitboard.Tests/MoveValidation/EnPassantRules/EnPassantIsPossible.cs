using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using MagicBitboard;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.EnPassantRules
{
    [TestFixture]
    class SourceIsPawn : MagicBitboard.MoveValidation.EnPassantRules.SourceIsPawn
    {
        ulong[][] pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenSourceIsNotPawn()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/3Bp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveExceptionType.EP_SourceIsNotPawn, Validate(board, pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenSourceIsPawn()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, pmb, move));
        }
    }

    [TestFixture]
    class SourceIsCorrectRank : MagicBitboard.MoveValidation.EnPassantRules.SourceIsCorrectRank
    {
        ulong[][] pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(27, 44);
            Assert.AreEqual(MoveExceptionType.EP_WrongSourceRank, Validate(board, pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, pmb, move));
        }
        [Test]
        public void ShouldReturnErrorWhenWrongSourceRank_Black()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(35, 28);
            Assert.AreEqual(MoveExceptionType.EP_WrongSourceRank, Validate(board, pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenGoodSource_Black()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp1pppp/8/8/1P1pP3/P7/2PP1PPP/RNBQKBNR b KQkq e3 0 1");
            var move = MoveHelpers.GenerateMove(27, 20);
            Assert.IsNull(Validate(board, pmb, move));
        }
    }

    [TestFixture]
    class EnPassantSquareIsAttackedBySource : MagicBitboard.MoveValidation.EnPassantRules.EnPassantSquareIsAttackedBySource
    {
        ulong[][] pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotAttackedBySource()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 45);
            var actual = Validate(board, pmb, move);
            Assert.AreEqual(MoveExceptionType.EP_NotAttackedBySource, actual);
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsAttackedBySource()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, pmb, move));
        }
    }
    [TestFixture()]
    class EnPassantIsPossible : MagicBitboard.MoveValidation.EnPassantRules.EnPassantIsPossible
    {
        ulong[][] pmb = new ulong[2][];
        [Test]
        public void ShouldReturnErrorWhenEnPassantIsNotPossible()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp2ppp/3p4/3Pp3/8/5N2/PPP1PPPP/RNBQKB1R w KQkq - 0 3");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.AreEqual(MoveExceptionType.Ep_NotAvailalbe, Validate(board, pmb, move));
        }

        [Test]
        public void ShouldReturnNullWhenEnPassantIsPossible()
        {
            var board = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 2");
            var move = MoveHelpers.GenerateMove(35, 44);
            Assert.IsNull(Validate(board, pmb, move));
        }
    }
}
