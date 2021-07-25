// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using EnumsNET;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [TestCase(Color.Black, Color.White)]
        [TestCase(Color.White, Color.Black)]
        public void TestOpponentColor(Color input, Color expected)
        {
            var board = new Board
            {
                ActivePlayer = input
            };
            Assert.AreEqual(expected, board.OpponentColor);
        }


        [Test]
        public void CloneTest()
        {
            var board = new FenReader().GetBoard("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2");
            var clone = board.Clone();
            Assert.AreNotSame(board, clone);
            Assert.AreEqual(board, clone);
        }

        [Test]
        public void CloneOccupancyTest()
        {
            var board = new Board();
            var clone = board.CloneOccupancy();
            Assert.AreNotSame(board.Occupancy, clone);
            foreach (var color in Enums.GetValues<Color>())
            {
                foreach (var piece in Enums.GetValues<Piece>())
                {
                    Assert.AreEqual(board.Occupancy.Occupancy(color, piece), clone.Occupancy(color, piece));
                }
            }
        }
        private static readonly FenReader FenReader = new FenReader();
        [TestCaseSource(nameof(GetEqualsTestCases))]
        public void BoardEqualsOverrideTest(TestCase<bool, Board> testCase)
        {
            var equalsInput = testCase.AdditionalInputs.Single();
            var actual = testCase.TestMethodInputValue.Equals((object)equalsInput);
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }
        protected static IEnumerable<TestCase<bool, Board>> GetEqualsTestCases()
        {
            yield return new TestCase<bool, Board>(true, new Board(), "Initial boards, different reference", new Board());
            var englishSetupBoard = FenReader.GetBoard("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2") ;
            yield return new TestCase<bool, Board>(false,
                englishSetupBoard, "Two different boards - English vs Initial Board", new Board());
            var pieceRearrangedBoard = FenReader.GetBoard("rbnqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            yield return new TestCase<bool, Board>(false,
                pieceRearrangedBoard, "Pieces same, rearranged",
                new Board());
            var downAPawnBoard = FenReader.GetBoard("rbnqkbnr/ppp1pppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
            yield return new TestCase<bool, Board>(false, downAPawnBoard, "Black's Down a pawn", new Board());

            yield return new TestCase<bool, Board>(false, new Board(), "null Board", (Board)null);
            var board = new Board();
            yield return new TestCase<bool, Board>(true, board, "Same reference", board);
        }
    }
}