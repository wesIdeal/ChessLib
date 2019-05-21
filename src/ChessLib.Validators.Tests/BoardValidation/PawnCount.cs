using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Validators.BoardValidators;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public class PawnCount
    {
        [OneTimeSetUp]
        public void Setup()
        {

        }
        [TestCase("rnbqkbnr/pppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.WhiteTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.BlackTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.WhiteTooManyPawns | BoardException.BlackTooManyPawns)]
        [TestCase(FENHelpers.FENInitial, BoardException.None)]
        public void TestPawnCounts(string fen, BoardException expectedResult)
        {
            var board = new BoardFENInfo(fen);
            var pawnCountRule = new PawnCountRule();
            var actual = pawnCountRule.Validate(board);
            Assert.AreEqual(expectedResult, actual);
        }

        [TestCase("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62", BoardException.Stalemate)]
        [TestCase("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57", BoardException.Stalemate)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57", BoardException.Stalemate)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", BoardException.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1", BoardException.None)]
        //[TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", BoardException.None)]
        //[TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", BoardException.None)]
        //[TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", BoardException.None)]
        //[TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1", BoardException.Checkmate)]
        //[TestCase("3qk3/5Q1p/8/p1p1N3/Pp2bP1P/1P1r4/8/4RnK1 b - - 6 38", BoardException.Checkmate)]
        //[TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55", BoardException.Checkmate)]
        //[TestCase("4R3/2p3pk/pp3p2/5n1p/2P2P1P/P5r1/1P4q1/3QR2K w - - 6 41", BoardException.Checkmate)]
        public void ValidateEndOfGame(string fen, BoardException expectedException)
        {

            var board = new BoardFENInfo(fen);
            var rule = new EndOfGameRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }
    }
}
